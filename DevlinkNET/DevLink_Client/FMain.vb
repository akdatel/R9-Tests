Imports DelinkNET
Imports System.Threading

Public Class FMain
    Dim ThreadCount As Integer = 0
    Friend MyThread As Thread


    Friend Ipo As DevLinkNet.Devlink

    'Private Delegate UpdateTSPStatusDelegate()
    Private Delegate Sub UpdateThreadDelegate()
    Private Delegate Sub UpdateGridDelegate_S(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_S_Parameter)
    Private Delegate Sub UpdateGridDelegate_A(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_A_Parameter)
    Private Delegate Sub UpdateGridDelegate_D(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_D_Parameter)




    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim IpoList As List(Of IpOffice) = IpoXml.DeSerializeIpo
        Dim IsOk As Boolean = False
        Try

            Button2.Enabled = True
            Button1.Enabled = False
            Button3.Enabled = False
            Ipo = New DevLinkNet.Devlink

            Ipo.CallLogEventType = DevLinkNet.CallLogType.BaseAndAdvanced
           
            If FLogBase.IsHandleCreated Then
                FLogBase.CreaEventi()
                FLogBase.WindowState = FormWindowState.Normal
            Else
                FLogBase.Show()
            End If

            If Fconsole.IsHandleCreated Then
                Fconsole.CreaEventi()
                Fconsole.WindowState = FormWindowState.Normal
            Else
                Fconsole.Show()
            End If

            AddHandler Ipo.CallLog_Event_A, AddressOf CallsLog_A
            AddHandler Ipo.CallLog_Event_D, AddressOf CallsLog_D
            AddHandler Ipo.CallLog_Event_S, AddressOf CallsLog_S

            For Each row As IpOffice In IpoList
                If row.IpoEnabled Then
                    Ipo.StartMonitor(row.IpoID, row.IpoAddress, row.IpoPassword)
                    IsOk = True
                End If

            Next

            If Not IsOk Then
                Button2.PerformClick()
                MsgBox("No active IpOffice  present!!!")
            End If

            UpdateThead()

        Catch ex As Exception
            Button2.Enabled = False
            Button1.Enabled = True
            MsgBox("Error : " & ex.Message)
        End Try

    End Sub
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim IpoList As List(Of IpOffice) = IpoXml.DeSerializeIpo
        Try
            If Not IsNothing(Ipo) Then
                For Each row As IpOffice In IpoList
                    If row.IpoEnabled Then Ipo.StopMonitor(row.IpoID)
                Next
                RemoveHandler Ipo.CallLog_Event_A, AddressOf CallsLog_A
                RemoveHandler Ipo.CallLog_Event_D, AddressOf CallsLog_D
                RemoveHandler Ipo.CallLog_Event_S, AddressOf CallsLog_S

            End If

            'Fconsole.Close()
            'FLogBase.Close()
            Ipo = Nothing
            UpdateThead()

        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
        End Try

        Button2.Enabled = False
        Button1.Enabled = True
        Button3.Enabled = True
    End Sub


    Private Function CheckDirectionInbound(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_S_Parameter) As Boolean
        CheckDirectionInbound = False

        If Param.LogInfo.CalledPartyNumber.Trim.Length > 1 And Param.LogInfo.CalledPartyNumber.Trim.Length < 5 Then
            CheckDirectionInbound = True
        End If


    End Function


    Private Sub UpdateGrid_S(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_S_Parameter)
        If DataGridView1.InvokeRequired Then
            Dim d As New UpdateGridDelegate_S(AddressOf Me.UpdateGrid_S)
            Me.BeginInvoke(d, New Object() {Param})
        Else
            Dim Dr As DataGridViewRow = New DataGridViewRow
            Dim CellValue()
            Dim bUpdate As Boolean = False


            With Param.LogInfo
                CellValue = {Param.IdPbx, .CallID, If(CheckDirectionInbound(Param), Img1.Images.Item(1), Img1.Images.Item(2)), .DialledPartyNumber, .CalledPartyNumber, .CallingPartyNumber, .Aname, .Bname, .Astate.ToString, .Bstate.ToString, .OriginalHuntGroupName, If(.AisMusic = 0, Img1.Images.Item(0), Img1.Images.Item(3)), If(.BisMusic = 0, Img1.Images.Item(0), Img1.Images.Item(3)), .CallWaiting, .Transferring}
            End With


            For Each row As DataGridViewRow In DataGridView1.Rows
                If IsNothing(row.Tag) Then Exit For
                If row.Tag = Param.LogInfo.CallID Then
                    row.SetValues(CellValue)
                    bUpdate = True
                    Exit For
                End If

            Next

            If Not bUpdate Then
                Dr.Tag = Param.LogInfo.CallID
                Dr.CreateCells(DataGridView1, CellValue)
                DataGridView1.Rows.Add(Dr)
            End If



        End If


        UpdateThead()
    End Sub

    Private Sub UpdateGrid_D(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_D_Parameter)
        If DataGridView1.InvokeRequired Then
            Dim d As New UpdateGridDelegate_D(AddressOf Me.UpdateGrid_D)
            Me.BeginInvoke(d, New Object() {Param})
        Else
            Dim Dr As DataGridViewRow = New DataGridViewRow

            For Each row As DataGridViewRow In DataGridView1.Rows
                If row.Tag = Param.LogInfo.CallID Then
                    DataGridView1.Rows.Remove(row)
                    Exit For
                End If

            Next

            Dr.Dispose()

        End If

        UpdateThead()
    End Sub

    Private Sub UpdateGrid_A(Param As DevLinkNet.CallLogEvent_Parameter.CallLog_A_Parameter)
        If DataGridView1.InvokeRequired Then
            Dim d As New UpdateGridDelegate_A(AddressOf Me.UpdateGrid_A)
            Me.BeginInvoke(d, New Object() {Param})
        Else
            Dim Dr As DataGridViewRow = New DataGridViewRow

            For Each row As DataGridViewRow In DataGridView1.Rows
                If row.Tag = Param.LogInfo.CallID Then
                    DataGridView1.Rows.Remove(row)
                    Exit For
                End If

            Next

            Dr.Dispose()

        End If

        UpdateThead()
    End Sub


    Private Sub CallsLog_A(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_A_Parameter)
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf UpdateGrid_A), e)
        UpdateThead()
    End Sub

    Private Sub CallsLog_D(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_D_Parameter)
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf UpdateGrid_D), e)
        UpdateThead()
    End Sub

    Private Sub CallsLog_S(sender As Object, e As DevLinkNet.CallLogEvent_Parameter.CallLog_S_Parameter)
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf UpdateGrid_S), e)
        UpdateThead()
    End Sub

    Private Sub UpdateThead()
        If StatusStrip1.InvokeRequired Then
            Dim d As New UpdateThreadDelegate(AddressOf Me.UpdateThead)
            Me.BeginInvoke(d, New Object() {})
        Else
            Dim MaxI = 0
            Dim MaxJ = 0
            Dim AvaiableI = 0
            Dim AvaiableJ = 0
            Dim CurrentI = 0
            ThreadPool.GetMaxThreads(MaxI, MaxJ)
            ThreadPool.GetAvailableThreads(MaxI, MaxJ)

            CurrentI = MaxI - MaxJ
            TSProgressBar1.Minimum = 0
            TSProgressBar1.Maximum = 100

            If CurrentI > 100 Then
                TSProgressBar1.Maximum = CurrentI
            Else
                TSProgressBar1.Maximum = 100
            End If

            TSProgressBar1.Value = CurrentI
            TspStatus.Text = CurrentI
        End If

    End Sub



    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        FSetup.Show()
    End Sub
End Class
