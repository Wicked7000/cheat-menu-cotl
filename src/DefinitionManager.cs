using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace cheat_menu;

public static class DefinitionManager{
    public static List<Definition> GetAllCheatMethods(){
        List<Definition> methodsRet = new List<Definition>();

        foreach(var classDef in ReflectionHelper.GetLoadableTypes(typeof(DefinitionManager).Assembly)){
            if(typeof(IDefinition).IsAssignableFrom(classDef) && classDef.IsClass){
                CheatCategory category = ReflectionHelper.HasAttribute<CheatCategory>(classDef);
                MethodInfo[] methods = classDef.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach(var method in methods){
                    if(Definition.IsCheatMethod(method)){
                        Definition newDef = new Definition(method, category.Category);
                        methodsRet.Add(newDef);
                    }
                }
            }
        }

        return methodsRet;
    }

    public static Dictionary<string, Definition> CheatFunctionToDetails(List<Definition> allCheats){
        Dictionary<string, Definition> cheatFunctionToDetails = new Dictionary<string, Definition>();

        foreach(var cheat in allCheats){
            if(cheatFunctionToDetails.ContainsKey(cheat.MethodInfo.Name)){
                throw new Exception($"MethodInfo conflict with name {cheat.MethodInfo.Name}, please fix!");
            }
            cheatFunctionToDetails[cheat.MethodInfo.Name] = cheat;
        }

        return cheatFunctionToDetails;
    }

    public static Dictionary<CheatCategoryEnum, List<Definition>> GroupCheatsByCategory(List<Definition> allCheats){
        Dictionary<CheatCategoryEnum, List<Definition>> categoryCheats = new Dictionary<CheatCategoryEnum, List<Definition>>();

        foreach(var cheat in allCheats){
            List<Definition> defs = null;
            if(!categoryCheats.TryGetValue(cheat.CategoryEnum, out defs)){
                defs = new List<Definition>();
            }
            defs.Add(cheat);
            categoryCheats[cheat.CategoryEnum] = defs;
        }

        return categoryCheats;
    }

    public static Action BuildGUIContentFn(){
        DynamicMethod guiContentMethod = new DynamicMethod("", typeof(void), new Type[]{});
        
        //Method defs we need to use in the DynamicMethod
        var guiUtilsCategoryButton = typeof(GUIUtils).GetMethod("CategoryButton", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButton = typeof(GUIUtils).GetMethod("Button", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButtonWithFlagSimple = typeof(GUIUtils).GetMethod("ButtonWithFlagS", BindingFlags.Static | BindingFlags.Public);
        var guiUtilsButtonWithFlag = typeof(GUIUtils).GetMethod("ButtonWithFlag", BindingFlags.Static | BindingFlags.Public);
        var IsWithinCategory = typeof(GUIUtils).GetMethod("IsWithinCategory", BindingFlags.Static | BindingFlags.Public);
        var IsWithinSpecificCategory = typeof(GUIUtils).GetMethod("IsWithinSpecificCategory", BindingFlags.Static | BindingFlags.Public);
        var IsFlagEnabledStr = typeof(Singleton).GetMethod("IsFlagEnabledStr", BindingFlags.Static | BindingFlags.Public);
        var InvertBoolRef = typeof(CheatUtils).GetMethod("InvertBool", BindingFlags.Static | BindingFlags.Public);
        var BackButton = typeof(GUIUtils).GetMethod("BackButton", BindingFlags.Static | BindingFlags.Public);

        var ilGenerator = guiContentMethod.GetILGenerator();

        List<Definition> methods = GetAllCheatMethods();
        Dictionary<CheatCategoryEnum, List<Definition>> groupedCheats = GroupCheatsByCategory(methods);

        Label startOfInnerCategoryButtons = ilGenerator.DefineLabel();  
        Label endOfFunction = ilGenerator.DefineLabel();

        ilGenerator.EmitCall(OpCodes.Call, IsWithinCategory, null); // [] -> [bool];
        ilGenerator.Emit(OpCodes.Brtrue, startOfInnerCategoryButtons);
        
        foreach(var category in groupedCheats.Keys){
            ilGenerator.Emit(OpCodes.Ldstr, category.GetCategoryName()); // [] -> ["category"]
            ilGenerator.EmitCall(OpCodes.Call, guiUtilsCategoryButton, null); // ["category"] -> [bool]
            ilGenerator.Emit(OpCodes.Pop); // [bool] -> []
        }
        ilGenerator.Emit(OpCodes.Br, endOfFunction); // Always jump to end if we rendered category buttons
        
        ilGenerator.MarkLabel(startOfInnerCategoryButtons);
        ilGenerator.EmitCall(OpCodes.Call, BackButton, null);
        ilGenerator.Emit(OpCodes.Pop);
        foreach(var group in groupedCheats){           
            foreach(var def in group.Value){     
                if(def.IsWIPCheat && !CheatUtils.IsDebugMode){
                    //Don't include WIP cheats in release builds!
                } else {
                    Label endOfElem = ilGenerator.DefineLabel();          

                    ilGenerator.Emit(OpCodes.Ldstr, def.CategoryName); // [] -> ["category"]
                    ilGenerator.EmitCall(OpCodes.Call, IsWithinSpecificCategory, null); // ["category"] -> [bool]
                    ilGenerator.Emit(OpCodes.Brfalse, endOfElem); // [bool] -> [] (will skip cheat if we aren't in correct category)

                    if(!def.Details.IsMultiNameFlagCheat){
                        ilGenerator.Emit(OpCodes.Ldstr, def.Details.Title); // [] -> ["Title"]
                        if(!def.IsModeCheat){
                            ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButton, null); // ["title"] -> [bool]
                        } else {
                            ilGenerator.Emit(OpCodes.Ldstr, def.Flag.ToString()); // ["title"] -> ["title", "flag"]
                            ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButtonWithFlagSimple, null); // ["title", "flag"] -> [bool]
                        }                        
                    } else {
                        ilGenerator.Emit(OpCodes.Ldstr, def.Details.OffTitle); // [] -> ["OffTitle"]
                        ilGenerator.Emit(OpCodes.Ldstr, def.Details.OnTitle); // ["OffTitle"] -> ["OffTitle", "OnTitle"]                        
                        ilGenerator.Emit(OpCodes.Ldstr, def.Flag.ToString()); // ["OnTitle", "OffTitle"] -> ["OnTitle", "OffTitle", "flag"]
                        ilGenerator.EmitCall(OpCodes.Callvirt, guiUtilsButtonWithFlag, null); // ["OnTitle", "OffTitle", "flag"] -> [bool]
                    }
                    
                    ilGenerator.Emit(OpCodes.Brfalse, endOfElem); // [bool] -> []
                    if(def.MethodInfo.GetParameters().Length == 1 && def.IsModeCheat){
                        ilGenerator.Emit(OpCodes.Ldstr, def.Flag.ToString()); // [] -> ["flag"]
                        ilGenerator.EmitCall(OpCodes.Call, IsFlagEnabledStr, null); // ["flag"] -> [bool]
                        ilGenerator.EmitCall(OpCodes.Call, InvertBoolRef, null); // [bool] -> [!bool]
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