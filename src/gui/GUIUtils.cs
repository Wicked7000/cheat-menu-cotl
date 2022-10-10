using System;
using UnityEngine;
using System.Collections.Generic;
using UnityAnnotationHelpers;

namespace CheatMenu;

public static class GUIUtils {    
    private static Font s_uiFont;
    
    private static GUIStyle s_buttonStyle = null;
    private static GUIStyle s_selectedButtonStyle = null;
    private static GUIStyle s_titleBarStyle = null;

    [Init]
    public static void Init(){
        string[] fonts = Font.GetOSInstalledFontNames();
        List<string> fontsList = new(fonts);

        s_uiFont = fontsList.Contains("Arial") ? Font.CreateDynamicFontFromOSFont("Arial", 16) : Font.CreateDynamicFontFromOSFont(fonts[0], 16);
    }

    [Unload]
    public static void Unload()
    {
        UnityEngine.Object.Destroy(s_uiFont);
        s_buttonStyle = null;
    }

    public static GUIStyle GetGUIWindowStyle()
    {
        GUIStyleState normalStyle = new()
        {
            background = TextureHelper.GetSolidTexture(new Color(0.15f, 0.15f, 0.15f, 1f), true),
            textColor = new Color(0.2f, 0.2f, 0.2f, 1f),
        };

        GUIStyle styleObj = new()
        {
            normal = normalStyle,
            active = normalStyle,
            alignment = TextAnchor.MiddleCenter,
            font = s_uiFont,
            wordWrap = true
        };

        return styleObj;
    }


    public static GUIStyle GetGUILabelStyle(int width, float sizeModifier = 1.0f)
    {
        GUIStyleState normalStyle = new()
        {
            textColor = Color.white
        };

        GUIStyle styleObj = new()
        {
            normal = normalStyle,
            active = normalStyle,
            alignment = TextAnchor.MiddleCenter,
            font = s_uiFont,
            fontSize = (int)(width / 20 * sizeModifier),
            wordWrap = true
        };

        return styleObj;
    }

    public static GUIStyle GetGUIPanelStyle(int width)
    {
        GUIStyleState normalStyle = new()
        {
            background = TextureHelper.GetSolidTexture(new Color(0.3f, 0.3f, 0.3f, 1f), true),
            textColor = Color.white
        };

        GUIStyle styleObj = new()
        {
            normal = normalStyle,
            onNormal = normalStyle,
            alignment = TextAnchor.MiddleCenter,
            font = s_uiFont,
            fontSize = width / 8,
            wordWrap = true
        };

        return styleObj;
    }

    public static GUIStyle GetGUIButtonSelectedStyle()
    {
        if(s_selectedButtonStyle != null)
        {
            return s_selectedButtonStyle;
        }

        //Default styling
        GUIStyleState normalStyle = new()
        {
            background = TextureHelper.GetSolidTexture(new Color(0.7f, 0.7f, 0.7f, 1f), true),
            textColor = Color.white
        };

        //Hover
        GUIStyleState hoverStyle = new()
        {
            textColor = Color.white,
            background = TextureHelper.GetSolidTexture(new Color(0.6f, 0.6f, 0.6f, 1f), true)
        };

        //Clicked
        GUIStyleState activeStyle = new()
        {
            textColor = Color.white,
            background = TextureHelper.GetSolidTexture(new Color(0.5f, 0.5f, 0.5f, 1f), true)
        };

        GUIStyle styleObj = new()
        {
            normal = normalStyle,
            onNormal = normalStyle,
            active = activeStyle,
            onActive = activeStyle,
            hover = hoverStyle,
            onHover = hoverStyle,
            font = s_uiFont,
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };

        s_selectedButtonStyle = styleObj;
        return styleObj;
    }

    public static GUIStyle GetTitleBarStyle()
    {
        if(s_titleBarStyle != null)
        {
            return s_titleBarStyle;
        }

        //Default styling
        GUIStyleState normalStyle = new()
        {
            background = TextureHelper.GetSolidTexture(new Color(0.5f, 0.5f, 0.5f, 1f), true),
            textColor = Color.white
        };

        GUIStyle styleObj = new()
        {
            normal = normalStyle,
            onNormal = normalStyle,
            font = s_uiFont,
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };

        s_titleBarStyle = styleObj;
        return styleObj;
    }

