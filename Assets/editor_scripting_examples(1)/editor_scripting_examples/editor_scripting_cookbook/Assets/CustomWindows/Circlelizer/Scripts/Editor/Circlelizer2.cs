using UnityEditor;
using UnityEngine;

/** 
 * MVP of the Circlelizer window
 */
public class Circlelizer2 : EditorWindow
{
	[MenuItem("Custom menu/Cirlelizer yeah! (2)")]
	private static void init()
	{
		Circlelizer2 window = GetWindow<Circlelizer2>("Circlelizer yeah!");
		window.minSize = window.maxSize = new Vector2(300, 200);
		window.Show();
	}

	//define some basic settings such as center and radius (facing is hardcoded at the moment)
	//note that these settings are not saved (close and reopen the window to test this out)
	private Vector3 _centerPoint = Vector3.zero;
	private float _radius = 5;

	private void OnGUI()
	{
		_centerPoint = EditorGUILayout.Vector3Field("Center", _centerPoint);
		_radius = EditorGUILayout.FloatField("Radius", _radius);
		if (GUILayout.Button("Circlelize!")) updateLayout();
	}

	private void updateLayout()
	{
		int count = Selection.gameObjects.Length;                       //cache the nr of objects
		if (count == 0) return;

		float angleStep = 2 * Mathf.PI / count;							//calculate the angle step per object
		Quaternion facing = Quaternion.Euler(new Vector3(90,0,0));		//assume an upward facing for now
		Vector3 right = facing * Vector3.right * _radius;				//calculate the right vector for this facing
		Vector3 up = facing * Vector3.up * _radius;                     //calculate the up vector for this facing

		for (int i = 0; i < count; i++)
		{
			float angle = i * angleStep;

			//apply basic 2d rotation formula using different basis vectors
			Selection.gameObjects[i].transform.position =				
				_centerPoint + Mathf.Cos(angle) * right + Mathf.Sin(angle) * up;	
		}
	}

}

