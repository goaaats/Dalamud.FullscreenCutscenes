# Ultrawide Cutscenes

Dalamud plugin that removes the letterbox bars from Final Fantasy XIV cutscenes on ultrawide monitors.

This can reveal content outside the intended 16:9 frame, including NPCs or scene elements that are normally hidden.

## Building

1. Install the Dalamud development environment.
2. Open `Dalamud.FullscreenCutscenes.sln` in Visual Studio, Rider, or another C# IDE.
3. Build the solution.

The debug plugin DLL is written to:

```text
Dalamud.FullscreenCutscenes/bin/x64/Debug/Dalamud.FullscreenCutscenes.dll
```

## Loading In Dalamud

1. Launch the game.
2. Open Dalamud settings with `/xlsettings`.
3. Under `Experimental`, add the full path to the built `Dalamud.FullscreenCutscenes.dll` as a dev plugin location.
4. Open the plugin installer with `/xlplugins`.
5. Enable `Ultrawide Cutscenes` from `Dev Tools > Installed Dev Plugins`.

## Usage

Use `/pcutscenes` to toggle the plugin on or off.

You can also pass an explicit boolean value:

```text
/pcutscenes true
/pcutscenes false
```

The Dalamud config window exposes the same setting as a checkbox.

## Credits

Thanks to aers for finding this.
