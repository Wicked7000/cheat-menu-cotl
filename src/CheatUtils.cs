using System.Collections.Generic;

namespace CheatMenu;

public static class CheatUtils
{
    //Will not clone references of the items themselves just the list
    public static List<T> CloneList<T>(List<T> list)
    {
        List<T> clone = new();
        foreach(var item in list){
            clone.Add(item);
        }
        return clone;
    }

    public static bool IsDebugMode {
        get {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }        
    }

    public static bool InvertBool(bool value){
        return !value;
    }

    public static T[] Concat<T>(T[] arrayOne, T[] arrayTwo){
        T[] combined = new T[arrayOne.Length + arrayTwo.Length];
        arrayOne.CopyTo(combined, 0);
        arrayTwo.CopyTo(combined, arrayOne.Length);
        return combined;
    }
}

