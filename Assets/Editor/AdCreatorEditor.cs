using UnityEngine;
using UnityEditor;

public class AdCreatorEditor : EditorWindow
{
    private GameObject prefabToSpawn;
    private Vector3? firstClickPosition = null;
    private Vector3 secondClickPosition;
    private bool isAwaitingSecondClick = false;

    [MenuItem("Tools/Ad Creator Editor")]
    public static void ShowWindow()
    {
        GetWindow<AdCreatorEditor>("Ad Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Configure Prefab Spawner", EditorStyles.boldLabel);
        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Plane Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);

        if (GUILayout.Button("Start Placement"))
        {
            if (prefabToSpawn == null)
            {
                EditorUtility.DisplayDialog("Prefab Not Set", "Please assign a plane prefab to spawn.", "OK");
                return;
            }
            SceneView.duringSceneGui += OnSceneGUI;
            firstClickPosition = null;
            isAwaitingSecondClick = false;
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 2) // Changed to middle mouse button
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (firstClickPosition == null)
                {
                    firstClickPosition = hit.point;
                    isAwaitingSecondClick = true;
                    EditorUtility.DisplayDialog("First Point Set", "Now click the opposite corner of the plane.", "OK");
                }
                else
                {
                    secondClickPosition = hit.point;
                    SpawnPlane();
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
            }

            Event.current.Use(); // Consume the event so it doesn't propagate further
        }
    }

    private void SpawnPlane()
    {
        Vector3 center = (firstClickPosition.Value + secondClickPosition) * 0.5f;
        Vector3 size = new Vector3(Mathf.Abs(secondClickPosition.x - firstClickPosition.Value.x),
                                   0,
                                   Mathf.Abs(secondClickPosition.z - firstClickPosition.Value.z));
        GameObject planeInstance = Instantiate(prefabToSpawn, center, Quaternion.identity);
        planeInstance.transform.localScale = new Vector3(size.x, 1, size.z); // Assuming the prefab is 1x1 units
        firstClickPosition = null;
        isAwaitingSecondClick = false;
    }
}
