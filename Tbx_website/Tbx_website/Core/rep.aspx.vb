Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Configuration
Imports System.Diagnostics
Imports EO.Pdf.Acm
Imports EO.Pdf
Imports htmlGen
Imports System.IO

Partial Class reportGen
    Inherits System.Web.UI.Page
    Protected returnOut As String = ""

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim requestList As New List(Of KeyValuePair(Of DateTime, Integer))()
        Dim engine As New otrackEngine()
        If Not FileIO.FileSystem.FileExists(Server.MapPath("../rports/reports.q")) Then
            Return
        End If
        Using srd As StreamReader = New StreamReader(Server.MapPath("../rports/reports.q"))
            Try
                ' Create an instance of StreamReader to read from a file.
                Dim line As String
                ' Read and display the lines from the file until the end 
                ' of the file is reached.
                Do
                    line = srd.ReadLine()
                    If line.Length = 0 Then
                        Continue Do
                    End If
                    Dim splitArr() As String = line.Split("|")
                    If splitArr.Length <> 2 Then
                        Continue Do
                    End If
                    Dim requestDate As New DateTime()
                    Dim requestGroup As New Integer()
                    Try
                        requestDate = DateTime.Parse(splitArr(0))
                        requestGroup = Integer.Parse(splitArr(1))
                        If requestGroup = 0 Then
                            Dim groupList As List(Of Integer) = engine.GetAllGroups()
                            For Each groupID As Integer In groupList
                                requestList.Add(New KeyValuePair(Of DateTime, Integer)(requestDate, groupID))
                            Next
                        Else
                            requestList.Add(New KeyValuePair(Of DateTime, Integer)(requestDate, requestGroup))
                        End If

                    Catch parseE As Exception
                        Console.WriteLine("Parsing " & splitArr(0) & " " & splitArr(1) & " " & parseE.Message)
                    End Try
                    'Console.WriteLine(line)
                Loop Until line Is Nothing
                srd.Close()
            Catch requestFileE As Exception
                ' Let the user know what went wrong.
                Console.WriteLine("The file could not be read:")
                Console.WriteLine(requestFileE.Message)
            End Try
        End Using
        Try
            My.Computer.FileSystem.DeleteFile(Server.MapPath("../rports/reports.q"))
        Catch ex As Exception
            Console.WriteLine("file delete failed for " & Server.MapPath("../rports/reports.q") & " : " & ex.Message)
        End Try

        For Each requestItem As KeyValuePair(Of DateTime, Integer) In requestList
            Dim hourlyOptions As String
            Dim hourlyData As String = "[[]]"
            Dim hourlyChartTitle As String = "Default"
            Dim StartDate As DateTime = requestItem.Key
            returnOut = ""
            Dim Colors As New List(Of String)()
            Colors.Add("#4572A7")
            Colors.Add("#AA4643")
            Colors.Add("#89A54E")
            Colors.Add("#80699B")
            Colors.Add("#3D96AE")
            Colors.Add("#DB843D")
            Colors.Add("#92A8CD")
            Colors.Add("#A47D7C")
            Colors.Add("#B5CA92")
            Dim colorOptions As String = "["
            For Each color As String In Colors
                colorOptions &= "'" & color & "',"
            Next
            colorOptions &= "]"
            Dim fontColor As String = "#1C94C4"
            Dim CleanupFileList As New List(Of String)()
            Dim AllWeeklyOutput As New Dictionary(Of Integer, String)()
            Dim AllWeeklyTableSummary As New List(Of String)()
            AllWeeklyTableSummary.Add("<tr>" &
                "<th  style='width:100px'>Branch</th>" &
                "<th style='background:#CCCCCC'>Mon</th>" &
                "<th>Tue</th>" &
                "<th style='background:#CCCCCC'>Wed</th>" &
                "<th>Thu</th>" &
                "<th style='background:#CCCCCC'>Fri</th>" &
                "<th>Sat</th>" &
                "<th style='background:#CCCCCC'>Sun</th>" &
                "<th>Total</th>" &
                "</tr>")
            Dim AllHtmlCode As New List(Of String)()
            Dim weeklyDateRange As String = StartDate.ToString("ddd dd MMMM yyyy") + " - " + StartDate.AddDays(6).ToString("ddd dd MMMM yyyy")

            Session("secureToken") = "defaulttoken"
            Session("gid") = requestItem.Value.ToString()
            engine = New otrackEngine()
            If IO.Directory.Exists(Server.MapPath("../rports/" & Session("gid"))) Then
            Else
                IO.Directory.CreateDirectory(Server.MapPath("../rports/" & Session("gid")))
            End If
            'get all sites

            hourlyOptions = "{chart: { renderTo: 'hourly-portlet-chart', height: 200, width:600, type: 'spline' },rangeSelector: { enabled: false},series : [{name: 'name', data: " & hourlyData & ", marker : {enabled : true, radius : 2}, dataLabels: { enabled: true }}],navigator: { enabled: false},exporting: { enabled: false},credits : {enabled : false},legend : {enabled : false},title: { text: '24 Hours', style: {fontSize: '14px',color: '#1C94C4' } },xAxis: {type: 'datetime', tickPixelInterval: 60, tickInterval: 3600 * 1000, tickWidth: 3, labels: {align: 'center',rotation: 270,align: 'right',formatter: function () {return Highcharts.dateFormat('%l%p', this.value);}} },yAxis : {title: { text: false }, min:0},tooltip: { crosshairs: true, shared: true},plotOptions: { series: {shadow: false}, color: " & colorOptions & "}};"

            Dim sites As Dictionary(Of Integer, String) = engine.Report_SiteID_And_SiteName_ByGID()
            Dim siteindex As Integer = 0
            For Each Site As KeyValuePair(Of Integer, String) In sites
                Dim seriesColor As String = "'" & Colors(Int(siteindex Mod 9)) & "'"
                siteindex += 1

                'If (Site.Key <> 10 And Site.Key <> 11) Then
                '    Continue For
                'End If

                Console.WriteLine(Site.Key, Site.Value)

                'traverse through days of week
                Dim CurrentDate As DateTime = StartDate
                Dim i As Integer = 0
                Dim OptionsFileList As New List(Of String)
                Dim PowerHourList As New Dictionary(Of DateTime, Integer)()
                While i < 7
                    CurrentDate = StartDate.AddDays(i)
                    Dim powerHour As DateTime = CurrentDate
                    Dim powerHourVal As Integer = -1
                    Dim hourlyOutput As String = engine.GetHoursByDate(Site.Key, CurrentDate.ToString("yyyy-MM-dd"), CurrentDate.AddDays(1).ToString("yyyy-MM-dd"), powerHour, powerHourVal)
                    PowerHourList.Add(powerHour, powerHourVal)
                    Console.WriteLine(CurrentDate, " ", hourlyOutput)
                    hourlyData = "[" & hourlyOutput & "]"
                    hourlyChartTitle = Site.Value + " " + CurrentDate.ToString("dddd, dd MMMM yyyy")
                    hourlyOptions = "{chart: { renderTo: 'hourly-portlet-chart', height: 200, width:600, type: 'spline' },rangeSelector: { enabled: false},series : [{name: 'name', color: " & seriesColor & ", data: " & hourlyData & ", marker : {enabled : true, radius : 2}, dataLabels: { enabled: true }}],navigator: { enabled: false},exporting: { enabled: false},credits : {enabled : false},legend : {enabled : false},title: { text: '" & hourlyChartTitle & "', style: {fontSize: '14px',color: '" & fontColor & "' } },xAxis: {type: 'datetime', tickPixelInterval: 60, tickInterval: 3600 * 1000, tickWidth: 3, labels: {align: 'center',rotation: 270,align: 'right',formatter: function () {return Highcharts.dateFormat('%l%p', this.value);}} },yAxis : {title: { text: false }, min:0},tooltip: { crosshairs: true, shared: true},plotOptions: { series: {shadow: false}}};"
                    i = i + 1
                    Dim hourlyOptionsFilepath As String = "../rports/" & Session("gid") & "/options_" & Site.Key & "_" & CurrentDate.ToString("yyyy-MM-dd") & ".json"
                    OptionsFileList.Add(hourlyOptionsFilepath)
                    CleanupFileList.Add(hourlyOptionsFilepath)
                    My.Computer.FileSystem.WriteAllText(Server.MapPath(hourlyOptionsFilepath), hourlyOptions, False)
                End While

                For Each OptionsFile As String In OptionsFileList
                    RunPhantomProcess(Server.MapPath(OptionsFile))
                    CleanupFileList.Add(OptionsFile.Replace(".json", ".svg"))
                Next

                'GENERATE HOURLY HTML PAGE
                Dim HTML_HourlyPage_Filename As String = "../rports/" & Session("gid") & "/hourlypage_" & Site.Key & "_" & StartDate.ToString("yyyy-MM-dd") & ".html"

                Dim HTML_HourlyPage As String
                HTML_HourlyPage = "<!DOCTYPE HTML>" &
                  "<html>" &
                  "<head>" &
                  "<meta http-equiv='content-type' content='text/html;charset=utf-8'/>" &
                   "<link rel='stylesheet' href='../css/reset.css'/>" &
                   "<link rel='stylesheet' href='../css/styles.css'/>" &
                   "<title> HOURLY SUMMARY </title>" &
                  "</head>" &
                  "<body style=""width:800px"">" &
                  "<div id='body-div'>" &
                  "<img id='logo' src='../logo.png' style=""height:40px;width:auto;""/>" &
                  "<h1 id='company'>" & Site.Value & "</h1>" &
                  "<article style='text-align:center'>"
                For index As Integer = 0 To OptionsFileList.Count - 1
                    Dim OptionsFile As String = OptionsFileList(index)
                    If index = 4 Then
                        HTML_HourlyPage &= "</br></br></br></br></br></br></br></br>"
                    End If
                    If index < PowerHourList.Count Then
                        Dim powerHourDT As DateTime = PowerHourList.Keys(index)
                        Dim powerHourVal As Integer = PowerHourList.Values(index)
                        If powerHourVal <= 0 Then
                            HTML_HourlyPage &= "<div><h2> Power Hour : --:-- </h2></div>"
                        Else
                            HTML_HourlyPage &= "<div><h2> Power Hour : " & powerHourVal & " from " & powerHourDT.ToString("h:mm tt") & " to " & powerHourDT.AddMinutes(30).ToString("h:mm tt") & " " & "</h2></div>"
                        End If
                    Else
                        HTML_HourlyPage &= "<div><h2> Power Hour : --:-- </h2></div>"
                    End If

                    If My.Computer.FileSystem.FileExists(Server.MapPath(OptionsFile.Replace(".json", ".svg"))) Then
                        HTML_HourlyPage &= "<div>" & "<img src='../" & OptionsFile.Replace(".json", ".svg") & "'/>" & "</div>"
                    Else
                        HTML_HourlyPage &= "<div> <h2>NO DATA: " & OptionsFile.Replace(".json", "") & "</h2><div> "
                    End If
                Next
                'For Each OptionsFile As String In OptionsFileList
                '    HTML_HourlyPage &= "<div><h2></h2></div>"
                '    If My.Computer.FileSystem.FileExists(Server.MapPath(OptionsFile.Replace(".json", ".svg"))) Then
                '        HTML_HourlyPage &= "<div>" & "<img src='../" & OptionsFile.Replace(".json", ".svg") & "'/>" & "</div>"
                '    Else
                '        HTML_HourlyPage &= "<div> <h2>NO DATA: " & OptionsFile.Replace(".json", "") & "</h2><div> "
                '    End If
                'Next
                HTML_HourlyPage &= "</article>" &
                     "</div>" &
                     "</body>" &
                     "</html>"
                My.Computer.FileSystem.WriteAllText(Server.MapPath(HTML_HourlyPage_Filename), HTML_HourlyPage, False)
                'Branch Overview Page
                Dim weeklyOptionsFilepath As String = "../rports/" & Session("gid") & "/options_" & Site.Key & "_weekly_" & StartDate.ToString("yyyy-MM-dd") & ".json"
                Dim weekLowest, weekHighest, weekTotal As Integer
                Dim weekHighestDT, weekLowestDT As New DateTime()
                Dim dailyData, dailyDataPrev As New Dictionary(Of DateTime, Integer)
                Dim weeklyOutput As String = engine.Report_Daily_Between(Site.Key, StartDate.ToString("yyyy-MM-dd"), StartDate.AddDays(7).ToString("yyyy-MM-dd"), weekTotal, weekLowest, weekLowestDT, weekHighest, weekHighestDT, dailyData)
                AllWeeklyOutput.Add(Site.Key, weeklyOutput)
                Dim weekLowestPrev, weekHighestPrev, weekTotalPrev As Integer
                Dim weekHighestDTPrev, weekLowestDTPrev As New DateTime()
                Dim weeklyOutputPrev As String = engine.Report_Daily_Between(Site.Key, StartDate.AddDays(-7).ToString("yyyy-MM-dd"), StartDate.ToString("yyyy-MM-dd"), weekTotalPrev, weekLowestPrev, weekLowestDTPrev, weekHighestPrev, weekHighestDTPrev, dailyDataPrev)

                Console.WriteLine(StartDate, " ", weeklyOutput)
                Dim weeklyData As String = "[" & weeklyOutput & "]"
                Dim weeklyChartTitle As String = Site.Value + " " + StartDate.ToString("ddd dd MMMM yyyy") + " - " + StartDate.AddDays(6).ToString("ddd dd MMMM yyyy")

                Dim weeklyOptions As String = "{chart: { renderTo: 'weekly-portlet-chart', height: 300, width:800, type: 'column' },rangeSelector: { enabled: false},series : [{name: 'name', color: " & seriesColor & ", data: " & weeklyData & ", marker : {enabled : true, radius : 2}, dataLabels: { enabled: true }}],navigator: { enabled: false},exporting: { enabled: false},credits : {enabled : false},legend : {enabled : false},title: { text: '" & weeklyChartTitle & "', style: {fontSize: '14px',color: '" & fontColor & "' } },xAxis: {type: 'datetime', tickPixelInterval: 60, tickInterval: 24 * 3600 * 1000, tickWidth: 3, labels: {align: 'center',rotation: 0,align: 'center',formatter: function () {return Highcharts.dateFormat('%a %d %b', this.value);}} },yAxis : {title: { text: false }, min:0},tooltip: { crosshairs: true, shared: true},plotOptions: { series: {shadow: false}}};"
                My.Computer.FileSystem.WriteAllText(Server.MapPath(weeklyOptionsFilepath), weeklyOptions, False)
                RunPhantomProcess(Server.MapPath(weeklyOptionsFilepath))
                CleanupFileList.Add(weeklyOptionsFilepath)
                CleanupFileList.Add(weeklyOptionsFilepath.Replace(".json", ".svg"))

                'GENERATE WEEKLY HTML PAGE
                Dim HTML_Weekly_Page_Filename As String = "../rports/" & Session("gid") & "/weekly_" & Site.Key & "_" & StartDate.ToString("yyyy-MM-dd") & ".html"

                Dim HTML_WeeklyPage As String
                HTML_WeeklyPage = "<!DOCTYPE HTML>" &
                  "<html>" &
                  "<head>" &
                  "<meta http-equiv='content-type' content='text/html;charset=utf-8'/>" &
                   "<link rel='stylesheet' href='../css/reset.css'/>" &
                   "<link rel='stylesheet' href='../css/styles.css'/>" &
                   "<title> HOURLY SUMMARY </title>" &
                  "</head>" &
                  "<body style=""width:800px"">" &
                  "<div id='body-div'>" &
                  "<img id='logo' src='../logo.png' style=""height:40px;width:auto;""/>" &
                  "<h1 id='sitename' style='text-align:center'>" & Site.Value & "</h1>" &
                  "<br/><h2 id='daterange' style='text-align:center'>" & weeklyDateRange & "</h2>" &
                  "<article style='text-align:center'>"
                HTML_WeeklyPage &= "<div>" & "<img src='../" & weeklyOptionsFilepath.Replace(".json", ".svg") & "'/>" & "</div>"

                HTML_WeeklyPage &= "<div>" &
                  "<br/><h1 id='weeklytotal' style=""text-align:center""> THIS WEEK STATS </h1>" &
                  "<br/><table style=""width:300px"">" &
                  "<tr><th>TOTAL</th><th>" & weekTotal & "</th><th>" & engine.PercentageParser(weekTotal, weekTotalPrev, "P") & "</th></tr>" &
                  "<tr><th>LOW</th><th>" & weekLowest & " " & weekLowestDT.ToString("(ddd)") & "</th><th>" & engine.PercentageParser(weekLowest, weekLowestPrev, "P") & " " & weekLowestDTPrev.ToString("(ddd)") & "</th></tr>" &
                  "<tr><th>HIGH</th><th>" & weekHighest & " " & weekHighestDT.ToString("(ddd)") & "</th><th>" & engine.PercentageParser(weekHighest, weekHighest, "P") & " " & weekHighestDTPrev.ToString("(ddd)") & "</th></tr>" &
                  "<tr><th>AVG</th><th>" & Int(weekTotal / 7) & "</th><th>" & engine.PercentageParser(weekTotal / 7, weekTotalPrev / 7, "P") & "</th></tr>" &
                  "</table></div>"

                HTML_WeeklyPage &= "<br/><div><table style=""width:800px"">"
                HTML_WeeklyPage &= "<tr>" &
                 "<th style='background:#CCCCCC'>Mon</th>" &
                 "<th>Tue</th>" &
                 "<th style='background:#CCCCCC'>Wed</th>" &
                 "<th>Thu</th>" &
                 "<th style='background:#CCCCCC'>Fri</th>" &
                 "<th>Sat</th>" &
                 "<th style='background:#CCCCCC'>Sun</th>" &
                 "</tr>"

                Dim dailyTableString As String = ""
                dailyTableString &= "<tr>"
                dailyTableString &= "<th>" & Site.Value & "</th>"
                HTML_WeeklyPage &= "<tr>"
                For Each daily As KeyValuePair(Of DateTime, Integer) In dailyData
                    Try
                        Dim dailyPrev As Integer
                        dailyDataPrev.TryGetValue(daily.Key.AddDays(-7), dailyPrev)
                        dailyTableString &= "<th>" & daily.Value & " " & engine.PercentageParser(daily.Value, dailyPrev, "P") & "</th>"
                        HTML_WeeklyPage &= "<th>" & daily.Value & " " & engine.PercentageParser(daily.Value, dailyPrev, "P") & "</th>"
                    Catch ex As Exception
                        HTML_WeeklyPage &= "<th>" & "-" & " " & "N/A%" & "</th>"
                    End Try
                Next
                dailyTableString &= "<th>" & weekTotal & " " & engine.PercentageParser(weekTotal, weekTotalPrev, "P") & "</th>"
                dailyTableString &= "</tr>"
                AllWeeklyTableSummary.Add(dailyTableString)
                HTML_WeeklyPage &= "</tr>"
                HTML_WeeklyPage &= "<tr>"
                For Each powerHour As KeyValuePair(Of DateTime, Integer) In PowerHourList
                    If powerHour.Value <= 0 Then
                        HTML_WeeklyPage &= "<th>--:--</th>"
                    Else
                        HTML_WeeklyPage &= "<th>" & powerHour.Value & " @ " & powerHour.Key.ToString("h:mmtt") & " - " & powerHour.Key.AddMinutes(30).ToString("h:mmtt") & "</th>"
                    End If
                Next
                HTML_WeeklyPage &= "</tr>"
                HTML_WeeklyPage &= "</table></div>"
                HTML_WeeklyPage &= "</article>" &
                     "</div>" &
                     "</body>" &
                     "</html>"

                My.Computer.FileSystem.WriteAllText(Server.MapPath(HTML_Weekly_Page_Filename), HTML_WeeklyPage, False)
                AllHtmlCode.Add(HTML_Weekly_Page_Filename)
                AllHtmlCode.Add(HTML_HourlyPage_Filename)
                CleanupFileList.Add(HTML_Weekly_Page_Filename)
                CleanupFileList.Add(HTML_HourlyPage_Filename)
            Next 'end of rendering sites

            'generate chart for intra-store comparison
            Dim AllWeeklyOptionsFilepath As String = "../rports/" & Session("gid") & "/options_" & "allsites_" & StartDate.ToString("yyyy-MM-dd") & ".json"
            Dim AllWeeklyChartTitle As String = " " + StartDate.ToString("ddd dd MMMM yyyy") + " - " + StartDate.AddDays(6).ToString("ddd dd MMMM yyyy")
            Dim seriesJSON As String = ""
            Dim siteindex2 As Integer = 0
            For Each output As KeyValuePair(Of Integer, String) In AllWeeklyOutput
                Dim seriesColor2 As String = "'" & Colors(Int(siteindex2 Mod 9)) & "'"
                Dim seriesName As String = ""
                sites.TryGetValue(output.Key, seriesName)
                Dim seriesData As String = "[" & output.Value & "]"
                seriesJSON &= "{name: '" & seriesName & "', data: " & seriesData & ", color: " & seriesColor2 & ", marker : {enabled : true, radius : 2}, dataLabels: { enabled: true }},"
                siteindex2 += 1
            Next
            Dim AllWeeklyOptions As String = "{chart: { renderTo: 'weekly-portlet-chart', height: 565, width:800, type: 'line' },rangeSelector: { enabled: false},series : [" & seriesJSON & "],navigator: { enabled: false},exporting: { enabled: false},credits : {enabled : false},legend : {enabled : true},title: { text: '" & AllWeeklyChartTitle & "', style: {fontSize: '14px',color: '" & fontColor & "' } },xAxis: {type: 'datetime', tickPixelInterval: 60, tickInterval: 24 * 3600 * 1000, tickWidth: 3, labels: {align: 'center',rotation: 0,align: 'center',formatter: function () {return Highcharts.dateFormat('%a %d %b', this.value);}} },yAxis : {title: { text: false }, min:0},tooltip: { crosshairs: true, shared: true},plotOptions: { series: {shadow: false}}};"
            'Dim AllWeeklyOptions As String = "{chart: { renderTo: 'weekly-portlet-chart', height: 800, width:1130, type: 'line' },rangeSelector: { enabled: false},series : [" & seriesJSON & "],navigator: { enabled: false},exporting: { enabled: false},credits : {enabled : false},legend : {enabled : true},title: { text: '" & AllWeeklyChartTitle & "', style: {fontSize: '14px',color: '" & fontColor & "' } },xAxis: {type: 'datetime', tickPixelInterval: 60, tickInterval: 24 * 3600 * 1000, tickWidth: 3, labels: {align: 'center',rotation: 0,align: 'center',formatter: function () {return Highcharts.dateFormat('%a %d %b', this.value);}} },yAxis : {title: { text: false }, min:0},tooltip: { crosshairs: true, shared: true},plotOptions: { series: {shadow: false}}};"
            My.Computer.FileSystem.WriteAllText(Server.MapPath(AllWeeklyOptionsFilepath), AllWeeklyOptions, False)
            RunPhantomProcess(Server.MapPath(AllWeeklyOptionsFilepath))
            CleanupFileList.Add(AllWeeklyOptionsFilepath)
            CleanupFileList.Add(AllWeeklyOptionsFilepath.Replace(".json", ".svg"))
            'GENERATE WEEKLY INTRA STORE COMPARISON HTML PAGE
            Dim HTML_AllSites_Page_Filename As String = "../rports/" & Session("gid") & "/allsites_" & StartDate.ToString("yyyy-MM-dd") & ".html"

            Dim HTML_AllSitesPage As String
            HTML_AllSitesPage = "<!DOCTYPE HTML>" &
               "<html>" &
               "<head>" &
               "<meta http-equiv='content-type' content='text/html;charset=utf-8'/>" &
                "<link rel='stylesheet' href='../css/reset.css'/>" &
                "<link rel='stylesheet' href='../css/styles.css'/>" &
                "<title> HOURLY SUMMARY </title>" &
               "</head>" &
               "<body style=""width:800px"">" &
               "<div style='width: 600px; border:0px'>" &
               "<div style='width:140px'>" &
                "<img id='logo' src='../logo.png' style='float:left;height: 40px; width: auto;'/>" &
               "</div>" &
               "<div>" &
                "<h1 id='sitename' style='text-align:center; padding-top:10px'>WEEKLY REPORT</h1>" &
               "</div>" &
               "</div>" &
               "<div id='body-div'>" &
               "<div>" &
               "<h2 id='companyname' style='text-align: center'>" &
               "All Sites Summary for " & engine.getCompanyName() & "</h2>" &
               "</br>" &
               "<h2 id='daterange' style='text-align: center'>" &
               weeklyDateRange &
               "</h2>" &
               "<article>"
            HTML_AllSitesPage &= "<div>" & "<img src='../" & AllWeeklyOptionsFilepath.Replace(".json", ".svg") & "'/>" & "</div>"
            HTML_AllSitesPage &= "<br/><div><table style=""width:800px"">"
            For Each tablerow As String In AllWeeklyTableSummary
                HTML_AllSitesPage &= tablerow
            Next
            HTML_AllSitesPage &= "</table></div>"
            HTML_AllSitesPage &= "</article>" &
                 "</div>" &
                 "</body>" &
                 "</html>"
            My.Computer.FileSystem.WriteAllText(Server.MapPath(HTML_AllSites_Page_Filename), HTML_AllSitesPage, False)
            CleanupFileList.Add(HTML_AllSites_Page_Filename)
            'PDF GENERATION
            Dim reportdoc As New PdfDocument()
            HtmlToPdf.ConvertUrl(Server.MapPath(HTML_AllSites_Page_Filename), reportdoc)
            For Each htmlcode As String In AllHtmlCode
                HtmlToPdf.ConvertUrl(Server.MapPath(htmlcode), reportdoc)
            Next
            Dim pdfDocName As String = "../rports/" & Session("gid") & "/" & StartDate.ToString("dd MMM yyyy") & " - " & StartDate.AddDays(6).ToString("dd MMM yyyy") & ".pdf"

            reportdoc.Save(Server.MapPath(pdfDocName))

            'CLEANUP SVG, JSON AND HTML files
            For Each filename As String In CleanupFileList
                Try
                    My.Computer.FileSystem.DeleteFile(Server.MapPath(filename))
                Catch ex As Exception
                    Console.WriteLine("file delete failed for " & Server.MapPath(filename) & " : " & ex.Message)
                End Try
            Next

            'Save the PDF file
            'Dim filepath As String = "../reports" & "/Oline_Report_" & Session("gid") & ".pdf"

            'doc.Save(Server.MapPath("../reports" & "/Oline_Report_" & Session("gid") & ".pdf"))

            'Response.Clear()
            'Response.ClearHeaders()
            'Response.ContentType = "application/pdf"
            'Response.AppendHeader("Content-Disposition", "attachment; filename=Oline_Report.pdf")
            'Response.TransmitFile(Server.MapPath("../reports" & "/Oline_Report_" & Session("gid") & ".pdf"))
            'Response.End()
        Next
    End Sub
    Public Function RunPhantomProcess(ByVal OptionsFile As String) As String
        Dim startInfo As New ProcessStartInfo
        startInfo.FileName = Server.MapPath("../rports/phantomjs/phantomjs.exe")
        startInfo.Arguments = " highcharts-convert.js -infile " & OptionsFile & " -outfile " & OptionsFile.Replace(".json", ".svg") & " -width 300 -constr Chart"
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        startInfo.RedirectStandardOutput = True
        startInfo.RedirectStandardError = True
        startInfo.RedirectStandardInput = True
        startInfo.WorkingDirectory = Server.MapPath("../rports/phantomjs")
        Process.Start(startInfo)

        Dim p As New Process()
        p.StartInfo = startInfo
        p.Start()
        p.WaitForExit(5000)
        'Read the Error:
        Dim stderror As String = p.StandardError.ReadToEnd()
        'Read the Output:
        Dim stdoutput As String = p.StandardOutput.ReadToEnd()
        Console.WriteLine(stderror)
        Console.WriteLine(stdoutput)
        Return ""
    End Function
End Class
