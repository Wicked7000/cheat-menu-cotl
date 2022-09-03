using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static cheat_menu.Singleton;

namespace cheat_menu;

public static class Definitions{

    public static List<MethodInfo> getAllCheatMethods(){
        List<MethodInfo> methodsRet = new List<MethodInfo>();
        MethodInfo[] methods = typeof(Definitions).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
        foreach(var defMethod in methods){
            var attributes = defMethod.Attributes;
            CheatDetails details = ReflectionHelper.HasAttribute<CheatDetails>(defMethod);
            if(details != null){
                methodsRet.Add(defMethod);
            }
        }
        return methodsRet;
    }

    public static Action BuildGUIContentFn(){
        DynamicMethod guiContentMethod = new DynamicMethod("", typeof(void), new Type[]{});
        var guiUtilsButton = typeof(GUIUtils).GetMethod("Button", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButtonWithFlagSimple = typeof(GUIUtils).GetMethod("ButtonWithFlagS", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButtonWithFlag = typeof(GUIUtils).GetMethod("ButtonWithFlag", BindingFlags.Static | BindingFlags.Public);
        var IsFlagEnabledStr = typeof(Singleton).GetMethod("IsFlagEnabledStr", BindingFlags.Static | BindingFlags.Public);
        var InvertBoolRef = typeof(CheatUtils).GetMethod("InvertBool", BindingFlags.Static | BindingFlags.Public);

        var ilGenerator = guiContentMethod.GetILGenerator();

        List<MethodInfo> methods = getAllCheatMethods();
        foreach(var defMethod in methods){
            var attributes = defMethod.Attributes;
            CheatDetails details = ReflectionHelper.HasAttribute<CheatDetails>(defMethod);
            if(details != null){
                CheatWIP cheatWip = ReflectionHelper.HasAttribute<CheatWIP>(defMethod);
                CheatFlag flag = ReflectionHelper.HasAttribute<CheatFlag>(defMethod);                
                Label endOfElem = ilGenerator.DefineLabel();
                
                if(cheatWip != null && !CheatUtils.IsDebugMode){
                    //Don't include WIP cheats in release builds!
                } else {
                    if(flag == null || !details.IsMultiNameFlagCheat){
                        ilGenerator.Emit(OpCodes.Ldstr, details.Title); // [] -> ["Title"]
                        if(flag == null){
                            ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButton, null); // ["title"] -> [bool]
                        } else {
                            ilGenerator.Emit(OpCodes.Ldstr, flag.Flag.ToString()); // ["title"] -> ["title", "flag"]
                            ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButtonWithFlagSimple, null); // ["title", "flag"] -> [bool]
                        }                        
                    } else {
                        ilGenerator.Emit(OpCodes.Ldstr, details.OffTitle); // [] -> ["OffTitle"]
                        ilGenerator.Emit(OpCodes.Ldstr, details.OnTitle); // ["OffTitle"] -> ["OffTitle", "OnTitle"]                        
                        ilGenerator.Emit(OpCodes.Ldstr, flag.Flag.ToString()); // ["OnTitle", "OffTitle"] -> ["OnTitle", "OffTitle", "flag"]
                        ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButtonWithFlag, null); // ["OnTitle", "OffTitle", "flag"] -> [bool]
                    }
                    
                    ilGenerator.Emit(OpCodes.Brfalse, endOfElem); // [bool] -> []
                    if(defMethod.GetParameters().Length == 1 && flag != null){
                        ilGenerator.Emit(OpCodes.Ldstr, flag.Flag.ToString()); // [] -> ["flag"]
                        ilGenerator.EmitCall(OpCodes.Call, IsFlagEnabledStr, null); // ["flag"] -> [bool]
                        ilGenerator.EmitCall(OpCodes.Call, InvertBoolRef, null); // [bool] -> [!bool]
                    } 
                    ilGenerator.EmitCall(OpCodes.Call, defMethod, null); // ([] | [bool]) -> []
                    ilGenerator.MarkLabel(endOfElem);
                }
            }
        }
        ilGenerator.Emit(OpCodes.Ret);
                        
