using UnityEditor;
using UnityEngine;

public class Circlelizer1 : EditorWindow
{
	[MenuItem("Custom menu/Cirlelizer yeah! (1)")]
	private static void init()
	{
		//Create a window if it didn't exist, or get it if it did... then show it.
		Circlelizer1 window = GetWindow<Circlelizer1>("Circlelizer yeah!");
		window.minSize = window.maxSize = new Vector2(300, 200);
		window.Show();
	}
}

