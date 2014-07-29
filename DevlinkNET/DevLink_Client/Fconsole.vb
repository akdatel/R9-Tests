Imports DelinkNET
Imports System.Threading

Public Class Fconsole
    Dim ipo As DevLinkNet.Devlink

    Private Delegate Sub AggiornaVideoDelegate(msg As String, Color As Color)

    Private Sub AggiornaVideo(ByVal msg As String, Color As Color)
        If RichTextBox1.InvokeRequired Then
            Dim d As New AggiornaVideoDelegate(AddressOf Me.AggiornaVideo)
            Me.BeginInvoke(d, New Object() {msg, Color})
        Else
            Try
                RichTextBox1.SelectionColor = Color
                RichTextBox1.AppendText(msg + vbCrLf)
                'RichTextBox1.SelectionStart = RichTextBox1.Text.Length - 1
                RichTextBox1.ScrollToCaret()
                RichTextBox1.Refresh()
            Catch ex As Exception
                MsgBox("[AggiornaVideo] - Errore : " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub Fconsole_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.WindowState = FormWindowState.Minimized
        End If
    End Sub


    Private Sub Fconsole_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.TopLevel = True
        Me.Text = "Wrapper Console"
        CreaEventi()

    End Sub

    Private Sub CallsBack(seender As Object, e As DevLinkNet.CommsEvents_Parameter.CommEvent)
        AggiornaVideo(e.comm_state, Color.Green)
    End Sub

    Private Sub StatoConnessione(sender As Object, e As DevLinkNet.Connection_Parameter.Connection_Status_Paramenter)
        AggiornaVideo("Stato : " & e.StatusMessage, Color.Yellow)
    End Sub

    Public Sub CreaEventi()
        ipo = FMain.Ipo

        RemoveHandler ipo.Comms_Event, AddressOf CallsBack
        RemoveHandler ipo.ConnectionStatus, AddressOf StatoConnessione

        AddHandler ipo.Comms_Event, AddressOf CallsBack
        AddHandler ipo.ConnectionStatus, AddressOf StatoConnessione
    End Sub
End Class