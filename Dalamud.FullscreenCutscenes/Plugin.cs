using System.Runtime.InteropServices;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;

namespace Dalamud.FullscreenCutscenes
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Ultrawide Cutscenes";

        private const string commandName = "/pcutscenes";

        private delegate nint UpdateLetterboxingDelegate(nint thisPtr);

        private Hook<UpdateLetterboxingDelegate>? updateLetterboxingHook;

        private IDalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }   
        private Configuration Configuration { get; init; }
        private ICondition Condition { get; init; }
        public Plugin(
             IDalamudPluginInterface pluginInterface,
             ICommandManager commandManager,
             ISigScanner targetScanner,
             IGameInteropProvider gameInteropProvider,
             ICondition condition)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.Condition = condition;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            //this.PluginUi = new PluginUI(this.Configuration, goatImage);

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            if (targetScanner.TryScanText("E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ??", out var ptr))
            {
                this.updateLetterboxingHook = gameInteropProvider.HookFromAddress<UpdateLetterboxingDelegate>(ptr, UpdateLetterboxingDetour);
                this.updateLetterboxingHook.Enable();
            }

            //this.PluginInterface.UiBuilder.Draw += DrawUI;
            //this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private unsafe nint UpdateLetterboxingDetour(nint thisptr)
        {
            bool isWatchingCutscene = Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
                                      Condition[ConditionFlag.WatchingCutscene78];
            if (this.Configuration.IsEnabled && isWatchingCutscene)
            {
                SomeConfig* config = (SomeConfig*) thisptr;
                config->ShouldLetterBox &= ~(1 << 5);
            }

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
        
        [StructLayout(LayoutKind.Explicit)]
        public partial struct SomeConfig
        {
            [FieldOffset(0x40)] public int ShouldLetterBox;
        }
    }
}