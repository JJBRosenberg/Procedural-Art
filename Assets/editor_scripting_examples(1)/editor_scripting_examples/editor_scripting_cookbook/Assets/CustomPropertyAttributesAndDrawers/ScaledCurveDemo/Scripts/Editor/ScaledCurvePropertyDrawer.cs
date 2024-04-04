using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScaledCurve))]
public class ScaledCurvePropertyDrawer : PropertyDrawer
{
    const int curveWidth = 50;
    const float min = 0;
    const float max = 1;

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty scale = prop.FindPropertyRelative("scale");
        SerializedProperty curve = prop.FindPropertyRelative("curve");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Scaled curve", GUILayout.MinWidth(0));
		scale.floatValue = EditorGUILayout.Slider(scale.floatValue, 0, 1);
		//1/scale.floatValue is a dirty trick to make it seem the slider affects the actual curve
        EditorGUILayout.CurveField(curve, Color.green, new Rect(0, 0, 1, 1/scale.floatValue), new GUIContent(""));
		EditorGUILayout.EndHorizontal();
    }

}