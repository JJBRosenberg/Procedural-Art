using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Curve))]
public class CurveEditor : Editor
{
	// This method is called by Unity (in edit mode) whenever it renders the inspector for a Curve Component.
	// We use it to add a custom button to the inspector.
	public override void OnInspectorGUI() {
		if (GUILayout.Button("Recalculate Mesh")) {
			Curve spline = target as Curve;
			spline.Apply();
		}
		DrawDefaultInspector();
	}


	// This method is called by Unity whenever it renders the scene view.
	// We use it to draw gizmos, and deal with changes (dragging objects)
	void OnSceneGUI() {
		Curve curve = target as Curve;
		if (curve.points==null)
			return;

		DrawAndMoveCurve();

		bool dirty = false;

		// Add new points if needed:
		Event e = Event.current;
		if ((e.type==EventType.KeyDown && e.keyCode == KeyCode.Space)) {
			Debug.Log("Space pressed - trying to add point to curve");
			dirty |= AddPoint();
		}

		dirty |= ShowAndMovePoints();

		if (dirty) {
			curve.Apply();
		}
 	}

	// Draws a handle gizmo at the pivot of this game object,
	//  and checks whether it has been moved.
	void DrawAndMoveCurve() {
		Curve curve = target as Curve;

		Transform handleTransform = curve.transform;
		Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;

		EditorGUI.BeginChangeCheck();
		Vector3 newPosition = Handles.PositionHandle(handleTransform.position, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(curve, "Move curve");
			curve.transform.position = newPosition;
			EditorUtility.SetDirty(curve);
		}
	}

	// Tries to add a point to the curve, where the mouse is in the scene view.
	// Returns true if a change was made.
	bool AddPoint() {
		Curve curve = target as Curve;
		bool dirty = false;
		Transform handleTransform = curve.transform;

		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			Debug.Log("Adding spline point at mouse position: "+hit.point);
			curve.points.Add(handleTransform.InverseTransformPoint(hit.point));
			dirty=true;
		}
		return dirty;
	}

	// Show points in scene view, and check if they're changed:
	bool ShowAndMovePoints() {
		Curve curve = target as Curve;
		bool dirty = false;
		Transform handleTransform = curve.transform;

		Vector3 previousPoint = Vector3.zero;
		for (int i = 0; i < curve.points.Count; i++) {
			Vector3 currentPoint = handleTransform.TransformPoint(curve.points[i]);
			Handles.color=Color.white;
			if (i>0) {
				Handles.DrawLine(previousPoint, currentPoint);
			}
			previousPoint=currentPoint;

			// See https://docs.unity3d.com/ScriptReference/Handles.PositionHandle.html
			//  for an explanation of drawing position handles + checking whether
			//  they changed this frame.
			EditorGUI.BeginChangeCheck();
			currentPoint=Handles.DoPositionHandle(currentPoint, Quaternion.identity);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(curve, "Move point");
				EditorUtility.SetDirty(curve);
				curve.points[i]=handleTransform.InverseTransformPoint(currentPoint);
				dirty=true;
			}
		}
		return dirty;
	}
}
