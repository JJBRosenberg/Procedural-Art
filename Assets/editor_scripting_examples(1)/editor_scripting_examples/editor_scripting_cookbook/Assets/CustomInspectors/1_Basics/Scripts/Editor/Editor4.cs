using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Component4)), CanEditMultipleObjects]
public class Editor4 : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
		editor3Example();

		serializedObject.ApplyModifiedProperties();
	}

	//or if you want to replicate example 3 more truthfully:
	private void editor3Example()
	{
		SerializedProperty value = serializedObject.FindProperty("value");

		GUILayout.BeginHorizontal();
				
		EditorGUILayout.PropertyField(value);
		if (GUILayout.Button("-")) value.intValue--;
		if (GUILayout.Button("+")) value.intValue++;

		GUILayout.EndHorizontal();
	}

}

