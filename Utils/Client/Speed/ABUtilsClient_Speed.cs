using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ABUtils.Utils.Client.Speed
{
    public static class ABUtilsClient_Speed
    {
        public static AssetBundle LoadAssetBundle(string path, Assembly assembly)
        {
            Assembly tasm = assembly ?? Assembly.GetExecutingAssembly();
            try
            {
                using (Stream stream = tasm.GetManifestResourceStream(path))
                {
                    if (stream == null)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to find the resource stream at: {path}");
                        return null;
                    }

                    return AssetBundle.LoadFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset bundle: {ex.Message}");
                return null;
            }
        }

        public static T GetAssetFromBundle<T>(AssetBundle bundle, string name) where T : UnityEngine.Object
        {
            if (bundle == null)
            {
                Debug.LogError($"[{PluginInfo.Name}] AssetBundle is null.");
                return null;
            }
            try
            {
                T asset = bundle.LoadAsset<T>(name);
                if (asset == null)
                {
                    Debug.LogError($"[{PluginInfo.Name}] Failed to load asset: {name}");
                }
                return asset;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset '{name}': {ex.Message}");
                return null;
            }
        }
    }
}
