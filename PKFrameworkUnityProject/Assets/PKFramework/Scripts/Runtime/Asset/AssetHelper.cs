using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace PKFramework.Runtime
{
    public static class AssetHelper
    {
        public static string EditorPathPrefix => "Assets/";

        public static string GetAssetTypeExtension(Type t)
        {
            if (t == typeof(GameObject))
            {
                return ".prefab";
            }
            else if (t == typeof(AudioClip))
            {
                return ".aif";
            }
            else if (t == typeof(Texture))
            {
                return ".png";
            }
            else if (t == typeof(Sprite))
            {
                return ".png";
            }
            else if (t == typeof(Font))
            {
                return ".ttf";
            }
            else if (t == typeof(AnimatorController))
            {
                return ".controller";
            }

            PKLogger.LogError($"Invalid Type: {t.Name}");
            return "";
        }
    }
}