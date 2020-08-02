Imports QC.QMSLPhone
Imports QC.QTMDotNetKernelInterface.Environment
Imports SwDownload
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Management
Imports System.Threading
Imports System.Windows.Forms
Imports System.IO.Ports
Public Class Form1

    Private Sub btnprogrammerpath_Click(sender As Object, e As EventArgs) Handles btnprogrammerpath.Click
        Me.LoadProgrammerFileName(Me.strProgrammerName)
        If Me.strProgrammerName = "" Then
            Return
        End If
        Me.textboxprogrammer.Text = Me.strProgrammerName
        Me.strSearchPath = Path.GetDirectoryName(Me.strProgrammerName)
        Me.strMbnName = Me.strSearchPath + "\boot_images\build\ms\bin\8909\emmc\prog_emmc_firehose_8909_ddr.mbn"
        If Not File.Exists(Me.strMbnName) Then
            Me.UpdateLog(Me.strSearchPath + ",this is an invalid directory.")
            Return
        End If
        Me.UpdateLog("Load the contents.xml file... ...")
        If Me.strSearchPath.IndexOf("LYF-F120B") >= 0 Then
            Me.swmodel.Text = "LYF-F120B"
            Me.strSWModel = swVersion.F120B
        ElseIf Me.strSearchPath.IndexOf("LYF-LF2403N") >= 0 Then
            Me.swmodel.Text = "LYF-LF2403N"
            Me.strSWModel = swVersion.LF2403N
        ElseIf Me.strSearchPath.IndexOf("LYF-LF2403S") >= 0 Then
            Me.swmodel.Text = "LYF-LF2403S"
            Me.strSWModel = swVersion.LF2403S
        ElseIf Me.strSearchPath.IndexOf("LYF-LF2403") >= 0 Then
            Me.swmodel.Text = "LYF-LF2403"
            Me.strSWModel = swVersion.LF2403
        ElseIf Me.strSearchPath.IndexOf("LYF-F220B") >= 0 Then
            Me.swmodel.Text = "LYF-F220B"
            Me.strSWModel = swVersion.F220B
        ElseIf Me.strSearchPath.IndexOf("LYF-F300B") >= 0 Then
            Me.swmodel.Text = "LYF-F300B"
            Me.strSWModel = swVersion.F300B
        End If
        Me.testStatus1 = TestStatus.READY
        If Me.comportStatus1 <> ComportStatus.nophone Then
            Me.download.Enabled = True
            If Me.checkBoxauto.CheckState = CheckState.Checked Then
                Delay(1000)
                Me.download.PerformClick()
            End If
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    
    End Sub


    Public Sub New()
        Me.strFirehoseProgrammer = ""
        Me.strMbnName = ""
        Me.strProgrammerName = ""
        Me.strSearchPath = ""
        Me.cmdCommand = ""
        Me.adbArg = ""
        Me.phoneSWVersion = ""
        Me.physicalPortAddress = ""
        Me.comPortstr = ""
        Me.strImei = ""
        Me.comDes = "No Port Available"
        Me.InitializeComponent()
        MyBase.MaximizeBox = False

        Init()


        Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))
        Me.UsbCheckThread.Start()

        Me.download.Enabled = False
        Me.testStatus1 = TestStatus.TESTING
        Me.qsprEnv = New QSPREnvironment("111")
        Me.dlEvent = AddressOf Me.OnSwDownloadEvent
        Me.dltest = New SwDownloadTest(Me.qsprEnv)
        Me.OnUpdateProgression = AddressOf Me.OnUpdateUIProgression
        Me.emmcDL = New EMMCDownload(Me.qsprEnv)
    End Sub

   














































    Private Delegate Sub setText()
    Public Delegate Sub OnUpdateParameter()

    Public Enum ComportStatus
        nophone
        EDL
        HLOS
    End Enum

    Private Enum TestStatus
        READY
        TESTING
        FAIL
    End Enum

    Private Enum swVersion
        other
        LF2403
        LF2403N
        LF2403S
        F120B
        F220B
        F300B
    End Enum

    Private Enum phoneBootMode
        HLOS
        FFBM
    End Enum








    Public Enum HardwareEnum

        Win32_BIOS
        Win32_ParallelPort
        Win32_SerialPort
        Win32_SerialPortConfiguration
        Win32_SoundDevice
        Win32_SystemSlot
        Win32_USBController
        Win32_NetworkAdapter
        Win32_NetworkAdapterConfiguration
        Win32_Printer
        Win32_PrinterConfiguration
        Win32_PrintJob
        Win32_TCPIPPrinterPort
        Win32_POTSModem
        Win32_POTSModemToSerialPort
        Win32_DesktopMonitor
        Win32_DisplayConfiguration
        Win32_DisplayControllerConfiguration
        Win32_VideoController
        Win32_VideoSettings
        Win32_TimeZone
        Win32_SystemDriver
        Win32_DiskPartition
        Win32_LogicalDisk
        Win32_LogicalDiskToPartition
        Win32_LogicalMemoryConfiguration
        Win32_PageFile
        Win32_PageFileSetting
        Win32_BootConfiguration
        Win32_ComputerSystem
        Win32_OperatingSystem
        Win32_StartupCommand
        Win32_Service
        Win32_Group
        Win32_GroupUser
        Win32_UserAccount
        Win32_Process
        Win32_Thread
        Win32_Share
        Win32_NetworkClient
        Win32_NetworkProtocol
        Win32_PnPEntity
    End Enum

    Public Const WM_DEVICE_CHANGE As Integer = 537

    Public Const DBT_DEVICEARRIVAL As Integer = 32768

    Public Const DBT_DEVICE_REMOVE_COMPLETE As Integer = 32772

    Public Const DBT_DEVTYP_PORT As UInteger = 3UI
    Private qsprEnv As QSPREnvironment

    Private dltest As SwDownloadTest

    Private dlEvent As SwDownloadEventHandler

    Private emmcDL As EMMCDownload

    Private strFirehoseProgrammer As String = ""

    Private strMbnName As String = ""

    Private strProgrammerName As String = ""

    Private strSearchPath As String = ""

    Private strSWModel As swVersion

    Private strPhoneModel As swVersion

    Private cmdCommand As String = ""

    Private adbArg As String = ""

    Private phoneSWVersion As String = ""

    Private strQcnBackupFile As String

    Private physicalPortAddress As String = ""

    Private comPortstr As String = ""

    Private comPort As Integer

    Private adbMode As Boolean

    Private strImei As String = ""

    Private testStatus1 As TestStatus

    Private comportStatus1 As ComportStatus

    Private comDes As String = "No Port Available"

    Private WorkThread As Thread

    Private UsbCheckThread As Thread

    Private BackupQCNThread As Thread

    Private RestoreQCNThread As Thread

    Private bProgress As UInteger

    Public OnUpdateProgression As OnUpdateParameter

    Private progression As Integer

    Public Event ReadStdOutput As DelReadStdOutput

    Public Event ReadErrOutput As DelReadErrOutput



    Public Event onDownLoadProgress As dDownloadProgress

    Private Sub download_Click(sender As Object, e As EventArgs) Handles download.Click
        Me.adbMode = False
        Me.phonemodel.Text = ""
        Me.ProgressBar1.Value = 0
        Me.phoneSWVersion = ""
        Me.download.Enabled = False
        Me.btnexit.Enabled = False
        Me.btnprogrammerpath.Enabled = False
        Me.strProgrammerName = textboxprogrammer.Text
        If Me.strProgrammerName = "" Then
            Me.UpdateLog("Please select the contents.xml file for Meta build first.")
            Me.download.Enabled = True
            UpdateLog(vbLf & "Download failed")
            Me.UpdateLog("Finish Download")



            Return
        End If
        testStatus1 = TestStatus.TESTING
        Me.download.Enabled = False
        Me.WorkThread = New Thread(New ThreadStart(AddressOf FirehoseDownload))



        Me.WorkThread.Start()
        Me.bProgress = 1UI
        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
    End Sub


    Private Sub UpdateLog(information As String)
        Control.CheckForIllegalCrossThreadCalls = False
        Me.infowindow.AppendText(information + Environment.NewLine)
        Me.infowindow.ScrollToCaret()
    End Sub




    '// firehouse functions 

    ' SubsysSwdlDemo.SubsysSwdl
    Public Sub FirehoseDownload()
        Try
            Me.GetParamtersFromUI()
            If comportStatus1 = ComportStatus.nophone Then
                Me.UpdateLog("Phone status is wrong.")
                Me.testUIInit()
            ElseIf Me.comportStatus1 = ComportStatus.HLOS Then
                Dim num2 As Integer
                If Me.checkpPhoneModel.CheckState = CheckState.Checked Then
                    Dim num As UShort = 0US
                    Dim b As Byte = 0
                    Dim b2 As Byte = 0
                    Dim b3 As Byte = 0
                    Dim b4 As Byte = 0
                    Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                    Me.adbArg = "kill-server"
                    Me.RealAction(Me.cmdCommand, Me.adbArg)
                    Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                    Me.adbArg = "devices"
                    Me.RealAction(Me.cmdCommand, Me.adbArg)
                    Me.bProgress += 2UI

                    RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                    If Me.adbMode Then
                        num2 = 2
                        Do
                            Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                            Me.adbArg = "shell getprop ro.build.version.incremental"
                            num2 -= 1
                            Delay(1000)
                            Me.RealAction(Me.cmdCommand, Me.adbArg)
                            Me.bProgress += 2UI
                            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                            Delay(1000)
                            Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                            Me.adbArg = "kill-server"
                            Me.RealAction(Me.cmdCommand, Me.adbArg)
                        Loop While Me.phoneSWVersion = "" AndAlso num2 <> 0
                    End If
                    If Me.phoneSWVersion = "" Then
                        Me.setupPhoneCom()
                        If Not Me.dltest.QPHONEMS_FFBM_GET_MODE(num, b, b2, b3, b4) Then
                            Me.UpdateLogCallback(Me.dltest, "Fail to get FFBM  MODE")
                            Me.testUIInit()
                            Return
                        End If
                        If b = 0 Then
                            Dim num3 As UShort = 0US
                            If Not Me.dltest.QPHONEMS_FFBM_SET_MODE(1, 0, num3) Then
                                Me.UpdateLogCallback(Me.dltest, "Fail to set FFBM  MODE")
                                Me.testUIInit()
                                Return
                            End If
                            If Not Me.dltest.QPHONEMS_DIAG_CONTROL_F(mode_enum_type.MODE_RESET_F, 2000) Then
                                Me.UpdateLogCallback(Me.dltest, "Fail to RESET PHONE ")
                                Me.testUIInit()
                                Return
                            End If
                            If Not Me.dltest.DisconnectFromServer() Then
                                Me.UpdateLog("Fail to Disconnect From Server")
                            Else
                                Me.UpdateLog("Disconnect From Server")
                            End If
                            Delay(10000)

                            Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))

                            Me.UsbCheckThread.Start()
                            Me.UsbCheckThread.Priority = ThreadPriority.Highest
                            Me.dltest.StartTime()
                            Dim num4 As Double = 0.0
                            Dim num5 As Double = 30.0
                            Do
                                Me.dltest.EndTime(num4)
                                If Not Me.UsbCheckThread.IsAlive Then
                                    Exit Do
                                End If
                                Delay(1000)
                                Me.bProgress += 1UI
                                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                            Loop While num4 < num5
                            Me.setupPhoneCom()
                        End If
                        Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                        Me.adbArg = "kill-server"
                        Me.RealAction(Me.cmdCommand, Me.adbArg)
                        num2 = 3
                        Delay(5000)
                        Do
                            Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                            Me.adbArg = "shell getprop ro.build.version.incremental"
                            num2 -= 1
                            Delay(3000)
                            Me.RealAction(Me.cmdCommand, Me.adbArg)
                            Me.bProgress += 2UI
                            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                            Delay(3000)
                            Me.cmdCommand = Environment.CurrentDirectory + "\adb.exe"
                            Me.adbArg = "kill-server"
                            Me.RealAction(Me.cmdCommand, Me.adbArg)
                        Loop While Me.phoneSWVersion = "" AndAlso num2 <> 0
                        Dim num6 As UShort = 0US
                        If Not Me.dltest.QPHONEMS_FFBM_GET_MODE(num, b, b2, b3, b4) Then
                            Delay(5000)
                            If Not Me.dltest.QPHONEMS_FFBM_GET_MODE(num, b, b2, b3, b4) Then
                                Me.UpdateLogCallback(Me.dltest, "Fail to get FFBM  MODE")
                                Me.testUIInit()
                                Return
                            End If
                        End If
                        If b = 1 Then
                            If Not Me.dltest.QPHONEMS_FFBM_SET_MODE(0, 0, num6) Then
                                Me.UpdateLogCallback(Me.dltest, "Fail to set FFBM  MODE")
                                Me.testUIInit()
                                Return
                            End If
                            If Not Me.dltest.QPHONEMS_DIAG_CONTROL_F(mode_enum_type.MODE_RESET_F, 2000) Then
                                Me.UpdateLogCallback(Me.dltest, "Fail to RESET PHONE ")
                                Me.testUIInit()
                                Return
                            End If
                            If Not Me.dltest.DisconnectFromServer() Then
                                Me.UpdateLog("Fail to Disconnect From Server")
                            Else
                                Me.UpdateLog("Disconnect From Server")
                            End If
                            Delay(8000)
                            Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))
                            Me.UsbCheckThread.Start()
                            Me.UsbCheckThread.Priority = ThreadPriority.Highest
                            Me.dltest.StartTime()
                            Dim num7 As Double = 0.0
                            Dim num8 As Double = 30.0
                            Do
                                Me.dltest.EndTime(num7)
                                If Not Me.UsbCheckThread.IsAlive Then
                                    Exit Do
                                End If
                                Delay(1000)
                                Me.bProgress += 1UI
                                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                            Loop While num7 < num8
                            Me.bProgress += 2UI
                            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                        End If
                    End If
                    If Me.phoneSWVersion = "" Then
                        Me.UpdateLogCallback(Me.dltest, "Fail to get PHONE  MODE")
                        Me.testUIInit()
                        Return
                    End If
                    If Me.phoneSWVersion.IndexOf("LYF-F120B") >= 0 Then
                        Me.strPhoneModel = swVersion.F120B
                        Me.phonemodel.Text = Me.phoneSWVersion
                    ElseIf Me.phoneSWVersion.IndexOf("LYF-LF2403N") >= 0 Then
                        Me.strPhoneModel = swVersion.LF2403N
                        Me.phonemodel.Text = Me.phoneSWVersion
                    ElseIf Me.phoneSWVersion.IndexOf("LYF-LF2403S") >= 0 Then
                        Me.strPhoneModel = swVersion.LF2403S
                        Me.phonemodel.Text = Me.phoneSWVersion
                    ElseIf Me.phoneSWVersion.IndexOf("LYF-LF2403") >= 0 Then
                        Me.strPhoneModel = swVersion.LF2403
                        Me.phonemodel.Text = Me.phoneSWVersion
                    ElseIf Me.phoneSWVersion.IndexOf("LYF-F220B") >= 0 Then
                        Me.strPhoneModel = swVersion.F220B
                        Me.phonemodel.Text = Me.phoneSWVersion
                    ElseIf Me.phoneSWVersion.IndexOf("LYF-F300B") >= 0 Then
                        Me.strPhoneModel = swVersion.F300B
                        Me.phonemodel.Text = Me.phoneSWVersion
                    End If
                    Dim str As String = "Download software model : " + Me.strSWModel
                    Dim str2 As String = "Phone software version: " + Me.phoneSWVersion
                    If Me.strPhoneModel <> Me.strSWModel Then
                        MessageBox.Show(str + " ;" & vbLf + str2 + " ;" & vbLf & vbLf & "Phone Model is not matched with Download software version!", "Error Message")
                        Me.UpdateLogCallback(Me.dltest, "Phone Model is not matched with Download software version!")
                        Me.testUIInit()
                        Return
                    End If
                    If Me.strPhoneModel = Me.strSWModel Then
                        Dim dialogResult As DialogResult = MessageBox.Show(str + " ;" & vbLf + str2 + " ;" & vbLf & vbLf & "Do you want to continue downloading?", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                        If dialogResult <> dialogResult.OK Then
                            Me.UpdateLog("Finish Download")
                            Me.testStatus1 = TestStatus.READY
                            Me.btnprogrammerpath.Enabled = True
                            btnexit.Enabled = True
                            If Me.comportStatus1 <> ComportStatus.nophone Then
                                Me.download.Enabled = True
                            End If
                            Return
                        End If
                    End If
                End If
                If Me.checkBox_BackupQCN.CheckState = CheckState.Checked Then
                    If Not Me.dltest.DisconnectFromServer() Then
                        Me.UpdateLog("Fail to Disconnect From Server")
                    Else
                        Me.UpdateLog("Disconnect From Server")
                    End If
                    Me.setupPhoneCom()
                    Dim text As String = ""
                    If Not Me.dltest.QPHONEMS_ReadIMEI(Me.strImei) Then
                        Me.UpdateLogCallback(Me.dltest, "Fail to READ IMEI")
                        Me.testUIInit()
                        Return
                    End If
                    If Me.checkBox_RandomSPC.CheckState = CheckState.Checked Then
                        Dim text2 As String = Me.strImei.Substring(Me.strImei.Length - 6)
                        Dim array As Char() = text2.ToCharArray()
                        Dim array2 As UInteger() = New UInteger(5) {}
                        Dim array3 As UInteger() = array2
                        For i As Integer = 0 To 6 - 1
                            array3(i) = array3(i) * 10UI + CUInt(Val(array(i))) - 48UI
                            array3(i) = array3(i) * 2UI Mod 10UI
                        Next
                        For j As Integer = 0 To 6 - 1
                            text += array3(j).ToString()
                        Next
                    Else
                        If Me.spccode.Text.Length <> 6 Then
                            Me.UpdateLogCallback(Me.dltest, "SPCCODE:" + text)
                            Me.UpdateLogCallback(Me.dltest, "Invalid SPC Code")
                            Me.testUIInit()
                            Return
                        End If
                        text = Me.spccode.Text
                    End If
                    Me.UpdateLogCallback(Me.dltest, "SPCCODE:" + text)
                    If Not Me.dltest.QPHONEMS_SendDefaultSPC(text) Then
                        Me.UpdateLogCallback(Me.dltest, "Fail to Send SPC")
                        Me.testUIInit()
                        Return
                    End If
                    If Not Me.BackupQCNtest() Then
                        Me.UpdateLogCallback(Me.dltest, "Fail to Backup QCN to Mobile ")
                        Me.testUIInit()
                        Return
                    End If
                    If Me.bProgress < 25UI Then
                        If Not Me.dltest.DisconnectFromServer() Then
                            Me.UpdateLog("Fail to Disconnect From Server")
                        Else
                            Me.UpdateLog("Disconnect From Server")
                        End If
                        Delay(3000)
                        Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))
                        Me.UsbCheckThread.Start()
                        Me.UsbCheckThread.Priority = ThreadPriority.Highest
                        Me.dltest.StartTime()
                        Dim num9 As Double = 0.0
                        Dim num10 As Double = 30.0
                        Do
                            Me.dltest.EndTime(num9)
                            If Not Me.UsbCheckThread.IsAlive Then
                                Exit Do
                            End If
                            Delay(1000)
                            Me.bProgress += 1UI
                            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                        Loop While num9 < num10
                        Me.setupPhoneCom()
                        If Not Me.BackupQCNtest() Then
                            Me.UpdateLogCallback(Me.dltest, "Fail to Backup QCN to Mobile ")
                            Me.testUIInit()
                            Return
                        End If
                    End If
                    Dim information As String = "Downloading QCN file 100% "
                    Me.UpdateLogCallback(Me.dltest, information)
                    information = "Done downloading the qcn file: " + Me.strQcnBackupFile
                    Me.UpdateLogCallback(Me.dltest, information)
                    Me.bProgress = 100UI
                    RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                    If Not Me.dltest.DisconnectFromServer() Then
                        Me.UpdateLog("Fail to Disconnect From Server")
                    Else
                        Me.UpdateLog("Disconnect From Server")
                    End If
                End If
                Delay(2000)
                Me.bProgress = 0UI
                If Not Me.dltest.DisconnectFromServer() Then
                    Me.UpdateLog("Fail to Disconnect From Server")
                Else
                    Me.UpdateLog("Disconnect From Server")
                End If
                Me.setupPhoneCom()
                If Not Me.dltest.QPHONEMS_SwitchToEDL() Then
                    Delay(5000)
                    If Not Me.dltest.QPHONEMS_SwitchToEDL() Then
                        Me.UpdateLog("Fail to Switch To EDL")
                        Me.testUIInit()
                        Return
                    End If
                End If
                Me.UpdateLog("Switch To EDL")
                Me.bProgress += 2UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), True)
                If Not Me.dltest.DisconnectFromServer() Then
                    Me.UpdateLog("Fail to Disconnect From Server")
                Else
                    Me.UpdateLog("Disconnect From Server")
                End If
                Dim portDescription As String = "QDLoader"
                Dim s As String = ""
                num2 = 10
                Do
                    num2 -= 1
                    Me.dltest.WaitForDevice(3)
                    Me.emmcDL.Util_GetPortAfterPowerCycleByPhysicalAddress(Me.physicalPortAddress, portDescription, s)
                    Me.comPort = Integer.Parse(s)
                Loop While Not Me.dltest.CheckIfPortExists(Me.comPort) AndAlso Me.comPort = 0 AndAlso num2 <> 0
                Me.dltest.PortNumber = Me.comPort
                Me.bProgress += 2UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                Me.comPortstr = "COM" + Me.comPort.ToString()
                Me.cmdCommand = Environment.CurrentDirectory + "\QSaharaServer.exe"
                Dim startFileArg As String = " -p \\.\" + Me.comPortstr + " -s 13:" + Me.strMbnName
                Delay(10000)
                Me.RealAction(Me.cmdCommand, startFileArg)
                Me.bProgress += 10UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                Delay(5000)
                Me.cmdCommand = Environment.CurrentDirectory + "\fh_loader.exe"
                Dim startFileArg2 As String = String.Concat(New String() {" --port=\\.\", Me.comPortstr, "  --contentsxml=", Me.strProgrammerName, " --flavor=asic --noprompt --showpercentagecomplete --memoryname=emmc"})
                Me.bProgress += 1UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                Me.RealAction(Me.cmdCommand, startFileArg2)
                Delay(3000)
                If Me.ProgressBar1.Value <> 100 Then
                    Me.UpdateLogCallback(Me.dltest, "Fail to Download ,please check")
                    Me.testUIInit()
                Else
                    If Me.checkBox_BackupQCN.CheckState = CheckState.Checked Then
                        Me.bProgress = 0UI
                        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), True)
                        Me.cmdCommand = Environment.CurrentDirectory + "\fh_loader.exe"
                        Dim startFileArg3 As String = " --port=\\.\" + Me.comPortstr + " --reset --noprompt --showpercentagecomplete --zlpawarehost=1  --memoryname=emmc"
                        Me.RealAction(Me.cmdCommand, startFileArg3)
                        Delay(5000)
                        Me.bProgress += 5UI
                        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                        Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))
                        Me.UsbCheckThread.Start()
                        Me.UsbCheckThread.Priority = ThreadPriority.Highest
                        Me.dltest.StartTime()
                        Dim num11 As Double = 0.0
                        Dim num12 As Double = 30.0
                        Do
                            Me.dltest.EndTime(num11)
                            If Not Me.UsbCheckThread.IsAlive Then
                                Exit Do
                            End If
                            Delay(1000)
                            Me.bProgress += 1UI
                            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                        Loop While num11 < num12
                        Me.setupPhoneCom()
                        Me.bProgress = 0UI
                        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), True)
                        If Not Me.RestorQCNtest() Then
                            If Not Me.dltest.DisconnectFromServer() Then
                                Me.UpdateLog("Fail to Disconnect From Server")
                            Else
                                Me.UpdateLog("Disconnect From Server")
                            End If
                            Delay(3000)
                            Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))
                            Me.UsbCheckThread.Start()
                            Me.UsbCheckThread.Priority = ThreadPriority.Highest
                            Me.dltest.StartTime()
                            num11 = 0.0
                            num12 = 30.0
                            Do
                                Me.dltest.EndTime(num11)
                                If Not Me.UsbCheckThread.IsAlive Then
                                    Exit Do
                                End If
                                Delay(1000)
                                Me.bProgress += 1UI
                                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                            Loop While num11 < num12
                            Me.setupPhoneCom()
                            Me.bProgress = 0UI
                            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), True)
                            If Not Me.RestorQCNtest() Then
                                Me.UpdateLogCallback(Me.dltest, "Fail to Resotr QCN to Mobile ")
                                Me.testUIInit()
                                Return
                            End If
                            If Not Me.dltest.DisconnectFromServer() Then
                                Me.UpdateLog("Fail to Disconnect From Server")
                            Else
                                Me.UpdateLog("Disconnect From Server")
                            End If
                        End If
                        Dim information2 As String = "Restore QCN file  100%"
                        Me.UpdateLogCallback(Me.dltest, information2)
                        information2 = "Restore the qcn file: " + Me.strQcnBackupFile
                        Me.UpdateLogCallback(Me.dltest, information2)
                        Me.bProgress = 100UI
                        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                    End If
                    Me.download.Enabled = True
                    btnexit.Enabled = True
                    Me.btnprogrammerpath.Enabled = True
                    testStatus1 = TestStatus.READY
                    Me.UpdateLogCallback(Me.dltest, "Download Succeed")
                    Me.UpdateLogCallback(Me.dltest, "Finish Download")
                End If
            ElseIf Me.comportStatus1 = ComportStatus.EDL Then
                Me.bProgress += 2UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                Me.comPortstr = "COM" + Me.comPort.ToString()
                Me.cmdCommand = Environment.CurrentDirectory + "\QSaharaServer.exe"
                Dim startFileArg4 As String = " -p \\.\" + Me.comPortstr + " -s 13:" + Me.strMbnName
                Me.RealAction(Me.cmdCommand, startFileArg4)
                Me.bProgress += 10UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                Delay(5000)
                Me.cmdCommand = Environment.CurrentDirectory + "\fh_loader.exe"
                Dim startFileArg5 As String = String.Concat(New String() {" --port=\\.\", Me.comPortstr, "  --contentsxml=", Me.strProgrammerName, " --flavor=asic --noprompt --showpercentagecomplete --memoryname=eMMC"})
                Me.bProgress += 20UI
                RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
                Me.RealAction(Me.cmdCommand, startFileArg5)
                Delay(3000)
                If Me.ProgressBar1.Value <> 100 Then
                    Me.UpdateLogCallback(Me.dltest, "Fail to Download ,please check")
                    Me.testUIInit()
                Else
                    Me.download.Enabled = True
                    Me.btnexit.Enabled = True
                    Me.btnprogrammerpath.Enabled = True
                    Me.testStatus1 = TestStatus.READY
                    Me.UpdateLogCallback(Me.dltest, "Finish Download")
                End If
            Else
                Me.download.Enabled = True
                Me.btnexit.Enabled = True
                Me.btnprogrammerpath.Enabled = True
                Me.testStatus1 = TestStatus.READY
            End If
        Catch ex As Exception
            Me.UpdateLog("Download Fail:" + ex.Message)
            Me.testStatus1 = TestStatus.READY
            Me.btnprogrammerpath.Enabled = True
            Me.btnexit.Enabled = True
            If Me.comportStatus1 <> ComportStatus.nophone Then
                Me.download.Enabled = True
            End If
        End Try
    End Sub



    Private Sub checkpPhoneModel_CheckedChanged(sender As Object, e As EventArgs) Handles checkpPhoneModel.CheckedChanged
        If checkpPhoneModel.CheckState = CheckState.Checked Then
            Label1.Enabled = True
            Label2.Enabled = True
            swmodel.Enabled = True
            phonemodel.Enabled = True


        Else


            Label1.Enabled = False
            Label2.Enabled = False
            swmodel.Enabled = False
            phonemodel.Enabled = False

        End If
    End Sub




    Public Sub BackupQCN()
        Dim num As Integer
        Dim text As String = ""
        Dim text2 As String = ""
        If Not Me.dltest.QPHONEMS_BackupNVFromMobileToQCN(Me.strQcnBackupFile, False, SwDownloadTest.SelectOption.No, SwDownloadTest.SelectOption.No, SwDownloadTest.SelectOption.No, SwDownloadTest.SelectOption.No, num, text, text2) Then
            Me.UpdateLogCallback(Me.dltest, "Fail to Backup NV From Mobile To QCN")
            Me.testUIInit()
            Me.testStatus1 = TestStatus.FAIL
        End If
    End Sub
    Private Function BackupQCNtest() As Boolean
        Me.strQcnBackupFile = "C:\TEMP\BackupQCN_" + Me.strImei + ".qcn"
        Me.BackupQCNThread = New Thread(New ThreadStart(AddressOf BackupQCN))
        Me.BackupQCNThread.Start()
        Me.BackupQCNThread.Priority = ThreadPriority.Highest
        Dim num As Integer = 200000
        Dim num2 As Double = 0.0
        Me.dltest.StartTime()
        Me.bProgress = 0UI
        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), True)
        While True
            Me.dltest.EndTime(num2)
            If Not Me.BackupQCNThread.IsAlive Then
                Exit While
            End If
            Delay(2000)
            Me.bProgress += 1UI
            If Me.bProgress > 95UI Then
                Me.bProgress = 95UI
            End If
            Dim information As String = "Downloading QCN file " + Me.bProgress.ToString() + "%"
            Me.UpdateLogCallback(Me.dltest, information)
            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
            If num2 >= CDec(num) Then
                Return True
            End If
        End While
        If Me.testStatus1 = TestStatus.FAIL Then
            Me.UpdateLogCallback(Me.dltest, "Fail to Backup NV From Mobile To QCN")
            Me.testUIInit()
            Return False
        End If
        Return True
    End Function

    Public Shared Sub Delay(mm As Integer)
        While DateTime.Now.AddMilliseconds(CDec(mm)) > DateTime.Now
            Application.DoEvents()
        End While
    End Sub
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso Me.components IsNot Nothing Then
            Me.components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub downloader_onDownLoadProgress(ByVal total As Long, ByVal current As Long, ByVal breset As Boolean)
        If MyBase.InvokeRequired Then
            MyBase.Invoke(New dDownloadProgress(AddressOf Me.downloader_onDownLoadProgress), New Object() {total, current, breset})
            Return
        End If
        Me.ProgressBar1.Maximum = CInt(total)
        If breset Then
            Me.ProgressBar1.Value = 0
        End If
        If current >= CLng(Math.Truncate(Me.ProgressBar1.Maximum)) Then
            Me.ProgressBar1.Value = Me.ProgressBar1.Maximum
            Return
        End If
        If Me.ProgressBar1.Value < Me.ProgressBar1.Maximum Then
            If CLng(Math.Truncate(Me.ProgressBar1.Value)) > current Then
                Me.ProgressBar1.Value += 1
                Return
            End If
            Me.ProgressBar1.Value = CInt(current)
        End If
    End Sub


    Private Sub checkBox_BackupQCN_CheckedChanged(sender As Object, e As EventArgs) Handles checkBox_BackupQCN.CheckedChanged
        If Me.checkBox_BackupQCN.CheckState <> CheckState.Checked Then
            Me.Label8.Enabled = False
            Me.checkBox_RandomSPC.Enabled = False
            Return
        End If
        Me.checkBox_RandomSPC.Enabled = True
        If Me.checkBox_RandomSPC.CheckState = CheckState.Checked Then
            Me.Label8.Enabled = False
            Me.spccode.Enabled = False
            Return
        End If
        Me.Label8.Enabled = True
        Me.spccode.Enabled = True
    End Sub

    Private Sub checkBox_RandomSPC_CheckedChanged(sender As Object, e As EventArgs) Handles checkBox_RandomSPC.CheckedChanged
        If Me.checkBox_RandomSPC.CheckState = CheckState.Checked Then
            Me.Label8.Enabled = False
            Me.spccode.Enabled = False
            Return
        End If
        Me.Label8.Enabled = True
        Me.spccode.Enabled = True
    End Sub


    Private Sub GetParamtersFromUI()
        Me.strFirehoseProgrammer = Path.GetFileName(Me.textboxprogrammer.Text)
    End Sub

    Public Sub GetUsbInfor()
        Me.comPort = GetComNum(Me.comDes, comportStatus1)
        Dim method As setText = AddressOf SetLabel3
        MyBase.Invoke(method)
        If Me.testStatus1 = TestStatus.READY AndAlso Me.comportStatus1 <> ComportStatus.nophone Then
            Me.download.Enabled = True
            If Me.checkBoxauto.CheckState = CheckState.Checked Then
                Delay(1000)
                Me.download.PerformClick()
            End If
        End If
    End Sub



    Private Sub Init()
        AddHandler ReadStdOutput, AddressOf Me.ReadStdOutputAction
        AddHandler ReadErrOutput, AddressOf Me.ReadErrOutputAction
        AddHandler onDownLoadProgress, AddressOf Me.downloader_onDownLoadProgress

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim dialogResult As DialogResult = MessageBox.Show("do you want close", "Application exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If dialogResult = dialogResult.Yes Then

            Application.Exit()

            e.Cancel = False
            Return
        End If
        e.Cancel = True
    End Sub

    Private Shared Function GetComNum(ByRef x As String, ByRef portstatus As ComportStatus) As Integer
        Dim result As Integer = -1
        portstatus = ComportStatus.nophone




        Dim array() As String = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name")

        Dim array2() As String = array




        For i As Integer = 0 To array2.Length - 1

            Dim text As String = array2(i)





            If text.Length >= 23 AndAlso text.Contains("Qualcomm HS-USB Diagnostics") Then
                Dim num As Integer = text.IndexOf("(") + 3
                Dim num2 As Integer = text.IndexOf(")")
                x = text
                result = Convert.ToInt32(text.Substring(num + 1, num2 - num - 1))
                portstatus = ComportStatus.HLOS
            End If
        Next i
        Dim array3() As String = array
        For j As Integer = 0 To array3.Length - 1
            Dim text2 As String = array3(j)
            If text2.Length >= 23 AndAlso text2.Contains("Qualcomm HS-USB QDLoader") Then
                Dim num3 As Integer = text2.IndexOf("(") + 3
                Dim num4 As Integer = text2.IndexOf(")")
                x = text2
                result = Convert.ToInt32(text2.Substring(num3 + 1, num4 - num3 - 1))
                portstatus = ComportStatus.EDL
            End If
        Next j

        Return result



    End Function

    Private Shared Function MulGetHardwareInfo(hardType As HardwareEnum, propKey As String) As String()
        Dim list As New List(Of String)
        Dim result() As String
        Try
            Using managementObjectSearcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_PnPEntity")
                Using enumerator As ManagementObjectCollection.ManagementObjectEnumerator = managementObjectSearcher.Get().GetEnumerator()
                    Do While enumerator.MoveNext()
                        Dim managementObject As ManagementObject = CType(enumerator.Current, ManagementObject)
                        If managementObject.Properties(propKey).Value IsNot Nothing Then
                            Dim item As String = managementObject.Properties(propKey).Value.ToString()
                            list.Add(item.ToString)




                        End If
                    Loop
                End Using
            End Using


            result = list.ToArray()
        Catch ex As Exception

            result = Nothing
        Finally
            list = Nothing

        End Try

        Return result


    End Function


    Private Sub OnSwDownloadEvent(sender As Object, information As String)
        Me.UpdateLogCallback(sender, information)
    End Sub






    Private Sub ReadStdOutputAction(result As String)
        If result <> "" Then
            If Me.cmdCommand.IndexOf("adb.exe") >= 0 Then
                If Me.adbArg.IndexOf("devices") >= 0 AndAlso result.IndexOf("daemon started successfully") >= 0 Then
                    Me.adbMode = True
                End If
                If Me.adbArg.IndexOf("shell getprop ro.build.version.incremental") >= 0 AndAlso result.IndexOf("LYF") >= 0 Then
                    Me.phoneSWVersion = result
                End If
            End If
            Me.UpdateLog(result)
            Dim num As Integer = result.LastIndexOf("%"c)
            If num > 6 Then
                Dim num2 As Integer = Integer.Parse(result.Substring(num - 6, 3))
                RaiseEvent onDownLoadProgress(100L, CLng(num2), False)
            End If
        End If
    End Sub
    Private WithEvents Process As Process

    Private Sub RealAction(StartFileName As String, StartFileArg As String)

        process.StartInfo.FileName = StartFileName
        process.StartInfo.Arguments = StartFileArg
        process.StartInfo.CreateNoWindow = True
        process.StartInfo.UseShellExecute = False
        process.StartInfo.RedirectStandardInput = True
        process.StartInfo.RedirectStandardOutput = True
        process.StartInfo.RedirectStandardError = True
        AddHandler process.OutputDataReceived, AddressOf p_OutputDataReceived
        AddHandler process.ErrorDataReceived, AddressOf p_ErrorDataReceived
        process.EnableRaisingEvents = True
        AddHandler process.Exited, AddressOf Me.CmdProcess_Exited
        process.Start()
        process.BeginOutputReadLine()
        process.BeginErrorReadLine()
        process.WaitForExit()
    End Sub
    Private Sub CmdProcess_Exited(sender As Object, e As EventArgs)
    End Sub


    Public Sub RestoreQCN()
        Dim num As Integer
        Dim text As String = ""
        Dim text2 As String = ""
        If Not Me.dltest.QPHONEMS_UploadQcnFileToPhone(Me.strQcnBackupFile, False, SwDownloadTest.SelectOption.No, SwDownloadTest.SelectOption.No, SwDownloadTest.SelectOption.No, SwDownloadTest.SelectOption.No, num, text, text2) Then
            Me.UpdateLogCallback(Me.dltest, "Fail to Restore QCN")
            Me.testUIInit()
            Me.testStatus1 = TestStatus.FAIL
        End If
    End Sub

    Private Function RestorQCNtest() As Boolean
        Me.RestoreQCNThread = New Thread(New ThreadStart(AddressOf RestoreQCN))
        Me.RestoreQCNThread.Start()
        Me.RestoreQCNThread.Priority = ThreadPriority.Highest
        Dim num As Integer = 200000
        Dim num2 As Double = 0.0
        Me.dltest.StartTime()
        Dim num3 As Integer = 0
        While True
            Me.dltest.EndTime(num2)
            If Not Me.RestoreQCNThread.IsAlive Then
                Exit While
            End If
            Delay(1000)
            Me.bProgress += 2UI
            num3 += 2
            If Me.bProgress > 95UI Then
                Me.bProgress = 95UI
            End If
            Dim information As String = "Restore QCN file " + num3.ToString() + "%"
            Me.UpdateLogCallback(Me.dltest, information)
            RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
            If num2 >= CDec(num) Then
                Return True
            End If
        End While
        If Me.testStatus1 = TestStatus.FAIL Then
            Me.UpdateLogCallback(Me.dltest, "Fail to Resotr QCN to Mobile ")
            Me.testUIInit()
            Return False
        End If
        Return True
    End Function

    Public Sub SetLabel3()
        Label3.Text = Me.comDes.ToString


    End Sub


    Private Sub setupPhoneCom()
        Dim num As Integer = 10
        Dim flag As Boolean
        Do
            num -= 1
            Me.dltest.WaitForDevice(3)
            flag = Me.dltest.CheckIfPortExists(Me.comPort)
        Loop While Not flag AndAlso num <> 0
        Me.bProgress += 2UI
        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
        If Not flag Then
            Me.UpdateLogCallback(Me.dltest, "Fail to open comport,please check")
            Me.testUIInit()
            Return
        End If
        Me.UpdateLog("Success to identify Port")
        Me.dltest.PortNumber = Me.comPort
        Me.dltest.UseQpst = SwDownloadTest.SelectOption.No
        If Not Me.dltest.ConnectToPhone_Simple() Then
            Me.UpdateLogCallback(Me.dltest, "Fail to connect Phone,please check")
            Me.testUIInit()
            Return
        End If
        Me.UpdateLog("Success to connect Phone")
        Me.bProgress += 2UI
        RaiseEvent onDownLoadProgress(100L, CLng((CULng(Me.bProgress))), False)
        If Not Me.emmcDL.Util_GetPhysicalPortAddress(Me.comPort.ToString(), Me.physicalPortAddress) Then
            Me.UpdateLog("Fail to get PhysicalPortAddress")
            Return
        End If
        Me.UpdateLog("PhysicalPortAddress: " + Me.physicalPortAddress)
    End Sub

    Private Sub testUIInit()
        Me.UpdateLog(vbLf & "Download failed")
        Me.UpdateLog("Finish Download")
        Me.testStatus1 = TestStatus.READY
        Me.btnprogrammerpath.Enabled = True
        Me.btnexit.Enabled = True
        If Me.comportStatus1 <> ComportStatus.nophone Then
            Me.download.Enabled = True
        End If
    End Sub



    Private Sub OnUpdateUIProgression()
        Try
            UIHelper.InvokeOnUIifRequired(Me, Sub()
                                              End Sub)
        Catch ex As Exception

        End Try


    End Sub

    Private Sub UpdateLogCallback(sender As Object, information As String)
        Try
            If sender.ToString() <> "Progression" Then
                Me.UpdateLog(information)
            Else
                Me.progression = CInt(Convert.ToDouble(information))
            End If
        Catch ex As Exception
            Me.UpdateLog("Update info exception:" + ex.Message)
        End Try
    End Sub

    Private Shared Sub InvokeDelegate(func As OnUpdateParameter)
        If func IsNot Nothing Then
            func()
        End If
    End Sub

    Private Sub LoadProgrammerFileName(ByRef fileName As String)
        Dim openFileDialog As OpenFileDialog = New OpenFileDialog()
        If fileName <> "" Then
            openFileDialog.InitialDirectory = Path.GetDirectoryName(fileName)
        End If
        fileName = ""
        openFileDialog.Filter = "MetaBuild Configuration|contents.xml;*.xml"
        If openFileDialog.ShowDialog().Equals(DialogResult.OK) Then
            fileName = openFileDialog.FileName
        End If
    End Sub







    Private Sub ReadErrOutputAction(result As String)
        Me.UpdateLog(result)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)


        If m.Msg = 537 Then
            Dim num As Integer = m.WParam.ToInt32()
            If num <> 32768 Then
                If num = 32772 Then
                    Me.download.Enabled = False

                    Me.comDes = "No Port Available"

                    Label3.Text = comDes.ToString
                End If
            Else
                Me.UsbCheckThread = New Thread(New ThreadStart(AddressOf GetUsbInfor))
                Me.UsbCheckThread.Start()
            End If
        End If
        MyBase.WndProc(m)
    End Sub
    Private Sub p_ErrorDataReceived(ByVal sender As Object, ByVal e As System.Diagnostics.DataReceivedEventArgs) Handles Process.ErrorDataReceived
        If e.Data IsNot Nothing Then
            MyBase.Invoke(New DelReadErrOutput(AddressOf Me.ReadErrOutputAction), New Object() {e.Data})
        End If
    End Sub
    Private Sub p_OutputDataReceived(ByVal sender As Object, ByVal e As System.Diagnostics.DataReceivedEventArgs) Handles Process.OutputDataReceived


        If e.Data IsNot Nothing Then
            MyBase.Invoke(New DelReadStdOutput(AddressOf Me.ReadStdOutputAction), New Object() {e.Data})
        End If
    End Sub



    Private Sub btnexit_Click(sender As Object, e As EventArgs) Handles btnexit.Click
        Application.Exit()

    End Sub

    Private Sub ProgressBar1_Click(sender As Object, e As EventArgs) Handles ProgressBar1.Click

    End Sub
End Class
