Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BellminderTimeSync
    Inherits System.ServiceProcess.ServiceBase

    'UserService overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' The main entry point for the process
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' More than one NT Service may run within the same process. To add
        ' another service to this process, change the following line to
        ' create a second service object. For example,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New BellminderTimeSync}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.evtLog1 = New System.Diagnostics.EventLog()
        Me.tmrSync = New System.Windows.Forms.Timer(Me.components)
        CType(Me.evtLog1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'tmrSync
        '
        Me.tmrSync.Enabled = True
        Me.tmrSync.Interval = 60000
        '
        'BellminderTimeSync
        '
        Me.ServiceName = "Service1"
        CType(Me.evtLog1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

    Friend WithEvents evtLog1 As EventLog
    Friend WithEvents tmrSync As Windows.Forms.Timer
End Class
