using UnityEngine;

/**
 * Utility class to draw some additional shapes.
 * 
 * @author J.C. Wichman - InnerDriveStudios.com
 */
public class GizmoUtility {

	/**
	 * Draws an ellipse at the given position using the given orientation, color, size, 
	 * number of sides, phase offset and thickness.
	 * 
	 * @param pPosition the world position at which to draw the circle
	 * @param pOrientation the way the circle's flatside should be facing
	 * @param pColor the color to use for drawing
	 * @param pWidth the x width of the circle (for elipses)
	 * @param pHeight the y height of the circle (for elipses)
	 * @param pSides the nr of sides of the circle, eg 4 draws a diamond or square shape (depending on the phase offset)
	 * @param pPhaseOffset the phase offset to use for the drawn points (fed into the trig functions used to generate the circle)
	 * @param pThickness the thickness of the line in screenpixels (between 1 and 10)
	 */
	public static void DrawEllipse(
		Vector3 pPosition, Quaternion pOrientation, Color pColor, float pWidth = 1, float pHeight = 1, int pSides = 32, float pPhaseOffset = 0, int pThickness = 1
		)
	{
		//set and store the color so we can revert it back at the end
		Color previousColor = Gizmos.color;
		Gizmos.color = pColor;

		//make sure we don't go overboard with the line thickness. Bad for Unity's health.
		pThickness = Mathf.Clamp(pThickness, 1, 10);

		//get an indication of how many units make up one pixel in screenspace
		float offsetPerIteration = getOffsetPerIterationIndicator(pPosition, pOrientation, pThickness);

		//draw the shape "thickness" number of times, increasing the size each iteration by "1" pixel
		for (int i = 0; i < pThickness; i++)
		{
			float offset = i * offsetPerIteration;
			_innerDrawEllipse(pPosition, pOrientation, pColor, pWidth + offset, pHeight + offset, pSides, pPhaseOffset);
		}

		Gizmos.color = previousColor;
	}

	/**
	 * Draws an ellipse at the given position using the given orientation, color, size, number of sides and phase offset.
	 * 
	 * @see DrawEllipse for a description of the parameters.
	 */
	private static void _innerDrawEllipse(
			Vector3 pPosition, Quaternion pOrientation, Color pColor, float pWidth = 1, float pHeight = 1, int pSides = 32, float pPhaseOffset = 0
		)
	{
		//sanitize some parameters 
		pSides = Mathf.Clamp(pSides, 3, 36);
		pWidth /= 2;
		pHeight /= 2;

		//a whole circle in trig is 2PI so if we have pSides, our phasestep is 2pi/sides
		float phaseStep = 2 * Mathf.PI / pSides;

		//phaseoffset almost works like rotation, but when xscale != yscale the effect is different,
		//so we also have a pPhaseOffset next to a rotation
		Vector3 start = new Vector3(pWidth * Mathf.Cos(pPhaseOffset), pHeight * Mathf.Sin(pPhaseOffset), 0);

		for (int i = 0; i < pSides; i++)
		{
			float phase = phaseStep * (i+1) + pPhaseOffset;
			//generate the basic vector
			Vector3 end = new Vector3(Mathf.Cos(phase) * pWidth, Mathf.Sin(phase) * pHeight, 0);
			//reorient the vector to the given orientation and offset it with the start position
			Gizmos.DrawLine(pPosition + pOrientation * start, pPosition + pOrientation * end);
			start = end;
		}
	}

	/**
	 * Draws a square at the given position using the given orientation, color, size and thickness.
	 * 
	 * @param pPosition the world position at which to draw the square
	 * @param pOrientation the way the square's flatside should be facing
	 * @param pColor the color to use for drawing
	 * @param pWidth the x width of the square
	 * @param pHeight the y height of the square
	 * @param pThickness the thickness of the line in screenpixels (between 1 and 10)
	 */
	public static void DrawSquare(
		Vector3 pPosition, Quaternion pOrientation, Color pColor, float pWidth = 1, float pHeight = 1, int pThickness = 1
		)
	{
		Color previousColor = Gizmos.color;
		Gizmos.color = pColor;

		//make sure we don't go overboard with the line thickness. Bad for Unity's health.
		pThickness = Mathf.Clamp(pThickness, 1, 10);

		//get an indication of how many units make up one pixel in screenspace
		float offsetPerIteration = getOffsetPerIterationIndicator(pPosition, pOrientation, pThickness);
		//draw the shape "thickness" number of times, increasing the size each iteration by "1" pixel
		for (int i = 0; i < pThickness; i++)
		{
			float offset = i * offsetPerIteration;
			_innerDrawSquare(pPosition, pOrientation, pColor, pWidth + offset, pHeight + offset);
		}

		Gizmos.color = previousColor;
	}

