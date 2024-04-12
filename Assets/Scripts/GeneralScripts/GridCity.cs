using UnityEngine;

namespace Demo
{
    public class GridCity : MonoBehaviour
    {

        public float cylinderHeight;
        public int rows = 10;
        public int columns = 10;
        public int rowWidth = 10;
        public int columnWidth = 10;
        public GameObject[] buildingPrefabs;
        public GameObject debugPrefab;
        public GameObject noBuildZonePrefab;

        public GameObject largeCylinderPrefab;
        public int centralAreaWidth = 4;
        public int centralAreaHeight = 4;
        public bool useRandomSeed = false;
        public GenerationAlgorithm selectedAlgorithm = GenerationAlgorithm.MarchingSquares;
        public int seed;

        private ValueGrid valueGrid;

        public enum GenerationAlgorithm
        {
            MarchingSquares,
            BSP
        }

        void Start()
        {
            valueGrid = GetComponent<ValueGrid>();
            InitializeSeed();
            valueGrid.InitializeGrid();
            GenerateCity();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                InitializeSeed();
                GenerateCity();
            }
        }

        void InitializeSeed()
        {
            if (useRandomSeed)
            {
                seed = Random.Range(1000, 9999);
                Random.InitState(seed);
            }
            else
            {
                Random.InitState(seed);
            }
        }

        void GenerateCity()
        {
            valueGrid.InitializeGrid();
            DestroyChildren();
            if (selectedAlgorithm == GenerationAlgorithm.BSP)
            {
                GenerateCityUsingBSP();
            }
            else
            {
                float noBuildProbability = Mathf.Clamp01((float)seed / 100000f);
                UseMarchingSquares(noBuildProbability);
            }
            //center cylinder
            int centerRow = rows / 2;
            int centerCol = columns / 2;
            Vector3 centerPosition = new Vector3(centerCol * columnWidth, 0, centerRow * rowWidth);
            InstantiateLargeCylinder(centerPosition);
        }

        void UseMarchingSquares(float noBuildProbability)
        {
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

        void GenerateCityUsingBSP()
        {
            int partitionSize = Mathf.Max(1, columns / 4);
            PartitionArea(0, 0, columns, rows);
            PlaceBuildingsAndSpaces();
        }

        void PlaceBuildingsAndSpaces()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = new Vector3(col * columnWidth, 0, row * rowWidth);
                    if (!IsCentralArea(row, col))
                    {
                        float noBuildProbability = Mathf.Clamp01((float)seed / 100000f);
                        int index = valueGrid.GetMarchingSquareIndex(col, row);
                        PlaceBuildingOrSpace(index, position, noBuildProbability);
                    }
                }
            }
        }

        void PartitionArea(int startX, int startY, int width, int height)
        {
            if (width < 2 || height < 2) return; 
            bool splitHorizontally = (width < height) ? true : Random.value > 0.5;
            if (splitHorizontally)
            {
                int split = Random.Range(1, height - 1); 
                PartitionArea(startX, startY, width, split);
                PartitionArea(startX, startY + split, width, height - split);
            }
            else
            {
                int split = Random.Range(1, width - 1); 
                PartitionArea(startX, startY, split, height);
                PartitionArea(startX + split, startY, width - split, height);
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
                valueGrid.SetCell(GetCellPosition(position), -1); // Mark cell as no-building zone
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
            valueGrid.SetCell(GetCellPosition(position), 1); // Mark cell as building
        }

        void InstantiateBuildingAtSingleCorner(int index, Vector3 basePosition)
        {
            Vector3 offset = index switch
            {
                1 => new Vector3(0, 0, rowWidth),
                2 => new Vector3(columnWidth, 0, rowWidth),  
                4 => new Vector3(columnWidth, 0, 0),
                8 => new Vector3(0, 0, 0),  
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

        Vector2Int GetCellPosition(Vector3 position)
        {
            int row = Mathf.FloorToInt(position.z / rowWidth);
            int col = Mathf.FloorToInt(position.x / columnWidth);
            return new Vector2Int(row, col);
        }


        void InstantiateLargeCylinder(Vector3 position)
        {
            Vector3 cylinderScale = new Vector3(1, cylinderHeight, 1);
            GameObject largeCylinder = Instantiate(largeCylinderPrefab, position, Quaternion.identity, transform);
            largeCylinder.transform.localScale = cylinderScale;
        }
    }
}
