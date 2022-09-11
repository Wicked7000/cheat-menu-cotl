using System.Reflection;
using static cheat_menu.Singleton;

namespace cheat_menu;

public class Definition{
    private MethodInfo info;
    private CheatCategoryEnum category;
    private string categoryName;

    private CheatDetails details;
    private CheatWIP cheatWIP;
    private CheatFlag flag;
    
    public static bool IsCheatMethod(MethodInfo info){
        CheatDetails details = ReflectionHelper.HasAttribute<CheatDetails>(info);
        return details != null;
    }

    public Definition(MethodInfo info, CheatCategoryEnum category){
        this.info = info;
        this.category = category;

        this.categoryName = category.GetCategoryName();
        this.details = ReflectionHelper.HasAttribute<CheatDetails>(info);
        this.cheatWIP = ReflectionHelper.HasAttribute<CheatWIP>(info);
        this.flag = ReflectionHelper.HasAttribute<CheatFlag>(info);                
    }

    public virtual CheatCategoryEnum CategoryEnum {
        get {return category;}
    }

    public virtual string CategoryName {
        get {return categoryName;}
    }

    public virtual MethodInfo MethodInfo {
        get {return info;}
    }

    public virtual bool IsWIPCheat{
        get {return cheatWIP != null;}
    }

    public virtual CheatDetails Details {
        get {return details;}
    }

    public virtual bool IsModeCheat {
        get {return flag != null;}
    }

    public virtual CheatFlags Flag {
        get {return flag.Flag;}
    }
}