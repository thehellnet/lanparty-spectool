using System;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanPartySpecTool.protocol
{
    public static class CommandParser
    {
        public static Command Parse(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
            {
                return null;
            }

            JObject rawCommand;

            try
            {
                rawCommand = JObject.Parse(commandString);
            }
            catch (JsonReaderException)
            {
                return null;
            }

            var actionString = rawCommand.GetValue("action")?.ToString();
            if (actionString == null)
            {
                return null;
            }

            CommandAction action;

            try
            {
                action = (CommandAction) Enum.Parse(typeof(CommandAction), actionString);
            }
            catch (ArgumentException)
            {
                return null;
            }

            var command = new Command(action);
            return command;
        }
    }
}