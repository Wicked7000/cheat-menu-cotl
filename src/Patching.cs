using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Lamb.UI.FollowerSelect;
using Lamb.UI;
using src.Extensions;
using static cheat_menu.Singleton;
using System.Collections;
using UnityEngine;
using static StructuresData;

namespace cheat_menu;

public class Patching {
    private static Harmony harmonyInstance = null;

    private static Action<bool, NotificationData> maxFaithDelegate = delegate (bool b, NotificationData notif)
        {
            if (Instance.IsFlagEnabled(CheatFlags.MaxFaithMode) && CultUtils.GetCurrentFaith() < 85f)
            {
                CultUtils.ModifyFaith(85f, "Setting faith to max (MaxFaithMode)", true);
            }
        };

public static void PatchAll()
    {
        harmonyInstance = Harmony.CreateAndPatchAll(typeof(Patching));

        // Patch to allow for max faith mode! 
        var onUpdateFaith = Traverse.Create(typeof(CultFaithManager)).Field("OnUpdateFaith").GetValue<Action<bool, NotificationData>>();
        Instance.cultFaithManager.Field("OnUpdateFaith").SetValue(Action.Combine(onUpdateFaith, maxFaithDelegate));
    }

    public static void UnpatchAll()
    {
        harmonyInstance.UnpatchSelf();
        var onUpdateFaith = Traverse.Create(typeof(CultFaithManager)).Field("OnUpdateFaith").GetValue<Action<bool, NotificationData>>();
        Instance.cultFaithManager.Field("OnUpdateFaith").SetValue(Action.Remove(onUpdateFaith, maxFaithDelegate));
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

