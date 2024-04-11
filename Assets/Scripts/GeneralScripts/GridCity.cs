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
        public GameObject debugPrefab; 

        public float buildDelaySeconds = 0.1f;
        public int townCenterRows; 
        public int townCenterColumns; 

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

                    // Check if the current cell is within the inner area
                    if (IsInnerArea(row, col))
                    {
                        Debug.Log($"Inner area at ({row}, {col})");
                        // Instantiate debug cube in the inner area
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

        bool IsInnerArea(int row, int col)
        {
            int startRow = (rows - townCenterRows) / 2;
            int startCol = (columns - townCenterColumns) / 2;
            return row >= startRow && row < startRow + townCenterRows && col >= startCol && col < startCol + townCenterColumns;
        }
    }
}
