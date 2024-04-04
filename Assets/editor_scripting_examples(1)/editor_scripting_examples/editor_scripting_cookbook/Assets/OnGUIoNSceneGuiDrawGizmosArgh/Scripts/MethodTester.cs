using UnityEngine;

public class MethodTester : MonoBehaviour
{

	private void OnGUI()
	{
		GUILayout.Label("MethodTester.OnGUI: I only work in play mode on screen or in an editorwindow");
	}

	private void OnDrawGizmos()
	{
		//Gizmos can only be used in DrawGizmos
		Gizmos.color = Color.green;
		Gizmos.DrawCube(Vector3.left, Vector3.one);
	}

	private void OnDrawGizmosSelected()
	{
		//Gizmos can only be used in DrawGizmos
		Gizmos.color = Color.red;
		Gizmos.DrawCube(Vector3.right, Vector3.one);
	}

}
