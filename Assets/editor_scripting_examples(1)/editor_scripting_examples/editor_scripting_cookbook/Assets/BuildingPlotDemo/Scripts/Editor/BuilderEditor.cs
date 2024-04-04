using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Builder))]
public class BuilderEditor : Editor
{
	Vector3 startPoint = Vector3.zero;
	Vector3 currentPoint = Vector3.zero;
	bool isDragging = false;
	Tool currentTool;


	//Plane plane = new Plane();

	private void OnSceneGUI()
	{
		Builder builder = (target as Builder);
		//plane.normal = builder.transform.up;
		//plane.distance = Vector3.Dot(builder.transform.position, plane.normal);

		if (Event.current.type == EventType.MouseDown && !isDragging && Event.current.modifiers == EventModifiers.None)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit info;
			if (Physics.Raycast(ray, out info) && info.transform == builder.transform)
			{
				isDragging = true;
				startPoint = currentPoint = info.point;
				GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
				Event.current.Use();
				currentTool = Tools.current;
				Tools.current = Tool.None;
			}
		}

		if (isDragging)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit info;
			if (Physics.Raycast(ray, out info))
			{
				currentPoint = info.point;
			}

			Handles.DrawSolidDisc(startPoint, info.normal, 0.2f);
			Handles.DrawSolidDisc(currentPoint, info.normal, 0.2f);

			float minX = Mathf.Min(startPoint.x, currentPoint.x);
			float maxX = Mathf.Max(startPoint.x, currentPoint.x);
			float minZ = Mathf.Min(startPoint.z, currentPoint.z);
			float maxZ = Mathf.Max(startPoint.z, currentPoint.z);

			Vector3 leftBack = new Vector3(minX, 0.1f, minZ);
			Vector3 rightBack = new Vector3(maxX, 0.1f, minZ);
			Vector3 leftFront = new Vector3(minX, 0.1f, maxZ);
			Vector3 rightFront = new Vector3(maxX, 0.1f, maxZ);

			Handles.DrawAAConvexPolygon(leftBack, rightBack, rightFront, leftFront, leftBack);

			if (Event.current.type == EventType.MouseUp)
			{
				isDragging = false;
				GUIUtility.hotControl = 0;
				Event.current.Use();
				Tools.current = currentTool;

				if (builder.buildingSpawners == null || builder.buildingSpawners.Length == 0)
				{
					Debug.Log("No building prefabs specified");
					return;
				} else
				{
					Vector3 buildingPos = (leftBack + rightFront) / 2;
					Vector2 plotSize = new Vector2(Mathf.Abs(leftBack.x - rightFront.x), Mathf.Abs(leftBack.z - rightFront.z));
					//note that we are not creating a prefab, we are just (ab)using it to create other stuff
					//we could also use scriptable objects for this, but students might be more familiar with this approach
					AbstractBuildingSpawner buildingSpawner = builder.buildingSpawners[Random.Range(0, builder.buildingSpawners.Length)];
					Undo.RegisterCreatedObjectUndo(
						buildingSpawner.Initialize(buildingPos, plotSize),
						"building"
					);
				}
			}

			SceneView.currentDrawingSceneView.Repaint();

		}
	}


}
