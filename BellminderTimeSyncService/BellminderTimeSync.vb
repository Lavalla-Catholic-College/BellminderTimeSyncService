Imports System.IO.Ports
Imports System.Diagnostics
Imports Microsoft.Win32
Imports System.Threading

Public Class BellminderTimeSync

    ' Whether the COM port is open at the moment. 
    Private isCOMPortOpen As Boolean = False
    Private logger As New EventLog
    Private comPort As Integer
    Private baudRate As Integer
    Private interval As Integer
    Private timer As Timer

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.

        If Not EventLog.SourceExists("BellminderTimeSync") Then
            EventLog.CreateEventSource("BellminderTimeSync", "Application")
        End If

        logger.Source = "BellminderTimeSync"
        logger.Log = "Application"

        comPort = LoadSettings().comPort
        baudRate = LoadSettings().baudRate
        interval = LoadSettings().interval

        WriteLog(String.Format("Started Bellminder Time Sync. COM port is {0} and baud rate is {1} with a refresh rate of {2}", comPort, baudRate, tmrSync.Interval), EventLogEntryType.Information)

        ' Initialize the timer to fire every 10 minutes (600000 milliseconds)
        Dim callback As New TimerCallback(AddressOf TimerElapsed)
        timer = New Timer(callback, Nothing, 0, interval)

        SendTimeDate()

    End Sub

    Protected Overrides Sub OnStop()
        ' Stop the timer
        If timer IsNot Nothing Then
            timer.Dispose()
            timer = Nothing
        End If
    End Sub

    Public Sub SendHexDataToSerialPort(portName As String, baudRate As Integer, dataToSend As Byte(), Optional KeepOpen As Boolean = False)

        Try
            ' Create a new SerialPort instance
            Using serialPort As New SerialPort(portName, baudRate, Parity.None, 8, 1)

                ' If the COM port isn't open. 
                If Not isCOMPortOpen Then
                    ' Open the serial port
                    serialPort.Open()
                    ' And set the relevant flag 
                    isCOMPortOpen = True
                End If

                ' Send the data to the serial port
                serialPort.Write(dataToSend, 0, dataToSend.Length)

                ' If we're done with the connection
                If Not KeepOpen Then
                    ' Close the serial port
                    serialPort.Close()
                    isCOMPortOpen = False
                End If
            End Using
        Catch ex As Exception
            ' If we failed for whatever reason, log the error message and when the error occured.
            WriteLog("Error sending data to serial port: " & ex.Message, EventLogEntryType.Error)
            isCOMPortOpen = False
        End Try
    End Sub

    Private Sub WriteLog(message As String, severity As EventLogEntryType)
        logger.WriteEntry(message, severity)
    End Sub

    ' Helper function that returns a date.
    ' If you pass it an hour, minute and second, it'll return today's date with the time
    ' set to that. If you don't pass anything, it'll return the current date
    Public Function GetTime(Optional hour As Integer = -1, Optional minute As Integer = -1, Optional second As Integer = -1) As Date
        Dim currentDate As Date = Date.Now

        ' If we haven't set an hour, just default to the current time 
        If hour = -1 Then
            Return currentDate
        Else
            Return New Date(currentDate.Year, currentDate.Month, currentDate.Day, hour, minute, second)
        End If

    End Function

    ' Sends the specified time to the Bellminder unit. If no time is set, it'll just send the current date. 
    Public Sub SendTimeDate(Optional time As Date = Nothing)

        ' Get the current date
        Dim d As Date = Date.Now

        ' If we haven't set a date, default to the current date
        If time <> Nothing Then
            d = time
        End If

        ' The BellMinder unit wants a 2 digit year. There's a few ways to get it, but
        ' the easiest way is to just take the current year (e.g. 2023) and subtract 2000
        ' from it. It's a bit hacky, but it works.
        Dim twoDigitYear As Integer = d.Year - 2000

        ' First we prepare a command that tells the Bellminder to expect the date
        Dim hexData1 As Byte() = {
            &H1B, ' 1B is Bellminder header
            &H41 ' And 41 is the command to set the time
        }

        ' Then we prepare the actual time and date.
        ' It's formatted as hour (24 hour notation), minute,
        ' second, year, month, day, all converted to bytes 
        Dim hexData2 As Byte() = {
            BitConverter.GetBytes(d.Hour).First,
            BitConverter.GetBytes(d.Minute).First,
            BitConverter.GetBytes(d.Second).First,
            BitConverter.GetBytes(twoDigitYear).First,
            BitConverter.GetBytes(d.Month).First,
            BitConverter.GetBytes(d.Day).First
        }

        ' Dim status As String = $"Preparing to send {twoDigitYear}/{d.Month}/{d.Day} {d.Hour}:{d.Minute}:{d.Second} "
        ' WriteLog(status, EventLogEntryType.Information)

        ' Now we send both messages. They have to be sent separately
        SendHexDataToSerialPort(String.Format("COM{0}", comPort), baudRate, hexData1)
        SendHexDataToSerialPort(String.Format("COM{0}", comPort), baudRate, hexData2)

        ' Update the window and tray icon to say it was successful
        WriteLog(String.Format("Last Synchronized {0}:{1}:{2}", d.Hour.ToString, d.Minute.ToString, d.Second.ToString), EventLogEntryType.Information)
    End Sub

    Public Sub GetCurrentTime()
        ' 1B 58
    End Sub
    Private Sub TimerElapsed(state As Object)
        ' Call the SendTimeDate method every time the timer fires
        SendTimeDate()
    End Sub

    Public Function LoadSettings() As (comPort As Integer, baudRate As Integer, interval As Integer)
        Dim RegistryKeyPath As String = "SOFTWARE\BellminderTimeSync"

        Try
            Using keyLocal As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                Using key As RegistryKey = keyLocal.OpenSubKey(RegistryKeyPath, False)
                    If key IsNot Nothing Then
                        comPort = CInt(key.GetValue("COMPort", 1))
                        baudRate = CInt(key.GetValue("BaudRate", 9600))
                        interval = CInt(key.GetValue("Interval", 1000 * 60 * 10)) ' 10 minutes
                    Else
                        WriteLog("Unable to load registry key for settings. Defaulting to COM Port 1 and 9600 Baud Rate with a refresh rate of 60000", EventLogEntryType.Warning)
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle any exceptions, such as logging the error
            WriteLog("Error loading settings: " & ex.Message, EventLogEntryType.Error)
        End Try

        Return (comPort, baudRate, interval)
    End Function

End Class
