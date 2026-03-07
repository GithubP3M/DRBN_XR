using UnityEngine;
<<<<<<< HEAD
#if UNITY_EDITOR
using UnityEditor;
=======
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35

public class VolumeLoader
{
    // copilot generated code
    public static void SaveVolume(RenderTexture texture, string path)
    {
        if (texture == null)
        {
            Debug.LogError("RenderTexture is null. Cannot save.");
            return;
        }

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is null or empty. Cannot save mesh.");
            return;
        }

        // Create a new asset file at the specified path
        var assetPath =
            path.EndsWith(".asset") ? path :
            System.IO.Path.Combine(path, $"{texture.name}.asset");
<<<<<<< HEAD
        AssetDatabase.CreateAsset(texture, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
=======
        UnityEditor.AssetDatabase.CreateAsset(texture, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35

        Debug.Log($"RenderTexture saved to {assetPath}");
    }

    // copilot generated code
    public static RenderTexture LoadVolume(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is null or empty. Cannot load mesh.");
            return null;
        }

        // Load the mesh asset from the specified path
<<<<<<< HEAD
        var texture = AssetDatabase.LoadAssetAtPath<RenderTexture>(path);
=======
        var texture = UnityEditor.AssetDatabase.LoadAssetAtPath<RenderTexture>(path);
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35
        if (texture == null)
        {
            Debug.LogError($"No mesh found at {path}");
            return null;
        }

        Debug.Log($"RenderTexture loaded from {path}");
        return texture;
    }
<<<<<<< HEAD
}
#endif
=======
}
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35
