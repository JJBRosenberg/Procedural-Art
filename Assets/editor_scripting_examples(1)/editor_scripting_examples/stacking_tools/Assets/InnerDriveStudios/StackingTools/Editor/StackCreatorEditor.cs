using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StackCreator))]

/**
 * Minimal editor to make it easy to call the CreateStack methods on the simple stack creator.
 */
public class StackCreatorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		StackCreator stackCreator = (StackCreator)target;

		DrawDefaultInspector();

		if (GUILayout.Button("Create new stack"))
		{
			stackCreator.CreateStack();
		}

		if (GUILayout.Button("Replace last"))
		{
			stackCreator.DeleteLastStack();
			stackCreator.CreateStack();
		}

		if (GUILayout.Button("Delete last"))
		{
			stackCreator.DeleteLastStack();
		}

	}

	void OnSceneGUI()
	{
		StackCreator stackCreator = (StackCreator)target;

		Event e = Event.current;

		switch (e.type)
		{
			case EventType.KeyDown:

				if (e.keyCode == stackCreator.placementKey)
				{
					stackCreator.CreateStack();
				}
				else if (e.keyCode == stackCreator.replacementKey)
				{
					stackCreator.DeleteLastStack();
					stackCreator.CreateStack();
				}
				break;
		}
	}

}
