using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class CustomPlaneSpawnerEditor
{
    private static List<Vector3> clickPoints = new List<Vector3>();
    private static GameObject planePrefab;

    static CustomPlaneSpawnerEditor()
    {
        SceneView.duringSceneGui += SceneGUI;
        planePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/RoadBlock.prefab");
    }

    static void SceneGUI(SceneView sceneView)
    {
        if (!planePrefab)
        {
            Debug.LogError("CustomPlane prefab not found. Please ensure it's located at 'Assets/Prefab/RoadBlock.prefab'.");
            return;
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                clickPoints.Add(hit.point);

                if (clickPoints.Count == 2)
                {
                    Vector3 pointA = clickPoints[0];
                    Vector3 pointB = clickPoints[1];
                    Vector3 midPoint = (pointA + pointB) / 2;
                    Quaternion rotation = Quaternion.LookRotation(pointB - pointA);
                    GameObject planeInstance = PrefabUtility.InstantiatePrefab(planePrefab) as GameObject;

                    planeInstance.transform.position = midPoint + new Vector3(0, 1.5f, 0); // Assuming the plane's pivot is at the bottom, and it's 3m high
                    planeInstance.transform.rotation = rotation;
                    planeInstance.transform.Rotate(90, 0, 0); // Rotate to face upwards

                    clickPoints.Clear();
                }

                Event.current.Use();
            }
        }
    }
}
