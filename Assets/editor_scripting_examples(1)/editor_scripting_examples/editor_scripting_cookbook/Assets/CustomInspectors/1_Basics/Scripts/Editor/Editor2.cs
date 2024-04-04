using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Component2))]
public class Editor2 : Editor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Say hello"))
		{
			Debug.Log("Hello!");
		}
	}

}