    public static GUIStyle GetGUIButtonStyle()
    {
        if(s_buttonStyle != null)
        {
            return s_buttonStyle;
        }

        //Default styling
        GUIStyleState normalStyle = new()
        {
            background = TextureHelper.GetSolidTexture(new Color(0.3f, 0.3f, 0.3f, 1f), true),
            textColor = Color.white
        };

        //Hover
        GUIStyleState hoverStyle = new()
        {
            textColor = Color.white,
            background = TextureHelper.GetSolidTexture(new Color(0.2f, 0.2f, 0.2f, 1f), true)
        };

        //Clicked
        GUIStyleState activeStyle = new()
        {
            textColor = Color.white,
            background = TextureHelper.GetSolidTexture(new Color(0.1f, 0.1f, 0.1f, 1f), true)
        };

        GUIStyle styleObj = new()
        {
            normal = normalStyle,
            onNormal = normalStyle,
            active = activeStyle,
            onActive = activeStyle,
            hover = hoverStyle,
            onHover = hoverStyle,
            font = s_uiFont,
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };

        s_buttonStyle = styleObj;
        return styleObj;
    }

    public static Rect GetCenterRect(int width, int height){
        var centerX = (Screen.width - width) / 2;
        var centerY = (Screen.height - height) / 2;
        return new Rect(centerX, centerY, width, height);
    }

    public static void TitleBar(string titleText, float width){
        GUI.Box(new Rect(0, 0, width, 20), titleText, GetTitleBarStyle());
    }

    public struct WindowParams{
        public Rect ClientRect;
        public string Title;
        public int? WindowID = null;

        public WindowParams(string title, Rect clientRect){
            ClientRect = clientRect;
            Title = title;
        }
    }

    public static WindowParams CustomWindow(WindowParams windowParams, Action guiContents){
        GUI.DragWindow(new Rect(0, 0, windowParams.ClientRect.width, 20));
        WindowParams newWindowParams = new(windowParams.Title, windowParams.ClientRect);
        if(newWindowParams.WindowID == null){
            newWindowParams.WindowID = GUIManager.GetNextAvailableWindowID();
        }

        Rect newRect = GUI.Window((int)newWindowParams.WindowID, windowParams.ClientRect, delegate {
            TitleBar(newWindowParams.Title, newWindowParams.ClientRect.width);
            guiContents();
        }, "", GetGUIWindowStyle());
        return newWindowParams;
    }

    public class ScrollableWindowParams{
        public Vector2 ScrollPosition;
        public string Title;
        public Rect ClientRect;
        public float ScrollHeight;
        public int? WindowID = null;

        public ScrollableWindowParams(string title, Rect clientRect, float? scrollHeight = null, Vector2? scrollPosition = null){
            Title = title;
            ClientRect = clientRect;
            ScrollHeight = scrollHeight == null ? ClientRect.height * 2 : (float)scrollHeight;
            ScrollPosition = scrollPosition == null ? Vector2.zero : (Vector2)scrollPosition;
        }
    }

    public static ScrollableWindowParams CustomWindowScrollable(ScrollableWindowParams scrollParams, Action guiContents){        
        if(scrollParams.WindowID == null){
            scrollParams.WindowID = GUIManager.GetNextAvailableWindowID();
        }

        scrollParams.ClientRect = GUI.Window((int)scrollParams.WindowID, scrollParams.ClientRect, delegate {
            TitleBar(scrollParams.Title, scrollParams.ClientRect.width);
            Rect modifiedScrollPosition = new(
                0, 
                20, 
                scrollParams.ClientRect.width,
                scrollParams.ClientRect.height - 20
            );
            scrollParams.ScrollPosition = GUI.BeginScrollView(
                modifiedScrollPosition, 
                scrollParams.ScrollPosition, 
                new Rect(0, 0, scrollParams.ClientRect.width-100, scrollParams.ScrollHeight),
                false,
                true
            );
            guiContents();
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }, "", GetGUIWindowStyle());

        return scrollParams;
    }

    // 0 neither button clicked, 1 first button clicked, 2 second button clicked
    public static int ToggleButton(Rect sizeAndPlacement, string buttonOneText, string buttonTwoText, int state = 0){
        bool firstClicked = GUI.Button(
            new Rect(sizeAndPlacement.x, sizeAndPlacement.y, sizeAndPlacement.width/2, sizeAndPlacement.height),
            buttonOneText,
            state == 1 ? GetGUIButtonSelectedStyle() : GetGUIButtonStyle()
        );
        bool secondClicked = GUI.Button(
            new Rect(sizeAndPlacement.x + sizeAndPlacement.width/2, sizeAndPlacement.y, sizeAndPlacement.width/2, sizeAndPlacement.height),
            buttonTwoText,
            state == 2 ? GetGUIButtonSelectedStyle() : GetGUIButtonStyle()
        );
        if(firstClicked || secondClicked){
            return firstClicked ? 1 : 2;
        }
        return state;
    }

    public static int GetButtonHeight(){
        return 50;
    }

    public static bool Button(int y, int width, string buttonText){
        var btn = GUI.Button(new Rect(0, y, width, GetButtonHeight()), buttonText, GetGUIButtonStyle());
        return btn;
    }
}