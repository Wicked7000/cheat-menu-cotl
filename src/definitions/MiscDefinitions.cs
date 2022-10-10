using System;
using System.Reflection;

using HarmonyLib;

namespace CheatMenu;

[CheatCategory(CheatCategoryEnum.MISC)]
public class MiscDefinitions : IDefinition{
    [CheatDetails("Noclip", "Collide with nothing!", true)]
    public static void Noclip(){
        Traverse.Create(typeof(CheatConsole)).Method("ToggleNoClip").GetValue();
    }

    [CheatDetails("FPS Debug", "Displays the built-in FPS Debug menu", true)]
    public static void FPSDebug(){
        Traverse.Create(typeof(CheatConsole)).Method("FPS").GetValue();
    }

    [CheatDetails("Follower Debug", "Shows Follower Debug Information", true)]
    public static void FollowerDebug(){
        Traverse.Create(typeof(CheatConsole)).Method("FollowerDebug").GetValue();    
    }

    [CheatDetails("Structure Debug", "Shows Structure Debug Information", true)]
    public static void StructureDebug(){
        Traverse.Create(typeof(CheatConsole)).Method("StructureDebug").GetValue();    
    }

    [CheatDetails("Hide/Show UI", "Hide UI", "Show UI", "Show/Hide the UI of the game", true)]
    public static void ShowUI(bool flag){
        if(flag){
            Traverse.Create(typeof(CheatConsole)).Method("HideUI").GetValue();
        } else {
            Traverse.Create(typeof(CheatConsole)).Method("ShowUI").GetValue();
        }        
    }

    [CheatDetails("Skip Hour", "Skip an hour of game time")]
    public static void SkipHour(){
       Traverse.Create(typeof(CheatConsole)).Method("SkipHour").GetValue();      
    }

    [CheatDetails("Complete All Quests", "Complete All Quests")]
    [CheatWIP]
    public static void CompleteAllQuests(){
       CultUtils.CompleteAllQuests();
    }
}