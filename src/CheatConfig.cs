using BepInEx.Configuration;
using UnityEngine;

public class CheatConfig{
    private static CheatConfig instance;

    public ConfigEntry<KeyboardShortcut> guiKeybind;
    public ConfigEntry<KeyboardShortcut> backCategory;
    public ConfigEntry<bool> closeGuiOnEscape;

    public CheatConfig(ConfigFile config){
        guiKeybind = config.Bind(
            new ConfigDefinition("Keybinds", "GUIKey"),
            new KeyboardShortcut(UnityEngine.KeyCode.M),
            new ConfigDescription("The key pressed to open and close the CheatMenu GUI")
        );
        backCategory = config.Bind(
            new ConfigDefinition("Keybinds", "Back Category"),
            new KeyboardShortcut(UnityEngine.KeyCode.N),
            new ConfigDescription("The key pressed to go back to the previous category/menu")
        );
        closeGuiOnEscape = config.Bind(
            new ConfigDefinition("Options", "Close GUI on escape"),
            true,
            new ConfigDescription("Disable/Enable closing the cheat menu GUI when escape is preseed")
        );
        CheatConfig.instance = this;
    }

    public static CheatConfig Instance {
        get{return instance;}
    }
}