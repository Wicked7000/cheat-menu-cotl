using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityAnnotationHelpers;

namespace CheatMenu;

public static class GUIManager{
    private static Dictionary<string, Action> s_guiFunctions;
    private static int s_nextAvailableWindowID = 0;

    [Init]
    public static void Init(){
        s_guiFunctions = new();
    }

    public static Action[] GetAllGuiFunctions(){
        return s_guiFunctions.Values.ToArray();
    }

    public static int GetNextAvailableWindowID(){
        int nextWindowID = s_nextAvailableWindowID;
        s_nextAvailableWindowID += 1;
        return nextWindowID;
    }

    //Flips all flags to false and unregisters all their functions
    public static void ClearAllGuiBasedCheats(){
        string[] guiKeys = s_guiFunctions.Keys.ToArray();
        foreach(string key in guiKeys){
            RemoveGuiFunction(key);
            FlagManager.SetFlagValue(key, false);
        }
    }

    public static void SetGuiWindowFunction(GUIUtils.WindowParams windowParams, Action innerContent){
        SetGuiFunction(delegate{
            windowParams = GUIUtils.CustomWindow(windowParams, innerContent);
        });
    }
    
    public static string SetGuiWindowScrollableFunction(GUIUtils.ScrollableWindowParams windowParams, Action innerContent){
        string flagId = Definition.GetCheatFlagID(ReflectionHelper.GetCallingMethod());
        return SetGuiFunctionInternal(flagId, delegate{
            windowParams = GUIUtils.CustomWindowScrollable(windowParams, innerContent);
        });
    }

    private static string SetGuiFunctionInternal(string flagId, Action guiFunction){
        s_guiFunctions[flagId] = guiFunction;
        UnityEngine.Debug.Log($"[GUIManager] {flagId} has registered its GUI function");
        return flagId;
    }

    public static string SetGuiFunctionKey(string flagId, Action guiFunction){
        SetGuiFunctionInternal(flagId, guiFunction);
        return flagId;
    }

    public static string SetGuiFunction(Action guiFunction){
        string flagId = Definition.GetCheatFlagID(ReflectionHelper.GetCallingMethod());
        SetGuiFunctionInternal(flagId, guiFunction);
        return flagId;
    }

    private static void RemoveGuiFunction(string key){
        if(s_guiFunctions.ContainsKey(key)){
            s_guiFunctions.Remove(key);
            UnityEngine.Debug.Log($"[GUIManager] {key} has removed its GUI function");
        }
    }

    public static void RemoveGuiFunction(){
        string key = Definition.GetCheatFlagID(ReflectionHelper.GetCallingMethod());
        RemoveGuiFunction(key);
    }

    //Flips flag along with deregistering the function
    public static void CloseGuiFunction(string key){
        RemoveGuiFunction(key);
        FlagManager.SetFlagValue(key, false);
    }

    public static void RemoveGuiFunction(MethodInfo callingMethod){
        string key = Definition.GetCheatFlagID(callingMethod);
        RemoveGuiFunction(key);
    }
}