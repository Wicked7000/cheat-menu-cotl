using System.Reflection;
using System.Reflection.Emit;
using System;
using System.Collections.Generic;

namespace cheat_menu;

public class ReflectionHelper {
    public static T HasAttribute<T>(MethodInfo method) where T : Attribute {
        return (T)method.GetCustomAttribute(typeof(T));
    }

    public static List<MethodInfo> GetAllMethodsInAssemblyWithAnnotation(Type annotationType){
        List<MethodInfo> endMethods = new List<MethodInfo>();
        List<MethodInfo> methods = new List<MethodInfo>();
        Type[] executionTypes = Assembly.GetExecutingAssembly().GetTypes();
        
        foreach(Type innerType in executionTypes){
            MethodInfo[] innerMethodsStatic = innerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo[] innerMethodsNonStatic = innerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo[] combinedInnerMethods = CheatUtils.Concat(innerMethodsNonStatic, innerMethodsStatic);
            foreach(MethodInfo method in combinedInnerMethods){
                MethodInfo hasAttributeCustom = typeof(ReflectionHelper).GetMethod("HasAttribute")
                             .MakeGenericMethod(new Type[] { annotationType });
                object returnAttribute = hasAttributeCustom.Invoke(null, new object[]{method});
                if(returnAttribute != null){
                    EnforceOrderLast enforceLast = HasAttribute<EnforceOrderLast>(method);
                    if(enforceLast == null){
                        methods.Add(method);
                    } else {
                        endMethods.Add(method);
                    }
                }
            }
        }

        endMethods.Sort(delegate(MethodInfo a, MethodInfo b){
            return a.Name.CompareTo(b.Name);
        });
        methods.AddRange(endMethods);

        return methods;
    }

    public static void InvokeAllWithAnnotation(Type annotationType, object[] parameters = null){
        List<MethodInfo> methods = ReflectionHelper.GetAllMethodsInAssemblyWithAnnotation(annotationType);
        foreach(var method in methods){            
            object[] methodParams = null;
            if(parameters != null && method.GetParameters().Length > 0){
                methodParams = new object[method.GetParameters().Length];
                for(int i = 0; i < method.GetParameters().Length; i++){
                    methodParams[i]  = parameters[i];
                }
            }

            if(method.IsStatic){
                method.Invoke(null, methodParams);
            } else {
                object instance = method.DeclaringType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                method.Invoke(instance, methodParams);
            }            
        }
    }

    public static Action BuildCallAllFunction(Type annotation){
        DynamicMethod callAllFunction = new DynamicMethod("", typeof(void), new Type[]{});
        var ilGenerator = callAllFunction.GetILGenerator();

        List<MethodInfo> methods = ReflectionHelper.GetAllMethodsInAssemblyWithAnnotation(annotation);
        foreach(var defMethod in methods){
            ilGenerator.EmitCall(OpCodes.Call, defMethod, null); // [] -> [?]

        }
        ilGenerator.Emit(OpCodes.Ret);
                        
        Action delegateFn = (Action)callAllFunction.CreateDelegate(typeof(Action));
        return delegateFn;
    }
}