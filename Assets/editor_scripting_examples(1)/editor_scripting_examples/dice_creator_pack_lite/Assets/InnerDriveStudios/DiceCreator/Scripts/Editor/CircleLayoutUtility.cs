using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using InnerDriveStudios.DiceCreator;

public class CircleLayoutUtility : EditorWindow
{
	[SerializeField]
	private DieSides[] _dice;
	private float _radius = 3;
	private bool _putFirstDieInCenter = true;
	private Vector3 _center = new Vector3(0, 1, 0);

	SerializedObject _dieSidesSO;
	SerializedProperty _dieSidesSP;

	[MenuItem("IDS/DiceCreator/Circle layout utility")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<CircleLayoutUtility>();
	}

	private void OnEnable()
	{
		_dieSidesSO = new SerializedObject(this);
		_dieSidesSP = _dieSidesSO.FindProperty("_dice");
		_dieSidesSP.isExpanded = true;
		_dieSidesSP.arraySize = Mathf.Max(_dieSidesSP.arraySize, 1);

		titleContent = new GUIContent("Circle layout utility");
	}

	private void OnGUI()
	{
		if (_dieSidesSP == null) return;

		GUI.enabled = true;
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Which dice would you like to process?", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(_dieSidesSP, true); // True means show children
		_dieSidesSO.ApplyModifiedProperties();            // Remember to apply modified properties

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Radius:", "Adjust if needed."));
		_radius = EditorGUILayout.Slider(_radius, 1, 10);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Use first die as center:", "Adjust if needed."));
		_putFirstDieInCenter = EditorGUILayout.Toggle(_putFirstDieInCenter);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Center:", "Adjust if needed."));
		_center = EditorGUILayout.Vector3Field("Center:", _center);
		EditorGUILayout.EndHorizontal();

		GUI.enabled = _dice != null && _dice.Length > 0;

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if (GUILayout.Button("Go !"))
		{
			processAll();
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		GUI.enabled = true;
	}

	private void processAll()
	{
		int amountOfDieInCircle = _putFirstDieInCenter ? _dice.Length - 1 : _dice.Length;
		float phaseOffset = (2 * Mathf.PI) / (float)amountOfDieInCircle;

		for (int i = 0; i < _dice.Length; i++)
		{
			if (_dice[i] == null) continue;

			Vector3 position = _center + new Vector3(
					_radius * Mathf.Cos(phaseOffset * i),
					0,
					_radius * Mathf.Sin(phaseOffset * i)
				);

			_dice[i].transform.position = position;
		}

		if (_putFirstDieInCenter && _dice[0] != null)
		{
			_dice[0].transform.position = _center;
		}

	}
		

}

