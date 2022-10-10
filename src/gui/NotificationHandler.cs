using UnityEngine;
using UnityAnnotationHelpers;

namespace CheatMenu;

public class NotificationHandler {

    private static string s_message;    
    private static float s_timeToDisplay;    
    private static float s_timer;    

    [OnGui]
    public static void OnGUI(){
        var width = Screen.width / 5;
        var height = Screen.height / 6;
        Rect sizeAndLocation = GUIUtils.GetCenterRect(width, height);

        if(s_message != null){
            GUI.Window(2, sizeAndLocation, NotificationWindow, "", GUIUtils.GetGUIWindowStyle());
            s_timer += Time.deltaTime;
            if(s_timer >= s_timeToDisplay){
                s_message = null;
                s_timer = 0f;
                s_timeToDisplay = 0f;
            }            
        }
    }
    
    private static void NotificationWindow(int id){
        var width = Screen.width / 5;
        var height = Screen.height / 6;

        GUI.Label(new Rect(0, 0, width, height), s_message, GUIUtils.GetGUILabelStyle(width));
    }

    public static void CreateNotification(string message, int displayTimeSeconds){
        s_message = message;
        s_timeToDisplay = displayTimeSeconds;
        s_timer = 0f;
    }
}