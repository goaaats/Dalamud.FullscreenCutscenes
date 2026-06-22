using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace Dalamud.FullscreenCutscenes
{
    public class ConfigWindow : Window
    {
        private readonly Configuration configuration;

        public ConfigWindow(Configuration configuration) : base("Ultrawide Cutscenes", ImGuiWindowFlags.AlwaysAutoResize)
        {
            this.configuration = configuration;
        }

        public override void Draw()
        {
            var enabled = this.configuration.IsEnabled;
            if (ImGui.Checkbox("Enable Ultrawide Cutscenes", ref enabled))
            {
                this.configuration.IsEnabled = enabled;
                this.configuration.Save();
            }
        }
    }
}
