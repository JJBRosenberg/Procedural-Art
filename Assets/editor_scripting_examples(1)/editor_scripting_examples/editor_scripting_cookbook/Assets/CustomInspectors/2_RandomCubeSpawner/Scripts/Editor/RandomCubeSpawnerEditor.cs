using UnityEditor;
using UnityEngine;

//the true flag is to indicate that this editor should work on all subclass of RandomCubeSpawner1 as well
[CustomEditor(typeof(RandomCubeSpawner1), true)]
public class RandomCubeSpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("GO"))
		{
			((RandomCubeSpawner1)target).Generate();
		}
	}
}
