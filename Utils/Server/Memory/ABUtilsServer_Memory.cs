using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ABUtils.Utils.Server.Memory
{
    public static class ABUtilsServer_Memory
    {
        private const int DefaultBufferSize = 4096;
        private static readonly HttpClient httpClient = new HttpClient();

        public static AssetBundle LoadAssetBundleFromServer(string url, int bufferSize = DefaultBufferSize)
        {
            try
            {
                string tempFilePath = Path.Combine(Application.temporaryCachePath, $"server_bundle_{Guid.NewGuid()}.bundle");

                using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                {
                    var operation = webRequest.SendWebRequest();

                    while (!operation.isDone)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to download asset bundle: {webRequest.error}");
                        return null;
                    }

                    File.WriteAllBytes(tempFilePath, webRequest.downloadHandler.data);
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
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset bundle from server: {ex.Message}");
                return null;
            }
        }

        public static async Task<AssetBundle> LoadAssetBundleFromServerAsync(string url)
        {
            try
            {
                string tempFilePath = Path.Combine(Application.temporaryCachePath, $"server_bundle_{Guid.NewGuid()}.bundle");

                using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                {
                    var operation = webRequest.SendWebRequest();

                    while (!operation.isDone)
                    {
                        await Task.Delay(10);
                    }

                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to download asset bundle: {webRequest.error}");
                        return null;
                    }

                    File.WriteAllBytes(tempFilePath, webRequest.downloadHandler.data);
                }

                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(tempFilePath);
                while (!bundleLoadRequest.isDone)
                {
                    await Task.Delay(10);
                }

                AssetBundle bundle = bundleLoadRequest.assetBundle;

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
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset bundle from server: {ex.Message}");
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
