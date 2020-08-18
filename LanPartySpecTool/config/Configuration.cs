using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using LanPartySpecTool.Annotations;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanPartySpecTool.config
{
    public class Configuration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ushort _socketPort = Defaults.SocketPort;
        private string _serverAddress = Defaults.ServerAddress;
        private ushort _serverPort = Defaults.ServerPort;
        private string _gameExe = Defaults.GameExe;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ushort SocketPort
        {
            get => _socketPort;
            set
            {
                if (_socketPort == value) return;
                _socketPort = value;
                OnPropertyChanged(nameof(SocketPort));
            }
        }

        public string ServerAddress
        {
            get => _serverAddress;
            set
            {
                if (_serverAddress == null || _serverAddress == value) return;
                _serverAddress = value;
                OnPropertyChanged(nameof(ServerAddress));
            }
        }

        public ushort ServerPort
        {
            get => _serverPort;
            set
            {
                if (_serverPort == value) return;
                _serverPort = value;
                OnPropertyChanged(nameof(ServerPort));
            }
        }

        public string GameExe
        {
            get => _gameExe;
            set
            {
                if (_gameExe == null || _gameExe == value) return;
                _gameExe = value;
                OnPropertyChanged(nameof(GameExe));
            }
        }
    }
}