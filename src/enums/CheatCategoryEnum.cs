using System.Reflection;
using System.Collections.Generic;
using System;
using UnityAnnotationHelpers;

namespace CheatMenu;

public static class CheatCategoryEnumExtensions {
    private static Dictionary<CheatCategoryEnum, string> s_forwardsCache = new();
    private static Dictionary<string, CheatCategoryEnum> s_backwardsCache = new();

    [Init]
    public static void Init(){
        s_forwardsCache = new Dictionary<CheatCategoryEnum, string>();
        s_backwardsCache = new Dictionary<string, CheatCategoryEnum>();
    }

    public static string GetCategoryName(this CheatCategoryEnum enumValue){
        if (s_forwardsCache.TryGetValue(enumValue, out string enumName))
        {
            return enumName;
        }

        StringEnum stringEnumValue = ReflectionHelper.GetAttributeOfTypeEnum<StringEnum>(enumValue);
        if(stringEnumValue == null){
            throw new Exception("Expected StringEnum on CheatCategory enum but not found!");
        }

        s_forwardsCache[enumValue] = stringEnumValue.Value;
        return stringEnumValue.Value;
    }

    public static CheatCategoryEnum GetEnumFromName(string name){
        if (s_backwardsCache.TryGetValue(name, out CheatCategoryEnum enumValue))
        {
            return enumValue;
        }

        Type enumType = typeof(CheatCategoryEnum);
        FieldInfo[] fields = enumType.GetFields();
        foreach(var member in fields){
            StringEnum stringEnumAnnotation = (StringEnum)member.GetCustomAttribute(typeof(StringEnum));
            if(stringEnumAnnotation != null && stringEnumAnnotation.Value == name){
                CheatCategoryEnum enumVal = (CheatCategoryEnum)member.GetValue(null);
                s_backwardsCache[name] = enumVal;
                return enumVal;
            }
        }

        s_backwardsCache[name] = CheatCategoryEnum.NONE;
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