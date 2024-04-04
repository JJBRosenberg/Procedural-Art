using UnityEngine;

/**
 * Just a sample script to demonstrate some custom inspector principles.
 */
public class RandomCubeSpawner1: MonoBehaviour
{
	//all our building 'parameters'
	[Range(5, 10)] public int rows = 5;
	[Range(5, 10)] public int columns = 5;
	[Range(1.1f, 2)] public float plotSize = 2;
	[Range(1, 10)] public float minHeight = 3;
	[Range(1, 10)] public float maxHeight = 6;

	//trigger some validation every time values are modified in the inspector (triggers on ANY value change)
	private void OnValidate()
	{
		float min = Mathf.Min(minHeight, maxHeight);
		float max = Mathf.Max(minHeight, maxHeight);

		minHeight = min;
		maxHeight = max;
	}

	private void Awake()
	{
		Generate();
	}

	[ContextMenu("Generate")]
	virtual public void Generate()
	{
		for (int x = 0; x < columns; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				t.name = $"Cube_{x}_{y}";
				t.localScale = new Vector3(1, Random.Range(minHeight, maxHeight), 1);
				t.Translate(new Vector3(x, 0, y) * plotSize + Vector3.up * t.localScale.y * 0.5f);
			}
		}
	}

}

