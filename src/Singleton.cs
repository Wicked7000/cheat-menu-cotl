using System.Collections.Generic;
using System;
using HarmonyLib;

namespace cheat_menu;
public sealed class Singleton
{
    private static readonly Singleton instance = new Singleton();
    public bool guiEnabled = false;
    public Traverse cultFaithManager;
    public Traverse cheatConsoleInstance = Traverse.Create(typeof(CheatConsole));
    private Dictionary<CheatFlags, bool> cheatFlags = new Dictionary<CheatFlags, bool>();

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

    [Init]
    private void Init()
    {
        cultFaithManager = Traverse.Create(CultFaithManager.Instance);
        cheatConsoleInstance = Traverse.Create(typeof(CheatConsole));
    }

    [Unload]
    private void Reset(){
        cultFaithManager = null;
        cheatConsoleInstance = null;
    }

    public static bool IsFlagEnabledStr(string flagID)
    {
        CheatFlags flagEnum = (CheatFlags)Enum.Parse(typeof(CheatFlags), flagID);
        bool flag = false;
        Singleton.Instance.cheatFlags.TryGetValue(flagEnum, out flag);
        return flag;
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