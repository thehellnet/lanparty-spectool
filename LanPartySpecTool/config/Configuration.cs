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

        private const string ConfigFileName = "config.json";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Configuration));

        private ushort _port = Defaults.Port;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ushort Port
        {
            get => _port;
            set
            {
                if (_port == value) return;
                _port = value;
                OnPropertyChanged(nameof(Port));
                Save();
            }
        }

        public void Load()
        {
            Logger.Info("Loading configuration to file");

            if (!File.Exists(ConfigFileName)) return;

            var jsonString = File.ReadAllText(ConfigFileName);
            var json = JObject.Parse(jsonString);

            Port = (json.GetValue("port") ?? Defaults.Port).Value<ushort>();
        }

        public void Save()
        {
            Logger.Info("Saving configuration from file");

            var json = new
            {
                port = Port,
            };

            var jsonString = JsonConvert.SerializeObject(json, (Formatting) System.Xml.Formatting.Indented);
            File.WriteAllText(ConfigFileName, jsonString);
        }
    }
}