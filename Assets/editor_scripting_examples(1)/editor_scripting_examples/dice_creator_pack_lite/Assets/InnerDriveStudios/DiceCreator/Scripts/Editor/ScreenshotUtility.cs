using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using InnerDriveStudios.DiceCreator;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;

public class ScreenshotUtility : Editor
{
	static int i = 0;
	static MaterialSet[] materialSets;
	static IEnumerable<GameObject> gameObjects;
	static bool running = false;
	static string folder = @"D:\Dropbox\HW\saxion\courses_cmgt\2_1_procedural_art\editor_scripting_lecture\unity\dice_creator_pack\screenshots\";

	[MenuItem("IDS/DiceCreator/Screen shot utility")]
	public static void TakeScreenshots()
	{
		if (running) return;
		
		if (!Directory.Exists(folder))
		{
			Debug.Log("Screen shot path does not exist.");
			return;
		} else
		{
			//System.Diagnostics.Process.Start("explorer.exe", "/select," + folder);
		}

		//EditorApplication.isPlaying = true;

		string[] materialSetCollectionGUIDList = AssetDatabase.FindAssets("t:MaterialSetCollection", new[] { PathConstants.MATERIALS_FOLDER });

		if (materialSetCollectionGUIDList.Length == 1)
		{
			MaterialSetCollection msc = AssetDatabase.LoadAssetAtPath<MaterialSetCollection>(AssetDatabase.GUIDToAssetPath(materialSetCollectionGUIDList[0]));
			materialSets = msc.materialSets;
			gameObjects = GameObject.FindObjectsOfType<DieSides>().AsEnumerable<DieSides>().Select (x => x.gameObject);
			i = 0;
			EditorApplication.update += MyMethod;
			running = true;
		}
	}

	private static void MyMethod()
	{
		try
		{
			MaterialSet ms = materialSets[i];

			Debug.Log("Grabbing:" + ms.name);
			MaterialSetUtility.MapMaterialSetToGameObjects(ms, gameObjects);
			
			ScreenCapture.CaptureScreenshot(folder + "M_"+ ms.name + ".png");
			//Thread.Sleep(1000);

		}
		catch { }

		i++;

		if (i > materialSets.Length-1)
		{
			EditorApplication.update -= MyMethod;
			running = false;
		}
	}
}

