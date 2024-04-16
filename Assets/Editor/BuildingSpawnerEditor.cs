using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BuildingSpawnerEditor : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private int selectedPrefabIndex = 0;
    private bool useRandomPrefab = true;
    private float minHeight = 1f;
    private float maxHeight = 10f;
    private Vector3 spawnPosition = Vector3.zero; 
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private const string defaultPrefabPath = "Assets/Prefabs/ProceduralSimpleBuilding1.prefab";

    [MenuItem("Tools/Building Spawner")]
    public static void ShowWindow()
    {
        BuildingSpawnerEditor window = GetWindow<BuildingSpawnerEditor>("Building Spawner");
        window.InitializePrefabList();
        window.Show();
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void InitializePrefabList()
    {
        GameObject defaultPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(defaultPrefabPath);
        if (defaultPrefab != null)
        {
            prefabs.Add(defaultPrefab);
        }
        else
        {
            Debug.LogError("Default prefab not found at: " + defaultPrefabPath);
        }
    }

    void OnGUI()
    {
        if (prefabs == null)
            prefabs = new List<GameObject>();

        EditorGUI.BeginChangeCheck();
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Prefabs", prefabs.Count));
        while (newCount < prefabs.Count)
            prefabs.RemoveAt(prefabs.Count - 1);
        while (newCount > prefabs.Count)
            prefabs.Add(null);

        for (int i = 0; i < prefabs.Count; i++)
        {
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField("Prefab " + i, prefabs[i], typeof(GameObject), false);
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }

        useRandomPrefab = EditorGUILayout.Toggle("Use Random Prefab", useRandomPrefab);

        EditorGUI.BeginDisabledGroup(useRandomPrefab);
        selectedPrefabIndex = EditorGUILayout.IntSlider("Select Prefab Index", selectedPrefabIndex, 0, prefabs.Count - 1);
        EditorGUI.EndDisabledGroup();

        minHeight = EditorGUILayout.FloatField("Min Height", minHeight);
        maxHeight = EditorGUILayout.FloatField("Max Height", maxHeight);

        EditorGUILayout.LabelField("Spawn Position: " + spawnPosition.ToString());

        if (GUILayout.Button("Spawn Prefab"))
        {
            SpawnPrefab();
        }

        if (GUILayout.Button("Undo Last Spawned"))
        {
            UndoLastSpawned();
        }

        if (GUILayout.Button("Clear All Spawned"))
        {
            ClearAllSpawned();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.type == EventType.MouseUp && e.button == 2)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                spawnPosition = hit.point;
                SceneView.RepaintAll();
            }
            e.Use();
        }
    }

    private void SpawnPrefab()
    {
        if (prefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs available to spawn.");
            return;
        }

        GameObject prefabToSpawn = useRandomPrefab ? prefabs[Random.Range(0, prefabs.Count)] : prefabs[selectedPrefabIndex];
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Selected prefab is null.");
            return;
        }

        GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(instance);
        Undo.RegisterCreatedObjectUndo(instance, "Spawned Prefab");
    }

    private void UndoLastSpawned()
    {
        if (spawnedObjects.Count > 0)
        {
            GameObject lastObject = spawnedObjects[spawnedObjects.Count - 1];
            Undo.DestroyObjectImmediate(lastObject);
            spawnedObjects.RemoveAt(spawnedObjects.Count - 1);
        }
    }

    private void ClearAllSpawned()
    {
        List<GameObject> tempSpawnedObjects = new List<GameObject>(spawnedObjects);

        foreach (GameObject obj in tempSpawnedObjects)
        {
            if (obj != null) 
            {
                Undo.DestroyObjectImmediate(obj); 
            }
        }
        spawnedObjects.Clear(); 
        Debug.Log("All spawned objects have been cleared.");
    }

}
