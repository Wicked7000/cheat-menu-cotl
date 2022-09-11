using static cheat_menu.Singleton;

namespace cheat_menu;

[CheatCategory(CheatCategoryEnum.MISC)]
public class MiscDefinitions : IDefinition{
    [CheatDetails("Noclip", "Collide with nothing!")]
    [CheatFlag(CheatFlags.NoClip)]
    public static void Noclip(){
        Instance.cheatConsoleInstance.Method("ToggleNoClip").GetValue();
    }

    [CheatDetails("FPS Debug", "Displays the built-in FPS Debug menu")]
    [CheatFlag(CheatFlags.FPSDebug)]
    public static void FPSDebug(){
        Instance.cheatConsoleInstance.Method("FPS").GetValue();
    }

    [CheatDetails("Follower Debug", "Shows Follower Debug Information")]
    [CheatFlag(CheatFlags.FollowerDebug)]
    public static void FollowerDebug(){
        Instance.cheatConsoleInstance.Method("FollowerDebug").GetValue();    
    }

    [CheatDetails("Structure Debug", "Shows Structure Debug Information")]
    [CheatFlag(CheatFlags.StructureDebug)]
    public static void StructureDebug(){
        Instance.cheatConsoleInstance.Method("StructureDebug").GetValue();    
    }

    [CheatDetails("Hide/Show UI", "Hide UI", "Show UI", "Show/Hide the UI of the game")]
    [CheatFlag(CheatFlags.HidingUI)]
    public static void ShowUI(bool flag){
        if(flag){
            Instance.cheatConsoleInstance.Method("ShowUI").GetValue();
        } else {
            Instance.cheatConsoleInstance.Method("HideUI").GetValue();
        }        
    }

    [CheatDetails("Skip Hour", "Skip an hour of game time")]
    public static void SkipHour(){
       Instance.cheatConsoleInstance.Method("SkipHour").GetValue();      
    }

    [CheatDetails("Complete All Objectives", "Complete All Current quest objectives")]
    [CheatWIP]
    public static void CompleteAllObjectives(){
        if(DataManager.Instance.Objectives.Count > 0)
        {
            foreach(var quest in DataManager.Instance.Objectives)
            {
                Objectives_Custom customObjective = quest as Objectives_Custom;
                if (quest != null)
                {
                    customObjective.ResultFollowerID = -1;
                    ObjectiveManager.UpdateObjective(customObjective);
                }
            }
        }
    }
}