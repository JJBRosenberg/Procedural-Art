using MenuItemCookbookRecipes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This menu item cookbook shows most if not all different ways of adding menu items 
 * in various places. The name of this class does not matter.
 * 
 * Detailed references for more info:
 * https://docs.unity3d.com/ScriptReference/MenuItem.html
 * 
 * @author J.C. Wichman, Inner Drive Studios.
 */
public class MenuItemCookbook
{
	//Menu item definitions also allow const variables to be used
	const string customMenuName = "Custom menu";

	////////////////////////////////////////////////////////////////////////////////////
	////	Recipe 1 : Basic menu items					
	////////////////////////////////////////////////////////////////////////////////////

	/**
	 * Main menu items require 2 things:
	 * - 1 or more MenuItem tags in the format [MenuItem("MainMenuName/[Optional Submenu name/]Menu item name")]
	 * - a static method to follow it
	 * 
	 * Things to note:
	 * - Main menu name must be present
	 * - Main menu name can either be an existing menu name or a new one
	 * - You can have as menu sub menus as you like
	 * - The method must be static but can be private/public & void/etc
	 * - You can use the following codes to add hotkeys to your menu items:
	 * 
	 *		% – CTRL on Windows / CMD on OSX
	 *		# – Shift
	 *		& – Alt
	 *		_ - No modifier
	 *		LEFT/RIGHT/UP/DOWN – Arrow keys
	 *		F1…F2 – F keys
	 *		HOME, END, PGUP, PGDN
	 */
	[MenuItem("Edit/Show a dialog")]
	[MenuItem("GameObject/Show a dialog A")]
	[MenuItem("GameObject/Show a dialog B", false, 10)]	//false == no validation, 10 influences order
	[MenuItem("Assets/Show a dialog")]
	[MenuItem("Assets/Create/Show a dialog")]
	[MenuItem("Custom menu/Show a dialog _F1")]
	[MenuItem(customMenuName+"/I am a sub menu/Show a dialog")]
	private static void TheNameOfThisMethodDoesNotMatterButItMustBeStatic()
    {
        EditorUtility.DisplayDialog("Main menu item demo", "It works :)", "Awesome, thanks.");
    }

	////////////////////////////////////////////////////////////////////////////////////
	////	Recipe 2 : Menu item validation
	////////////////////////////////////////////////////////////////////////////////////

	/**
	 * First define a regular menu item as done above.
	 */
	[MenuItem("Custom menu/Log Selected Transform Name")]
	static void LogSelectedTransformName()
	{
		Debug.Log("Selected Transform is on " + Selection.activeTransform.gameObject.name + ".");
	}

	/**
	 * Then define:
	 * - a static bool method
	 * - and tag it with a MenuItem tag WITH EXACTLY THE SAME NAME, true
	 * 
	 * The 'true' indicates it is a validation method.
	 * The tag matches it with the previous method.
	 * The return value indicates whether the 'original' menu item is enabled or not
	 */
	[MenuItem("Custom menu/Log Selected Transform Name", true)]
	static bool LogSelectedTransformNameValidator()
	{
		// Return false if no transform is selected.
		return Selection.activeTransform != null;
	}

	////////////////////////////////////////////////////////////////////////////////////
	////	Recipe 3 : Menu item order
	////////////////////////////////////////////////////////////////////////////////////

	/**
	 * As expected, order defines the order of your menu items ;).
	 * 
	 * Can be both negative and positive, leave more than 50 in between two items and
	 * a separator will be inserted.
	 */
	[MenuItem("Custom menu/Order demo/A", false, 49)]
	[MenuItem("Custom menu/Order demo/B", false, 50)]
	[MenuItem("Custom menu/Order demo/C", false, 51)]
	[MenuItem("Custom menu/Order demo/D", false, 0)]
	[MenuItem("Custom menu/Order demo/E", false, -50)]
	private static void TheNameOfThisMethodDoesNotMatterButItMustBeStaticToo()
	{
		EditorUtility.DisplayDialog("Main menu item demo", "It works :)", "Awesome, thanks.");
	}


	////////////////////////////////////////////////////////////////////////////////////
	////	Recipe 4 : Creating custom GameObjects through the GameObject menu
	////////////////////////////////////////////////////////////////////////////////////

