using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Polygon))]
public class PolygonEditor : Editor
{
	// This method is called by Unity whenever it renders the scene view.
	// We use it to draw gizmos, and deal with changes (dragging objects)
	void OnSceneGUI() {
		Polygon curve = target as Polygon;

		if (curve.points==null) return;

		bool dirty = false;

		// Add new points if needed:
		Event e = Event.current;
		if ((e.type==EventType.KeyDown && e.keyCode == KeyCode.Space)) {
			Debug.Log("Space pressed - trying to add point to curve");
			dirty |= AddPoint();
		}

		dirty |= ShowAndMovePoints();
 	}

	// Tries to add a point to the curve, where the mouse is in the scene view.
	// Returns true if a change was made.
	bool AddPoint() {
		Polygon curve = target as Polygon;
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
		Polygon curve = target as Polygon;
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
				curve.points[i]=handleTransform.InverseTransformPoint(currentPoint);
				dirty=true;
			}
		}
		return dirty;
	}
}
