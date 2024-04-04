using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor
{
    private SerializedProperty _waypointArray;

    public void OnEnable()
    {
		_waypointArray = serializedObject.FindProperty("waypoints");
    }

	public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < _waypointArray.arraySize; i++)
        {
            GUILayout.BeginHorizontal();

            Waypoint result = EditorGUILayout.ObjectField(GetWaypoint(i), typeof(Waypoint), true) as Waypoint;

            if (GUI.changed)
            {
                SetWaypoint(i, result);
            }

            bool oldEnabled = GUI.enabled;

            GUI.enabled = oldEnabled && i > 0;
            if (GUILayout.Button("U", GUILayout.Width(20f))) SwapWayPoint(i, i-1);
            GUI.enabled = oldEnabled && i < _waypointArray.arraySize - 1;
            if (GUILayout.Button("D", GUILayout.Width(20f))) SwapWayPoint(i, i+1);
            GUI.enabled = oldEnabled;
            if (GUILayout.Button("-", GUILayout.Width(20f))) RemoveWaypointAtIndex(i);
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Waypoint"))
        {
			_waypointArray.arraySize++;
            SetWaypoint(_waypointArray.arraySize - 1, null);
        }

        DropAreaGUI();

        serializedObject.ApplyModifiedProperties();
    }

    private void DropAreaGUI()
    {
        var evt = Event.current;

        var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Add waypoint");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if(evt.type == EventType.DragPerform)
                {
                    //required for the next step
                    DragAndDrop.AcceptDrag();

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        var go = draggedObject as GameObject;
                        if (!go) continue;

                        var waypoint = go.GetComponent<Waypoint>();
                        if (!waypoint) continue;

						_waypointArray.arraySize++;
						SetWaypoint(_waypointArray.arraySize - 1, waypoint);
					}
				}
                Event.current.Use();
                break;
        }
    }

	private void SetWaypoint(int index, Waypoint waypoint)
	{
		_waypointArray.GetArrayElementAtIndex(index).objectReferenceValue = waypoint;
	}

	private Waypoint GetWaypoint(int index)
	{
		return _waypointArray.GetArrayElementAtIndex(index).objectReferenceValue as Waypoint;
	}

	private void SwapWayPoint(int i, int v)
	{
		Waypoint tmp = GetWaypoint(i);
		SetWaypoint(i, GetWaypoint(v));
		SetWaypoint(v, tmp);
	}

	private void RemoveWaypointAtIndex(int index)
	{
		for (int i = index; i < _waypointArray.arraySize - 1; i++)
		{
			SetWaypoint(i, GetWaypoint(i + 1));
		}

		_waypointArray.arraySize--;
	}

	/*
	[DrawGizmo(GizmoType.Selected)]
	private static void drawGizmo(WaypointManager wpm, GizmoType type)
	{
		Gizmos.color = Color.green;
		foreach (Waypoint wp in wpm.waypoints) {
			if (wp != null) Gizmos.DrawSphere(wp.transform.position, 0.2f);
		}
	}
	*/
}

