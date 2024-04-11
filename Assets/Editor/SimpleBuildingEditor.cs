using UnityEngine;
using UnityEditor;
using Demo;

[CustomEditor(typeof(SimpleBuilding))]
public class SimpleBuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleBuilding buildingScript = (SimpleBuilding)target;

        if (GUILayout.Button("Reset and Execute"))
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Operation requires play mode.");
                return;
            }

            buildingScript.ResetAndExecute();
        }
    }
}
