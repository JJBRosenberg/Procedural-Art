using System.Collections.Generic;
using UnityEngine;

/**
 * Creates stacks of items with different scale and rotation settings.
 * 
 * @author J.C. Wichman - InnerDriveStudios.com
 */
 [ExecuteInEditMode]
public class StackCreator : MonoBehaviour {

	[Header("Placement settings")]
	[Tooltip ("If the parent name is not empty, a new object with the given name will be created as parent for the created objects.")]
	public string newParentName = "Stack";
	[Tooltip("If checked, a ray will be cast down to automatically locate a surface below the stackcreator to place the objects on.")]
	public bool keepGrounded = true;
	[Tooltip("The layer mask for valid ground objects. Note that ground objects need to have a collider in order to be hit with a ray.")]
	public LayerMask groundMask;

	[Header("Prefab settings")]
	[Tooltip("All prefabs in this list will randomly be picked to built a stack from.")]
	public GameObject[] stackPrefabs;

	[Header("Stacksize settings")]
	[Tooltip("The minimum amount of objects in the stack.")]
	[Range(1,100)]
	public int minStackSize = 5;
	[Range(1,100)]
	[Tooltip("The maximum amount of objects in the stack.")]
	public int maxStackSize = 10;

	[Header("Scale settings")]
	[Range(0.5f, 2)]
	[Tooltip("The scale for the object on the bottom of the stack. Interpolated towards the top scale to get a base scale for the current object in the stack.")]
	public float bottomObjectScale = 1;
	[Range(0.5f, 2)]
	[Tooltip("The scale for the object on the top of the stack. Interpolated from the bottom scale to get a base scale for the current object in the stack.")]
	public float topObjectScale = 1;
	[Range(0, 0.5f)]
	[Tooltip("The variation in scale for all the objects in the stack." +
		"The actual scale will be between (1-variation) * the base scale and (1+variation) * the base scale.")]
	public float scaleVariation = 0.5f;

	[Header("Position settings")]
	[Range(0, 0.5f)]
	[Tooltip("A randomly added position offset added to the xz direction to make stacks look less organized.")]
	public float randomPositionOffset = 0;

	[Header("Rotation settings")]
	[Range(0, 180)]
	[Tooltip("A random rotation between -VALUE and +VALUE for the first object in the stack.")]
	public float startRotationVariance = 0;
	[Range(0, 180)]
	[Tooltip("A random rotation between -VALUE and +VALUE for each next object in the stack.")]
	public float objectRotationVariance = 0;
	[Range(-180, 180)]
	[Tooltip ("A fixed offset rotation that is added for each next object in the stack.")]
	public float objectRotationOffset = 0;

	[Header("Collider settings")]
	[Tooltip("Specifies whether to add a compound capsule collider to the stack parent (if applicable)")]
	public bool compoundCapsuleCollider = false;

	[Header("Other")]
	[Tooltip("Change this to adjust the auto calculated y offset between the objects. Automatically adjusted by the scale. Default 0.")]
	[Range(-0.1f, 0.1f)]
	public float yOffset;
	[Tooltip("Drag the object that contains this component through the scene and press this key to place a stack.")]
	public KeyCode placementKey = KeyCode.S;
	[Tooltip("Drag the object that contains this component through the scene and press this key to replace the last stack.")]
	public KeyCode replacementKey = KeyCode.R;

	void OnValidate()
	{
		minStackSize = Mathf.Min(minStackSize, maxStackSize);
		maxStackSize = Mathf.Max(minStackSize, maxStackSize);
	}

	private void OnDrawGizmos()
	{
		//set our starting values based on our own transform
		Vector3 stackPosition = transform.position;
		float stackRotation = transform.rotation.eulerAngles.y;

		//get an indication of the size of the bottom object in the stack
		float width = 1f;
		float height = 1f;

		if (stackPrefabs != null && stackPrefabs.Length > 0)
		{
			Bounds? bounds = StackUtility.FindBounds(stackPrefabs[0]);
			if (bounds != null)
			{
				Bounds actualBounds = (Bounds)bounds;
				width = actualBounds.size.x;
				height = actualBounds.size.z;
			}
		}

		//now see if we can find the ground below us
		Vector3 surfacePosition = stackPosition;
		bool groundFound = StackUtility.FindStackPositionBelow(ref surfacePosition, groundMask);

		if (keepGrounded)
		{
			//if we want to keep the stack grounded, draw an indicator of where that is going to happen
			if (groundFound)
			{
				drawStackIndicator(stackPosition, stackRotation, Color.gray * 0.5f, width, height);
				drawStackIndicator(surfacePosition, stackRotation, Color.green, width, height);
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(stackPosition, surfacePosition);
			}
			else //or a red square if we could not locate the ground
			{
				drawStackIndicator(stackPosition, stackRotation, Color.red, width, height);
			}
		} else
		{
			//if we do not want to keep the stack grounded, we still draw a sort of shadow indicator
			//because it makes placement easier
			drawStackIndicator(stackPosition, stackRotation, Color.green, width, height);
			if (groundFound)
			{
				drawStackIndicator(surfacePosition, stackRotation, Color.gray, width, height);
			}
		}
	}
	
	/**
	 * Draws a square with a cross using the given color.
	 */
	private void drawStackIndicator (Vector3 pPosition, float pYRotation, Color pColor, float pWidth = 1f, float pHeight = 0.75f)
	{
		GizmoUtility.DrawSquare(pPosition, Quaternion.Euler(90, pYRotation, 0), pColor, pWidth, pHeight,4);
		GizmoUtility.DrawCross(pPosition, Quaternion.Euler(90, pYRotation, 0), pColor, pWidth/2, pHeight/2, 4);
	}

	private List<GameObject> _history;

	//helper method to create stack according to settings above
	public void CreateStack()
	{
		//check our creation settings
		Vector3 stackPosition = transform.position;
		Vector3 surfacePosition = stackPosition;
		bool groundFound = StackUtility.FindStackPositionBelow(ref surfacePosition, groundMask);

		if (keepGrounded && !groundFound)
		{
			Debug.Log("Cannot place stack using current settings. No ground found.");
			return;
		}

		_history = StackUtility.CreateStack(
				newParentName,
				keepGrounded && groundFound ? surfacePosition : stackPosition,
				transform.rotation.eulerAngles.y,
				Random.Range (minStackSize, maxStackSize+1),
				stackPrefabs,
				bottomObjectScale,
				topObjectScale,
				scaleVariation,
				randomPositionOffset,
				startRotationVariance, 
				objectRotationVariance,
				objectRotationOffset,
				yOffset,
				compoundCapsuleCollider
		);
	}

	public void DeleteLastStack()
	{
		if (_history == null) return;

		for (int i = _history.Count - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(_history[i]);
		}
	}

}
