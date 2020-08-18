using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using LanPartySpecTool.config;
using LanPartySpecTool.utility;
using log4net;
using log4net.Core;
using Microsoft.Win32;

namespace LanPartySpecTool.windows
{
    public partial class MainWindow
    {
        private const int MaxLogLines = 100;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));

        private readonly Configuration _configuration;
        private readonly DispatcherTimer _clockTimer = new DispatcherTimer();
        private readonly List<dynamic> _logList = new List<dynamic>();

        public MainWindow(Configuration configuration)
        {
            _configuration = configuration;

            InitializeComponent();

            InitUi();

            UpdateClock();
        }

        public void OnAgentStart()
        {
            UpdateAgentStatus(true);
        }

        public void OnAgentStop()
        {
            UpdateAgentStatus(false);
        }

        private void InitUi()
        {
            Title = $"{Application.ResourceAssembly.GetName().Name} {Application.ResourceAssembly.GetName().Version}";

            _clockTimer.Tick += (sender, args) => UpdateClock();
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Start();

            SocketPort.Text = _configuration.SocketPort.ToString();

            ServerAddress.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath("ServerAddress"),
                Source = _configuration,
                Mode = BindingMode.TwoWay
            });

            ServerPort.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath("ServerPort"),
                Source = _configuration,
                Mode = BindingMode.TwoWay
            });

            GameExe.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath("GameExe"),
                Source = _configuration,
                Mode = BindingMode.TwoWay
            });

            UpdateAgentStatus(false);

            LogEvent.OnLogEvent += NewLogEvent;
        }

        private void UpdateClock()
        {
            ClockText.Text = $"{DateTime.Now}";
        }

        private void UpdateAgentStatus(bool status)
        {
            AgentStatus.Dispatcher.Invoke(() => AgentStatus.Text = status ? "Running" : "Stop");
        }

        private void NewLogEvent(Level level, string message)
        {
            var color = Brushes.Gray;
            var weight = FontWeights.Normal;
            var style = FontStyles.Italic;

            if (level == Level.Error)
            {
                color = Brushes.Red;
                weight = FontWeights.Bold;
                style = FontStyles.Normal;
            }
            else if (level == Level.Warn)
            {
                color = Brushes.Orange;
                weight = FontWeights.Normal;
                style = FontStyles.Normal;
            }
            else if (level == Level.Info)
            {
                color = Brushes.Black;
                weight = FontWeights.Normal;
                style = FontStyles.Normal;
            }

            dynamic logItem = new ExpandoObject();
            logItem.color = color;
            logItem.weight = weight;
            logItem.style = style;
            logItem.message = message.Trim();

            _logList.Add(logItem);
            while (_logList.Count > MaxLogLines) _logList.RemoveAt(0);
            var newLogList = _logList.ToArray();

            try
            {
                LogText.Dispatcher.Invoke(() => UpdateLogText(newLogList));
            }
            catch (TaskCanceledException)
            {
                //ignore
            }
        }

        private void UpdateLogText(IEnumerable<dynamic> newLogList)
        {
            var paragraph = new Paragraph();

            foreach (var log in newLogList)
            {
                var textBlock = new TextBlock
                {
                    FontFamily = new FontFamily("Courier New"),
                    Margin = new Thickness(0),
                    Foreground = log.color,
                    FontWeight = log.weight,
                    FontStyle = log.style,
                    Text = log.message
                };

                paragraph.Inlines.Add(textBlock);

                var lineBreak = new LineBreak();
                paragraph.Inlines.Add(lineBreak);
            }

            var document = new FlowDocument(paragraph);

            LogText.Document = document;
            LogText.ScrollToEnd();
        }

        private void StartGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                GameUtility.LaunchGameClient(
                    _configuration.GameExe,
                    _configuration.ServerAddress,
                    _configuration.ServerPort
                );
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void ShowCodKeyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var codKey = GameUtility.ReadCodKey();
            var message = $"Key configured in registry: {GameUtility.FormatCodKey(codKey)}";
            MessageBox.Show(message, "Configured key", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GameExeChooseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filter = "CoD4 MultiPlayer|iw3mp.exe|Executable (*.exe)|*.exe|All files (*.*)|*.*";
            var initialDirectory = Path.GetDirectoryName(_configuration.GameExe) ?? "";

            var dialog = new OpenFileDialog
            {
                Title = "Choose game EXE",
                Filter = filter,
                Multiselect = false,
                ValidateNames = true,
                InitialDirectory = initialDirectory,
                CheckFileExists = true
            };

            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            var fileName = dialog.FileName;
            Logger.Info($"Updating Game EXE: {fileName}");
            _configuration.GameExe = fileName;
        }
    }
}