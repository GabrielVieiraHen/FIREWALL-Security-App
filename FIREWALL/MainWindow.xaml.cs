using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
// Removida a diretiva 'using System.IO;' para evitar ambiguidade. Usaremos o nome completo 'System.IO.Path'.
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes; // Este é o 'Path' da interface
using System.Windows.Threading;

namespace FIREWALL
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _scanTimer;
        private int _currentPercentage = 0;

        public ObservableCollection<string> LogMessages { get; set; }
        private System.IO.FileSystemWatcher logWatcher; // Usando o nome completo
        private readonly string logPath = @"C:\temp\firewall_log.txt";
        private long lastReadPosition = 0;

        public MainWindow()
        {
            InitializeComponent();
            SetupTimer();
            LogMessages = new ObservableCollection<string>();
            LogListView.ItemsSource = LogMessages;
            SetupLogWatcher();
        }

        #region Log Real-time Display Logic
        private void SetupLogWatcher()
        {
            try
            {
                // Usando o nome completo para evitar conflito
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));
                if (!System.IO.File.Exists(logPath))
                {
                    System.IO.File.Create(logPath).Close();
                }

                ReadNewLogEntries();

                logWatcher = new System.IO.FileSystemWatcher
                {
                    Path = System.IO.Path.GetDirectoryName(logPath),
                    Filter = System.IO.Path.GetFileName(logPath),
                    NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size
                };

                logWatcher.Changed += OnLogFileChanged;
                logWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                LogMessages.Add($"Erro ao iniciar monitorização de logs: {ex.Message}");
            }
        }

        private void OnLogFileChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(ReadNewLogEntries);
        }

        private void ReadNewLogEntries()
        {
            try
            {
                using (var fs = new System.IO.FileStream(logPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                using (var sr = new System.IO.StreamReader(fs))
                {
                    fs.Seek(lastReadPosition, System.IO.SeekOrigin.Begin);
                    string newLine;
                    while ((newLine = sr.ReadLine()) != null)
                    {
                        LogMessages.Add(newLine);
                    }
                    lastReadPosition = fs.Position;
                }

                if (LogListView.Items.Count > 0)
                {
                    LogListView.ScrollIntoView(LogListView.Items[LogListView.Items.Count - 1]);
                }
            }
            catch { }
        }
        #endregion

        #region Scan Logic
        // ... O resto do código (SetupTimer, ScanTimer_Tick, UpdateProgressRing, StartScanButton_Click) não muda ...
        private void SetupTimer()
        {
            _scanTimer = new DispatcherTimer();
            _scanTimer.Interval = TimeSpan.FromMilliseconds(50);
            _scanTimer.Tick += ScanTimer_Tick;
        }

        private void ScanTimer_Tick(object sender, EventArgs e)
        {
            _currentPercentage++;
            PercentageText.Text = $"{_currentPercentage}%";
            UpdateProgressRing(_currentPercentage);

            if (_currentPercentage >= 100)
            {
                _scanTimer.Stop();
                StatusText.Text = "Scan Complete!";
                StatusText.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
        }

        private void UpdateProgressRing(double percentage)
        {
            if (percentage < 0) percentage = 0; if (percentage > 100) percentage = 100;
            double angle = (percentage / 100) * 359.99;
            double radius = 200; double thickness = 15; double effectiveRadius = radius - thickness / 2;
            double startX = radius; double startY = thickness / 2;
            Point center = new Point(radius, radius);
            double angleRad = (Math.PI / 180.0) * (angle - 90);
            double endX = center.X + effectiveRadius * Math.Cos(angleRad); double endY = center.Y + effectiveRadius * Math.Sin(angleRad);
            Point endPoint = new Point(endX, endY);
            Size size = new Size(effectiveRadius, effectiveRadius);
            bool isLargeArc = angle > 180;
            PathFigure figure = new PathFigure { StartPoint = new Point(startX, startY), IsClosed = false };
            ArcSegment segment = new ArcSegment { Point = endPoint, Size = size, IsLargeArc = isLargeArc, SweepDirection = SweepDirection.Clockwise };
            figure.Segments.Add(segment);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            // Aqui o 'ProgressRingPath' se refere ao elemento XAML, então não há conflito.
            ProgressRingPath.Data = geometry;
        }

        private void StartScanButton_Click(object sender, RoutedEventArgs e)
        {
            if (_scanTimer.IsEnabled) return;
            _currentPercentage = 0;
            StatusText.Text = "Scanning...";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xCF, 0xD8, 0xDC));
            _scanTimer.Start();
        }
        #endregion

        #region Window Controls
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
        private void CloseWindow_Click(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
        private void MaximizeRestoreWindow_Click(object sender, RoutedEventArgs e) { if (this.WindowState == WindowState.Normal) { this.WindowState = WindowState.Maximized; MaximizeButton.Content = "\uE923"; } else { this.WindowState = WindowState.Normal; MaximizeButton.Content = "\uE922"; } }
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e) { this.WindowState = WindowState.Minimized; }
        private void GithubButton_Click(object sender, RoutedEventArgs e) { string url = "https://github.com/GabrielVieiraHen"; try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch (Exception ex) { MessageBox.Show($"Could not open link: {ex.Message}"); } }
        #endregion
    }
}