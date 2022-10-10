using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using UnityAnnotationHelpers;
using System.Text.RegularExpressions;

namespace CheatMenu;

[CheatCategory(CheatCategoryEnum.CULT)]
public class CultDefinitions : IDefinition {
    private static GUIUtils.ScrollableWindowParams s_ritualGui;
    private static GUIUtils.ScrollableWindowParams s_docterineGui;

    [Init]
    public static void Init(){
        s_ritualGui = new GUIUtils.ScrollableWindowParams(
            "Unlock All Rituals",
            GUIUtils.GetCenterRect(500, 500)
        );

        s_docterineGui = new GUIUtils.ScrollableWindowParams(
            "Change Doctrines",
            GUIUtils.GetCenterRect(500, 500)
        );
    }


    [CheatDetails("Teleport to Cult", "Teleports the player to the Base")]
    public static void TeleportToBase(){
        Traverse.Create(typeof(CheatConsole)).Method("ReturnToBase").GetValue();
    }

    [CheatDetails("Rename Cult", "Bring up the UI to rename the cult")]
    public static void RenameCult(){
        CultUtils.RenameCult();
    }

    [CheatDetails("Allow Shrine Creation", "Allow Shrine Creation (OFF)", "Allow Shrine Creation (ON)", "Allows the Shrine to be created from the building menu", true)]
    public static void AllowShrineCreation(bool flag){
        DataManager.Instance.BuildShrineEnabled = flag;
    }

    [CheatDetails("Clear Base Rubble", "Removes any stones and large rubble")]
    public static void ClearBaseRubble(){
        Traverse.Create(typeof(CheatConsole)).Method("ClearRubble").GetValue();
    }

    [CheatDetails("Clear Base Trees", "Removes all trees in the base")]
    public static void ClearBaseTrees(){
        CultUtils.ClearBaseTrees();
    }

    [CheatDetails("Clear Vomit", "Clear any vomit on the floor!")]
    public static void ClearVomit(){
        CultUtils.ClearVomit();
    }

    [CheatDetails("Clear Poop", "Clear any poop on the floor, giving the fertilizer directly!")]
    public static void ClearPoop(){
        CultUtils.ClearPoop();
    }

    [CheatDetails("Clear Dead bodies", "Clears any dead bodies on the floor, giving follower meat!")]
    public static void ClearDeadBodies(){
        CultUtils.ClearBodies();
    }

    public static bool Prefix_UpgradeSystem_AddCooldown(){
        //Just skip adding cooldown
        return false;
    }

    [CheatDetails("Auto Clear Ritual Cooldowns", "Set ritual cooldowns to zero while active", true)]
    public static void ZeroRitualCooldown(bool flagStatus){
        UpgradeSystem.ClearAllCoolDowns();
        if(flagStatus){
            ReflectionHelper.PatchMethodPrefix(
                typeof(UpgradeSystem), 
                "AddCooldown", 
                ReflectionHelper.GetMethodStaticPublic("Prefix_UpgradeSystem_AddCooldown"),
                BindingFlags.Static | BindingFlags.Public
            );
        } else {
            ReflectionHelper.UnpatchTracked(typeof(UpgradeSystem), "AddCooldown");
        }
    }

    public static bool Prefix_CostFormatter_FormatCost(StructuresData.ItemCost itemCost, ref string __result)
    {
        __result = CostFormatter.FormatCost(itemCost.CostItem, 0, Inventory.GetItemQuantity(itemCost.CostItem), false, true);
        return false;
    }

    [CheatDetails("Free Building Mode", "Buildings can be placed for free", true)]
    public static void FreeBuildingMode(bool flagStatus){
        if(flagStatus){
            MethodInfo patchMethod = typeof(CultDefinitions).GetMethod("Prefix_CostFormatter_FormatCost");
            ReflectionHelper.PatchMethodPrefix(
                typeof(CostFormatter), 
                "FormatCost", 
                patchMethod, 
                BindingFlags.Public | BindingFlags.Static,
                new Type[] {typeof(StructuresData.ItemCost), typeof(bool), typeof(bool)}
            );
        } else {
            ReflectionHelper.UnpatchTracked(typeof(CostFormatter), "FormatCost");
        }
        Traverse.Create(typeof(CheatConsole)).Field("BuildingsFree").SetValue(flagStatus);
    }

