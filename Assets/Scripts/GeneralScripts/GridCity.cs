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
        public GameObject cylinderPrefab; // GameObject for the cylinder
        public float cylinderHeight = 20f; // Public variable for the height of the cylinder

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
            int centerRow = (rows - townCenterRows) / 2 + townCenterRows / 2;
            int centerCol = (columns - townCenterColumns) / 2 + townCenterColumns / 2;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 worldPosition = new Vector3(col * columnWidth, 0, row * rowWidth);

                    if (IsInnerArea(row, col))
                    {
                        if (row == centerRow && col == centerCol)
                        {
                            GameObject cylinder = Instantiate(cylinderPrefab, worldPosition, Quaternion.identity, transform);
                            cylinder.transform.localScale = new Vector3(cylinder.transform.localScale.x, cylinderHeight, cylinder.transform.localScale.z);
                        }
                        else
                        {
                            Instantiate(debugPrefab, worldPosition, Quaternion.identity, transform);
                        }
                    }
                    else
                    {
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
