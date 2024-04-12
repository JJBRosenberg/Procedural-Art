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
        public GameObject noBuildZonePrefab; 
        public int centralAreaWidth = 4;  
        public int centralAreaHeight = 4;
        public int seed;  

        private ValueGrid valueGrid;

        void Start()
        {
            valueGrid = GetComponent<ValueGrid>();
            Random.InitState(seed); 
            valueGrid.InitializeGrid();
            GenerateCity();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Random.InitState(seed);
                GenerateCity();
            }
        }

        void GenerateCity()
        {
            valueGrid.InitializeGrid();
            DestroyChildren();
            float noBuildProbability = Mathf.Clamp01((float)seed / 100000f);  

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = new Vector3(col * columnWidth, 0, row * rowWidth);
                    int index = valueGrid.GetMarchingSquareIndex(col, row);
                    if (!IsCentralArea(row, col))
                    {
                        PlaceBuildingOrSpace(index, position, noBuildProbability);
                    }
                }
            }
        }

        bool IsCentralArea(int row, int col)
        {
            int centralRowStart = (rows - centralAreaHeight) / 2;
            int centralColStart = (columns - centralAreaWidth) / 2;
            return row >= centralRowStart && row < centralRowStart + centralAreaHeight &&
                   col >= centralColStart && col < centralColStart + centralAreaWidth;
        }

        void PlaceBuildingOrSpace(int index, Vector3 position, float noBuildProbability)
        {
            Vector3 centerOffset = new Vector3(columnWidth / 2, 0, rowWidth / 2);
            bool placeNoBuildZone = Random.value < noBuildProbability;

            if (placeNoBuildZone)
            {
                Instantiate(noBuildZonePrefab, position + centerOffset, Quaternion.identity, transform);
                return; 
            }

            switch (index)
            {
                case 1:
                case 2:
                case 4:
                case 8:
                    InstantiateBuildingAtSingleCorner(index, position);
                    break;
                case 3:
                case 6:
                case 9:
                case 12:
                    InstantiateBuilding(position + centerOffset);
                    break;
                case 5:
                case 10:
                    InstantiateBuildingsAtDiagonalCorners(index, position);
                    break;
                case 15:
                    InstantiateBuilding(position + centerOffset);
                    break;
                case 0:
                default:
                    Instantiate(debugPrefab, position + centerOffset, Quaternion.identity, transform);
                    break;
            }
        }

        void InstantiateBuilding(Vector3 position)
        {
            GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
            Instantiate(buildingPrefab, position, Quaternion.identity, transform);
        }

        void InstantiateBuildingAtSingleCorner(int index, Vector3 basePosition)
        {
            Vector3 offset = index switch
            {
                1 => new Vector3(0, 0, rowWidth),  // Bottom-left
                2 => new Vector3(columnWidth, 0, rowWidth),  // Bottom-right
                4 => new Vector3(columnWidth, 0, 0),  // Top-right
                8 => new Vector3(0, 0, 0),  // Top-left
                _ => Vector3.zero
            };
            InstantiateBuilding(basePosition + offset);
        }

        void InstantiateBuildingsAtDiagonalCorners(int index, Vector3 basePosition)
        {
            InstantiateBuilding(basePosition + new Vector3(0, 0, (index == 5) ? rowWidth : 0));
            InstantiateBuilding(basePosition + new Vector3(columnWidth, 0, (index == 5) ? 0 : rowWidth));
        }

        void DestroyChildren()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}