    [CheatDetails("Build All Structures", "Instantly build all structures")]
    public static void BuildAllStructures(){
        Traverse.Create(typeof(CheatConsole)).Method("BuildAll").GetValue();
    }

    [CheatDetails("Unlock All Structures", "Unlocks all buildings")]
    public static void UnlockAllStructures(){
        Traverse.Create(typeof(CheatConsole)).Method("UnlockAllStructures").GetValue();
    }

    [CheatDetails("Clear Outhouses", "Clears all outhouses of poop and adds the contents to your inventory.")]
    public static void ClearAllOuthouses(){
        CultUtils.ClearOuthouses();
    }

    [CheatDetails("Change Rituals", "Change Rituals",  "Change Rituals (Close)", "Lets you change the selected Rituals along with unlocking not yet acquired ones", true)]
    public static void ChangeAllRituals(bool flag){
        if(flag) {
            List<Tuple<UpgradeSystem.Type, UpgradeSystem.Type>> pairs = CultUtils.GetRitualPairs();
            int currentHeight = 20;
            string guiFunctionKey = "";
            int[] pairStates = new int[pairs.Count];

            //Pre-populate ritual states
            for(int pairIdx = 0; pairIdx < pairStates.Length; pairIdx++){
                var tuplePair = pairs[pairIdx];
                bool hasItem1 = UpgradeSystem.UnlockedUpgrades.Contains(tuplePair.Item1);
                bool hasItem2 = UpgradeSystem.UnlockedUpgrades.Contains(tuplePair.Item1);
                if(hasItem1 || hasItem2){
                    pairStates[pairIdx] = hasItem1 ? 1 : 2;
                }
                pairStates[pairIdx] = 0;
            }
            
            void Confirm()
            {
                for(int i = 0; i < pairStates.Length; i++){
                    var pair = pairs[i];
                    int pairState = pairStates[i];
                    if(pairState == 1){
                        UpgradeSystem.UnlockAbility(pair.Item1);
                    } else if(pairState == 2) {
                        UpgradeSystem.UnlockAbility(pair.Item2);
                    }
                }

                foreach(var ritual in UpgradeSystem.SecondaryRituals){
                    UpgradeSystem.UnlockAbility(ritual);
                }
                UpgradeSystem.UnlockAbility(UpgradeSystem.PrimaryRitual1);
            };

            void GuiContents()
            {
                GUI.Label(new Rect(0, 20, 485, 50), "Select one ritual from each pair below, then press confirm at the bottom", GUIUtils.GetGUILabelStyle(500));
                currentHeight = 90;

                for(int idx = 0; idx < pairs.Count; idx++){
                    Tuple<UpgradeSystem.Type, UpgradeSystem.Type> tupleSet = pairs[idx];
                    string ritualOneName = UpgradeSystem.GetLocalizedName(tupleSet.Item1);
                    string ritualTwoName = UpgradeSystem.GetLocalizedName(tupleSet.Item2);
                    pairStates[idx] = GUIUtils.ToggleButton(new Rect(0, currentHeight, 490, 100), $"{ritualOneName}", $"{ritualTwoName}", pairStates[idx]);
                    currentHeight += 100;
                }
                if(GUIUtils.Button(currentHeight, 490, "Confirm Selection")){
                    Confirm();
                    CultUtils.PlayNotification("Rituals unlocked!");
                    GUIManager.CloseGuiFunction(guiFunctionKey);
                }
                currentHeight += GUIUtils.GetButtonHeight();
                s_ritualGui.ScrollHeight = currentHeight;
            }

            guiFunctionKey = GUIManager.SetGuiWindowScrollableFunction(s_ritualGui, GuiContents);
        } else {
            GUIManager.RemoveGuiFunction();
        }
    }

    [CheatDetails("Clear All Doctrines", "Clears all docterine categories and rewards (Apart from special rituals)")]
    public static void ClearAllDoctrines(){
        CultUtils.ClearAllDocterines();
    }

