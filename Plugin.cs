using System;
using BepInEx;
using Photon.Pun;
using UnityEngine;
using ABUtils.Utils.Client.Memory;
using ABUtils.Utils.Client.Speed;
using ABUtils.Utils.Server.Memory;
using ABUtils.Utils.Server.Speed;
using System.Reflection;

namespace ABUtils
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public bool enableTests = false;

        public void Start() => GorillaTagger.OnPlayerSpawned(Init);

        public void Init()
        {
            Debug.Log($"[{PluginInfo.Name}] v{PluginInfo.Version} initializing...");

            try
            {
                if(enableTests)
                {
                    AssetBundle clientB = Utils.Client.Memory.ABUtilsClient_Memory.LoadAssetBundle("ABUtils.Tests.Resources.abtc", Assembly.GetExecutingAssembly(), 4096);
                    AssetBundle serverB = Utils.Server.Speed.ABUtilsServer_Speed.LoadAssetBundleFromServer("https://quantumleapstudios.org/abutils_tests/abts");

                    if (clientB == null)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to load client asset bundle.");
                    }
                    else
                    {
                        Debug.Log($"[{PluginInfo.Name}] Client asset bundle loaded successfully.");
                    }
                    if (serverB == null)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to load server asset bundle.");
                    }
                    else
                    {
                        Debug.Log($"[{PluginInfo.Name}] Server asset bundle loaded successfully.");
                    }

                    var clientAsset = Utils.Client.Speed.ABUtilsClient_Speed.GetAssetFromBundle<GameObject>(clientB, "T_Cube");
                    if (clientAsset == null)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to load client asset from bundle.");
                    }
                    else
                    {
                        Debug.Log($"[{PluginInfo.Name}] Client asset loaded successfully: {clientAsset.name}");
                    }
                    var serverAsset = Utils.Client.Speed.ABUtilsClient_Speed.GetAssetFromBundle<GameObject>(serverB, "T_Cube");
                    if (serverAsset == null)
                    {
                        Debug.LogError($"[{PluginInfo.Name}] Failed to load server asset from bundle.");
                    }
                    else
                    {
                        Debug.Log($"[{PluginInfo.Name}] Server asset loaded successfully: {serverAsset.name}");
                    }
                    if (clientAsset != null && serverAsset != null)
                    {
                        Debug.Log($"[{PluginInfo.Name}] Both client and server assets loaded successfully.");
                    }
                    else
                    {
                        Debug.LogError($"[{PluginInfo.Name}] One or both assets failed to load.");
                    }

                    if (clientB != null) clientB.Unload(false);
                    if (serverB != null) serverB.Unload(false);

                    Debug.Log($"[{PluginInfo.Name}] Asset bundle tests completed.");
                }

                Debug.Log($"[{PluginInfo.Name}] Initialization complete!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{PluginInfo.Name}] Error during initialization: {ex.Message}");
                Debug.LogError($"[{PluginInfo.Name}] Stack trace: {ex.StackTrace}");
            }
        }
    }

    public class PluginInfo
    {
        internal const string
            GUID = "Steve.ABUtils",
            Name = "ABUtils",
            Desc = "Asset Bundle Utilities for Gorilla Tag",
            Version = "1.0.0";
    }
}
