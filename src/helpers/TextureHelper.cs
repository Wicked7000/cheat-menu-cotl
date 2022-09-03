using UnityEngine;
using System;
using System.Collections.Generic;

namespace cheat_menu;

public class TextureHelper {
    private static Dictionary<string, Texture2D> textureHelper;

    [Init]
    private static void Init(){
        textureHelper = new Dictionary<string, Texture2D>();
    }

    [Unload]
    private static void Unload(Action<UnityEngine.Object> destroyFn){
        foreach(var value in textureHelper.Values){
            destroyFn(value);
        }
        
        textureHelper.Clear();
    }

    private static void SaveTexture(Texture2D tex, Color color){
        textureHelper[GetTextureKey(color)] = tex;
    }

    private static Texture2D GetCachedTexture(Color color){
        string textureKey = GetTextureKey(color);
        Texture2D cachedTexture;
        if(textureHelper.TryGetValue(textureKey, out cachedTexture)){
            return cachedTexture;
        }
        return null;
    }

    private static string GetTextureKey(Color color){
        return $"{color.r}-{color.g}-{color.b}-${color.a}";
    }

    public static Texture2D GetSolidTexture(Color color)
    {
        Texture2D cached = GetCachedTexture(color);
        if(cached != null){
            return cached;
        }

        Texture2D tex = new Texture2D(100, 100);
        Color[] pixels = tex.GetPixels();
        for (var i = 0; i < pixels.Length; i += 1)
        {
            pixels[i] = color;
        }
        tex.SetPixels(pixels);
        tex.Apply();

        SaveTexture(tex, color);
        return tex;
    }
}