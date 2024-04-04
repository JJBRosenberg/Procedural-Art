using UnityEngine;

public class RandomCubeSpawner3: RandomCubeSpawner1
{

	override public void Generate()
	{
		//first clear all our children, note the backwards loop!
		for (int i = transform.childCount-1; i >= 0; i--)
		{
			if (Application.isPlaying)
			{
				Destroy(transform.GetChild(i).gameObject);
			}
			else 
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
				//or if you want to implement undo functionality ...
				/*
				#if UNITY_EDITOR
					UnityEditor.Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
				#endif
				*/
			}
		}

		for (int x = 0; x < columns; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				t.name = $"Cube_{x}_{y}";
				t.localScale = new Vector3(1, Random.Range(minHeight, maxHeight), 1);
				t.Translate(new Vector3(x, 0, y) * plotSize + Vector3.up * t.localScale.y * 0.5f);

				//nest it below our own object
				t.SetParent(transform, true);

#if UNITY_EDITOR
				UnityEditor.Undo.RegisterCreatedObjectUndo(t.gameObject, "random cubes");
#endif

			}
		}
	}

}

