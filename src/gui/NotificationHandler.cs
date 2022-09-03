using System.Collections.Generic;
using UnityEngine;

namespace cheat_menu;

public class NotificationHandler {

    private static string message;    
    private static float timeToDisplay;    
    private static float timer;    

    [OnGUI]
    private static void OnGUI(){
        var width = Screen.width / 5;
        var height = Screen.height / 6;

        var centerX = (Screen.width - width) / 2;
        var centerY = (Screen.height - height) / 2;

        if(message != null){
            GUI.Window(2, new Rect(centerX, centerY, width, height), NotificationWindow, "", GUIUtils.GetGUIWindowStyle());
            timer += UnityEngine.Time.deltaTime;
            if(timer >= timeToDisplay){
                message = null;
                timer = 0f;
                timeToDisplay = 0f;
            }            
        }
    }
    
    private static void NotificationWindow(int id){
        var width = Screen.width / 5;
        var height = Screen.height / 6;

        GUI.Label(new Rect(0, 0, width, height), message, GUIUtils.GetGUILabelStyle(width));
    }

    public static void CreateNotification(string message, int displayTimeSeconds){
        NotificationHandler.message = message;
        NotificationHandler.timeToDisplay = displayTimeSeconds;
        NotificationHandler.timer = 0f;
    }
}