	/**
	 * Draws a square at the given position using the given orientation, color and size.
	 * 
	 * @see DrawSquare for a description of the parameters.
	 */
	private static void _innerDrawSquare(Vector3 pPosition, Quaternion pOrientation, Color pColor, float pWidth = 1, float pHeight = 1)
	{
		pWidth /= 2; 
		pHeight /= 2;

		//get all positions adjusted to the new orientation offset by the position
		Vector3 topLeft = pPosition + pOrientation * new Vector3(-pWidth, pHeight);
		Vector3 topRight = pPosition + pOrientation * new Vector3(pWidth, pHeight);
		Vector3 bottomLeft = pPosition + pOrientation * new Vector3(-pWidth, -pHeight);
		Vector3 bottomRight = pPosition + pOrientation * new Vector3(pWidth, -pHeight);

		Gizmos.DrawLine(topLeft, topRight);
		Gizmos.DrawLine(topRight, bottomRight);
		Gizmos.DrawLine(bottomRight, bottomLeft);
		Gizmos.DrawLine(bottomLeft, topLeft);
	}

	/**
	 * Draws a cross at the given position using the given orientation, color, size and thickness.
	 * 
	 * @param pPosition the world position at which to draw the cross
	 * @param pOrientation the way the cross' flatside should be facing
	 * @param pColor the color to use for drawing
	 * @param pWidth the x width of the cross 
	 * @param pHeight the y height of the cross 
	 * @param pThickness the thickness of the line in screenpixels (between 1 and 10)
	 */
	public static void DrawCross(
		Vector3 pPosition, Quaternion pOrientation, Color pColor, float pWidth = 1, float pHeight = 1, int pThickness = 5
		)
	{
		Color previousColor = Gizmos.color;
		Gizmos.color = pColor;

		//make sure we don't go overboard with the line thickness. Bad for Unity's health.
		pThickness = Mathf.Clamp(pThickness, 1, 10);

		//get an indication of the direction of the crosses x axis
		Vector3 topLeft = new Vector3(-pWidth, pHeight);
		Vector3 topRight = new Vector3(pWidth, pHeight);
		Vector3 offsetVector = pOrientation * (topRight - topLeft).normalized;

		//get an indication of how many units make up one pixel in screenspace
		float offsetPerIteration = getOffsetPerIterationIndicator(pPosition, pOrientation, pThickness);
		Vector3 start = -offsetVector * pThickness * offsetPerIteration / 2;

		for (int i = 0; i < pThickness; i++)
		{
			Vector3 offset = offsetVector * (i * offsetPerIteration);
			_innerDrawCross(pPosition + start + offset, pOrientation, pColor, pWidth, pHeight);
		}

		Gizmos.color = previousColor;
	}

	/**
	 * Draws a cross at the given position using the given orientation, color and size.
	 * 
	 * @see DrawCross for a description of the parameters.
	 */
	private static void _innerDrawCross(Vector3 pPosition, Quaternion pOrientation, Color pColor, float pWidth = 1, float pHeight = 1)
	{
		pWidth /= 2;
		pHeight /= 2;

		Vector3 topLeft = pPosition + pOrientation * new Vector3(-pWidth, pHeight);
		Vector3 topRight = pPosition + pOrientation * new Vector3(pWidth, pHeight);
		Vector3 bottomLeft = pPosition + pOrientation * new Vector3(-pWidth, -pHeight);
		Vector3 bottomRight = pPosition + pOrientation * new Vector3(pWidth, -pHeight);

		Gizmos.DrawLine(topLeft, bottomRight);
		Gizmos.DrawLine(topRight, bottomLeft);
	}


	/**
	 * @return an offset to use for the lines based on the sceneview camera and the size of one unit in pixels,
	 * in the direction of the shapes positive x axis.
	 */
	private static float getOffsetPerIterationIndicator (Vector3 pPosition, Quaternion pRotation, int thickness)
	{
		Camera camera = Camera.current;
		if (camera == null) return 0.01f;

		//get the screenpoints for a position and the same position plus one unit to the right in a given orientation
		Vector3 pointA = camera.WorldToScreenPoint(pPosition);
		Vector3 pointB = camera.WorldToScreenPoint(pPosition + pRotation * Vector3.right);
		Vector3 diff = pointB - pointA;
		//the length of screenspace vector gives us an indication of the amount of pixels in that vector
		float pixelsPerUnit = diff.magnitude;
		//limit the pixelsPerUnit by some value related to the thickness to prevent overlay thick lines
		pixelsPerUnit = Mathf.Max(pixelsPerUnit, thickness * 4);
		//how many units is 1 pixel?
		return 1/pixelsPerUnit;

	}


}