        Action delegateFn = (Action)guiContentMethod.CreateDelegate(typeof(Action));
        return delegateFn;
    }
        
    [CheatDetails("Godmode", "Gives Invincibility")]
    [CheatFlag(CheatFlags.GodMode)]
    private static void GodMode(){
        Instance.cheatConsoleInstance.Method("ToggleGodMode").GetValue();
    }

    [CheatDetails("Noclip", "Collide with nothing!")]
    [CheatFlag(CheatFlags.NoClip)]
    private static void Noclip(){
        Instance.cheatConsoleInstance.Method("ToggleNoClip").GetValue();
    }

    [CheatDetails("Heal x1", "Heals a Red Heart of the Player")]
    private static void HealRed(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            gameObject.GetComponent<Health>().Heal(2f);
        }
    }

    [CheatDetails("Add x1 Blue Heart", "Adds a Blue Heart to the Player")]
    private static void AddBlueHeart(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            gameObject.GetComponent<Health>().BlueHearts += 2;
        }
    }

    [CheatDetails("Add x1 Black Heart", "Adds a Black Heart to the Player")]
    private static void AddBlackHeart(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            gameObject.GetComponent<Health>().BlackHearts += 2;
        }
    }

    [CheatDetails("Rename Cult", "Bring up the UI to rename the cult")]
    private static void RenameCult(){
        CultUtils.RenameCult();
    }

    [CheatDetails("Clear Base Rubble", "Removes any stones and large rubble")]
    private static void ClearBaseRubble(){
        Instance.cheatConsoleInstance.Method("ClearRubble").GetValue();
    }

    [CheatDetails("Weather: Rain", "Set weather to raining")]
    private static void WeatherRain(){
        WeatherController.Instance.SetRain();
    }

    [CheatDetails("Weather: Windy", "Set weather to windy")]
    private static void WeatherWindy(){
        WeatherController.Instance.SetWind();
    }

    [CheatDetails("Weather: Clear", "Set weather to clear")]
    private static void WeatherClear(){
        WeatherController.isRaining = false;
        Traverse.Create(WeatherController.Instance).Field("isRaining").SetValue(false);
        Traverse.Create(WeatherController.Instance).Field("RainIntensity").SetValue(0f);
        Traverse.Create(WeatherController.Instance).Field("windSpeed").SetValue(0f);
        Traverse.Create(WeatherController.Instance).Field("windDensity").SetValue(0f);
        Traverse.Create(WeatherController.Instance).Field("IsActive").SetValue(false);
        Traverse.Create(WeatherController.Instance).Field("weatherChanged").SetValue(true);
        WeatherController.Instance.CheckWeather();
    }

    [CheatDetails("Give Resources", "Gives 100 of the main primary resources")]
    private static void GiveResources(){
        Instance.cheatConsoleInstance.Method("GiveResources").GetValue();
    }

    [CheatDetails("Give Monster Heart", "Gives a heart of the heretic")]
    private static void GiveMonsterHeart(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.MONSTER_HEART, 10);
    }

    [CheatDetails("Give Food", "Gives all farming based foods")]
    private static void GiveFarmingFood(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.CAULIFLOWER, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.BERRY, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.BEETROOT, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.PUMPKIN, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.MUSHROOM_SMALL, 10);
    }

    [CheatDetails("Give Fish", "Gives all types of fish (x10)")]
    private static void GiveFish(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_BIG, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_CRAB, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_BLOWFISH, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_LOBSTER, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_OCTOPUS, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_SMALL, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_SQUID, 10);
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.FISH_SWORDFISH, 10);
    }

    [CheatDetails("Give Fertiziler", "Gives x100 Fertiziler (Poop)")]
    private static void GivePoop(){
        CultUtils.AddInventoryItem(InventoryItem.ITEM_TYPE.POOP, 100);
    }

    [CheatDetails("Hide/Show UI", "Hide UI", "Show UI", "Show/Hide the UI of the game")]
    [CheatFlag(CheatFlags.HidingUI)]
    private static void ShowUI(bool flag){
        if(flag){
            Instance.cheatConsoleInstance.Method("ShowUI").GetValue();
        } else {
            Instance.cheatConsoleInstance.Method("HideUI").GetValue();
        }        
    }

    [CheatDetails("Skip Hour", "Skip an hour of game time")]
    private static void SkipHour(){
       Instance.cheatConsoleInstance.Method("SkipHour").GetValue();      
    }

    [CheatDetails("Teleport to Base", "Teleports the player to the Base")]
    private static void TeleportToBase(){
        Instance.cheatConsoleInstance.Method("ReturnToBase").GetValue();
    }

    [CheatDetails("Die", "Kills the Player")]
    private static void Die(){
        GameObject gameObject = GameObject.FindWithTag("Player");
        if (gameObject != null)
        {
            Health healthComp = gameObject.GetComponent<Health>();
            healthComp.DealDamage(9999f, gameObject, gameObject.transform.position, false, Health.AttackTypes.Melee, false, (Health.AttackFlags)0);
        }
    }

    [CheatDetails("Spawn Follower (Worker)", "Spawns and auto-indoctrinates a follower as a worker")]
    private static void SpawnWorkerFollower(){
        CultUtils.SpawnFollower(FollowerRole.Worker);
    }

    [CheatDetails("Spawn Follower (Worshipper)", "Spawns and auto-indoctrinates a follower as a worshipper")]
    private static void SpawnWorkerWorshipper(){
        CultUtils.SpawnFollower(FollowerRole.Worshipper);
    }

    [CheatDetails("Spawn 'Arrived' Follower", "Spawns a follower ready for indoctrination")]
    private static void SpawnArrivedFollower(){
        FollowerManager.CreateNewRecruit(FollowerLocation.Base, NotificationCentre.NotificationType.NewRecruit);
    }

    [CheatDetails("Build All Structures", "Instantly build all structures")]
    private static void BuildAllStructures(){
        Instance.cheatConsoleInstance.Method("BuildAll").GetValue();
    }

    [CheatDetails("Revive All Followers", "Revive all currently dead followers")]
    [CheatWIP]
    private static void ReviveAllFollowers(){
        var followers = CheatUtils.cloneList(DataManager.Instance.Followers_Dead);
        foreach (var follower in followers)
        {
            CultUtils.ReviveFollower(follower);
        }
    }

    [CheatDetails("Turn all Followers Young", "Changes the age of all followers to young")]
    [CheatWIP]
    private static void TurnAllFollowersYoung(){
        var followers = DataManager.Instance.Followers;
        foreach(var follower in followers)
        {
            ThoughtData thought = CultUtils.HasThought(follower, Thought.OldAge);
            if (thought != null)
            {
                follower.Thoughts.Remove(thought);
                DataManager.Instance.Followers_Elderly_IDs.Remove(follower.ID);
            }
        }
    }

    [CheatDetails("Kill All Followers", "Kills all followers at the Base")]
    private static void KillAllFollowers(){
        var followers = DataManager.Instance.Followers;
        foreach (var follower in followers)
        {
            CultUtils.KillFollower(CultUtils.GetFollower(follower), false, false);
        }
    }

    [CheatDetails("Clear Faith", "Set the current faith to zero")]
    [CheatWIP]
    private static void ClearFaith(){
        CultUtils.ModifyFaith(0f, "Modifying faith to 0 (ClearFaith)");
    }

    //TODO: Possibly allow the patching to happen within the below function
    [CheatDetails("Max Faith Mode", "Keeps the faith of the cult maxed out")]
    [CheatFlag(CheatFlags.MaxFaithMode)]
    [CheatWIP]
    private static void MaxFaithMode(){}

    //TODO: Possibly allow the patching to happen within the below function
    [CheatDetails("Auto Clear Ritual Cooldowns", "Set ritual cooldowns to zero while active")]
    [CheatFlag(CheatFlags.NoRitualCooldown)]
    private static void ZeroRitualCooldown(){}

    [CheatDetails("Free Building Mode", "Buildings can be placed for free")]
    [CheatFlag(CheatFlags.FreeBuildingMode)]
    private static void FreeBuildingMode(bool flag){
        Instance.cheatConsoleInstance.Field("BuildingsFree").SetValue(flag);
    }

    [CheatDetails("Unlock All Structures", "Unlocks all buildings")]
    private static void UnlockAllStructures(){
        Instance.cheatConsoleInstance.Method("UnlockAllStructures").GetValue();
    }

    [CheatDetails("FPS Debug", "Displays the built-in FPS Debug menu")]
    [CheatFlag(CheatFlags.FPSDebug)]
    private static void FPSDebug(){
        Instance.cheatConsoleInstance.Method("FPS").GetValue();
    }

    [CheatDetails("Follower Debug", "Shows Follower Debug Information")]
    [CheatFlag(CheatFlags.FollowerDebug)]
    private static void FollowerDebug(){
        Instance.cheatConsoleInstance.Method("FollowerDebug").GetValue();    
    }

    [CheatDetails("Structure Debug", "Shows Structure Debug Information")]
    [CheatFlag(CheatFlags.StructureDebug)]
    private static void StructureDebug(){
        Instance.cheatConsoleInstance.Method("StructureDebug").GetValue();    
    }

    //Currently you can't pick which out the 'pairs' to unlock so leave as WIP for now.
    [CheatDetails("Unlock All Rituals", "Unlocks all rituals")]
    [CheatWIP]
    private static void UnlockAllRituals(){
        //WIP
    }

    [CheatDetails("Complete All Objectives", "Complete All Current quest objectives")]
    [CheatWIP]
    private static void CompleteAllObjectives(){
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

       