Public Class FSetup
    Dim IpoList As List(Of IpOffice) = IpoXml.DeSerializeIpo
    Private Sub FSetup_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Text = "Setup Ip Office System (" & Application.StartupPath & "\ipo.xml" & ")"
        Try


            If IpoList.Count = 0 Then
                IpoList.Add(New IpOffice With {
                           .IpoAddress = "",
                           .IpoPassword = "",
                           .IpoEnabled = True,
                           .IpoID = IpoList.Count + 1
                                   })
            End If
            DataGridView1.DataSource = IpoList
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
        End Try
    End Sub
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles BtnSave.Click
        Dim RigaId As Integer = 1

        Try


            IpoList = DataGridView1.DataSource

            Dim SaveIpOffice As List(Of IpOffice) = New List(Of IpOffice)



            ' Remove Empty rows
            For Each row As IpOffice In IpoList
                If row.IpoAddress.Trim.Length > 0 Then
                    SaveIpOffice.Add(row)
                End If
            Next

            ' Recreate Unique Ipoffice ID
            For Each row As IpOffice In SaveIpOffice
                row.IpoID = RigaId
                RigaId += 1
            Next

            ' Save to XML file
            IpoXml.SerializeIpo(SaveIpOffice)

            MsgBox("IpOffice System saved!!")

        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
        End Try
    End Sub

    Private Sub BtnAdd_Click(sender As System.Object, e As System.EventArgs) Handles BtnAdd.Click
        Try


            IpoList.Add(New IpOffice With {
                            .IpoAddress = "",
                            .IpoPassword = "",
                            .IpoEnabled = True,
                            .IpoID = IpoList.Count + 1
                        })

            DataGridView1.DataSource = Nothing
            DataGridView1.DataSource = IpoList
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
        End Try
    End Sub

    Private Sub BtnDel_Click(sender As System.Object, e As System.EventArgs) Handles BtnDel.Click
        Dim RigaId As Integer = 1
        Try
            IpoList.RemoveAt(DataGridView1.CurrentRow.Index)

            For Each row As IpOffice In IpoList
                row.IpoID = RigaId
                RigaId += 1
            Next

            DataGridView1.DataSource = Nothing
            DataGridView1.DataSource = IpoList
        Catch ex As Exception
            MsgBox("Error : " & ex.Message)
        End Try
    End Sub
End Class