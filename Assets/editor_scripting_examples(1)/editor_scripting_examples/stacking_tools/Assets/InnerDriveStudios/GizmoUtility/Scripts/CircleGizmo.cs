using UnityEngine;

/**
 * Simple wrapper around the GizmoUtility class for convenience and for testing.
 * 
 * @author J.C. Wichman - InnerDriveStudios.com
 */
public class CircleGizmo : MonoBehaviour {

	[Range(0.1f, 5)]
	public float width = 1;
	[Range(0.1f, 5)]
	public float height = 1;
	[Range(3, 36)]
	public int sides = 1;
	[Range(0, 2*Mathf.PI)]
	public float phaseOffset;
	public Color color = Color.black;
	[Range(0,20)]
	public int thickness = 5;
	
	private void OnDrawGizmos()
	{
		GizmoUtility.DrawEllipse(transform.position, transform.rotation, color, width, height, sides, phaseOffset, thickness);
	}
}
