<System.ComponentModel.RunInstaller(True)> Partial Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

    'Installer overrides dispose to clean up the component list.
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

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.svcProcInstall = New System.ServiceProcess.ServiceProcessInstaller()
        Me.svcInstall = New System.ServiceProcess.ServiceInstaller()
        '
        'svcProcInstall
        '
        Me.svcProcInstall.Account = System.ServiceProcess.ServiceAccount.LocalSystem
        Me.svcProcInstall.Password = Nothing
        Me.svcProcInstall.Username = Nothing
        '
        'svcInstall
        '
        Me.svcInstall.Description = "Synchronises a Bellminder box with the local machine's time"
        Me.svcInstall.DisplayName = "Bellminder Time Sync"
        Me.svcInstall.ServiceName = "BellminderTimeSync"
        Me.svcInstall.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'ProjectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.svcProcInstall, Me.svcInstall})

    End Sub

    Friend WithEvents svcProcInstall As ServiceProcess.ServiceProcessInstaller
    Friend WithEvents svcInstall As ServiceProcess.ServiceInstaller
End Class
