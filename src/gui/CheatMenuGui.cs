using System;
using UnityEngine;

namespace cheat_menu;

public class CheatMenuGui {    
    private static Action GUIContent = null;    
    private static bool guiEnabled = false;
    private static Rect windowRect = new Rect(100, 500, 500, 500);
    private static Vector2 scrollPosition = Vector2.zero;
    private static Rect scrollRectPosition = new Rect(0,0,0,0);
    private static Rect scrollViewRect = new Rect(0, 0, 0, 0);

    [Init]
    private static void Init(){
        GUIContent = Definitions.BuildGUIContentFn();         
        scrollPosition = Vector2.zero;
        scrollRectPosition = new Rect(5, 20, 490, 493);
    }
    
    [OnGUI]
    private static void OnGUI(){
        if (guiEnabled){
            windowRect = GUI.Window(0, windowRect, CheatWindow, "Cheat Menu - Wicked");
        }
    }

    private static void CheatWindow(int windowID)
    {
        //Make titlebar of the window draggable
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        scrollPosition = GUI.BeginScrollView(scrollRectPosition, scrollPosition, new Rect(0, 0, 490, GUIUtils.totalWindowCalculatedHeight), false, true);
        GUIUtils.OnGUI();
    
        GUIContent();
        GUI.EndScrollView();

       /*  
        Button("Skip Tutorials", delegate
        {
            DataManager privateInstance = Traverse.Create(typeof(DataManager)).Field("instance").GetValue<DataManager>();

            DataManager.Instance.AllowSaving = true;
            DataManager.Instance.EnabledHealing = true;
            DataManager.Instance.BuildShrineEnabled = true;
            privateInstance.CookedFirstFood = true;
            privateInstance.XPEnabled = true;
            DataManager.Instance.Tutorial_Second_Enter_Base = false;
            DataManager.Instance.AllowBuilding = true;
            DataManager.Instance.ShowLoyaltyBars = true;
            DataManager.Instance.RatExplainDungeon = false;
            DataManager.Instance.ShowCultFaith = true;
            DataManager.Instance.ShowCultHunger = true;
            DataManager.Instance.ShowCultIllness = true;
            DataManager.Instance.UnlockBaseTeleporter = false;
            DataManager.Instance.BonesEnabled = false;
            privateInstance.PauseGameTime = true;
            privateInstance.ShownDodgeTutorial = true;
            privateInstance.ShownInventoryTutorial = true;
            privateInstance.HasEncounteredTarot = true;
            DataManager.Instance.CurrentGameTime = 244f;
            DataManager.Instance.HasBuiltShrine1 = false;
            DataManager.Instance.OnboardedHomeless = false;
            DataManager.Instance.ForceDoctrineStones = false;
            DataManager.Instance.Tutorial_First_Indoctoring = true;
            privateInstance.HadInitialDeathCatConversation = false;
            privateInstance.PlayerHasBeenGivenHearts = true;
            privateInstance.BaseGoopDoorLocked = false;
            privateInstance.InTutorial = false;

            //Skip the indoctrinate phase
            CultUtils.SpawnFollower(FollowerRole.Worker);
            Onboarding.Instance.Rat1Indoctrinate.SetActive(false);
            Onboarding.CurrentPhase = DataManager.OnboardingPhase.Devotion;

            //Name the Cult
            CultUtils.RenameCult(delegate { DataManager.Instance.OnboardedCultName = true; });

            //Build the shrine
        });
        **/
    }

    [Update]
    private static void Update()
    {        
        bool keyDown = Input.GetKeyDown(CheatConfig.Instance.guiKeybind.Value.MainKey);
        if(CultUtils.IsInGame() && keyDown){
            guiEnabled = !guiEnabled;
            Singleton.Instance.guiEnabled = guiEnabled;
        } else if(keyDown) {
            NotificationHandler.CreateNotification("Cheat Menu can only be opened once in game!", 2);
        }
        
        if(guiEnabled && Input.GetKeyDown(KeyCode.Escape))
        {
            guiEnabled = false;
            Singleton.Instance.guiEnabled = guiEnabled;
        }
    }
}