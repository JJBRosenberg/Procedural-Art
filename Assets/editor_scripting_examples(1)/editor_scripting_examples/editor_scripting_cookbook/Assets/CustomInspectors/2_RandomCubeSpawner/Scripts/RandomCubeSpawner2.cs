using UnityEngine;

public class RandomCubeSpawner2: RandomCubeSpawner1
{

	override public void Generate()
	{
		for (int x = 0; x < columns; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				t.name = $"Cube_{x}_{y}";
				t.localScale = new Vector3(1, Random.Range(minHeight, maxHeight), 1);
				t.Translate(new Vector3(x, 0, y) * plotSize + Vector3.up * t.localScale.y * 0.5f);

				//the only downside of #tags is the horrible layout
				#if UNITY_EDITOR
					//Undo.IncrementCurrentGroup();
					UnityEditor.Undo.RegisterCreatedObjectUndo(t.gameObject, "random cubes");
				#endif

			}
		}
	}

}

