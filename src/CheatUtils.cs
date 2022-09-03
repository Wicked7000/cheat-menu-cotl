using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cheat_menu;

public static class CheatUtils
{
    //Will not clone references of the items themselves just the list
    public static List<T> cloneList<T>(List<T> list)
    {
        List<T> clone = new List<T>();
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

