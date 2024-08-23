/*using Lotlab.PluginCommon.Overlay;
using Newtonsoft.Json.Linq;
using RainbowMage.OverlayPlugin;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace CatPlugin
{
    /// <summary>
    /// 悬浮窗集成
    /// </summary>
    /// <remarks>
    /// 若不需要悬浮窗，则可直接删除此文件和对应的依赖项目
    /// </remarks>
    public partial class PluginCatTemplate : IOverlayAddonV2
    {
        void IOverlayAddonV2.Init()
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
            registry.RegisterOverlayPreset2(new OverlayPreset
            {
                Name = "喵",
                Url = "http://localhost:5173/",
                Size = new int[] { 300, 500 },
                Locked = false,
            });
            registry.RegisterOverlayPreset2(new OverlayPreset
            {
                Name = "喵",
                Url = "http://localhost:5173/",
                Size = new int[] { 300, 500 },
                Locked = false,
            });


        }

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
*/