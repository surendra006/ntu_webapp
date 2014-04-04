Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports System.Security.Cryptography
Imports System.Threading

Partial Class _Default
    Inherits System.Web.UI.Page

    Dim connectionString As String = WebConfigurationManager.ConnectionStrings("LocalSqlServer").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim username As String = Request.Form("login")
        Dim pass As String = Request.Form("pass")
        Dim myConnection As New SqlConnection(connectionString)
        Dim myCommand As New SqlCommand("SELECT * FROM account WHERE email='" & username & "' AND password='" & pass & "'", myConnection)
        Dim loginLogCommand As New SqlCommand()
        Dim reader As SqlDataReader
        Dim uid As String
        Dim secureToken As String = ""
        Dim redirectToIndex = False
        Dim redirectToLogin = False
        Dim LoginPage As String = ConfigurationManager.AppSettings("LoginPage")
        Try
            myConnection.Open()
            reader = myCommand.ExecuteReader()

            If reader.HasRows Then
                While reader.Read()
                    If Session("secureToken") IsNot Nothing Then
                        If CType(Session("secureToken"), String).Equals(reader("securityToken")) Then
                            redirectToIndex = True
                            'Response.Redirect("../../index.aspx", False)
                            'Context.ApplicationInstance.CompleteRequest()
                        End If
                    Else
                        uid = reader("userID")
                        Session("name") = reader("name")
                        Session("priv") = reader("privilege")
                        Session("uid") = uid
                        Session("gid") = reader("groupID")
                        If reader("threshold") IsNot Nothing Then
                            Session("threshold") = reader("threshold")
                        End If
                        secureToken = SHA256(reader("password") & Session.SessionID)
                        Session("secureToken") = secureToken

                        myCommand = New SqlCommand("update account set securityToken='" & secureToken &
                                                   "',lastLogin = getDate() where userID=" & uid, myConnection)
                        loginLogCommand = New SqlCommand("insert into oline_log_login (gid, uid, timeinfo) values(" & reader("groupID") & "," & reader("userID") & ",getdate())", myConnection)
                        Exit While
                    End If
                End While

                reader.Close()
                myCommand.ExecuteNonQuery()
                loginLogCommand.ExecuteNonQuery()
                myConnection.Close()
                redirectToIndex = True

            Else
                'My.Computer.FileSystem.WriteAllText(Server.MapPath("../../exception.log"), "\n" & DateTime.Now().ToString("yyyy-MM-dd HH:mm:ss") & " Invalid Login: " & username, True)
                redirectToLogin = True
                'Response.Redirect("../../login.aspx", False)
                'Context.ApplicationInstance.CompleteRequest()
            End If
            If redirectToIndex = True Then
                'My.Computer.FileSystem.WriteAllText(Server.MapPath("../../exception.log"), "\n Redirect to index", True)
                Response.Redirect("../../index.aspx", False)
                Context.ApplicationInstance.CompleteRequest()
            End If
            If redirectToLogin = True Then
                'My.Computer.FileSystem.WriteAllText(Server.MapPath("../../exception.log"), "\n Redirecting to login...", True)
                Response.Redirect("../../" + LoginPage, False)
                Context.ApplicationInstance.CompleteRequest()
            End If
        Catch threaderras As ThreadAbortException
            redirectToLogin = True
            'My.Computer.FileSystem.WriteAllText(Server.MapPath("../../exception.log"), "\n" & DateTime.Now().ToString("yyyy-MM-dd HH:mm:ss") & " " & Err.ToString(), True)
        Catch err As Exception
            ' Handle an error by displaying the information.
            'My.Computer.FileSystem.WriteAllText(Server.MapPath("../../exception.log"), "\n" & DateTime.Now().ToString("yyyy-MM-dd HH:mm:ss") & " " & err.ToString(), True)
            redirectToLogin = True
            'Response.Redirect("../../login.aspx", False)
            'Context.ApplicationInstance.CompleteRequest()
        End Try

    End Sub


    Public Function SHA256(ByVal stringToHash As String) As String
        Dim sha As New SHA256CryptoServiceProvider
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(stringToHash)

        bytesToHash = sha.ComputeHash(bytesToHash)

        Dim out1 As String = ""

        For Each b As Byte In bytesToHash
            out1 += b.ToString("x2")
        Next
        Return out1
    End Function
End Class
