using UnityEditor;

[CustomEditor(typeof(Component1))]
public class Editor1 : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//or DrawDefaultInspector(); which does the same thing

		EditorGUILayout.HelpBox("Booh!", MessageType.Warning);
	}
}

