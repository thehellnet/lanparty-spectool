using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using LanPartySpecTool.config;
using log4net.Core;

namespace LanPartySpecTool.windows
{
    public partial class MainWindow
    {
        private const int MaxLogLines = 100;

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

            SocketPortValue.Text = _configuration.Port.ToString();
            SocketPortValue.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath("Port"),
                Source = _configuration,
                Mode = BindingMode.OneWay
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
            AgentStatusValue.Dispatcher.Invoke(() => AgentStatusValue.Text = status ? "Running" : "Stop");
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

            LogText.Dispatcher.Invoke(() => UpdateLogText(newLogList));
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
    }
}