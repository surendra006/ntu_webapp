Imports auth
Imports dbEngine
Imports System.Web.Configuration

Partial Class Core_settingsHandler
    Inherits System.Web.UI.Page
    Protected output As String
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim access As New auth()
        Dim deviceOp As New device()
        Dim engine As New dbEngine()
        Dim deviceDict As New Dictionary(Of String, String)
        Dim secureToken As String
        Dim file As String
        Dim deviceID As String
        If (Session("secureToken")) IsNot Nothing Then
            secureToken = CType(Session("secureToken"), String)
            If (access.authenticate(secureToken)) Then
                deviceID = Request("deviceID")
                file = Server.MapPath("~/oline/config/" & deviceID & ".config")
                If My.Computer.FileSystem.FileExists(file) Then
                    deviceDict = deviceOp.loadConfig(file)
                    If Request("auth") IsNot Nothing Then
                        If engine.authGroup(Request("1")) Then
                            output = deviceDict("ftpserver") & "," & deviceDict("ftpuser") & "," & deviceDict("ftppass")
                        End If
                    End If
                    If Request("update") IsNot Nothing Then
                        If engine.deviceUpdateCheck(deviceID, deviceDict("currentver")).Equals("1") Then
                            deviceDict("updatever") = engine.getDeviceVersion(deviceID)
                            deviceDict("updateflag") = "1"
                            deviceOp.saveConfig(file, deviceDict)
                        Else
                            output = "update request failed"
                        End If
                    End If
                    If Request("checkStatus") IsNot Nothing Then
                        If deviceDict("updateflag").Equals("0") Then
                            output = deviceDict("currentver")
                        Else
                            output = "0"
                        End If
                    End If
                    If Request("ss") IsNot Nothing Then
                        Dim currTime As Date = DateTime.UtcNow.AddSeconds(Convert.ToDouble(engine.getSSTime(deviceID)))
                        deviceDict("requestscreenshot") = currTime.Year.ToString & currTime.ToString("MM") &
                                                            currTime.ToString("dd") & currTime.ToString("HH") & currTime.ToString("mm") &
                                                         currTime.ToString("ss")
                        deviceOp.saveConfig(file, deviceDict)
                    End If

                    If Request("siteChange") IsNot Nothing Then
                        output = engine.deviceUpdateCheck(deviceID, deviceDict("currentver"))
                        If deviceDict("updateflag").Equals("1") Then
                            output = "~"
                        End If

                        output &= "," & engine.getSiteNameByDevId(deviceID) & "," & deviceDict("currentver") & "," & deviceDict("updatever") &
                            "," & deviceDict("height") & "," & deviceDict("fov") & "," & deviceDict("fliph") & "," & deviceDict("flipv") &
                            "," & deviceDict("epos") & "," & deviceDict("erot") & "," & deviceDict("xpos") & "," & deviceDict("xrot") &
                            "," & deviceDict("roix") & "," & deviceDict("roiy") & "," & (Convert.ToInt32(deviceDict("roiw")) + Convert.ToInt32(deviceDict("roix"))) &
                            "," & (Convert.ToInt32(deviceDict("roih")) + Convert.ToInt32(deviceDict("roiy"))) & "," & deviceDict("stride")
                        If Session("groupAuth") IsNot Nothing Then
                            output &= "," & deviceDict("ftpserver")
                            output &= "," & deviceDict("ftpuser")
                            output &= "," & deviceDict("ftppass")
                        End If
                    End If

                    If Request("imgAreaUpdate") IsNot Nothing Then
                        deviceDict("roix") = Request("roix")
                        deviceDict("roiy") = Request("roiy")
                        deviceDict("roiw") = Request("roiw")
                        deviceDict("roih") = Request("roih")
                        deviceOp.saveConfig(file, deviceDict)
                    End If
                    'save changes
                    If Request("camHt") IsNot Nothing Then
                        engine.updateSiteName(deviceID, Request("siteName"))
                        deviceDict("height") = Request("camHt")
                        deviceDict("fov") = Request("fov")
                        deviceDict("stride") = Request("stride")
                        deviceDict("epos") = Request("epos")
                        deviceDict("erot") = Request("erot")
                        deviceDict("xpos") = Request("xpos")
                        deviceDict("xrot") = Request("xrot")
                        If Request("flipH").Equals("true") Then
                            deviceDict("fliph") = "1"
                        Else
                            deviceDict("fliph") = "0"
                        End If
                        If Request("flipV").Equals("true") Then
                            deviceDict("flipv") = "1"
                        Else
                            deviceDict("flipv") = "0"
                        End If
                        If Request("ftpserver") IsNot Nothing Then
                            deviceDict("ftpserver") = Request("ftpserver")
                            deviceDict("ftpuser") = Request("ftpuser")
                            deviceDict("ftppass") = Request("ftppass")
                        End If
                        Try
                            deviceOp.saveConfig(file, deviceDict)
                            output = "Update Successful!"
                        Catch ex As Exception
                            output = "Something Went Wrong!"
                        End Try
                    End If
                    If Request("checkMod") IsNot Nothing Then
                        file = Server.MapPath("~/oline/screenshot/" & deviceID & ".jpg")
                        If My.Computer.FileSystem.FileExists(file) Then
                            Dim currTime As Date = DateTime.UtcNow
                            Dim modTime As Date = My.Computer.FileSystem.GetFileInfo(file).LastWriteTimeUtc
                            If currTime.Subtract(modTime).Minutes <= 1 Then
                                output = "1"
                            Else
                                output = "0"
                            End If
                        End If
                    End If
                Else
                    output = "Config file missing!"
                End If

            End If
        End If
    End Sub
End Class
