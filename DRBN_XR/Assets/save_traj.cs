using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class GameObjectData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string parentName;
    public bool isActive;
}

[System.Serializable]
public class SceneData
{
    public List<GameObjectData> gameObjects = new List<GameObjectData>();
    public string timestamp;
}

public class save_traj : MonoBehaviour
{
    [Tooltip("Key to press to save the scene state")]
    public KeyCode saveKey = KeyCode.S;
    
    [Tooltip("Enable to save every X seconds")]
    public bool autoSave = false;
    
    [Tooltip("Time in seconds between auto-saves")]
    public float saveInterval = 60f;
    
    private float nextSaveTime = 0f;
    
    private void Update()
    {
        // Manual save with key press
        if (Input.GetKeyDown(saveKey))
        {
            SaveSceneState();
        }
        
        // Auto-save logic
        if (autoSave && Time.time >= nextSaveTime)
        {
            nextSaveTime = Time.time + saveInterval;
            SaveSceneState();
        }
    }
    
    public void SaveSceneState()
    {
        SceneData sceneData = new SceneData();
        sceneData.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // Get all active GameObjects in the scene
        //GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject[] molecules = GameObject.FindGameObjectsWithTag("molecule");

        
        //foreach (GameObject obj in allObjects)
        foreach (GameObject obj in molecules)
        {
            // Skip if the object is a child of another object (we'll handle it through hierarchy)
            if (obj.transform.parent != null)
                continue;

            // Save this object and all its children recursively
            SaveGameObjectHierarchy(obj, sceneData);
        }
        
        // Convert to JSON
        string json = JsonUtility.ToJson(sceneData, true);
        
        // Ensure the directory exists
        string directory = Path.Combine(Application.persistentDataPath, "SceneSaves");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Create a filename with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(directory, $"scene_state_{timestamp}.json");
        
        // Save to file
        File.WriteAllText(filePath, json);
        
        Debug.Log($"Scene state saved to: {filePath}");
    }
    
    private void SaveGameObjectHierarchy(GameObject obj, SceneData sceneData, string parentPath = "")
    {
        // Skip inactive objects
        if (!obj.activeInHierarchy)
            return;
            
        GameObjectData data = new GameObjectData();
        data.name = obj.name;
        data.position = obj.transform.position;
        
        data.rotation = obj.transform.eulerAngles;
        data.scale = obj.transform.localScale;
        data.parentName = parentPath;
        data.isActive = obj.activeSelf;
        
        sceneData.gameObjects.Add(data);
        
        // Build the path for children
        string currentPath = string.IsNullOrEmpty(parentPath) ? obj.name : $"{parentPath}/{obj.name}";
        
        // Process all children
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);
            SaveGameObjectHierarchy(child.gameObject, sceneData, currentPath);
        }
    }
    
    // Call this from a UI button or other script if needed
    public void ManualSave()
    {
        SaveSceneState();
    }
}