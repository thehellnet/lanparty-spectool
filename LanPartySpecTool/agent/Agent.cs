using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LanPartySpecTool.config;
using LanPartySpecTool.protocol;
using log4net;

namespace LanPartySpecTool.agent
{
    public class Agent
    {
        public delegate void AgentStartHandler();

        public delegate void AgentStopHandler();

        public event AgentStartHandler OnAgentStart;
        public event AgentStopHandler OnAgentStop;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Agent));

        private readonly object _sync = new object();

        private readonly Configuration _configuration;

        private Socket _socket;
        private Thread _socketThread;
        private bool _socketKeepRunning;

        private ulong _id;
        private readonly Dictionary<ulong, SocketClient> _clients = new Dictionary<ulong, SocketClient>();

        private Executor _executor;

        public Agent(Configuration configuration)
        {
            _configuration = configuration;
            _executor = new Executor(_configuration);
        }

        public void Start()
        {
            lock (_sync)
            {
                StartAgent();
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                StopAgent();
            }
        }

        public void Restart()
        {
            lock (_sync)
            {
                StopAgent();
                StartAgent();
            }
        }

        private void StartAgent()
        {
            Logger.Info("Agent start");

            _id = 0;

            try
            {
                StartSocket();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            OnAgentStart?.Invoke();

            Logger.Debug("Agent start sequence completed");
        }

        private void StopAgent()
        {
            Logger.Info("Agent stop");

            StopSocket();

            OnAgentStop?.Invoke();

            Logger.Debug("Agent stop sequence completed");
        }

        private void StartSocket()
        {
            if (_socket != null)
            {
                return;
            }

            Logger.Debug($"Starting socket on port {_configuration.Port}");

            var ipEndPoint = new IPEndPoint(IPAddress.Any, _configuration.Port);

            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            _socket.Bind(ipEndPoint);
            _socket.Listen(128);

            _socketKeepRunning = true;

            _socketThread = new Thread(SocketLoop);
            _socketThread.Start();
        }

        private void SocketLoop()
        {
            while (_socketKeepRunning)
            {
                Logger.Debug("Socket waiting for connections");

                var newSocket = _socket.Accept();
                CreateClient(_id, newSocket);

                _id++;
            }
        }

        private void StopSocket()
        {
            if (_socket == null)
            {
                return;
            }

            Logger.Debug("Stopping socket");

            _socketKeepRunning = false;

            try
            {
                _socketThread.Join();
            }
            catch (Exception)
            {
                // ignored
            }

            _socketThread = null;

            foreach (var clientId in _clients.Keys)
            {
                DestroyClient(clientId);
            }

            try
            {
                _socket.Close();
            }
            catch (Exception)
            {
                // ignored
            }

            _socket = null;
        }

        private void CreateClient(ulong clientId, Socket newSocket)
        {
            var socketClient = new SocketClient(clientId, newSocket);
            socketClient.OnNewCommand += HandleNewCommand;
            socketClient.OnClientStop += HandleOnClientStop;
            _clients.Add(_id, socketClient);
            socketClient.Start();
        }

        private void DestroyClient(ulong clientId)
        {
            if (!_clients.ContainsKey(clientId))
            {
                return;
            }

            var clientSocket = _clients[clientId];
            clientSocket.OnNewCommand -= HandleNewCommand;
            clientSocket.Stop();
            clientSocket.Join();
            _clients.Remove(clientId);
        }

        private void HandleNewCommand(ulong clientId, Command command)
        {
            Logger.Info($"Received command from client {clientId}");
            _executor.ExecuteCommand(clientId, command);
        }

        private void HandleOnClientStop(ulong clientId)
        {
            Task.Run(() => { DestroyClient(clientId); });
        }
    }
}