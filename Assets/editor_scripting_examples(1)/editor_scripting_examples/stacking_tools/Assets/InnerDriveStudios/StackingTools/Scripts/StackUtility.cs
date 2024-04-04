using System.Collections.Generic;
using UnityEngine;

/**
 * StackUtility class to create stacks of items through code.
 * 
 * @author J.C. Wichman - www.innerdrivestudios.com
 */
public class StackUtility {

	/**
	 * Helper method to find a valid stack position below a given stackposition.
	 * For example you can pick a point above a floor and this method will return 
	 * a position on the floor.
	 * 
	 * @param pStackPosition	the position to start looking from (downwards)
	 * @param pLayerMask		the layermask of the objects to include in the downward raycast
	 */
	public static bool FindStackPositionBelow (ref Vector3 pStackPosition, LayerMask pLayerMask )
	{
		//use given stackposition as start to cast a ray down to find the first target we hit
		//if we can't find a ray, just return false
		RaycastHit info;
		Ray ray = new Ray(pStackPosition, Vector3.down);
		if (Physics.Raycast(ray, out info, float.PositiveInfinity, pLayerMask))
		{
			pStackPosition = info.point;
			return true;
		}
		else
		{
			return false;
		}

	}

	/**
	 * Create a stack of objects, automatically randomizing them, scaling them, rotating them etc.
	 * 
	 * @param pParentName			if pParentName != null, all objects will be created as children of a parent with the given name
	 * @param pStackPosition		the world space position of the bottom of the stack
	 * @param pStackRotation		the world space y rotation of the stack (0,360)
	 * @param pStackSize			the amount of objects in the stack (1-...?)
	 *
	 * @param pPrefabs				the prefabs to choose from randomly when creating objects in the stack
	 * 
	 * @param pStartScale			the uniform scale for the object on the bottom of the stack (interpolated to top) 
	 * @param pEndScale				the uniform scale for the object on the top of the stack (interpolated from bottom) 
	 * @param pScaleVariation		the percentage of scale variation for each book (A range of 0..0.5f works best)
	 * 
	 * @param pRandomPositionOffset	a random offset added to each next book, calculated from the center
	 * 
	 * @param pStartRotationVariance	the variance in rotation for the first object in the stack (0 .. 180)
	 *									(calculated as a number between (-pObjectRotationVariance, pObjectRotationVariance)
 	 * @param pObjectRotationVariance	the variance in rotation for each subsequent object (0 .. 180)
	 *									(calculated as a number between (-pObjectRotationVariance, pObjectRotationVariance)
 	 * @param pObjectRotationOffset		the added rotation to each object (-180 .. 180)
	 * @param pYOffset				an additional yOffset padding between the objects automatically multiplied with the scale							 
	 *									
	 * @param pCompoundCapsuleCollider	should we create a compound capsule collider for the stack as a whole
	 * 
	 * @return a list of all created root objects (which is 1 if pParentName.Length > 0)
	 */
	public static List<GameObject> CreateStack(
		string pParentName,
		Vector3 pStackPosition,
		float pStackRotation,
		int pStackSize, 

		GameObject[] pPrefabs, 

		float pStartScale,
		float pEndScale,
		float pScaleVariation,

		float pRandomPositionOffset,

		float pStartRotationVariance,
		float pObjectRotationVariance,
		float pObjectRotationOffset,

		float pYOffset,
		bool pCompoundCapsuleCollider
	)
	{
		//during creation keep a list of created objects so we can return that to the caller
		List<GameObject> rootStackObjects = new List<GameObject>();

		//set some start variables that we can overwrite later
		//start by assuming the objects will be attached directly to the world
		Transform parent = null;
		Vector3 startPosition = pStackPosition;
		float startRotation = pStackRotation + Random.Range(-pStartRotationVariance, pStartRotationVariance);

		//if required created an addition stack parent and attach it to the main parent (if given)
		bool createIntermediateNode = pParentName != null && pParentName.Length > 0;
		if (createIntermediateNode)
		{
			//overwrite the null parent with this new intermediate node
			parent = new GameObject(pParentName).transform;
			rootStackObjects.Add(parent.gameObject);
			parent.position = pStackPosition;
			parent.rotation = Quaternion.AngleAxis(startRotation, Vector3.up);

			//but since all objects will now be nested under this new node, reset the object start position & rotation to 0
			startPosition = Vector3.zero;
			startRotation = 0;
		}

		//now build the stack of objects
		//this loop is fairly long, but all variables are so related that 
		//splitting it up into smaller parts does not improve the readability or performance.

		//stacktop is used to place a new book and for collider calculation
		float stackTop = 0;		
		//maxExtents is used to calculate a radius for a compound collider
		float maxExtents = 0;

		for (int i = 0; i < pStackSize; i++)
		{
			//create a random object, the object should be created from a prefab at (0,0,0), without rotation, keeping the original scale
			GameObject newObject = GameObject.Instantiate(pPrefabs[Random.Range(0, pPrefabs.Length)], Vector3.zero, Quaternion.identity);

			//calculate scale based on start/end/index
			float baseScale = (pStackSize > 1) ?
									Mathf.Lerp(pStartScale, pEndScale, ((float)i) / (pStackSize - 1)) :
									pStartScale;
			baseScale += Random.Range(-baseScale * pScaleVariation, baseScale * pScaleVariation);
			newObject.transform.localScale = newObject.transform.localScale * baseScale;

			//get the bounds for this object so we can use those values for positioning etc
			Bounds? bounds = FindBounds(newObject);
			if (bounds == null)
			{
				Debug.LogWarning("Created object has no meshrenderers, check your prefabs!");
				continue;
			}
			Bounds actualBounds = (Bounds)bounds;

			//now that we have the bounds, set the position with an optionally added random offset
			Vector2 randomOffset = Random.insideUnitCircle * pRandomPositionOffset;
			newObject.transform.localPosition =
				//the bottom of the stack
				startPosition +		
				new Vector3(
					0,
					//the distance from the bottom of the stack, to the top of the last object
					stackTop +
					//plus the offset from the center of the bounds to the bottom (or top) of the object
					actualBounds.extents.y - actualBounds.center.y 
					, 
					0
				) +
				new Vector3(randomOffset.x, 0, randomOffset.y);

			//set our top to be the new starting point
			stackTop += (2 * actualBounds.extents.y) + (pYOffset * baseScale);
			//calculate the max extents in case we want to add a compound collider
			maxExtents = Mathf.Max(Mathf.Max(actualBounds.extents.x, actualBounds.extents.z), maxExtents);

			//update parenting and history, if we have a parent, attach us, if not 
			//add us to the history list so that the caller can remove us if required
			if (parent != null) newObject.transform.SetParent(parent, false);
			else rootStackObjects.Add(newObject);

			//update object rotation
			newObject.transform.localRotation = Quaternion.AngleAxis(startRotation, Vector3.up);
			startRotation += Random.Range(-pObjectRotationVariance, pObjectRotationVariance) + pObjectRotationOffset;
		}

		//we can also add a compound collider assuming there is an intermediate stack mode
		if (pCompoundCapsuleCollider)
		{
			if (createIntermediateNode)
			{
				CapsuleCollider collider = parent.gameObject.AddComponent<CapsuleCollider>();
				collider.center = new Vector3(0, stackTop/2, 0);
				collider.height = stackTop;
				collider.radius = maxExtents;
			} else
			{
				Debug.Log("Cannot add a compound collider, since there is no intermediate node.");
			}
		}

		return rootStackObjects;
	}

	public static Bounds? FindBounds (GameObject pGameObject)
	{
		if (pGameObject == null) return null;

		MeshRenderer[] meshRenderers = pGameObject.GetComponentsInChildren<MeshRenderer>();

		if (meshRenderers.Length > 0)
		{
			Bounds bounds = meshRenderers[0].bounds;

			for (int i = 1; i < meshRenderers.Length; i++)
			{
				bounds.Encapsulate(meshRenderers[i].bounds);
			}

			return bounds;

		} else
		{
			return null;
		}
	}

}


