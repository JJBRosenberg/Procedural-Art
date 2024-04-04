using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Component3))]
public class Editor3 : Editor
{
	
	public override void OnInspectorGUI()
	{
		Component3 myComponent = target as Component3;

		GUILayout.BeginHorizontal();

			GUILayout.Label("Value:");

			if(GUILayout.Button("-")) myComponent.value--;
			myComponent.value = EditorGUILayout.IntField(myComponent.value);
			if(GUILayout.Button("+")) myComponent.value++;

		GUILayout.EndHorizontal();
	}

}

