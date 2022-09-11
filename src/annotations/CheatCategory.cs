using System;

namespace cheat_menu;

public class CheatCategory : Attribute {
    private CheatCategoryEnum category;

    public CheatCategory(CheatCategoryEnum enumValue){
        this.category = enumValue;
    }

    public virtual CheatCategoryEnum Category {
        get {return category;}
    }
}