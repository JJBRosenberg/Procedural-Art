using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/**
 * This is a sample editor window that shows how to generate stuff at editor time and persist values.
 * The great thing about doing it this way is that you don't have to explicitly save settings in OnDisable.
 * 
 * @author J.C. Wichman / InnerDriveStudios.com 
 */
public class PrefabSpawner : EditorWindow
{

	//the loaded scriptableobject that persists our settings
	PrefabSpawnerSettings settings;

	//Although not strictly necessary this also demonstrates how to edit the contents
	//of a scriptableobject without too much coding
	Editor sampleSettingsEditor;

	[MenuItem("Custom menu/Prefab spawner")]
	private static void init()
	{
		GetWindow<PrefabSpawner>().maxSize = new Vector2(300,300);
		//seems to also work without show...
	}

	
	private void OnEnable()
	{
		//load settings, we can enter a path directly, but the location of this script might change
		//try to find the path using monoscript, this works since editor derives from scriptable object
		string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
		//This is important otherwise you'll overwrite this script (been there done that ;)) and don't forget the .asset!
		path = path.Replace(".cs", "_data.asset");

		settings = AssetDatabase.LoadAssetAtPath<PrefabSpawnerSettings>(path);
		if (!settings)
		{
			settings = CreateInstance<PrefabSpawnerSettings>();
			AssetDatabase.CreateAsset(settings, path);
		}
	}
	

	void OnGUI()
	{
		//show the editor
		Editor.CreateCachedEditor(settings, null, ref sampleSettingsEditor);
		sampleSettingsEditor.OnInspectorGUI();

		if (GUILayout.Button("Generate"))
		{
			if (settings.prefabsToPlace != null && settings.prefabsToPlace.Length > 0)
			{
				for (int i = 0; i < settings.amountOfObjectsToPlace; i++)
				{
					Instantiate(
						settings.prefabsToPlace[Random.Range(0, settings.prefabsToPlace.Length)], 
						Random.onUnitSphere * 4, 
						Quaternion.identity
					);
				}

				//important to save the changes
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close", GUILayout.Width(100), GUILayout.Height(30)))
		{
			Close();
		}

		EditorGUILayout.EndHorizontal();
	}


}
