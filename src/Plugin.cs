using BepInEx;
using System;
using UnityAnnotationHelpers;

namespace CheatMenu;

[BepInPlugin("org.wicked.cheat_menu", "Cheat Menu", "1.0.3")]
public class Plugin : BaseUnityPlugin
{    
    private UnityAnnotationHelper _annotationHelper;
    private Action _updateFn = null;
    private Action _onGUIFn = null;

    public void Awake()
    {        
        new CheatConfig(Config);

        _annotationHelper = new UnityAnnotationHelper();
        _annotationHelper.RunAllInit();

        _onGUIFn = _annotationHelper.BuildRunAllOnGuiDelegate();
        _updateFn = _annotationHelper.BuildRunAllUpdateDelegate();
        UnityEngine.Debug.Log("CheatMenu patching and loading completed!");
    }

    public void OnDisable()
    {
        _annotationHelper.RunAllUnload();
    }

    public void OnGUI()
    {
        _onGUIFn();
    }

    public void Update()
    {        
        _updateFn();
    }
}
