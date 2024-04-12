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
        public GameObject cylinderPrefab; 
        public float cylinderHeight = 20f; 

        public float buildDelaySeconds = 0.1f;
        public int townCenterRows;
        public int townCenterColumns;

        private ValueGrid valueGrid;

        void Start()
        {
            valueGrid = GetComponent<ValueGrid>();
            valueGrid.InitializeGrid();  
            GenerateCity();  
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                valueGrid.InitializeGrid();  
                GenerateCity(); 
            }
        }

        void DestroyChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        void GenerateCity()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = new Vector3(col * columnWidth, 0, row * rowWidth);
                    if (!IsInnerArea(row, col))
                    {
                        if (!valueGrid.IsCellOccupied(row, col))
                        {
                            int buildingIndex = Random.Range(0, buildingPrefabs.Length);
                            GameObject building = Instantiate(buildingPrefabs[buildingIndex], position, Quaternion.identity, transform);
                            valueGrid.SetCellOccupied(row, col, true);
                        }
                    }
                    else
                    {
                        Instantiate(debugPrefab, position, Quaternion.identity, transform);
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
