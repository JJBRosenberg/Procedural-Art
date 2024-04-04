using UnityEngine;

public class Component2 : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUILayout.Button("Say hello"))
		{
			Debug.Log("Hello!");
		}
	}
}
