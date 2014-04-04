Imports auth
Imports dbEngine
Imports System.Web.Configuration

Partial Class Core_updateUser
    Inherits System.Web.UI.Page
    Protected output As String
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim access As New auth()
        Dim engine As New dbEngine()
        Dim secureToken As String
        Dim column As String
        Dim val As String
        Dim uid As String
        If (Session("secureToken")) IsNot Nothing Then
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                If (Request.Form("getUser")) IsNot Nothing Then
                    output = engine.getUserAtt(Request.Form("uid"))
                ElseIf (Request.Form("delUser")) IsNot Nothing Then
                    engine.deleteAcc(Request.Form("uid"))
                ElseIf (Request.Form("newUser")) IsNot Nothing Then
                    If (Int32.Parse(Session("usersCount")) < Int32.Parse(Session("threshold"))) Then
                        output = engine.newUser(Request.Form("1"), Request.Form("2"), Request.Form("3"), Request.Form("4"))
                    Else
                        output = "You have reached user creation limit!"
                    End If
                Else
                    column = Request.Form("1")
                    val = Request.Form("2")
                    uid = Request.Form("3")
                    output = engine.updateAcc(column, val, uid)
                End If

            End If
        End If
    End Sub
End Class
