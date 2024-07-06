using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace Dalamud.FullscreenCutscenes
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool IsEnabled { get; set; } = true;

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private IDalamudPluginInterface? pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}