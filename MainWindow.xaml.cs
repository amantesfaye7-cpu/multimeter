using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MultimeterDisplay
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private Thread readThread;
        private bool isRunning = false;
        private bool isLogging = false;
        private List<MeasurementData> measurementHistory = new List<MeasurementData>();
        private DispatcherTimer updateTimer;
        private DispatcherTimer elapsedTimer;
        private DateTime sessionStartTime;
        private double minValue = double.MaxValue;
        private double maxValue = double.MinValue;
        private double sumValues = 0;
        private int sampleCount = 0;
        private object lockObject = new object();

        public class MeasurementData
        {
            public DateTime Timestamp { get; set; }
            public double Value { get; set; }
            public string Unit { get; set; }
            public string Mode { get; set; }
            public string Range { get; set; }
            public string RawData { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadComPorts();
            InitializeTimers();
            SetupBaudRateCombo();
        }

        private void InitializeTimers()
        {
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            updateTimer.Tick += UpdateTimer_Tick;

            elapsedTimer = new DispatcherTimer();
            elapsedTimer.Interval = TimeSpan.FromSeconds(1);
            elapsedTimer.Tick += ElapsedTimer_Tick;
        }

        private void SetupBaudRateCombo()
        {
            BaudRateComboBox.SelectedItem = "19200";
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
                    UpdateStatus($"Found {ports.Length} COM port(s)", System.Windows.Media.Brushes.LimeGreen);
                }
                else
                {
                    UpdateStatus("No COM ports found", System.Windows.Media.Brushes.Orange);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error reading COM ports: {ex.Message}", System.Windows.Media.Brushes.Red);
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
                    UpdateStatus("Please select a COM port first", System.Windows.Media.Brushes.Red);
                    return;
                }

                string port = PortComboBox.SelectedItem.ToString();
                int baudRate = int.Parse(BaudRateComboBox.SelectedItem.ToString());

                serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;
                serialPort.Open();

                isRunning = true;
                readThread = new Thread(ReadMultimeter) { IsBackground = true };
                readThread.Start();

                sessionStartTime = DateTime.Now;
                elapsedTimer.Start();
                updateTimer.Start();

                UpdateStatus($"Connected to {port} @ {baudRate} baud", System.Windows.Media.Brushes.Lime);
                ConnectionIndicator.Text = "Connected";
                ConnectionIndicator.Foreground = System.Windows.Media.Brushes.Lime;

                ConnectBtn.IsEnabled = false;
                DisconnectBtn.IsEnabled = true;
                PortComboBox.IsEnabled = false;
                RefreshBtn.IsEnabled = false;
                StartLoggingBtn.IsEnabled = true;
                ExportBtn.IsEnabled = true;
                ClearBtn.IsEnabled = true;
                BaudRateComboBox.IsEnabled = false;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Connection failed: {ex.Message}", System.Windows.Media.Brushes.Red);
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
                            string valueStr = line.Substring(0, 8).Trim();      // Value part
                            string unit = line.Substring(8, 3).Trim();          // Unit part
                            string mode = line.Substring(11).Trim();            // Mode part

                            if (double.TryParse(valueStr, out double value))
                            {
                                lock (lockObject)
                                {
                                    // Update statistics
                                    if (value < minValue) minValue = value;
                                    if (value > maxValue) maxValue = value;
                                    sumValues += Math.Abs(value);
                                    sampleCount++;

                                    // Store measurement
                                    var measurement = new MeasurementData
                                    {
                                        Timestamp = DateTime.Now,
                                        Value = value,
                                        Unit = unit,
                                        Mode = mode,
                                        RawData = line
                                    };

                                    measurementHistory.Add(measurement);

                                    // Log if enabled
                                    if (isLogging && measurementHistory.Count > 0)
                                    {
                                        // Auto-export every 100 samples if enabled
                                        if (AutoExportCheckBox.IsChecked == true && sampleCount % 100 == 0)
                                        {
                                            AutoExport();
                                        }
                                    }

                                    // Update UI
                                    Dispatcher.Invoke(() =>
                                    {
                                        ReadingText.Text = valueStr;
                                        UnitText.Text = unit;
                                        ModeText.Text = mode;
                                        UpdateStats();
                                        UpdateRecentReadings();
                                    });
                                }
                            }
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
                        UpdateStatus($"Read error: {ex.Message}", System.Windows.Media.Brushes.Red);
                    });

                    if (AutoReconnectCheckBox.IsChecked == true && isRunning)
                    {
                        Thread.Sleep(2000);
                        // Auto-reconnect logic could be added here
                    }
                    else
                    {
                        isRunning = false;
                    }
                }
            }
        }

        private void UpdateStats()
        {
            if (sampleCount > 0)
            {
                double avgValue = sumValues / sampleCount;
                SampleCountText.Text = $"Samples: {sampleCount}";
                MinValueText.Text = minValue == double.MaxValue ? "---" : minValue.ToString("F4");
                MaxValueText.Text = maxValue == double.MinValue ? "---" : maxValue.ToString("F4");
                AvgValueText.Text = avgValue.ToString("F4");
            }
        }

        private void UpdateRecentReadings()
        {
            ReadingsList.Items.Clear();
            var recentReadings = measurementHistory.Skip(Math.Max(0, measurementHistory.Count - 10)).ToList();
            foreach (var reading in recentReadings)
            {
                string displayText = $"{reading.Timestamp:HH:mm:ss.fff} | {reading.Value,10:F4} {reading.Unit,-4} {reading.Mode}";
                ReadingsList.Items.Add(displayText);
            }
            ReadingsList.ScrollIntoView(ReadingsList.Items[ReadingsList.Items.Count - 1]);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (int.TryParse(RefreshRateTextBox.Text, out int refreshRate))
            {
                updateTimer.Interval = TimeSpan.FromMilliseconds(refreshRate);
            }
        }

        private void ElapsedTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - sessionStartTime;
            ElapsedTimeText.Text = elapsed.ToString(@"hh\:mm\:ss");
        }

        private void StartLogging_Click(object sender, RoutedEventArgs e)
        {
            isLogging = true;
            lock (lockObject)
            {
                sampleCount = 0;
                minValue = double.MaxValue;
                maxValue = double.MinValue;
                sumValues = 0;
                measurementHistory.Clear();
            }
            StartLoggingBtn.IsEnabled = false;
            StopLoggingBtn.IsEnabled = true;
            RecordingStatusText.Text = "Recording...";
            RecordingStatusText.Foreground = System.Windows.Media.Brushes.Red;
            UpdateStatus("Data logging started", System.Windows.Media.Brushes.Lime);
        }

        private void StopLogging_Click(object sender, RoutedEventArgs e)
        {
            isLogging = false;
            StartLoggingBtn.IsEnabled = true;
            StopLoggingBtn.IsEnabled = false;
            RecordingStatusText.Text = $"Stopped - {sampleCount} samples recorded";
            RecordingStatusText.Foreground = System.Windows.Media.Brushes.Orange;
            UpdateStatus("Data logging stopped", System.Windows.Media.Brushes.Orange);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lock (lockObject)
                {
                    if (measurementHistory.Count == 0)
                    {
                        UpdateStatus("No data to export", System.Windows.Media.Brushes.Orange);
                        return;
                    }

                    string fileName = $"multimeter_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = System.IO.Path.Combine(desktopPath, fileName);

                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        // Write header
                        writer.WriteLine("Timestamp,Value,Unit,Mode,Raw Data");

                        // Write data
                        foreach (var measurement in measurementHistory)
                        {
                            writer.WriteLine($"{measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff},{measurement.Value:F6},{measurement.Unit},{measurement.Mode},{measurement.RawData}");
                        }
                    }

                    UpdateStatus($"Data exported to {fileName}", System.Windows.Media.Brushes.LimeGreen);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Export failed: {ex.Message}", System.Windows.Media.Brushes.Red);
            }
        }

        private void AutoExport()
        {
            try
            {
                string fileName = $"multimeter_auto_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = System.IO.Path.Combine(desktopPath, fileName);

                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("Timestamp,Value,Unit,Mode,Raw Data");
                    foreach (var measurement in measurementHistory)
                    {
                        writer.WriteLine($"{measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff},{measurement.Value:F6},{measurement.Unit},{measurement.Mode},{measurement.RawData}");
                    }
                }
            }
            catch
            {
                // Silently fail on auto-export
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            lock (lockObject)
            {
                measurementHistory.Clear();
                sampleCount = 0;
                minValue = double.MaxValue;
                maxValue = double.MinValue;
                sumValues = 0;
            }
            ReadingsList.Items.Clear();
            SampleCountText.Text = "Samples: 0";
            MinValueText.Text = "---";
            MaxValueText.Text = "---";
            AvgValueText.Text = "---";
            UpdateStatus("Data cleared", System.Windows.Media.Brushes.LimeGreen);
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            isLogging = false;
            elapsedTimer.Stop();
            updateTimer.Stop();

            Thread.Sleep(100);

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }

            UpdateStatus("Disconnected", System.Windows.Media.Brushes.Orange);
            ReadingText.Text = "Waiting for connection...";
            ConnectionIndicator.Text = "Disconnected";
            ConnectionIndicator.Foreground = System.Windows.Media.Brushes.Orange;

            ConnectBtn.IsEnabled = true;
            DisconnectBtn.IsEnabled = false;
            PortComboBox.IsEnabled = true;
            RefreshBtn.IsEnabled = true;
            StartLoggingBtn.IsEnabled = false;
            StopLoggingBtn.IsEnabled = false;
            ExportBtn.IsEnabled = false;
            ClearBtn.IsEnabled = false;
            BaudRateComboBox.IsEnabled = true;

            StartLoggingBtn.IsEnabled = false;
            StopLoggingBtn.IsEnabled = false;
            RecordingStatusText.Text = "Not recording";
            RecordingStatusText.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void UpdateStatus(string message, System.Windows.Media.SolidColorBrush color)
        {
            StatusText.Text = message;
            StatusText.Foreground = color;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            isRunning = false;
            isLogging = false;

            if (updateTimer != null)
                updateTimer.Stop();

            if (elapsedTimer != null)
                elapsedTimer.Stop();

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }
    }
}