	/**
	 * Add a menu item to create custom GameObjects.
	 * Priority 1 ensures it is grouped with the other menu items of the same kind
	 * and propagated to the hierarchy dropdown and hierarchy context menus.
	 */
	[MenuItem("GameObject/Custom/Custom Game Object", false, 10)]
	static void CreateCustomGameObject(MenuCommand menuCommand)
	{
		// Create a custom game object
		GameObject go = new GameObject("Custom Game Object");
		go.AddComponent<CustomComponent>();
		go.AddComponent<Rigidbody>();
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system AND MARKS THE SCENE AS CHANGED
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	////////////////////////////////////////////////////////////////////////////////////
	////	Recipe 5 : Context menu's for existing component
	////////////////////////////////////////////////////////////////////////////////////

	/**
	 * You can add context menu items to an existing component, the component's name is matched by string.
	 */
	[MenuItem("CONTEXT/Rigidbody/Double Mass")]
	static void DoubleMass(MenuCommand command)
	{
		Rigidbody body = (Rigidbody)command.context;
		body.mass = body.mass * 2;
		Debug.Log("Doubled Rigidbody's Mass to " + body.mass + " from Context Menu.");
	}

	/**
	 * And to a custom component (also see CustomComponent for some variations)
	 */
	[MenuItem("CONTEXT/CustomComponent/What's my name?")]
	private static void PrintMyName(MenuCommand command)
	{
		CustomComponent cc = (CustomComponent)command.context;
		EditorUtility.DisplayDialog("Context demo", cc.gameObject.name, "OK");
	}

	////////////////////////////////////////////////////////////////////////////////////
	////	Recipe 6 : Processing objects in your scene
	////////////////////////////////////////////////////////////////////////////////////

	[MenuItem(customMenuName+"/Process gameobjects demo")]
	private static void processAll() {

		//What do you want to do? There are so many ways to collect scene objects to process.
		//Below are some examples which you can selectively (un)comment or use for inspiration.
		//One thing to keep in mind, that based on the method that you choose, you might end up
		//processing a single gameobject twice (eg you select both a parent and its child and 
		//choose a method which processes both of them recursively)

		//Warning: there are some very simple but also some very arcane constructions below (for inspiration)

		/* All gameobjects in the scene (this approach excludes the inactive) */

		GameObject[] objectsToProcess = GameObject.FindObjectsOfType<GameObject>();
		bool recursive = false;

		/* Process all selected gameobjects *

		GameObject[] objectsToProcess = Selection.gameObjects;
		//warning: this might cause objects to be processed twice
		bool recursive = true;

		/* Process all gameobjects active or not (forced by recursion of transform component) *

		GameObject[] objectsToProcess = SceneManager.GetActiveScene().GetRootGameObjects();
		bool recursive = true;

		/* All gameobjects with a certain component eg rigidbody, flexible/powerful but complex *

		bool includeInactive = true;
		GameObject[] objectsToProcess = SceneManager.GetActiveScene().GetRootGameObjects()
		.SelectMany(g => g.GetComponentsInChildren<Rigidbody>(includeInactive)).Select(g => g.gameObject).ToArray();
		//we already use getcomponentsinchildren, which already recurses
		recursive = false;

		/*
		For documentation's sake: a method to select just the components and not the gameobjects
		List<Rigidbody> result = SceneManager.GetActiveScene().GetRootGameObjects()
		 .SelectMany(g => g.GetComponentsInChildren<Rigidbody>(true))
		 .ToList();
		*/

		/* All roots of the selected objects recursively (or not) *

		GameObject[] objectsToProcess = Selection.gameObjects.Select(g => g.transform.root.gameObject).Distinct().ToArray();
		bool recursive = false;

		/* All gameobjects that are children of the selected objects roots with a rigidbody component *

		//yep it becomes completely unreadable ;)
		bool includeInactive = false;
		GameObject[] objectsToProcess = 
			Selection.gameObjects.Select(g => g.transform.root.gameObject).Distinct().
			SelectMany(g => g.GetComponentsInChildren<Rigidbody>(includeInactive)).
			Select(g => g.gameObject).ToArray();
		bool recursive = false;

		/**/

		//In other words you have to define really clearly what objects you really want to process ;)
		//Come to the labs if you need help with that.

		foreach (GameObject go in objectsToProcess)
		{
			processGameObject(go, recursive);
		}
	}

	private static void processGameObject (GameObject pGameObject, bool pRecursive = false, char pPrefix = ' ', int depth = 0)
	{
		Debug.Log(new string(pPrefix, depth) + "Processing: " + pGameObject.name);

		if (!pRecursive) return;

		foreach (Transform child in pGameObject.transform)
		{
			processGameObject(child.gameObject, pRecursive, pPrefix, depth+1);
		}
	}

}