using UnityEngine;
<<<<<<< HEAD
#if UNITY_EDITOR
using UnityEditor;
=======
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35

public class MeshLoader
{
    // copilot generated code
    public static void SaveMesh(Mesh mesh, string path)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null. Cannot save.");
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
            System.IO.Path.Combine(path, $"{mesh.name}.asset");
<<<<<<< HEAD
        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
=======
        UnityEditor.AssetDatabase.CreateAsset(mesh, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35

        Debug.Log($"Mesh saved to {assetPath}");
    }

    // copilot generated code
    public static Mesh LoadMesh(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is null or empty. Cannot load mesh.");
            return null;
        }

        // Load the mesh asset from the specified path
<<<<<<< HEAD
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
=======
        var mesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(path);
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35
        if (mesh == null)
        {
            Debug.LogError($"No mesh found at {path}");
            return null;
        }

        Debug.Log($"Mesh loaded from {path}");
        return mesh;
    }
<<<<<<< HEAD
}

#endif
=======
};
>>>>>>> ddcde55e08fbd0616c86f97205f00e80ee169f35
