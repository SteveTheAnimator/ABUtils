![ABUtils](https://raw.githubusercontent.com/SteveTheAnimator/ABUtils/master/Media/ABUtils.png)

ABUtils is a utility library for loading and managing Unity Asset Bundles in Gorilla Tag mods. It provides methods for loading asset bundles from embedded resources and remote servers, with options optimized for both performance and memory efficiency.

## Installation

1. Add ABUtils as a dependency in your BepInEx mod:
   - Add a reference to `ABUtils.dll` in your project
   - Add the dependency to your `BepInPlugin` attribute:
   

```csharp
[BepInDependency("Steve.ABUtils")]
[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public class YourPlugin : BaseUnityPlugin
{
    // Your code here
}

```

## Key Features

- Load asset bundles from embedded resources or remote servers
- Memory-optimized loading for large asset bundles
- Speed-optimized loading for smaller asset bundles
- Synchronous and asynchronous loading options
- Built-in error handling and logging

## Usage

### Loading Asset Bundles from Embedded Resources

#### Speed-Optimized (For smaller asset bundles)


```csharp
using ABUtils.Utils.Client.Speed;

// Load an asset bundle from embedded resources
AssetBundle bundle = ABUtilsClient_Speed.LoadAssetBundle(
    "YourNamespace.YourBundlePath", 
    Assembly.GetExecutingAssembly()
);

// Get an asset from the bundle
GameObject prefab = ABUtilsClient_Speed.GetAssetFromBundle<GameObject>(
    bundle, 
    "YourAssetName"
);

// Don't forget to unload when done
bundle.Unload(false);

```

#### Memory-Optimized (For larger asset bundles)


```csharp
using ABUtils.Utils.Client.Memory;

// Load an asset bundle from embedded resources with custom buffer size
AssetBundle bundle = ABUtilsClient_Memory.LoadAssetBundle(
    "YourNamespace.YourBundlePath", 
    Assembly.GetExecutingAssembly(), 
    8192 // Optional buffer size, default is 4096
);

// Get an asset from the bundle with automatic memory cleanup
GameObject prefab = ABUtilsClient_Memory.GetAssetFromBundle<GameObject>(
    bundle, 
    "YourAssetName", 
    true // Automatically unload unused assets
);

// Properly unload the bundle when done
ABUtilsClient_Memory.UnloadAssetBundle(bundle, true);

```

### Loading Asset Bundles from a Server

#### Speed-Optimized (Synchronous)


```csharp
using ABUtils.Utils.Server.Speed;

// Load an asset bundle from a URL
AssetBundle bundle = ABUtilsServer_Speed.LoadAssetBundleFromServer(
    "https://yourserver.com/yourbundle.bundle"
);

// Get an asset (using the client utility)
GameObject prefab = ABUtils.Utils.Client.Speed.ABUtilsClient_Speed.GetAssetFromBundle<GameObject>(
    bundle, 
    "YourAssetName"
);

// Don't forget to unload when done
bundle.Unload(false);

```

#### Speed-Optimized (Asynchronous)


```csharp
using ABUtils.Utils.Server.Speed;

// Async method
async Task LoadYourBundle()
{
    // Load an asset bundle from a URL asynchronously
    AssetBundle bundle = await ABUtilsServer_Speed.LoadAssetBundleFromServerAsync(
        "https://yourserver.com/yourbundle.bundle"
    );
    
    // Use the bundle...
    
    // Unload when done
    bundle.Unload(false);
}

// Coroutine method
private void StartLoading()
{
    StartCoroutine(ABUtilsServer_Speed.LoadAssetBundleCoroutine(
        "https://yourserver.com/yourbundle.bundle",
        OnBundleLoaded
    ));
}

private void OnBundleLoaded(AssetBundle bundle)
{
    // Use the bundle...
    
    // Unload when done
    bundle.Unload(false);
}

```

#### Memory-Optimized


```csharp
using ABUtils.Utils.Server.Memory;

// Load an asset bundle from a URL with memory optimization
AssetBundle bundle = ABUtilsServer_Memory.LoadAssetBundleFromServer(
    "https://yourserver.com/yourbundle.bundle",
    8192 // Optional buffer size, default is 4096
);

// Async method
async Task LoadBundleAsync()
{
    AssetBundle bundle = await ABUtilsServer_Memory.LoadAssetBundleFromServerAsync(
        "https://yourserver.com/yourbundle.bundle"
    );
    
    // Use the bundle...
    
    // Properly unload the bundle when done
    ABUtilsServer_Memory.UnloadAssetBundle(bundle, true);
}

```

## When to Use Which Method

- **Speed Optimized Methods**:
  - Use for smaller asset bundles (< 10MB)
  - When load time is more important than memory usage

- **Memory Optimized Methods**:
  - Use for larger asset bundles
  - When memory efficiency is critical
  - When loading many assets in succession

## Best Practices

1. **Always unload your asset bundles** when you're done with them to prevent memory leaks
2. **Choose the appropriate loading method** based on your asset bundle size
3. **Use asynchronous methods** when loading from servers to avoid freezing the game
4. **Handle errors** that might occur during loading
5. **Cache frequently used assets** rather than repeatedly loading them from bundles

## Example Integration


```csharp
using BepInEx;
using UnityEngine;
using ABUtils.Utils.Client.Memory;
using System.Reflection;

namespace YourMod
{
    [BepInDependency("Steve.ABUtils")]
    [BepInPlugin("com.yourname.yourmod", "YourMod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private AssetBundle customModels;
        
        void Start()
        {
            // Load embedded asset bundle when the mod starts
            customModels = ABUtilsClient_Memory.LoadAssetBundle(
                "YourMod.Resources.custom_models",
                Assembly.GetExecutingAssembly()
            );
            
            if (customModels == null)
            {
                Debug.LogError("[YourMod] Failed to load custom models!");
                return;
            }
            
            // Further initialization with your assets
        }
        
        void OnDisable()
        {
            // Clean up when the mod is disabled
            if (customModels != null)
            {
                ABUtilsClient_Memory.UnloadAssetBundle(customModels, true);
            }
        }
    }
}

```

## Troubleshooting

- If assets fail to load, check that the path or URL is correct
- For embedded resources, make sure the asset bundle file is properly marked as an embedded resource in your project
- Ensure asset names match exactly when loading specific assets from bundles
- Check Unity log for detailed error messages from ABUtils

---

This library was created to simplify asset bundle management in Gorilla Tag mods. For more information or to report issues, contact me.