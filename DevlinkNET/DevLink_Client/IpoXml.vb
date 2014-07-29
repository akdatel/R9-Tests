Imports System.IO
Imports System.Xml.Serialization

Public Class IpoXml
    Public Shared Sub SerializeIpo(Ipo As List(Of IpOffice))
        'Deserialize text file to a new object.
        Dim objStreamWriter As New StreamWriter(Application.StartupPath & "\ipo.xml")
        Dim Ipo1 As List(Of IpOffice) = Ipo
        Dim x As New XmlSerializer(GetType(List(Of IpOffice)))

        x.Serialize(objStreamWriter, Ipo)
        objStreamWriter.Close()
    End Sub

    Public Shared Function DeSerializeIpo() As List(Of IpOffice)
        'Deserialize text file to a new object.
        Dim objStreamReader As New StreamReader(Application.StartupPath & "\ipo.xml")
        Dim Ipo As List(Of IpOffice) = New List(Of IpOffice)
        Dim x As New XmlSerializer(Ipo.GetType)

        Ipo = x.Deserialize(objStreamReader)
        objStreamReader.Close()

        Return Ipo
    End Function
End Class
