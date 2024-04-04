using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MethodTester))]
public class MethodTesterEditor : Editor
{
	private Vector3 handlePosition;

	private void OnSceneGUI()
	{
		handlePosition = Handles.PositionHandle(handlePosition, Quaternion.identity);

		Handles.BeginGUI();
		GUILayout.Button("MethodTesterEditor.OnSceneGUI:This is a Scene View button");
		Handles.EndGUI();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.HelpBox("This is rendered by OnInspectorGUI in the MethodTesterEditor", MessageType.Info);
	}

	//This can be defined in any class, so it doesn't HAVE to be in a editor script
	//Signature is VERY important:
	// - method MUST be static, 
	// - first param MUST be the component's name
	// - second param MUST be gizmotype
	[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
	private static void drawGizmo(MethodTester instance, GizmoType gizmoType)
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawCube(Vector3.up, Vector3.one);
	}

}
