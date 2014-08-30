Imports DelinkNET
Imports System.Threading

Public Class FLogBase
    Dim ipo As DevLinkNet.Devlink

    Private Delegate Sub UpdateVideoDelegate(param As CallLogParameterBase)

    Private Sub FLogBase_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.WindowState = FormWindowState.Minimized
        End If
    End Sub

    Private Sub FLogBase_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.TopLevel = True
        Me.Text = "Wrapper Call Logs Events"
        CreaEventi()

    End Sub

    Private Sub UpdateVideo(param As CallLogParameterBase)
        If RichTextBox1.InvokeRequired Then
            Dim d As New UpdateVideoDelegate(AddressOf Me.UpdateVideo)
            Me.BeginInvoke(d, New Object() {param})
        Else
            Try
                RichTextBox1.SelectionColor = param.color
                RichTextBox1.AppendText(param.IdPbx & " : " & param.LogInfo + vbCrLf)
                'RichTextBox1.SelectionStart = RichTextBox1.Text.Length - 1
                RichTextBox1.ScrollToCaret()
                RichTextBox1.Refresh()
            Catch ex As Exception
                MsgBox("[UpdateVideo] - Errore : " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub CallsLog(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_Base_Parameter)
        'UpdateVideo("PBX : " & pbxh.ToString & " - " & info, Color.LightGreen)
        Dim param As CallLogParameterBase = New CallLogParameterBase
        Try
            param.IdPbx = e.IdPbx
            param.LogInfo = e.LogInfo
            param.color = Color.LightGreen
        Catch ex As Exception
            MsgBox("[CallsLog] - Errore : " & ex.Message)
        End Try


        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf UpdateVideo), param)
    End Sub

    Public Sub CreaEventi()
        ipo = FMain.Ipo

        RemoveHandler ipo.CallLog_Event, AddressOf CallsLog
        AddHandler ipo.CallLog_Event, AddressOf CallsLog
    End Sub

End Class