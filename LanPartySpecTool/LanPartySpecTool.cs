using System;
using System.Windows;
using LanPartySpecTool.agent;
using LanPartySpecTool.config;
using LanPartySpecTool.windows;
using log4net;

namespace LanPartySpecTool
{
    public class LanPartySpecTool : Application
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LanPartySpecTool));

        private Configuration _configuration;
        private ConfigurationManager _configurationManager;

        private Agent _agent;

        private MainWindow _mainWindow;

        [STAThread]
        public static void Main()
        {
            var lanPartySpecTool = new LanPartySpecTool();

            try
            {
                lanPartySpecTool.Run();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Logger.Info("Application START");

            _configuration = new Configuration();
            _configurationManager = new ConfigurationManager(_configuration);

            _agent = new Agent(_configuration);

            _mainWindow = new MainWindow(_configuration);

            _configurationManager.Load();
            _configurationManager.Init();

            _configuration.PropertyChanged += (sender, args) => _configurationManager.Save();

            _configurationManager.Save();

            _agent.OnAgentStart += OnAgentStart;
            _agent.OnAgentStop += OnAgentStop;

            _agent.Start();
            _mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mainWindow.Close();
            _agent.Stop();

            Logger.Info("Application END");

            base.OnExit(e);
        }

        private void OnAgentStart()
        {
            _mainWindow.OnAgentStart();
        }

        private void OnAgentStop()
        {
            _mainWindow.OnAgentStop();
        }
    }
}