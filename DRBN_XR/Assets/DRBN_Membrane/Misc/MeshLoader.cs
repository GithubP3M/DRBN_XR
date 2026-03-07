using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

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
        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

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
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (mesh == null)
        {
            Debug.LogError($"No mesh found at {path}");
            return null;
        }

        Debug.Log($"Mesh loaded from {path}");
        return mesh;
    }
}

#endif