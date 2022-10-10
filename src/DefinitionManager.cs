using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CheatMenu;

public static class DefinitionManager{
    public static List<Definition> GetAllCheatMethods(){
        List<Definition> methodsRet = new();
        
        foreach(var classDef in ReflectionHelper.GetLoadableTypes(typeof(DefinitionManager).Assembly)){
            if(typeof(IDefinition).IsAssignableFrom(classDef) && classDef.IsClass){
                CheatCategory category = ReflectionHelper.HasAttribute<CheatCategory>(classDef);
                MethodInfo[] methods = classDef.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach(var method in methods){
                    if(Definition.IsCheatMethod(method)){
                        Definition newDef = new(method, category.Category);
                        methodsRet.Add(newDef);
                    }
                }
            }
        }

        return methodsRet;
    }

    public static Dictionary<string, Definition> CheatFunctionToDetails(List<Definition> allCheats){
        Dictionary<string, Definition> cheatFunctionToDetails = new();

        foreach(var cheat in allCheats){
            if(cheatFunctionToDetails.ContainsKey(cheat.MethodInfo.Name)){
                throw new Exception($"MethodInfo conflict with name {cheat.MethodInfo.Name}, please fix!");
            }
            cheatFunctionToDetails[cheat.MethodInfo.Name] = cheat;
        }

        return cheatFunctionToDetails;
    }

    public static Dictionary<CheatCategoryEnum, List<Definition>> GroupCheatsByCategory(List<Definition> allCheats){
        Dictionary<CheatCategoryEnum, List<Definition>> categoryCheats = new();

        foreach(var cheat in allCheats){
            if (!categoryCheats.TryGetValue(cheat.CategoryEnum, out List<Definition> defs))
            {
                defs = new List<Definition>();
            }
            defs.Add(cheat);
            categoryCheats[cheat.CategoryEnum] = defs;
        }

        //Sort all cheat groups
        foreach(var cheatGroup in categoryCheats){
            cheatGroup.Value.Sort(delegate(Definition a, Definition b) {
                return String.Compare(a.Details.Title, b.Details.Title);
            });
        }

        return categoryCheats;
    }

    public static Action BuildGUIContentFn(){
        DynamicMethod guiContentMethod = new("", typeof(void), new Type[]{});
        
        //Method defs we need to use in the DynamicMethod
        var guiUtilsCategoryButton = typeof(CheatMenuGui).GetMethod("CategoryButton", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButton = typeof(CheatMenuGui).GetMethod("Button", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButtonWithFlagSimple = typeof(CheatMenuGui).GetMethod("ButtonWithFlagS", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButtonWithFlag = typeof(CheatMenuGui).GetMethod("ButtonWithFlag", BindingFlags.Static | BindingFlags.Public);
        var isWithinCategory = typeof(CheatMenuGui).GetMethod("IsWithinCategory", BindingFlags.Static | BindingFlags.Public);
        var isWithinSpecificCategory = typeof(CheatMenuGui).GetMethod("IsWithinSpecificCategory", BindingFlags.Static | BindingFlags.Public);
        var isFlagEnabledStr = typeof(FlagManager).GetMethod("IsFlagEnabledStr", BindingFlags.Static | BindingFlags.Public);
        var backButton = typeof(CheatMenuGui).GetMethod("BackButton", BindingFlags.Static | BindingFlags.Public);

        var ilGenerator = guiContentMethod.GetILGenerator();

        List<Definition> methods = GetAllCheatMethods();
        Dictionary<CheatCategoryEnum, List<Definition>> groupedCheats = GroupCheatsByCategory(methods);

        Label startOfInnerCategoryButtons = ilGenerator.DefineLabel();  
        Label endOfFunction = ilGenerator.DefineLabel();

        ilGenerator.EmitCall(OpCodes.Call, isWithinCategory, null); // [] -> [bool];
        ilGenerator.Emit(OpCodes.Brtrue, startOfInnerCategoryButtons);
        
        foreach(var category in groupedCheats.Keys){
            ilGenerator.Emit(OpCodes.Ldstr, category.GetCategoryName()); // [] -> ["category"]
            ilGenerator.EmitCall(OpCodes.Call, guiUtilsCategoryButton, null); // ["category"] -> [bool]
            ilGenerator.Emit(OpCodes.Pop); // [bool] -> []
        }
        ilGenerator.Emit(OpCodes.Br, endOfFunction); // Always jump to end if we rendered category buttons
        
        ilGenerator.MarkLabel(startOfInnerCategoryButtons);
        ilGenerator.EmitCall(OpCodes.Call, backButton, null);
        ilGenerator.Emit(OpCodes.Pop);
        foreach(var group in groupedCheats){           
            foreach(var def in group.Value){     
                if(def.IsWIPCheat && !CheatUtils.IsDebugMode){
                    //Don't include WIP cheats in release builds!
                } else {
                    Label endOfElem = ilGenerator.DefineLabel();          

                    ilGenerator.Emit(OpCodes.Ldstr, def.CategoryName); // [] -> ["category"]
                    ilGenerator.EmitCall(OpCodes.Call, isWithinSpecificCategory, null); // ["category"] -> [bool]
                    ilGenerator.Emit(OpCodes.Brfalse, endOfElem); // [bool] -> [] (will skip cheat if we aren't in correct category)

                    if(!def.Details.IsMultiNameFlagCheat){
                        ilGenerator.Emit(OpCodes.Ldstr, def.Details.Title); // [] -> ["Title"]
                        if(!def.IsModeCheat){
                            ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButton, null); // ["title"] -> [bool]
                        } else {
                            ilGenerator.Emit(OpCodes.Ldstr, def.FlagName); // ["title"] -> ["title", "flag"]
                            ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButtonWithFlagSimple, null); // ["title", "flag"] -> [bool]
                        }                        
                    } else {
                        ilGenerator.Emit(OpCodes.Ldstr, def.Details.OnTitle); // [] -> ["OnTitle"]
                        ilGenerator.Emit(OpCodes.Ldstr, def.Details.OffTitle); // ["OffTitle"] -> ["OnTitle", "OffTitle"]                        
                        ilGenerator.Emit(OpCodes.Ldstr, def.FlagName); // ["OnTitle", "OffTitle"] -> ["OnTitle", "OffTitle", "flag"]
                        ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButtonWithFlag, null); // ["OnTitle", "OffTitle", "flag"] -> [bool]
                    }
                    
                    ilGenerator.Emit(OpCodes.Brfalse, endOfElem); // [bool] -> []
                    if(def.MethodInfo.GetParameters().Length == 1 && def.IsModeCheat){
                        ilGenerator.Emit(OpCodes.Ldstr, def.FlagName); // [] -> ["flag"]
                        ilGenerator.EmitCall(OpCodes.Call, isFlagEnabledStr, null); // ["flag"] -> [bool]
                    }
                    ilGenerator.EmitCall(OpCodes.Call, def.MethodInfo, null); // ([] | [bool]) -> []
                    ilGenerator.MarkLabel(endOfElem);
                }
            }
        }
        ilGenerator.MarkLabel(endOfFunction);
        ilGenerator.Emit(OpCodes.Ret);
                        
        Action delegateFn = (Action)guiContentMethod.CreateDelegate(typeof(Action));
        return delegateFn;
    }
}