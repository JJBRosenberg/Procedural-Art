using UnityEngine;

/**
 * Simple wrapper around the GizmoUtility class for convenience and for testing.
 * 
 * @author J.C. Wichman - InnerDriveStudios.com
 */
public class CrossGizmo : MonoBehaviour {

	[Range(0.1f, 5)]
	public float width = 1;
	[Range(0.1f, 5)]
	public float height = 1;
	public Color color = Color.black;
	[Range(0, 20)]
	public int thickness = 5;

	private void OnDrawGizmos()
	{
		GizmoUtility.DrawCross(transform.position, transform.rotation, color, width, height, thickness);
	}
}
