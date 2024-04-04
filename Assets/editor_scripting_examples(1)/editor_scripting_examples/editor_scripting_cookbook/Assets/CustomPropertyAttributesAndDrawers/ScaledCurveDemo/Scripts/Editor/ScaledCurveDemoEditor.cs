using UnityEditor;

/**
 * To 'trigger' the rendering of custom property editors, you must have an editor for the component that uses them.
 */

[CustomEditor(typeof(ScaledCurveDemo))]
public class ScaledCurveDemoEditor : Editor
{
	// you don't even need to override OnInspectorGUI!
}