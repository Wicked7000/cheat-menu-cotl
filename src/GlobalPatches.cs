using System;
using System.Reflection;
using HarmonyLib;
using UnityAnnotationHelpers;

namespace CheatMenu;

public class GlobalPatches {
    [Init]
    [EnforceOrderLast]
    public static void Init()
    {
       MethodInfo interactorPatch = typeof(GlobalPatches).GetMethod("Prefix_Interactor_Update", BindingFlags.Static | BindingFlags.Public);
       ReflectionHelper.PatchMethodPrefix(typeof(Interactor), "Update", interactorPatch, BindingFlags.Instance | BindingFlags.NonPublic);

       MethodInfo upgradeSystemPatch = typeof(GlobalPatches).GetMethod("Prefix_UpgradeSystem_UnlockAbility", BindingFlags.Static | BindingFlags.Public);
       ReflectionHelper.PatchMethodPrefix(typeof(UpgradeSystem), "UnlockAbility", upgradeSystemPatch, BindingFlags.Static | BindingFlags.Public, new Type[]{typeof(UpgradeSystem.Type)});
    }

    [Unload]
    public static void UnpatchAll()
    {
        ReflectionHelper.UnpatchTracked(typeof(Interactor), "Update");
        ReflectionHelper.UnpatchTracked(typeof(UpgradeSystem), "UnlockAbility");
    }

    //If because of our mod we try to active both sides of a pair of a ritual we want
    //to fix it so that it displays correctly in menus.
    public static bool Prefix_UpgradeSystem_UnlockAbility(UpgradeSystem.Type Type)
    {
        var dictionary = CultUtils.GetDictionaryRitualPairs();
        if(dictionary.TryGetValue(Type, out UpgradeSystem.Type matchingItem)){
            if(UpgradeSystem.UnlockedUpgrades.Contains(matchingItem)){
                UpgradeSystem.UnlockedUpgrades.Remove(matchingItem);
            }
        }

        return true;
    }
    
    public static bool Prefix_Interactor_Update()
    {
        return !CheatMenuGui.GuiEnabled;
    }
}

