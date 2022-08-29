using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cheat_menu
{
    public static class Utils
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

    }
}
