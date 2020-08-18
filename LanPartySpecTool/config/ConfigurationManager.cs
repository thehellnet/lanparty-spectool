using System.IO;
using LanPartySpecTool.utility;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanPartySpecTool.config
{
    public class ConfigurationManager
    {
        private const string ConfigFileName = "config.json";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigurationManager));

        private readonly Configuration _configuration;

        public ConfigurationManager(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Load()
        {
            Logger.Info("Loading configuration to file");

            if (!File.Exists(ConfigFileName)) return;

            var jsonString = File.ReadAllText(ConfigFileName);
            var json = JObject.Parse(jsonString);

            _configuration.SocketPort = (json.GetValue("SocketPort") ?? Defaults.SocketPort).Value<ushort>();
            _configuration.ServerAddress = (json.GetValue("ServerAddress") ?? Defaults.ServerAddress).Value<string>();
            _configuration.ServerPort = (json.GetValue("ServerPort") ?? Defaults.ServerPort).Value<ushort>();
            _configuration.GameExe = (json.GetValue("GameExe") ?? Defaults.GameExe).Value<string>();
        }

        public void Save()
        {
            Logger.Info("Saving configuration from file");

            var json = new
            {
                _configuration.SocketPort,
                _configuration.ServerAddress,
                _configuration.ServerPort,
                _configuration.GameExe
            };

            var jsonString = JsonConvert.SerializeObject(json, (Formatting) System.Xml.Formatting.Indented);
            File.WriteAllText(ConfigFileName, jsonString);
        }

        public void Init()
        {
            Logger.Info("Initializing configuration");

            if (_configuration.GameExe == "")
            {
                Logger.Debug("Setting default Game EXE");
                _configuration.GameExe = GameUtility.DefaultGameExe();
            }
        }
    }
}