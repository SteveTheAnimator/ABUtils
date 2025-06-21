using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ABUtils.Utils.Server.Speed
{
    public static class ABUtilsServer_Speed
    {
        public static AssetBundle LoadAssetBundleFromServer(string url)
        {
            try
            {
                using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
                {
                    request.SendWebRequest();

                    while (!request.isDone)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to download asset bundle from: {url}, Error: {request.error}");
                        return null;
                    }

                    return DownloadHandlerAssetBundle.GetContent(request);
                }
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
                using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
                {
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                    while (!operation.isDone)
                    {
                        await Task.Delay(10);
                    }

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to download asset bundle from: {url}, Error: {request.error}");
                        return null;
                    }

                    return DownloadHandlerAssetBundle.GetContent(request);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error loading asset bundle from server: {ex.Message}");
                return null;
            }
        }

        public static IEnumerator LoadAssetBundleCoroutine(string url, Action<AssetBundle> onComplete)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                yield return request.SendWebRequest();

                AssetBundle bundle = null;
                if (request.result == UnityWebRequest.Result.Success)
                {
                    bundle = DownloadHandlerAssetBundle.GetContent(request);
                }
                else
                {
                    Debug.LogError($"[{PluginInfo.Name}] Failed to download asset bundle from: {url}, Error: {request.error}");
                }

                onComplete?.Invoke(bundle);
            }
        }
    }
}
