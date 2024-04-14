using UnityEngine;
using UnityEditor;

public class AdCreatorEditor : EditorWindow
{
    private GameObject prefabToSpawn;
    private adCreator spawner;

    [MenuItem("Tools/Ad Creator Editor")]
    public static void ShowWindow()
    {
        GetWindow<AdCreatorEditor>("Ad Creator");
    }

    void OnEnable()
    {
        // Try to find an existing spawner in the scene
        spawner = FindObjectOfType<adCreator>();
        if (spawner == null)
        {
            // If none found, create a new GameObject with the spawner
            GameObject spawnerObj = new GameObject("GameViewPrefabSpawner");
            spawner = spawnerObj.AddComponent<adCreator>();
            spawner.mainCamera = Camera.main;  // Automatically assign the main camera
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Configure Prefab Spawner", EditorStyles.boldLabel);
        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);

        if (prefabToSpawn != null && spawner != null)
        {
            spawner.prefabToSpawn = prefabToSpawn;  // Update the prefab on the spawner
        }
    }
}
