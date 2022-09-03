using System;
using static cheat_menu.Singleton;

namespace cheat_menu;

public class CheatFlag : Attribute {
    private CheatFlags flag;

    public CheatFlag(CheatFlags flag){
        this.flag = flag;
    }

    public virtual CheatFlags Flag {
        get { return flag; }
    }
}