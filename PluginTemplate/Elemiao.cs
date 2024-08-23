using Lotlab.PluginCommon;
using Lotlab.PluginCommon.FFXIV.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatPlugin
{
    class Elemiao : PropertyNotifier
    {
        public Config Config { get; }

        public SimpleLogger Logger { get; }
        NetworkParser parser { get; } = new NetworkParser();

        Action<string> logInAct { get; }

        bool overlayInited = false;
        public bool OverlayInited
        {
            get => overlayInited;
            set
            {
                overlayInited = value;
                OnPropertyChanged();
            }
        }
        public string DataPath { get; }

        public string AsmDir { get; }
        public Elemiao(Action<string> actLogFunc, string asmDir, string appDataPath) { 
            logInAct = actLogFunc;
            DataPath = appDataPath;
            AsmDir = asmDir;

            Logger = new SimpleLoggerSync(Path.Combine(DataPath, "app.log"));
            try
            {
                Config = new Config(Path.Combine(DataPath, "config.json"));
                Config.Load();
            }
            catch (Exception e)
            {
                Logger.LogError("配置加载失败", e);
            }

            Logger.SetFilter(Config.LogLevel);
        }

        public void NetworkReceive(byte[] message)
        {
            try
            {
                var packet = parser.ParsePacket(message);
                if (packet != null)
                {
                    Logger.LogDebug($"NetworkReceived: {packet}");
                }
            }
            catch (Exception e)
            {
                Logger.LogError("NetworkReceived: {0}", e);
            }
        }   
}
}
