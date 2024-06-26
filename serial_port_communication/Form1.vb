Imports System.IO.Ports
Imports System.Threading
Imports System.IO

Public Class Form1
    Dim WithEvents serialPort As SerialPort
    Dim rowIndex As Integer = 1
    Dim readingThread As Thread

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate COM ports
        For Each portName As String In SerialPort.GetPortNames()
            ComboBoxCOMPort.Items.Add(portName)
        Next

        ' Add baud rate options
        ComboBoxBaudRate.Items.AddRange({"9600", "19200", "38400", "57600", "115200"})

        ' Set default baud rate selection if items are available
        If ComboBoxBaudRate.Items.Count > 0 Then
            ComboBoxBaudRate.SelectedIndex = 0
        End If
    End Sub

    Private Sub ButtonStart_Click(sender As Object, e As EventArgs) Handles ButtonStart.Click
        ' Initialize SerialPort
        serialPort = New SerialPort(ComboBoxCOMPort.SelectedItem.ToString(), CInt(ComboBoxBaudRate.SelectedItem))
        serialPort.Open()

        ' Disable controls
        ComboBoxCOMPort.Enabled = False
        ComboBoxBaudRate.Enabled = False
        ButtonStart.Enabled = False
        ButtonStop.Enabled = True

        ' Start reading thread
        readingThread = New Thread(AddressOf ReadData)
        readingThread.Start()
    End Sub

    Private Sub ButtonStop_Click(sender As Object, e As EventArgs) Handles ButtonStop.Click
        ' Close serial port
        serialPort.Close()

        ' Enable controls
        ComboBoxCOMPort.Enabled = True
        ComboBoxBaudRate.Enabled = True
        ButtonStart.Enabled = True
        ButtonStop.Enabled = False

        ' Clear rich text boxes
        RichTextBoxInput.Clear()
        RichTextBoxOutput.Clear()

        ' Stop reading thread
        If readingThread IsNot Nothing Then
            readingThread.Abort()
        End If
    End Sub

    Private Sub ButtonWrite_Click(sender As Object, e As EventArgs) Handles ButtonWrite.Click
        ' Transmit data
        serialPort.WriteLine(RichTextBoxInput.Text)
    End Sub

    Private Sub ReadData()
        While serialPort.IsOpen
            Try
                ' Read data
                Dim data As String = serialPort.ReadExisting

                ' Display received data
                Invoke(Sub() RichTextBoxOutput.AppendText(data & vbCrLf))
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End While
    End Sub

    Private Sub ButtonExit_Click(sender As Object, e As EventArgs) Handles ButtonExit.Click
        Close()
    End Sub
End Class