using cheat_menu;
using System.Reflection;
using System.Collections.Generic;
using System;

public static class CheatCategoryEnumExtensions {
    private static Dictionary<CheatCategoryEnum, string> forwardsCache = new Dictionary<CheatCategoryEnum, string>();
    private static Dictionary<string, CheatCategoryEnum> backwardsCache = new Dictionary<string, CheatCategoryEnum>();

    [Init]
    private static void Init(){
        forwardsCache = new Dictionary<CheatCategoryEnum, string>();
        backwardsCache = new Dictionary<string, CheatCategoryEnum>();
    }

    public static string GetCategoryName(this CheatCategoryEnum enumValue){
        string enumName;
        if(forwardsCache.TryGetValue(enumValue, out enumName)){
            return enumName;
        }

        StringEnum stringEnumValue = ReflectionHelper.GetAttributeOfTypeEnum<StringEnum>(enumValue);
        if(stringEnumValue == null){
            throw new Exception("Expected StringEnum on CheatCategory enum but not found!");
        }

        forwardsCache[enumValue] = stringEnumValue.Value;
        return stringEnumValue.Value;
    }

    public static CheatCategoryEnum GetEnumFromName(string name){
        CheatCategoryEnum enumValue;
        if(backwardsCache.TryGetValue(name, out enumValue)){
            return enumValue;
        }

        Type enumType = typeof(CheatCategoryEnum);
        FieldInfo[] fields = enumType.GetFields();
        foreach(var member in fields){
            StringEnum stringEnumAnnotation = (StringEnum)member.GetCustomAttribute(typeof(StringEnum));
            if(stringEnumAnnotation != null && stringEnumAnnotation.Value == name){
                CheatCategoryEnum enumVal = (CheatCategoryEnum)member.GetValue(null);
                backwardsCache[name] = enumVal;
                return enumVal;
            }
        }

        backwardsCache[name] = CheatCategoryEnum.NONE;
        return CheatCategoryEnum.NONE;
    }
}

public enum CheatCategoryEnum {
    [StringEnum("NONE")]
    NONE,

    [StringEnum("Resources")]
    RESOURCE,
    
    [StringEnum("Rituals")]
    RITUALS,

    [StringEnum("Health")]
    HEALTH,

    [StringEnum("Structures")]
    STRUCTURES,

    [StringEnum("Weather")]
    WEATHER,

    [StringEnum("Follower")]
    FOLLOWER,

    [StringEnum("Cult")]
    CULT,

    [StringEnum("Misc")]
    MISC
}