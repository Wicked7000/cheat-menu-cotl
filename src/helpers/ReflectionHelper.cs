using System.Reflection;
using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using UnityAnnotationHelpers;

namespace CheatMenu;

public static class ReflectionHelper {
    private class PatchTrackerDetails {
        public MethodInfo OriginalMethod;
        public HarmonyPatchType PatchType;
        
        public PatchTrackerDetails(MethodInfo originalMethod, HarmonyPatchType patchType){
            OriginalMethod = originalMethod;
            PatchType = patchType;
        }
    }

    private readonly static string HarmonyId = "com.wicked.cheat_menu";
    private static Harmony s_harmonyInstance;
    private static Dictionary<string, PatchTrackerDetails> s_patchTracker;

    [Init]
    [EnforceOrderFirst(9)]
    public static void Init(){
        s_harmonyInstance = new Harmony(HarmonyId);
        s_patchTracker = new Dictionary<string, PatchTrackerDetails>();
    }

    [Unload]
    public static void Unload(){
        s_harmonyInstance.UnpatchSelf();
    }

    private static string GetPatchTrackerKey(Type classDef, string methodName){
        return $"{classDef.Name}-{methodName}";
    }

    private static string TrackPatch(Type classDef, MethodInfo method, HarmonyPatchType patchType){
        string patchKey = GetPatchTrackerKey(classDef, method.Name);
        s_patchTracker[patchKey] = new PatchTrackerDetails(method, patchType);
        return patchKey;
    }

    public static MethodBase GetCallingMethod(){
        return new StackFrame(2).GetMethod();
    }

    public static MethodBase GetFirstMethodInHierarchyWithAnnotation<T>() where T : Attribute{
        StackTrace trace = new();
        StackFrame[] frames = trace.GetFrames();
        foreach(var frame in frames){
            UnityEngine.Debug.Log($"frame: {frame.GetMethod().Name}");
            T attributeValue = HasAttribute<T>(frame.GetMethod());
            if(attributeValue != null){
                return frame.GetMethod();
            }
        }
        return null;
    }

    public static void UnpatchTracked(Type classDef, String methodName){
        string patchKey = GetPatchTrackerKey(classDef, methodName);
        if (s_patchTracker.TryGetValue(patchKey, out PatchTrackerDetails patchTrackedDetails))
        {
            s_harmonyInstance.Unpatch(patchTrackedDetails.OriginalMethod, patchTrackedDetails.PatchType, HarmonyId);
            UnityEngine.Debug.Log($"[ReflectionHelper] {classDef.Name}-{methodName} was unpatched to original state.");
        }
    }

    public static T GetAttributeOfTypeEnum<T>(Enum value){
        Type enumType = value.GetType();
        MemberInfo[] memInfo = enumType.GetMember(value.ToString());
        object[] attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return (attributes.Length > 0) ? (T)attributes[0] : default;
    }

    public static T HasAttribute<T>(Type type) where T : Attribute {
        return (T)type.GetCustomAttribute(typeof(T));
    }

    public static T HasAttribute<T>(MethodBase method) where T : Attribute {
        return (T)method.GetCustomAttribute(typeof(T));
    }

    public static T HasAttribute<T>(MethodInfo method) where T : Attribute {
        return (T)method.GetCustomAttribute(typeof(T));
    }

    public static List<Type> GetLoadableTypes(Assembly assembly){
        try
        {
            return assembly.GetTypes().ToList();
        }
        catch(ReflectionTypeLoadException e){
            return e.Types.Where(t => t != null).ToList();
        }
    }

    public static string PatchMethodPrefix(Type classDef, string methodName, MethodInfo patchMethod, BindingFlags flags = BindingFlags.Default, Type[] typeParams = null){
        if(patchMethod == null){
            UnityEngine.Debug.Log($"[ReflectionHelper] Can't patch method, passed patchMethod is null!");
            return null;
        }
        
        MethodInfo methodInfo;        
        if(typeParams == null){
            //Without passing any type params
            methodInfo = classDef.GetMethod(
                methodName, 
                flags
            );
        } else {
            methodInfo = classDef.GetMethod(
                methodName, 
                flags,
                Type.DefaultBinder, 
                typeParams, 
                null
            );
        }
         
        if(methodInfo == null){
            UnityEngine.Debug.LogError($"[ReflectionHelper] Method was not patched, unable to find method info {methodName} (Report To Wicked!)");
            return null;
        }

        s_harmonyInstance.Patch(methodInfo, prefix: new HarmonyMethod(patchMethod));
        UnityEngine.Debug.Log($"[ReflectionHelper] {classDef.Name}-{methodName} was patched with cheat replacement: {patchMethod.Name}");
        return TrackPatch(classDef, methodInfo, HarmonyPatchType.Prefix);
    }

    public static MethodInfo GetMethodStaticPublic(string name){   
        UnityEngine.Debug.Log(GetCallingMethod().DeclaringType);
        return GetCallingMethod().DeclaringType.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
    }

    public static MethodInfo GetMethodStaticPublic(Type type, string name){
        return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
    }

    public static MethodInfo GetMethodStaticPrivate(string name){        
        return GetCallingMethod().DeclaringType.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
    }

    public static MethodInfo GetMethodStaticPrivate(Type type, string name){
        return type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
    }

    public static List<MethodInfo> GetAllMethodsInAssemblyWithAnnotation(Type annotationType){
        List<MethodInfo> methods = new();
        Type[] executionTypes = Assembly.GetExecutingAssembly().GetTypes();
        
        foreach(Type innerType in executionTypes){
            MethodInfo[] innerMethodsStatic = innerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo[] innerMethodsNonStatic = innerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo[] combinedInnerMethods = CheatUtils.Concat(innerMethodsNonStatic, innerMethodsStatic);
            foreach(MethodInfo method in combinedInnerMethods){
                MethodInfo hasAttributeCustom = typeof(ReflectionHelper).GetMethod("HasAttribute", new Type[]{typeof(MethodInfo)})
                             .MakeGenericMethod(new Type[] { annotationType });
                object returnAttribute = hasAttributeCustom.Invoke(null, new object[]{method});
                if(returnAttribute != null){
                    methods.Add(method);
                }
            }
        }

        return methods;
    }
}