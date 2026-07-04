using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace MultimeterDisplay
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private Thread readThread;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadComPorts();
        }

        private void LoadComPorts()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                PortComboBox.Items.Clear();
                
                if (ports.Length > 0)
                {
                    foreach (string port in ports)
                    {
                        PortComboBox.Items.Add(port);
                    }
                    PortComboBox.SelectedIndex = 0;
                    StatusText.Text = $"Found {ports.Length} COM port(s)";
                    StatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
                }
                else
                {
                    StatusText.Text = "No COM ports found";
                    StatusText.Foreground = System.Windows.Media.Brushes.Orange;
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error reading COM ports: {ex.Message}";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadComPorts();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PortComboBox.SelectedItem == null)
                {
                    StatusText.Text = "Please select a COM port first";
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                    return;
                }

                string port = PortComboBox.SelectedItem.ToString();
                serialPort = new SerialPort(port, 19200, Parity.None, 8, StopBits.One);
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;
                serialPort.Open();
                
                isRunning = true;
                readThread = new Thread(ReadMultimeter) { IsBackground = true };
                readThread.Start();
                
                StatusText.Text = $"Connected to {port}";
                StatusText.Foreground = System.Windows.Media.Brushes.Lime;
                ConnectBtn.IsEnabled = false;
                DisconnectBtn.IsEnabled = true;
                PortComboBox.IsEnabled = false;
                RefreshBtn.IsEnabled = false;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Connection failed: {ex.Message}";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
                ReadingText.Text = "Error";
            }
        }

        private void ReadMultimeter()
        {
            while (isRunning)
            {
                try
                {
                    if (serialPort != null && serialPort.IsOpen)
                    {
                        string line = serialPort.ReadLine().Trim();
                        if (line.Length >= 13)
                        {
                            // Parse the 14-byte packet
                            // Format: +/-XXXXX.XX UNIT AC/DC
                            string reading = line.Substring(0, 8).Trim();      // Value part (e.g., "+00.0000")
                            string unit = line.Substring(8, 3).Trim();         // Unit part (e.g., "V", "mA", "Ω")
                            string mode = line.Substring(11, 2).Trim();        // Mode part (e.g., "DC", "AC")
                            
                            string display = string.IsNullOrEmpty(mode) 
                                ? $"{reading} {unit}" 
                                : $"{reading} {unit} {mode}";
                            
                            Dispatcher.Invoke(() => ReadingText.Text = display);
                        }
                    }
                }
                catch (TimeoutException)
                {
                    // Timeout is normal, just continue
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => 
                    {
                        StatusText.Text = $"Read error: {ex.Message}";
                        StatusText.Foreground = System.Windows.Media.Brushes.Red;
                    });
                    isRunning = false;
                }
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            Thread.Sleep(100); // Give read thread time to stop
            
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
            
            StatusText.Text = "Disconnected";
            StatusText.Foreground = System.Windows.Media.Brushes.Orange;
            ReadingText.Text = "Waiting for connection...";
            
            ConnectBtn.IsEnabled = true;
            DisconnectBtn.IsEnabled = false;
            PortComboBox.IsEnabled = true;
            RefreshBtn.IsEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            isRunning = false;
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }
    }
}