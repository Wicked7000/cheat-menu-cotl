using System;
using HarmonyLib;
using static cheat_menu.Singleton;

namespace cheat_menu;

public class Patching {
    private static Harmony harmonyInstance = null;

    [Init]
    private static void Init()
    {
        harmonyInstance = Harmony.CreateAndPatchAll(typeof(Patching));
    }

    [Unload]
    private static void UnpatchAll()
    {
        harmonyInstance.UnpatchSelf();
    }

    [HarmonyPatch(typeof(UpgradeSystem), "AddCooldown")]
    [HarmonyPrefix]
    public static bool enableAutoRefreshCooldown()
    {
        if (Instance.IsFlagEnabled(CheatFlags.NoRitualCooldown))
        {
            return false;
        }
        return true;
    }

    // Disable interactions while the GUI is open.
    [HarmonyPatch(typeof(Interactor), "Update")]
    [HarmonyPrefix]
    public static bool disableInteractions()
    {
        if (Instance.guiEnabled)
        {
            return false;
        }
        return true;
    }

    //
    [HarmonyPatch(typeof(CostFormatter), "FormatCost", new Type[] {typeof(StructuresData.ItemCost), typeof(bool), typeof(bool)})]
    [HarmonyPrefix]
    public static bool showZeroCostStructures(StructuresData.ItemCost itemCost, ref string __result)
    {
        if (Instance.IsFlagEnabled(CheatFlags.FreeBuildingMode))
        {
            __result = CostFormatter.FormatCost(itemCost.CostItem, 0, Inventory.GetItemQuantity(itemCost.CostItem), false, true);
            return false;
        }
        return true;
    }
}

