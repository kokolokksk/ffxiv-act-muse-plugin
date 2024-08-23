using Advanced_Combat_Tracker;
using CatPlugin;
using Lotlab.PluginCommon.FFXIV;
using Newtonsoft.Json.Linq;
using PluginTemplate;
using RainbowMage.OverlayPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CatTemplate
{
    public partial class PluginCatTemplate : IActPluginV1, IOverlayAddonV2
    {
        /// <summary>
        /// FFXIV 插件引用
        /// </summary>
        ACTPluginProxy ffxiv { get; set; } = null;

        /// <summary>
        /// 状态 Label
        /// </summary>
        Label statusLabel { get; set; } = null;

        Elemiao elemiao;

        /// <summary>
        /// ACT插件接口 - 初始化插件
        /// </summary>
        /// <remarks>
        /// 在这里初始化整个插件
        /// </remarks>
        /// <param name="pluginScreenSpace">插件所在的Tab页面</param>
        /// <param name="pluginStatusText">插件列表的状态标签</param>
        void IActPluginV1.InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            // 设置状态标签引用方便后续使用
            statusLabel = pluginStatusText;

            // 查找解析插件
            var plugins = ActGlobals.oFormActMain.ActPlugins;
            foreach (var item in plugins)
            {
                if (ACTPluginProxy.IsFFXIVPlugin(item.pluginObj))
                {
                    ffxiv = new ACTPluginProxy(item.pluginObj);
                    break;
                }
            }

            // 若没有找到，则直接退出
            if (ffxiv == null || !ffxiv.PluginStarted)
            {
                pluginStatusText.Text = "FFXIV ACT Plugin is not loaded.";
                return;
            }

            // 注册网络事件
            // Register events
            ffxiv.DataSubscription.NetworkSent += OnNetworkSend;
            ffxiv.DataSubscription.NetworkReceived += OnNetworkReceived;
            ffxiv.DataSubscription.LogLine += onLogLine;

            // 初始化UI
            var vm = new PluginControlViewModel();
            var control = new PluginControl();
            control.DataContext = vm;
            var host = new ElementHost()
            {
                Dock = DockStyle.Fill,
                Child = control
            };

            pluginScreenSpace.Text = "CatPlugin";
            pluginScreenSpace.Controls.Add(host);

            // 更新状态标签的内容
            statusLabel.Text = "CatPlugin Inited.";
            var appDataDir = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.Parent.FullName, "Elemiao");
            #if DEBUG
            appDataDir += "Debug";
            #endif
            prepareDir(appDataDir);

            elemiao = new Elemiao(actLog, ActGlobals.oFormActMain.AppDataFolder.FullName, appDataDir);
        }
        private void actLog(string str)
        {
            ActGlobals.oFormActMain.ParseRawLogLine(false, DateTime.Now, str);
        }
        void prepareDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        /// <summary>
        /// ACT插件接口 - 反初始化插件
        /// </summary>
        void IActPluginV1.DeInitPlugin()
        {
            // 反注册事件
            if (ffxiv != null)
            {
                ffxiv.DataSubscription.NetworkReceived -= OnNetworkReceived;
            }
            ffxiv = null;

            // 更新状态
            if (statusLabel != null)
            {
                statusLabel.Text = "Plugin Exit.";
            }
            statusLabel = null;
        }

        /// <summary>
        /// 收到网络数据包的回调
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="epoch"></param>
        /// <param name="message"></param>
        void OnNetworkReceived(string connection, long epoch, byte[] message)
        {
            elemiao.NetworkReceive(message);
        }

        private void onLogLine(uint EventType, uint Seconds, string logline)
        {
            if (EventType == 0x00)
            {
                
            }
        }
        private void OnNetworkSend(string connection, long epoch, byte[] message)
        {

        }

        public void Init()
        {

            var container = Registry.GetContainer();
            var registry = container.Resolve<Registry>();

            // 注册事件源
            var eventSource = new TemplateEventSource(container);
            //registry.StartEventSource(eventSource);
            var method = registry.GetType().GetMethod(nameof(registry.StartEventSource));
            if (method.IsGenericMethod)
                method = method.MakeGenericMethod(typeof(TemplateEventSource));

            method.Invoke(registry, new object[] { eventSource });
            // 注册悬浮窗预设
            /*registry.RegisterOverlayPreset2(new OverlayPreset
            {
                Name = "喵",
                Url = "http://localhost:5173/",
                Size = new int[] { 300, 500 },
                Locked = false,
            });*/
            registry.RegisterOverlayPreset2(new OverlayPreset
            {
                Name = "喵",
                Url = "http://localhost:5173/",
                Size = new int[] { 1600, 500 },
                Locked = false,
            });


        }

        /// <summary>
        /// 悬浮窗事件源
        /// </summary>
        public class TemplateEventSource : Lotlab.PluginCommon.Overlay.EventSourceBase
        {
            const string SAMPLE_EVENT = "templateSampleEvent";

            public TemplateEventSource(TinyIoCContainer c) : base(c)
            {
                // 设置事件源名称，必须是唯一的
                Name = "喵";

                // 注册数据源名称。
                // 此数据源提供给悬浮窗监听，悬浮窗使用addOverlayListener注册对应事件的回调。
                RegisterEventTypes(new List<string>()
            {
                SAMPLE_EVENT
            });

                // 注册事件接收器。
                // 注册完毕后，悬浮窗可以使用callOverlayHandler调用已经注册的方法
                RegisterEventHandler("templateSampleFunction", (obj) =>
                {
                    return JObject.FromObject(new
                    {
                        message = "Hello, world."
                    });
                });
            }
            public override Control CreateConfigControl()
            {
                return null;
            }

            public override void LoadConfig(IPluginConfig config)
            {
            }

            public override void SaveConfig(IPluginConfig config)
            {
            }

            public void InvokeSampleEvent(string data)
            {
                // 将数据发送给悬浮窗
                DispatchEvent(JObject.FromObject(new
                {
                    type = SAMPLE_EVENT,
                    data = data
                }));
            }
        }

        /// <summary>
        /// 悬浮窗预设
        /// </summary>
        struct OverlayPreset : IOverlayPreset
        {
            public string Name { get; set; }
            public string Type { get { return "MiniParse"; } }
            public string Url { get; set; }
            public int[] Size { get; set; }
            public bool Locked { get; set; }
            public List<string> Supports { get { return new List<string> { "modern" }; } }

        }
    }
}
