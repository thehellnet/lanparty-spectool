using System;
using System.Net.Sockets;
using LanPartySpecTool.protocol;
using LanPartySpecTool.utility;
using log4net;

namespace LanPartySpecTool.agent
{
    public class SocketClient : StoppableThread
    {
        public delegate void NewCommandHandler(ulong clientId, Command command);

        public delegate void ClientStopHandler(ulong clientId);

        public event NewCommandHandler OnNewCommand;
        public event ClientStopHandler OnClientStop;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SocketClient));

        private readonly ulong _id;
        private readonly Socket _socket;

        public SocketClient(ulong id, Socket socket)
        {
            _id = id;
            _socket = socket;

            _socket.ReceiveTimeout = 0;

            OnStop += () => OnClientStop?.Invoke(_id);
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
            if (rawData == null)
            {
                return;
            }

            var command = CommandParser.Parse(rawData);
            if (command == null)
            {
                Logger.Warn($"Invalid command: {rawData}");
                return;
            }

            Logger.Debug($"Invoking new command event for client {_id}");
            OnNewCommand?.Invoke(_id, command);
        }

        private string ReadDataFromSocket()
        {
            Logger.Debug($"Reading data from socket {_id}");

            var buffer = new byte[8192];

            int bytesRead;

            try
            {
                bytesRead = _socket.Receive(buffer);
            }
            catch (SocketException)
            {
                StopJob();
                return null;
            }

            var rawData = new char[bytesRead];
            Array.Copy(buffer, 0, rawData, 0, bytesRead);
            var data = new string(rawData);
            return data;
        }
    }
}