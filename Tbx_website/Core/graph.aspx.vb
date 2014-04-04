Imports auth
Imports dbEngine
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration

Partial Class Core_graph
    Inherits System.Web.UI.Page
    Protected output As String
    Dim connectionString As String = WebConfigurationManager.ConnectionStrings("LocalSqlServer").ConnectionString
    Dim myCommand As SqlCommand
    Dim conn As New SqlConnection(connectionString)

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
        Dim engine As New dbEngine()

        If (Session("secureToken")) IsNot Nothing Then
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                location = Request.Form("location") 'In numberID value (will populate client side value with db siteID)
                dataType = Request.Form("dataType") 'In number value e.g 1,2,3,4,5; using enum to get corres name 
                dateSelectionFrom = Request.Form("dateSelectionFrom") 'In YYYY-MM-DD'
                dateSelectionTo = Request.Form("dateSelectionTo") 'In YYYY-MM-DD'
                paramsArr = {location, dataType, dateSelectionFrom, dateSelectionTo}
                ParamsArrList.AddRange(paramsArr)
 
                If (Request.Form("graphType")) IsNot Nothing Then
                    output = engine.getTableView(dateSelectionFrom, dateSelectionTo)
                ElseIf (Request.Form("selected")) Is Nothing Then
                    viewType = Request.Form("viewType")  'BASIC select -- Give In number Value, 1 = day, 2 = week, 3 = monthly, 4 = yearly
                    ParamsArrList.Add(viewType)
                    output = engine.plot(ParamsArrList)
                Else
                    selType = Convert.ToInt32(Request.Form("selType")) 'ADVANCE selected --Give  In number, 1 = weekly, 2 = monthly, 3 = yearly
                    ParamsArrList.Add(selType & "")
                    Select Case selType
                        Case 1
                            selWeeklyDay = Request.Form("selWeeklyDay") 'In series of dayNames starting & ending with ' and delimited by "," and  if more than one. E.g. 'monday','tuesday'
                            ParamsArrList.Add(selWeeklyDay)
                        Case 2
                            selMonthlyDw = Request.Form("selMonthlyDw") 'Either day or week
                            selMonthlyDwVal = Request.Form("selMonthlyDwVal") 'If week, number (1-5), if day then dayNames(monday,tuesday,etc)
                            ParamsArrList.Add(selMonthlyDw)
                            ParamsArrList.Add(selMonthlyDwVal)
                        Case 3
                            selYearlyDm = Request.Form("selYearlyDm")    'Either day or month
                            selYearlyDmVal = Request.Form("selYearlyDmVal") ' In d/m format, e.g 5/1 being 5th Jan, if month In number, 1 - 12
                            ParamsArrList.Add(selYearlyDm)
                            ParamsArrList.Add(selYearlyDmVal)
                    End Select
                    output = engine.plot(ParamsArrList)
                End If
            End If
            Else

            End If
    End Sub

End Class
