using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static cheat_menu.Singleton;

namespace cheat_menu;

public static class GUIUtils {    
    private static Font uiFont;

    public static int currentButtonY = 0;
    public static int totalWindowCalculatedHeight = 0;
    public static GUIStyle buttonStyle = null;
    
    public static CheatCategoryEnum currentCategory = CheatCategoryEnum.NONE;

    public static bool IsWithinCategory() {
        return currentCategory != CheatCategoryEnum.NONE;
    }

    public static bool IsWithinSpecificCategory(string categoryString){
        var categoryEnum = CheatCategoryEnumExtensions.GetEnumFromName(categoryString);
        return categoryEnum.Equals(currentCategory);
    }

    [Init]
    private static void Init(){
        string[] fonts = Font.GetOSInstalledFontNames();
        List<string> fontsList = new List<string>(fonts);

        if(fontsList.Contains("Arial")){
            uiFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
        } else {
            uiFont = Font.CreateDynamicFontFromOSFont(fonts[0], 16);
        }
    }

    [Unload]
    private static void onDisable(Action<UnityEngine.Object> destroyFn)
    {
        destroyFn(uiFont);
        buttonStyle = null;
        currentButtonY = 0;
        totalWindowCalculatedHeight = 0;
    }

    [OnGUI]
    [EnforceOrderLast]
    public static void OnGUI(){
        currentButtonY = 0;
        totalWindowCalculatedHeight = 0;
    }

    public static GUIStyle GetGUIWindowStyle()
    {
        //Default styling
        var styleObj = new GUIStyle();
        GUIStyleState normalStyle = new GUIStyleState();
        normalStyle.background = TextureHelper.GetSolidTexture(new Color(0.3f, 0.3f, 0.3f, 1f));
        normalStyle.background.hideFlags = HideFlags.DontUnloadUnusedAsset;
        normalStyle.textColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        styleObj.normal = normalStyle;
        styleObj.active = normalStyle;
        styleObj.alignment = TextAnchor.MiddleCenter;
        styleObj.font = uiFont;
        styleObj.wordWrap = true;

        return styleObj;
    }


    public static GUIStyle GetGUILabelStyle(int width)
    {
        //Default styling
        var styleObj = new GUIStyle();
        GUIStyleState normalStyle = new GUIStyleState();
        normalStyle.textColor = Color.white;
        styleObj.normal = normalStyle;
        styleObj.active = normalStyle;
        styleObj.alignment = TextAnchor.MiddleCenter;
        styleObj.font = uiFont;
        styleObj.fontSize = width / 20;
        styleObj.wordWrap = true;

        return styleObj;
    }

    public static GUIStyle GetGUIPanelStyle(int width)
    {
        //Default styling
        var styleObj = new GUIStyle();
        GUIStyleState normalStyle = new GUIStyleState();
        normalStyle.background = TextureHelper.GetSolidTexture(new Color(0.3f, 0.3f, 0.3f, 1f));
        normalStyle.background.hideFlags = HideFlags.DontUnloadUnusedAsset;
        normalStyle.textColor = Color.white;
        styleObj.normal = normalStyle;
        styleObj.onNormal = normalStyle;
        styleObj.alignment = TextAnchor.MiddleCenter;
        styleObj.font = uiFont;
        styleObj.fontSize = width / 8;
        styleObj.wordWrap = true;

        return styleObj;
    }

    private static GUIStyle GetGUIButtonStyle()
    {
        if(buttonStyle != null)
        {
            return buttonStyle;
        }

        //Default styling
        var styleObj = new GUIStyle();
        GUIStyleState normalStyle = new GUIStyleState();
        normalStyle.background = TextureHelper.GetSolidTexture(new Color(0.3f, 0.3f, 0.3f, 1f));
        normalStyle.background.hideFlags = HideFlags.DontUnloadUnusedAsset;
        normalStyle.textColor = Color.white;
        styleObj.normal = normalStyle;
        styleObj.onNormal = normalStyle;
        styleObj.font = uiFont;
        styleObj.fontSize = 14;

        //Hover styling
        Texture2D hoverColor = TextureHelper.GetSolidTexture(new Color(0.2f, 0.2f, 0.2f, 1f));
        hoverColor.hideFlags = HideFlags.DontUnloadUnusedAsset;
        GUIStyleState hoverStyle = new GUIStyleState();
        hoverStyle.textColor = Color.white;
        hoverStyle.background = hoverColor;
        styleObj.hover = hoverStyle;
        styleObj.onHover = hoverStyle;
        styleObj.font = uiFont;
        styleObj.fontSize = 14;

        //Clicked styling
        Texture2D activeColor = TextureHelper.GetSolidTexture(new Color(0.1f, 0.1f, 0.1f, 1f));
        activeColor.hideFlags = HideFlags.DontUnloadUnusedAsset;
        GUIStyleState activeStyle = new GUIStyleState();
        activeStyle.textColor = Color.white;
        activeStyle.background = activeColor;
        styleObj.active = activeStyle;
        styleObj.onActive = activeStyle;
        styleObj.font = uiFont;
        styleObj.fontSize = 14;

        styleObj.alignment = TextAnchor.MiddleCenter;

        buttonStyle = styleObj;
        return styleObj;
    }

    public static bool CategoryButton(string categoryText){
        var buttonHeight = 50;
        var btn = GUI.Button(new Rect(0, currentButtonY, 480, buttonHeight), $"{categoryText} >", GetGUIButtonStyle());
        totalWindowCalculatedHeight += buttonHeight;
        currentButtonY += buttonHeight;
        if(btn){
            currentCategory = CheatCategoryEnumExtensions.GetEnumFromName(categoryText);
        }
        return btn;
    }

    public static bool BackButton(){
        var buttonHeight = 50;
        var btn = GUI.Button(new Rect(0, currentButtonY, 480, buttonHeight), $"< Back", GetGUIButtonStyle());
        totalWindowCalculatedHeight += buttonHeight;
        currentButtonY += buttonHeight;
        if(btn){
            currentCategory = CheatCategoryEnum.NONE;
        }
        return btn;
    }

    public static bool Button(string buttonText){
        var buttonHeight = 50;
        var btn = GUI.Button(new Rect(0, currentButtonY, 480, buttonHeight), buttonText, GetGUIButtonStyle());
        totalWindowCalculatedHeight += buttonHeight;
        currentButtonY += buttonHeight;
        return btn;
    }

    //Cheats that are based on flags (aka modes)
    public static bool ButtonWithFlag(String onText, String offText, String flagIDStr){
        var flagID = (CheatFlags) Enum.Parse(typeof(CheatFlags), flagIDStr);
        bool flag = Instance.IsFlagEnabled(flagID);
        var btnText = flag ? onText : offText;
        var btn = Button(btnText);
        if (btn)
        {
            Instance.FlipFlagValue(flagID);
        }
        return btn;
    }

    //Shorthand variation for mode type cheats
    public static bool ButtonWithFlagS(String text, String flagID){
        return ButtonWithFlag($"{text} (ON)", $"{text} (OFF)", flagID);
    }

    //Shorthand for flags that have no implementation (patcher based)
    public static bool ButtonWithFlagP(String text, String flagID){
        return ButtonWithFlagS(text, flagID);
    }
}