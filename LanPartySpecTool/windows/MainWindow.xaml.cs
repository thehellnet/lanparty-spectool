using System;
using System.Windows;
using System.Windows.Threading;

namespace LanPartySpecTool.windows
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _clockTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            InitUi();

            UpdateClock();
        }

        private void InitUi()
        {
            Title = $"{Application.ResourceAssembly.GetName().Name} {Application.ResourceAssembly.GetName().Version}";

            _clockTimer.Tick += (sender, args) => UpdateClock();
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Start();
        }

        private void UpdateClock()
        {
            ClockText.Text = $"{DateTime.Now}";
        }
    }
}