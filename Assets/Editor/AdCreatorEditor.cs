using UnityEngine;
using UnityEditor;

public class AdCreatorTool : EditorWindow
{
    private GameObject adPrefab;

    // Correct the MenuItem attribute to include a submenu path.
    [MenuItem("Tools/Ad Tool")]
    public static void ShowWindow()
    {
        // Opens the window, titled "Ad Spawner"
        GetWindow<AdCreatorTool>("Ad Spawner");
    }

    void OnGUI()
    {
        GUILayout.Label("Spawn Ad Prefab", EditorStyles.boldLabel);
        adPrefab = (GameObject)EditorGUILayout.ObjectField("Ad Prefab", adPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Activate Spawner"))
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject.Instantiate(adPrefab, hit.point + Vector3.up * 0.1f, Quaternion.identity);
            }
        }
    }
}
