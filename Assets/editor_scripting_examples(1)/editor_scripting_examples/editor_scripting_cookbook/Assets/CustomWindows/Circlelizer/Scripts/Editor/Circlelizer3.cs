using UnityEditor;
using UnityEngine;
using System.Linq;

/**
 * Extended version of the Circlelizer with a lot of EditorWindow possibilities demonstrated, 
 * such as an improved UI and Scene UI.
 */
public class Circlelizer3 : EditorWindow
{
	[MenuItem("Custom menu/Cirlelizer yeah! (3)")]
	private static void init()
	{
		Circlelizer3 window = GetWindow<Circlelizer3>("Circlelizer yeah!");
		window.minSize = window.maxSize = new Vector2(350, 200);
		window.Show();
	}

	//define all the values the user can set
	private Vector3 _centerPoint = Vector3.zero;
	private Quaternion _facing = Quaternion.identity;
	private float _radius = 5;
	private bool _spiral = false;
	private float _rotations = 1;
	private bool _lookAtCenter = false;
	private bool _liveUpdate = false;

	//we update this flag after each selection change
	private enum SelectionStatus { NONE, INVALID, OK };
	private SelectionStatus status = SelectionStatus.NONE;

	//define some default facings using the new C# 7.0 value tuple definition (gotta love it !).
	//This is similar to a dictionary but without the lookups.
	private static (string,Vector3)[] _facings = {
		("-X", Vector3.left),	("X", Vector3.right),
		("-Y", Vector3.down),	("Y", Vector3.up),
		("-Z", Vector3.back), ("Z", Vector3.forward)
	};

	//enable a scenegui for this editor window and update some settings the moment we activate
	private void OnEnable()
	{
		SceneView.duringSceneGui += onSceneGUI;
		OnSelectionChange();
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= onSceneGUI;
	}

	//anytime the selection in the scene changes, we check whether the selection is valid for our purposes,
	//repaint our window based on the new status, and update the layout of the selected objects based on our settings
	private void OnSelectionChange()
	{
		updateSelectionStatus();
		Repaint();
		if (_liveUpdate) updateLayout();
	}

	//OnGUI code is always horrible :(
	//This implements anything that must be draw in the window itself.
	//The window code MUST be in OnGUI
	private void OnGUI()
	{
		//vector3field layouts title above input by default, so that's why we need more cowbe.. code.
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Position");
			_centerPoint = EditorGUILayout.Vector3Field("",_centerPoint);
		EditorGUILayout.EndHorizontal();

		_radius = EditorGUILayout.FloatField("Radius", _radius);
		_lookAtCenter = EditorGUILayout.Toggle("Look at center?", _lookAtCenter);

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Reset facing:");
			foreach ((string,Vector3) facing in _facings)
			{
				if (GUILayout.Button(facing.Item1)) _facing = Quaternion.LookRotation(facing.Item2);
			}
		EditorGUILayout.EndHorizontal();

		_spiral = EditorGUILayout.Toggle("Spiral?", _spiral);
		_rotations = EditorGUILayout.FloatField("Rotations?", _rotations);
		_liveUpdate = EditorGUILayout.Toggle("Live update ?", _liveUpdate);

		switch (status)
		{
			case SelectionStatus.NONE:
				EditorGUILayout.HelpBox("Select some objects in the scene to get started!", MessageType.Info);
				break;
			case SelectionStatus.INVALID:
				EditorGUILayout.HelpBox("Circlelizer only works on scene objects !", MessageType.Warning);
				break;
			case SelectionStatus.OK:
				if (!_liveUpdate && GUILayout.Button("Circlelize!")) updateLayout();
				if (_liveUpdate && GUI.changed) updateLayout();
				break;
		}
	}

	//Everything that has to be drawn in the SceneView HAS to go into onSceneGUI (OnSceneGUI for an Editor)
	private void onSceneGUI(SceneView sceneView)
	{	
		//show the rotation handles 
		_facing = Handles.RotationHandle(_facing, _centerPoint);
		_facing.Normalize();

		//show the center point handles but respect the tools.pivotRotation setting
		_centerPoint = Handles.PositionHandle(
			_centerPoint, 
			Tools.pivotRotation == PivotRotation.Local?_facing:Quaternion.identity
		);

		//Show a nice disc so that we can see the facing and radius more clearly
		Vector3 forward = _facing * Vector3.forward;
		Vector3 right = _facing * Vector3.right;

		Handles.color = new Color(0, 1, 0, 0.1f);
		if (status == SelectionStatus.NONE) Handles.color = new Color(1, 0.8f, 0, 0.1f);
		if (status == SelectionStatus.INVALID) Handles.color = new Color(1, 0, 0, 0.1f);
		//use 359 instead 360 so we can see the starting point along the circle's edge
		Handles.DrawSolidArc(_centerPoint, forward, right, 359, _radius);

		//add a dot so we can change the radius (another 2 hours of my life I'll never get back ;))
		Handles.color = new Color(0, 1, 0);
		Vector3 currentPos = _centerPoint + _radius * right;
		EditorGUI.BeginChangeCheck();
		Vector3 newPos = Handles.Slider(currentPos, right, HandleUtility.GetHandleSize(currentPos) * 0.5f, Handles.SphereHandleCap, 0);
		if (EditorGUI.EndChangeCheck())	_radius = Vector3.Dot(newPos - _centerPoint, right);

		//If we changed anything in the sceneview repaint the editor window
		if (GUI.changed)
		{
			Repaint();
			if (_liveUpdate) updateLayout();
		}
	}

	private void updateLayout()
	{
		if (status != SelectionStatus.OK) return;

		int count = Selection.gameObjects.Length;                       //cache the nr of objects
		if (count == 0) return;

		float angleStep = _rotations * 2 * Mathf.PI / count;            //calculate the angle step per object
		Vector3 right = _facing * Vector3.right;						//calculate the right vector for this facing
		Vector3 up = _facing * Vector3.up;								//calculate the up vector for this facing

		for (int i = 0; i < count; i++)
		{
			float angle = i * angleStep;
			float radius = _spiral ? (i * _radius / count) : _radius;
			//apply basic 2d rotation formula using different basis vectors
			Selection.gameObjects[i].transform.position =
				_centerPoint + Mathf.Cos(angle) * right  * radius + Mathf.Sin(angle) * up * radius;

			if (_lookAtCenter)
			{
				Selection.gameObjects[i].transform.LookAt(_centerPoint, up);
			}
		}
	}

	private void updateSelectionStatus()
	{
		int count = Selection.gameObjects.Length;

		if (count == 0) {
			status = SelectionStatus.NONE;
			return;
		}

		for (int i = 0; i < count; i++)
		{
			if (!Selection.gameObjects[i].scene.IsValid())
			{
				status = SelectionStatus.INVALID;
				return;
			}
		}

		//We could also do the count above in a linq one liner
		//int count = Selection.gameObjects.Where(go => go.scene.IsValid()).Count();

		status = SelectionStatus.OK;
	}
}

