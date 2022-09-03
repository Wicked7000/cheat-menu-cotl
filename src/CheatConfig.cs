using BepInEx.Configuration;
using UnityEngine;

public class CheatConfig{
    private static CheatConfig instance;

    public ConfigEntry<KeyboardShortcut> guiKeybind;

    public CheatConfig(ConfigFile config){
        guiKeybind = config.Bind(
            new ConfigDefinition("Keybinds", "GUIKey"),
            new KeyboardShortcut(UnityEngine.KeyCode.M),
            new ConfigDescription("The key pressed to open and close the CheatMenu GUI")
        );
        CheatConfig.instance = this;
    }

    public static CheatConfig Instance {
        get{return instance;}
    }
}