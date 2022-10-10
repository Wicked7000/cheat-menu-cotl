using UnityEngine;
using System;
using System.Collections.Generic;
using UnityAnnotationHelpers;

namespace CheatMenu;

public class TextureHelper {
    private static Dictionary<string, Texture2D> s_textureHelper;

    [Init]
    public static void Init(){
        s_textureHelper = new Dictionary<string, Texture2D>();
    }

    [Unload]
    public static void Unload(){
        foreach(var value in s_textureHelper.Values){
            UnityEngine.Object.Destroy(value);
        }
        
        s_textureHelper.Clear();
    }

    private static void SaveTexture(Texture2D tex, Color color){
        s_textureHelper[GetTextureKey(color)] = tex;
    }

    private static Texture2D GetCachedTexture(Color color){
        string textureKey = GetTextureKey(color);
        return s_textureHelper.TryGetValue(textureKey, out Texture2D cachedTexture) ? cachedTexture : null;
    }

    private static string GetTextureKey(Color color){
        return $"{color.r}-{color.g}-{color.b}-${color.a}";
    }

    public static Texture2D GetSolidTexture(Color color, bool withHideFlags)
    {
        Texture2D cached = GetCachedTexture(color);
        if(cached != null){
            return cached;
        }

        Texture2D tex = new(100, 100);
        Color[] pixels = tex.GetPixels();
        for (var i = 0; i < pixels.Length; i += 1)
        {
            pixels[i] = color;
        }
        tex.SetPixels(pixels);
        tex.Apply();

        if(withHideFlags){
            tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        SaveTexture(tex, color);
        return tex;
    }
}