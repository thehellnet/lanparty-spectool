using System;
using System.Windows;
using LanPartySpecTool.windows;
using log4net;

namespace LanPartySpecTool
{
    public class LanPartySpecTool : Application
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LanPartySpecTool));

        private readonly MainWindow _mainWindow = new MainWindow();

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

            _mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mainWindow.Close();

            base.OnExit(e);
        }
    }
}