using System;
using System.Net.Sockets;
using LanPartySpecTool.protocol;
using LanPartySpecTool.utility;
using log4net;
using Newtonsoft.Json.Linq;

namespace LanPartySpecTool.agent
{
    public class SocketClient : StoppableThread
    {
        public delegate void NewCommandHandler(ulong clientId, Command command);

        public event NewCommandHandler OnNewCommand;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SocketClient));

        private readonly ulong _id;
        private readonly Socket _socket;

        public SocketClient(ulong id, Socket socket)
        {
            _id = id;
            _socket = socket;
        }

        public override void Start()
        {
            Logger.Info($"Starting socket client {_id}");

            base.Start();
        }

        public override void Stop()
        {
            Logger.Info($"Stop socket client {_id}");

            _socket.Close();

            base.Stop();
        }

        protected override void Job()
        {
            var rawData = ReadDataFromSocket();
            var command = ParseCommand(rawData);
            InvokeNewCommandEvent(command);
        }

        private string ReadDataFromSocket()
        {
            Logger.Debug($"Reading data from socket {_id}");

            var buffer = new byte[8192];
            var bytesRead = _socket.Receive(buffer);
            var rawData = BitConverter.ToString(buffer, 0, bytesRead);
            return rawData;
        }

        private static Command ParseCommand(string rawData)
        {
            Logger.Debug($"Parsing command");

            var rawCommand = JObject.Parse(rawData);

            var actionString = rawCommand.GetValue("action")?.ToString();
            if (actionString == null)
            {
                return null;
            }

            var action = (CommandAction) Enum.Parse(typeof(CommandAction), actionString);

            var command = new Command(action);
            return command;
        }

        private void InvokeNewCommandEvent(Command command)
        {
            if (command == null)
            {
                return;
            }

            Logger.Debug($"Invoking new command event for client {_id}");
            OnNewCommand?.Invoke(_id, command);
        }
    }
}