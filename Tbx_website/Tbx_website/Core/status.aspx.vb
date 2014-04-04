Imports auth
Imports dbEngine
Imports System.Web.Configuration


Partial Class Core_status
    Inherits System.Web.UI.Page
    Protected output As String
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim access As New auth()
        Dim engine As New dbEngine()
        Dim secureToken As String

        If (Session("secureToken")) IsNot Nothing Then
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                output = engine.getSiteStatus(Request("locations"))
            End If
        End If
    End Sub
End Class
