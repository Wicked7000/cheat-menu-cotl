using System;
using UnityEngine;
using UnityAnnotationHelpers;

namespace CheatMenu;

public static class CheatMenuGui {    
    public static bool GuiEnabled = false;
    public static CheatCategoryEnum CurrentCategory = CheatCategoryEnum.NONE;
    public static int CurrentButtonY = 0;
    public static int TotalWindowCalculatedHeight = 0;
    
    private static Action s_guiContent;
    private static GUIUtils.ScrollableWindowParams s_scrollParams = new(
        "Cheat Menu - Wicked",
        new(100, 500, 500, 500)
    );

    [Init]
    public static void Init(){
        CurrentButtonY = 0;
        TotalWindowCalculatedHeight = 0;
        s_scrollParams = new (
            "Cheat Menu - Wicked",
            new(100, 500, 500, 500)
        );
        s_guiContent = DefinitionManager.BuildGUIContentFn();
        CurrentCategory = CheatCategoryEnum.NONE;
    }
    

    public static bool IsWithinCategory() {
        return CurrentCategory != CheatCategoryEnum.NONE;
    }

    public static bool IsWithinSpecificCategory(string categoryString){
        var categoryEnum = CheatCategoryEnumExtensions.GetEnumFromName(categoryString);
        return categoryEnum.Equals(CurrentCategory);
    }

    public static bool CategoryButton(string categoryText){
        int buttonHeight = GUIUtils.GetButtonHeight();
        var btn = GUI.Button(new Rect(0, CurrentButtonY, 490, buttonHeight), $"{categoryText} >", GUIUtils.GetGUIButtonStyle());
        TotalWindowCalculatedHeight += buttonHeight;
        CurrentButtonY += buttonHeight;
        if(btn){
            CurrentCategory = CheatCategoryEnumExtensions.GetEnumFromName(categoryText);
        }
        return btn;
    }
    
    public static bool BackButton(){
        int buttonHeight = GUIUtils.GetButtonHeight();
        var btn = GUI.Button(new Rect(0, CurrentButtonY, 490, GUIUtils.GetButtonHeight()), $"< Back", GUIUtils.GetGUIButtonStyle());
        TotalWindowCalculatedHeight += buttonHeight;
        CurrentButtonY += buttonHeight;
        if(btn){
            CurrentCategory = CheatCategoryEnum.NONE;
        }
        return btn;
    }

    public static bool Button(string text) {
        var btn =  GUIUtils.Button(CurrentButtonY, 490, text);
        CurrentButtonY += GUIUtils.GetButtonHeight();
        TotalWindowCalculatedHeight += GUIUtils.GetButtonHeight();
        return btn;
    }

    public static bool ButtonWithFlag(string onText, string offText, string flagID){
        bool flag = FlagManager.IsFlagEnabled(flagID);
        var btnText = flag ? onText : offText;
        var btn = GUIUtils.Button(CurrentButtonY, 490, btnText);
        if (btn)
        {
            FlagManager.FlipFlagValue(flagID);
        }
        CurrentButtonY += GUIUtils.GetButtonHeight();
        TotalWindowCalculatedHeight += GUIUtils.GetButtonHeight();
        return btn;
    }

    public static bool ButtonWithFlagS(string text, string flagID){
        return ButtonWithFlag($"{text} (ON)", $"{text} (OFF)", flagID);
    }

    public static bool ButtonWithFlagP(string text, string flagID){
        return ButtonWithFlagS(text, flagID);
    }

    private static void ResetLayoutValues(){
        CurrentButtonY = 0;
        TotalWindowCalculatedHeight = 0;
    }

    [OnGui]
    public static void OnGUI(){
        if (GuiEnabled){
            s_scrollParams.Title = "Cheat Menu - Wicked";
            if(IsWithinCategory()){
                s_scrollParams.Title = $"{s_scrollParams.Title} ({CurrentCategory.GetCategoryName()})";
            }
            s_scrollParams = GUIUtils.CustomWindowScrollable(s_scrollParams, CheatWindow);
        }

        Action[] guiFunctions = GUIManager.GetAllGuiFunctions();
        if(guiFunctions.Length > 0){
            foreach(Action guiFn in guiFunctions){
                guiFn();
            }
        }
    }

    private static void CheatWindow()
    {
        s_guiContent();
        s_scrollParams.ScrollHeight = TotalWindowCalculatedHeight;
        ResetLayoutValues();
    }

    [Update]
    public static void Update()
    {                
        bool localGuiEnabled = GuiEnabled;
        bool keyDown = Input.GetKeyDown(CheatConfig.Instance.GuiKeybind.Value.MainKey);
        if(CultUtils.IsInGame() && keyDown){
            GuiEnabled = !GuiEnabled;
        } else if(keyDown) {
            NotificationHandler.CreateNotification("Cheat Menu can only be opened once in game!", 2);
        }
        
        if(GuiEnabled && Input.GetKeyDown(CheatConfig.Instance.BackCategory.Value.MainKey) && CurrentCategory!= CheatCategoryEnum.NONE)
        {
            CurrentCategory = CheatCategoryEnum.NONE;
            GUIManager.ClearAllGuiBasedCheats();
        }

        if(GuiEnabled && Input.GetKeyDown(KeyCode.Escape) && CheatConfig.Instance.CloseGuiOnEscape.Value)
        {
            GuiEnabled = false;
        }

        if(localGuiEnabled == true && GuiEnabled == false){
            GUIManager.ClearAllGuiBasedCheats();
        }
    }
}