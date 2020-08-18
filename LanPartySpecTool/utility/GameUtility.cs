using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using log4net;
using Microsoft.Win32;

namespace LanPartySpecTool.utility
{
    internal static class GameUtility
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameUtility));

        public static string DefaultGameExe()
        {
            var installPath = DefaultInstallationDir();
            if (installPath == "") return "";

            var gameExePath = Path.Combine(installPath, Constants.GameExeFilename);
            return File.Exists(gameExePath) ? gameExePath : "";
        }

        private static string DefaultInstallationDir()
        {
            foreach (var registryPath in Constants.Cod4RegistryPaths)
            {
                var installPath = (string) Registry.GetValue(registryPath, "InstallPath", null);
                if (installPath != null && Directory.Exists(installPath)) return installPath;
            }

            return "";
        }

        public static string ReadCodKey()
        {
            foreach (var registryPath in Constants.Cod4RegistryPaths)
            {
                var codKey = (string) Registry.GetValue(registryPath, "codkey", null);
                if (codKey != null && codKey.Length == 20) return codKey.ToUpper();
            }

            return "";
        }

        public static string FormatCodKey(string codKey)
        {
            var items = new List<string>();
            for (var i = 0; i < codKey.Length; i += 4) items.Add(codKey.Substring(i, 4));
            return string.Join("-", items);
        }

        public static void LaunchGameClient(string gameExePath, string serverAddress, ushort serverPort)
        {
            IPAddress[] addresses;

            try
            {
                addresses = Dns.GetHostAddresses(serverAddress);
            }
            catch (SocketException)
            {
                Logger.Error($"Unable to resolve {serverAddress}");
                return;
            }

            var realServerAddress = addresses[0].ToString();

            var arguments = $" +connect {realServerAddress}:{serverPort}";

            var directoryInfo = new FileInfo(gameExePath).Directory;
            if (directoryInfo == null) return;

            var workingDirectory = directoryInfo.FullName;

            var process = new ProcessStartInfo(gameExePath)
            {
                WorkingDirectory = workingDirectory,
                Arguments = arguments,
                UseShellExecute = true
            };

            Process.Start(process);
        }
    }
}