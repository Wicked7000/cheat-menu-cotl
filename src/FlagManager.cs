using System.Collections.Generic;
using UnityAnnotationHelpers;

namespace CheatMenu;

public sealed class FlagManager
{
    private Dictionary<string, bool> _cheatFlags = new();

    static FlagManager(){}
    private FlagManager(){}

    [Init]
    [EnforceOrderFirst(10)]
    public void Init(){
        _cheatFlags = new Dictionary<string, bool>();
    }

    public static void SetFlagValue(string flagID, bool value){
        Instance._cheatFlags[flagID] = value;
    }

    public static bool IsFlagEnabledStr(string flagID)
    {
        Instance._cheatFlags.TryGetValue(flagID, out bool flag);
        return flag;
    }

    public static bool IsFlagEnabled(string flagID)
    {
        Instance._cheatFlags.TryGetValue(flagID, out bool flag);
        return flag;
    }

    public static void FlipFlagValue(string flagID)
    {
        Instance._cheatFlags.TryGetValue(flagID, out bool flag);
        Instance._cheatFlags[flagID] = !flag;
    }

    public static FlagManager Instance { get; } = new FlagManager();
}