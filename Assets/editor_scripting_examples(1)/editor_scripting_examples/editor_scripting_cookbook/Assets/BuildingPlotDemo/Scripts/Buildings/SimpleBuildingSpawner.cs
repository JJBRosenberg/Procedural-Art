using UnityEngine;

public class SimpleBuildingSpawner : AbstractBuildingSpawner
{
	public Material[] buildingMaterials;

	public override GameObject Initialize(Vector3 position, Vector2 plotSize)
	{
		GameObject actualBuilding = GameObject.CreatePrimitive(PrimitiveType.Cube);
		float randomHeight = Random.Range(1, 5);
		actualBuilding.transform.localScale = new Vector3(plotSize.x, randomHeight, plotSize.y);
		actualBuilding.transform.position = position + Vector3.up * randomHeight * 0.5f;
		actualBuilding.name = "Awesome cube house";

		if (buildingMaterials != null && buildingMaterials.Length > 0)
		{
			actualBuilding.GetComponent<MeshRenderer>().sharedMaterial = buildingMaterials[Random.Range(0, buildingMaterials.Length)];
		}

		return actualBuilding;
	}
}
