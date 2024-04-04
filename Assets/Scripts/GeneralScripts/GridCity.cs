// Version 2023
//  (Updates: supports different root positions)
using UnityEngine;

namespace Demo {
	public class GridCity : MonoBehaviour {
		public int rows = 10;
		public int columns = 10;
		public int rowWidth = 10;
		public int columnWidth = 10;
		public GameObject[] buildingPrefabs;

		public float buildDelaySeconds = 0.1f;

		void Start() {
			Generate();
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.G)) {
				DestroyChildren();
				Generate();
			}
		}

		void DestroyChildren() {
			for (int i = 0; i<transform.childCount; i++) {
				Destroy(transform.GetChild(i).gameObject);
			}
		}

		void Generate()
		{
			for (int row = 0; row < rows; row++)
			{
				for (int col = 0; col < columns; col++)
				{
					// Create a new building, chosen randomly from the prefabs:
					int buildingIndex = Random.Range(0, buildingPrefabs.Length);
					GameObject newBuilding = Instantiate(buildingPrefabs[buildingIndex], transform);

					// Calculate position with random offsets:
					float offsetX = Random.Range(-columnWidth / 2f, columnWidth / 2f);
					float offsetZ = Random.Range(-rowWidth / 2f, rowWidth / 2f);
					Vector3 newPosition = new Vector3(col * columnWidth + offsetX, 0, row * rowWidth + offsetZ);

					// Place it in the grid:
					newBuilding.transform.localPosition = newPosition;

					// If the building has a Shape (grammar) component, launch the grammar:
					Shape shape = newBuilding.GetComponent<Shape>();
					if (shape != null)
					{
						shape.Generate(buildDelaySeconds);
					}
				}
			}
		}

	}
}