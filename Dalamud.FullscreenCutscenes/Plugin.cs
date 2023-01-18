using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Hooking;

namespace Dalamud.FullscreenCutscenes
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Ultrawide Cutscenes";

        private const string commandName = "/pcutscenes";

        private delegate IntPtr UpdateLetterboxingDelegate(IntPtr thisPtr);

        private Hook<UpdateLetterboxingDelegate>? updateLetterboxingHook;

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            [RequiredVersion("1.0")] SigScanner targetScanner)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            //this.PluginUi = new PluginUI(this.Configuration, goatImage);

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            if (targetScanner.TryScanText("4C 8B DC 55 48 8B EC", out var ptr))
            {
                this.updateLetterboxingHook = Hook<UpdateLetterboxingDelegate>.FromAddress(ptr, UpdateLetterboxingDetour);
                this.updateLetterboxingHook.Enable();
            }

            //this.PluginInterface.UiBuilder.Draw += DrawUI;
            //this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private IntPtr UpdateLetterboxingDetour(IntPtr thisptr)
        {
            if (this.Configuration.IsEnabled)
                return IntPtr.Zero;

            return this.updateLetterboxingHook!.Original(thisptr);
        }

        public void Dispose()
        {
            this.updateLetterboxingHook?.Disable();
            this.CommandManager.RemoveHandler(commandName);
        }

        private void OnCommand(string command, string args)
        {
            if (!string.IsNullOrWhiteSpace(args) && bool.TryParse(args, out var val))
            {
                this.Configuration.IsEnabled = val;
            }
            else
            {
                this.Configuration.IsEnabled = !this.Configuration.IsEnabled;
            }

            this.Configuration.Save();
        }
    }
}