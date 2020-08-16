using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using LanPartySpecTool.config;
using LanPartySpecTool.protocol;
using log4net;

namespace LanPartySpecTool.agent
{
    public class Executor
    {
        private const int KeypressWaitTimeout = 1500;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Executor));

        private readonly Configuration _configuration;
        private readonly InputSimulator _inputSimulator;

        public Executor(Configuration configuration)
        {
            _configuration = configuration;
            _inputSimulator = new InputSimulator();
        }

        public void ExecuteCommand(ulong clientId, Command command)
        {
            switch (command.Action)
            {
                case CommandAction.JoinSpectate:
                    JoinSpectate();
                    break;

                case CommandAction.SetReady:
                    SetReady();
                    break;

                case CommandAction.NextPlayer:
                    NextPlayer();
                    break;

                default:
                    Logger.Error($"Command not implemented: {command.Action}");
                    break;
            }
        }

        private void JoinSpectate()
        {
            Logger.Info("Join Spectate");

            KeyPress(VirtualKeyCode.VK_4);
            Thread.Sleep(KeypressWaitTimeout);
            KeyPress(VirtualKeyCode.ESCAPE);
        }

        private void SetReady()
        {
            Logger.Info("Set Ready");

            KeyPress(VirtualKeyCode.VK_F);
        }

        private void NextPlayer()
        {
            Logger.Info("Next Player");

            MouseLeftButtonClick();
        }

        private void KeyPress(VirtualKeyCode virtualKeyCode)
        {
            _inputSimulator.Keyboard.KeyPress(virtualKeyCode);
        }

        private void MouseLeftButtonClick()
        {
            _inputSimulator.Mouse.LeftButtonClick();
        }
    }
}