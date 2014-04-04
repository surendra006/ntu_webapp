Imports auth
Imports otrackEngine
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration

Partial Class otrackGraph
    Inherits System.Web.UI.Page
    Protected output As String
    Dim myCommand As SqlCommand

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim access As New auth()
        Dim secureToken As String
        Dim paramsArr() As String
        Dim ParamsArrList As New List(Of String)
        Dim location As String
        Dim dataType As String
        Dim selected As Boolean = False
        Dim viewType As String
        Dim dateSelectionFrom As String
        Dim dateSelectionTo As String
        Dim selType As Integer
        Dim selWeeklyDay As String
        Dim selMonthlyDw As String
        Dim selMonthlyDwVal As String
        Dim selYearlyDm As String
        Dim selYearlyDmVal As String
        Dim engine As New otrackEngine()

        If (Session("secureToken")) IsNot Nothing Then
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                output = "test return from vb"
                If Request.Form("type") = "Stock" Then
                    output = "[" & engine.PiePlot(Request.Form("location")) & "]"
                ElseIf Request.Form("type") = "HourlyStock" Then
                    output = "[" & engine.HoursPlot(Request.Form("location"), Request.Form("startdatetime"), Request.Form("enddatetime")) & "]"
                ElseIf Request.Form("type") = "HourlyPortlet" Then
                    output = "[" & engine.PastTwelveHours(Request.Form("location")) & "]"
                ElseIf Request.Form("type") = "HourlyPortletRange" Then
                    output = "[" & engine.GetHoursByDate(Request.Form("location"), Request.Form("today"), Request.Form("tomorrow")) & "]"
                ElseIf Request.Form("type") = "FortnightlyPortlet" Then
                    output = "[" & engine.PastFortnight(Request.Form("location")) & "]"
                ElseIf Request.Form("type") = "QuarterlyPortlet" Then
                    output = "[" & engine.PastQuarter(Request.Form("location")) & "]"
                ElseIf Request.Form("type") = "YearlyPortlet" Then
                    output = "[" & engine.PastYear(Request.Form("location")) & "]"
                ElseIf Request.Form("type") = "LiveStatsDaily" Then
                    output = "[" & engine.LiveStats("Daily") & "]"
                ElseIf Request.Form("type") = "LiveStatsWeekly" Then
                    output = "[" & engine.LiveStats("Weekly") & "]"
                ElseIf Request.Form("type") = "LiveStatsMonthly" Then
                    output = "[" & engine.LiveStats("Monthly") & "]"
                ElseIf Request.Form("type") = "LiveStatsYearly" Then
                    output = "[" & engine.LiveStats("Yearly") & "]"
                ElseIf Request.Form("type") = "HealthStats" Then
                    output = "[" & engine.HealthStats() & "]"
                ElseIf Request.Form("type") = "Campaigns" Then
                    output = "[" & engine.GetAllCampaigns("-1") & "]"
                ElseIf Request.Form("type") = "UpdateCampaignRow" Then
                    engine.executeNonQuery("UPDATE oline_campaigns SET title='" & Request.Form("title") & "', siteid= '" & Request.Form("location") & "', manager= '" & Request.Form("manager") & "', goal='" & Request.Form("goal") & "', startdate='" & Request.Form("begin") & "', enddate = '" & Request.Form("end") & "' WHERE id=" & Request.Form("id"))
                    output = engine.GetAllCampaigns(Request.Form("id"))
                ElseIf Request.Form("type") = "DuplicateCampaignRow" Then
                    output = engine.SingleIntReturnQuery("Insert Into oline_campaigns (gid, uid, siteid, startdate, enddate, goal, title, manager) output inserted.id Select gid, uid, siteid, startdate, enddate, goal, title, manager From oline_campaigns Where id=" & Request.Form("id"))
                    output = engine.GetAllCampaigns(output)
                ElseIf Request.Form("type") = "NewCampaignRow" Then
                    output = engine.SingleIntReturnQuery("insert into oline_campaigns (gid, uid, siteid) output inserted.id select " & engine.gid & ", " & engine.uid & ", a.topsite from (select TOP 1 siteid as topsite from oline_devices where groupid='" & engine.gid & "')a;")
                    output = engine.GetAllCampaigns(output)
                    Console.Write(output)
                ElseIf Request.Form("type") = "DeleteCampaignRow" Then
                    output = engine.executeNonQuery("DELETE from oline_campaigns WHERE id=" & Request.Form("id"))
                Else
                    output = "[]"
                End If
            End If
        End If
    End Sub

End Class
