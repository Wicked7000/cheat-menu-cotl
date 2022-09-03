using BepInEx;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using static cheat_menu.Singleton;

namespace cheat_menu;

[BepInPlugin("org.wicked.cheat_menu", "Cheat Menu", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    private CheatConfig config;
    private Action UpdateFn = null;
    private Action OnGUIFn = null;

    //Init
    private void Awake()
    {        
        config = new CheatConfig(Config);
        ReflectionHelper.InvokeAllWithAnnotation(typeof(Init));

        OnGUIFn = ReflectionHelper.BuildCallAllFunction(typeof(OnGUI));
        UpdateFn = ReflectionHelper.BuildCallAllFunction(typeof(Update));
        UnityEngine.Debug.Log("CheatMenu patching and loading completed!");
    }

    //Unload
    private void OnDisable()
    {
        Action<UnityEngine.Object> destroyFn = delegate(UnityEngine.Object obj) { Destroy(obj); };

        ReflectionHelper.InvokeAllWithAnnotation(typeof(Unload), new object[]{destroyFn});
    }

    private void OnGUI()
    {
        //Call all functions with OnGUI
        OnGUIFn();
    }

    private void Update()
    {        
        //Call all function with Update
        UpdateFn();
    }
}
