using BepInEx.Configuration;

namespace CheatMenu;

public class CheatConfig{
    public ConfigEntry<KeyboardShortcut> GuiKeybind;
    public ConfigEntry<KeyboardShortcut> BackCategory;
    public ConfigEntry<bool> CloseGuiOnEscape;

    public CheatConfig(ConfigFile config){
        GuiKeybind = config.Bind(
            new ConfigDefinition("Keybinds", "GUIKey"),
            new KeyboardShortcut(UnityEngine.KeyCode.M),
            new ConfigDescription("The key pressed to open and close the CheatMenu GUI")
        );
        BackCategory = config.Bind(
            new ConfigDefinition("Keybinds", "Back Category"),
            new KeyboardShortcut(UnityEngine.KeyCode.N),
            new ConfigDescription("The key pressed to go back to the previous category/menu")
        );
        CloseGuiOnEscape = config.Bind(
            new ConfigDefinition("Options", "Close GUI on escape"),
            true,
            new ConfigDescription("Disable/Enable closing the cheat menu GUI when escape is preseed")
        );
        Instance = this;
    }

    public static CheatConfig Instance { get; set; }
}