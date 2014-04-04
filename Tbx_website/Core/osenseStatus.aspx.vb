Imports auth
Imports osenseEngine
Imports System.Web.Configuration


Partial Class osenseStatus
    Inherits System.Web.UI.Page
    Protected output As String
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim access As New auth()
        Dim engine As New osenseEngine()
        Dim secureToken As String

        If (Session("secureToken")) IsNot Nothing Then
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                Dim locationsList As String = Request("locations")
                If locationsList IsNot Nothing Then
                    output = engine.getSiteStatus(locationsList)
                End If
            End If
        End If
    End Sub
End Class
