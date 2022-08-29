using System.Collections.Generic;
using UnityEngine;
using cheat_menu;
using HarmonyLib;

namespace cheat_menu;
public sealed class Singleton
{
    private static readonly Singleton instance = new Singleton();
    public bool guiEnabled = false;
    public int totalWindowSize = 0;
    public Traverse cultFaithManager;
    public Traverse cheatConsoleInstance = Traverse.Create(typeof(CheatConsole));

    public enum CheatFlags
    {
        HidingUI,
        FollowerDebug,
        FPSDebug,
        FreeBuildingMode,
        StructureDebug,
        RitualsAllUnlock,
        NoRitualCooldown,
        GodMode,
        NoClip,
        MaxFaithMode
    }

    public void Init()
    {
        cultFaithManager = Traverse.Create(CultFaithManager.Instance);
    }

    public bool IsFlagEnabled(Singleton.CheatFlags flagID)
    {
        bool flag = false;
        Singleton.Instance.cheatFlags.TryGetValue(flagID, out flag);
        return flag;
    }
    public void FlipFlagValue(Singleton.CheatFlags flagID)
    {
        bool flag = false;
        Singleton.Instance.cheatFlags.TryGetValue(flagID, out flag);
        Singleton.Instance.cheatFlags[flagID] = !flag;
    }

    public Dictionary<CheatFlags, bool> cheatFlags = new Dictionary<CheatFlags, bool>();

    static Singleton()
    {
    }

    private Singleton()
    {
    }

    public static Singleton Instance
    {
        get
        {
            return instance;
        }
    }
}