using System;

namespace CheatMenu;

public class CheatCategory : Attribute {
    private readonly CheatCategoryEnum _category;

    public CheatCategory(CheatCategoryEnum enumValue){
        this._category = enumValue;
    }

    public virtual CheatCategoryEnum Category {
        get {return _category;}
    }
}