    [CheatDetails("All Rituals", "All Rituals (Off)", "All Rituals (On)", "While enabled you will have access to all rituals (including both sides of every pair)")]
    public static void UnlockAllRituals(bool flag){
        CheatConsole.UnlockAllRituals = flag;
    }

    [CheatDetails("Change Doctrines", "Change Doctrines",  "Change Doctrines (Close)", "Allows the modification of selected doctrines", true)]
    public static void ChangeAllDoctrines(bool flag){
        if(flag) {
            Regex replaceRegex = new("<.*?>");
            var doctrinePairs = CultUtils.GetAllDoctrinePairs();
            SermonCategory currentCategory = SermonCategory.None;
            int[] pairStates = new int[4];
            string categoryName = "";
            int currentHeight = 20;            
            string guiFunctionKey = "";

            void Confirm()
            {
                CultUtils.ReapplyAllDoctrinesWithChanges(currentCategory, pairStates);
                CultUtils.PlayNotification("Doctrines changed!");
            }

            void GuiContents()
            {                
                //Display selection page
                if(currentCategory == SermonCategory.None){
                    currentHeight = 90;
                    GUI.Label(new Rect(0, 20, 485, 50), "Select a doctrine category below to modify", GUIUtils.GetGUILabelStyle(500));

                    foreach(var value in Enum.GetValues(typeof(SermonCategory))){
                        SermonCategory sermonCategory = (SermonCategory)value;
                        string innerCategoryName = DoctrineUpgradeSystem.GetSermonCategoryLocalizedName(sermonCategory);
                        if(innerCategoryName.StartsWith("DoctrineUpgradeSystem")){
                            continue;
                        }

                        if(GUIUtils.Button(currentHeight, 500, innerCategoryName)){
                            currentCategory = sermonCategory;
                            categoryName = innerCategoryName;
                            pairStates = CultUtils.GetDoctrineCategoryState(currentCategory);
                        }
                        currentHeight += GUIUtils.GetButtonHeight();
                    }
                } else {
                    int level = DoctrineUpgradeSystem.GetLevelBySermon(currentCategory);
                    
                    GUI.Label(new Rect(0,5, 500, 10), $"Current category level", GUIUtils.GetGUILabelStyle(500, 0.75f));
                    GUI.Label(new Rect(0,25, 500, 15), $"{categoryName}: {level}/4", GUIUtils.GetGUILabelStyle(500, 0.75f));
                    GUI.Label(new Rect(5,70, 480, 15), $"Select desired traits below and then confirm at the bottom to apply", GUIUtils.GetGUILabelStyle(500, 0.75f));
                    currentHeight = 110;

                    if(GUIUtils.Button(currentHeight, 500, "< Back")){
                        currentCategory = SermonCategory.None;
                    }
                    currentHeight += GUIUtils.GetButtonHeight();

                    var innerPairs = doctrinePairs[currentCategory];
                    for(int idx = 0; idx < innerPairs.Count; idx++){
                        var tupleSet = innerPairs[idx];
                    
                        string doctrineOneName = DoctrineUpgradeSystem.GetLocalizedName(tupleSet.Item1);
                        string doctrineTwoName = DoctrineUpgradeSystem.GetLocalizedName(tupleSet.Item2);
                        doctrineOneName = replaceRegex.Replace(doctrineOneName, "");
                        doctrineTwoName = replaceRegex.Replace(doctrineTwoName, "");

                        pairStates[idx] = GUIUtils.ToggleButton(new Rect(0, currentHeight, 490, 100), $"{doctrineOneName}", $"{doctrineTwoName}", pairStates[idx]);
                        currentHeight += 100;
                    }
                    if(GUIUtils.Button(currentHeight, 500, "Confirm")){                        
                        Confirm();
                        GUIManager.CloseGuiFunction(guiFunctionKey);
                        currentCategory = SermonCategory.None;
                    }
                    currentHeight += GUIUtils.GetButtonHeight();
                }

                s_docterineGui.ScrollHeight = currentHeight;
            }

            guiFunctionKey = GUIManager.SetGuiWindowScrollableFunction(s_docterineGui, GuiContents);
        } else {
            GUIManager.RemoveGuiFunction();
        }
    }
}