using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ABUtils.Utils.Client.Memory
{
    public static class ABUtilsClient_Memory
    {
        private const int DefaultBufferSize = 4096;    

        public static AssetBundle LoadAssetBundle(string path, Assembly assembly, int bufferSize = DefaultBufferSize)
        {
            Assembly tasm = assembly ?? Assembly.GetExecutingAssembly();
            try
            {
                using (Stream resourceStream = tasm.GetManifestResourceStream(path))
                {
                    if (resourceStream == null)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to find the resource stream at: {path}");
                        return null;
                    }

                    string tempFilePath = Path.Combine(Application.temporaryCachePath, $"temp_bundle_{Guid.NewGuid()}.bundle");

                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[bufferSize];
                        int bytesRead;

                        while ((bytesRead = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);

                            if (resourceStream.Position % (bufferSize * 256) == 0)
                            {
                                GC.Collect();
                            }
                        }
                    }

                    AssetBundle bundle = AssetBundle.LoadFromFile(tempFilePath);

                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[{PluginInfo.Name}] Failed to delete temporary bundle file: {ex.Message}");
                    }

                    return bundle;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset bundle: {ex.Message}");
                return null;
            }
        }

        public static T GetAssetFromBundle<T>(AssetBundle bundle, string name, bool unloadUnusedAssets = true) where T : UnityEngine.Object
        {
            if (bundle == null)
            {
                Debug.LogError($"[{PluginInfo.Name}] AssetBundle is null.");
                return null;
            }

            try
            {
                var assetLoadRequest = bundle.LoadAssetAsync<T>(name);

                while (!assetLoadRequest.isDone)
                {
                    System.Threading.Thread.Sleep(10);
                }

                T asset = assetLoadRequest.asset as T;

                if (asset == null)
                {
                    Debug.LogError($"[{PluginInfo.Name}] Failed to load asset: {name}");
                }

                if (unloadUnusedAssets)
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                }

                return asset;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset '{name}': {ex.Message}");
                return null;
            }
        }

        public static void UnloadAssetBundle(AssetBundle bundle, bool unloadAllLoadedObjects = true)
        {
            if (bundle == null) return;

            try
            {
                bundle.Unload(unloadAllLoadedObjects);
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error unloading asset bundle: {ex.Message}");
            }
        }
    }
}
