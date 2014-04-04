Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Configuration
Imports EO.Pdf
Imports htmlGen

Partial Class reportGen
    Inherits System.Web.UI.Page
    Protected returnOut As String = ""

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim access As New auth()
        Dim secureToken As String
        Dim engine As New dbEngine()
        Dim htmlgen As New htmlGen()
        Dim ParamsList As New List(Of String)
        Dim paramsArr() As String
        Dim selType As Integer
        Dim LoginPage As String = ConfigurationManager.AppSettings("LoginPage")

        returnOut = ""

        If (Request.Form("signout")) IsNot Nothing Then
            access.deAuthenticate()
            Session.Abandon()
            Response.Redirect(LoginPage)
        End If

        If (Session("secureToken")) Is Nothing Then
            Response.Redirect(LoginPage)
        Else
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                If (Request.Form("svgCode")) IsNot Nothing Then
                    paramsArr = {Request.Form("dateSelectionFrom"), Request.Form("dateSelectionTo")}
                    ParamsList.AddRange(paramsArr)
                    If (Request.Form("graphType")) IsNot Nothing Then
                        '--
                    ElseIf (Request.Form("selected")) Is Nothing Then
                        ParamsList.Add(Request.Form("dataType"))
                        ParamsList.Add(Request.Form("viewType"))
                    Else
                        ParamsList.Add("Advance Selection")
                        ParamsList.Add(Request.Form("dataType"))
                        selType = Convert.ToInt32(Request.Form("selType"))
                        Select Case selType
                            Case 1
                                ParamsList.Add("Week on week : " & Request.Form("selWeeklyDay"))
                            Case 2
                                ParamsList.Add("Month on month : " & Request.Form("selMonthlyDw") & " " & Request.Form("selMonthlyDwVal"))
                            Case 3
                                ParamsList.Add("Year on year : " & Request.Form("selYearlyDm") & " " & Request.Form("selYearlyDmVal"))
                        End Select
                    End If
                    ParamsList.Add(HttpUtility.UrlDecode(Request.Form("svgCode"), System.Text.Encoding.Default()))
                    ParamsList.Add(Request.Form("aggData"))
                    returnOut = htmlgen.generatePage(ParamsList)
                Else
                    Dim fileName As String = Server.MapPath("~/rports/" & Session("gid") & "/report.html")
                    If My.Computer.FileSystem.FileExists(fileName) Then
                        Dim doc As New PdfDocument()
                        Dim lol As New System.Drawing.SizeF(PdfPageSizes.A4.Height, PdfPageSizes.A4.Width)
                        HtmlToPdf.Options.PageSize = lol
                        HtmlToPdf.Options.AutoFitX = HtmlToPdfAutoFitMode.None
                        HtmlToPdf.Options.JpegQualityLevel = 100
                        HtmlToPdf.ConvertUrl("http://137.132.145.204:8080/rports/" & Session("gid") & "/report.html", doc)
                        doc.Save(Server.MapPath("~/rports/" & Session("gid") & "/Oline_Report.pdf"))
                        Response.Clear()
                        Response.ClearHeaders()
                        Response.ContentType = "application/pdf"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=Oline_Report.pdf")
                        Response.TransmitFile(Server.MapPath("~/rports/" & Session("gid") & "/Oline_Report.pdf"))
                        Response.End()
                    Else
                        returnOut = "0"
                    End If
                End If
            Else
                Response.Redirect(LoginPage)
            End If
        End If
    End Sub
End Class
