using UnityEngine;

/**
 * This is a sample class that you can use from your editor window to persist values.
 * 
 * @author J.C. Wichman / InnerDriveStudios.com
 */
public class PrefabSpawnerSettings : ScriptableObject
{
	public int amountOfObjectsToPlace;
	public GameObject[] prefabsToPlace;
}
