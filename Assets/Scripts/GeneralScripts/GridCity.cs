using UnityEngine;

namespace Demo
{
    public class GridCity : MonoBehaviour
    {
        public int rows = 10;
        public int columns = 10;
        public int rowWidth = 10;
        public int columnWidth = 10;
        public GameObject[] buildingPrefabs;
        public GameObject debugPrefab; // Add reference to the debug cube prefab

        public float buildDelaySeconds = 0.1f;

        private ValueGrid valueGrid;

        void Start()
        {
            valueGrid = GetComponent<ValueGrid>();
            Generate();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                DestroyChildren();
                Generate();
            }
        }

        void DestroyChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        void Generate()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 worldPosition = new Vector3(col * columnWidth, 0, row * rowWidth);
                    // Check if the current cell is part of the no-build zone
                    if (valueGrid != null && valueGrid.GetCell(row, col) == 1)
                    {
                        Debug.Log("No build zone!");
                        // Instantiate debug cube in the no-build zone
                        Instantiate(debugPrefab, worldPosition, Quaternion.identity, transform);
                    }
                    else
                    {
                        // Instantiate buildings as usual
                        int buildingIndex = Random.Range(0, buildingPrefabs.Length);
                        GameObject newBuilding = Instantiate(buildingPrefabs[buildingIndex], transform);

                        float offsetX = Random.Range(-columnWidth / 2f, columnWidth / 2f);
                        float offsetZ = Random.Range(-rowWidth / 2f, rowWidth / 2f);
                        Vector3 newPosition = new Vector3(col * columnWidth + offsetX, 0, row * rowWidth + offsetZ);

                        newBuilding.transform.localPosition = newPosition;

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
}
