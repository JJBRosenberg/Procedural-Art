using UnityEngine;

namespace Demo
{
    public class GridCity : MonoBehaviour
    {
        [Range(0, 100)]
        public float roadProbability = 50;

        public float cylinderHeight;
        public int rows = 10;
        public int columns = 10;
        public int rowWidth = 10;
        public int columnWidth = 10;
        public GameObject waterPrefab;

        private bool bigPrefabSpawned = false;
        public GameObject bigPrefab;
        public GameObject[] northPrefabs;
        public GameObject[] eastPrefabs;
        public GameObject[] southPrefabs;
        public GameObject[] westPrefabs;
        public GameObject[] centralPrefabs;

        public GameObject debugPrefab;
        public GameObject roadPrefab;

        public GameObject largeCylinderPrefab;
        public int centralAreaWidth = 4;
        public int centralAreaHeight = 4;
        public bool useRandomSeed = false;
        public int seed;

        private ValueGrid valueGrid;

        public enum GenerationAlgorithm
        {
            MarchingSquares,
            BSP
        }

        public GenerationAlgorithm selectedAlgorithm = GenerationAlgorithm.MarchingSquares;

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
            bigPrefabSpawned = false;
            float scaledNoBuildProbability = Mathf.Clamp01(roadProbability / 100);
            if (selectedAlgorithm == GenerationAlgorithm.BSP)
            {
                GenerateCityUsingBSP(0, 0, columns, rows);
            }
            else
            {
                UseMarchingSquares(scaledNoBuildProbability);
            }
        }

        void UseMarchingSquares(float noBuildProbability)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = new Vector3(col * columnWidth, col < columns / 4 ? 0.1f : 0, row * rowWidth);
                    if (IsExactCenterCell(row, col) && !bigPrefabSpawned)
                    {
                        Instantiate(bigPrefab, position, Quaternion.identity, transform);
                        bigPrefabSpawned = true;
                        continue;
                    }
                    if (IsCentralArea(row, col))
                    {
                        InstantiateRandomPrefab(centralPrefabs, position);
                        continue;
                    }
                    PlaceBuildingOrSpace(row, col, position, noBuildProbability);
                }
            }
        }

        void GenerateCityUsingBSP(int startX, int startY, int width, int height)
        {
            if (width < centralAreaWidth || height < centralAreaHeight) return;

            bool isBuilding = Random.value < Mathf.Clamp01(roadProbability / 100);
            Vector3 position = new Vector3(startX * columnWidth, 0, startY * rowWidth);
            Vector3 scale = new Vector3(width * columnWidth / 10.0f, 1, height * rowWidth / 10.0f);

            if (isBuilding)
            {
                GameObject[] prefabs = GetPrefabArrayForPosition(startY, startX);
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                GameObject instance = Instantiate(prefab, position, Quaternion.identity, transform);
                instance.transform.localScale = scale;
            }
            else
            {
                GameObject roadInstance = Instantiate(roadPrefab, position, Quaternion.identity, transform);
                roadInstance.transform.localScale = scale;
            }

            if (width == centralAreaWidth && height == centralAreaHeight) return; // Base case for recursion

            bool splitHorizontally = Random.value > 0.5;
            int split = Random.Range(1, splitHorizontally ? height : width);

            if (splitHorizontally)
            {
                GenerateCityUsingBSP(startX, startY, width, split);
                GenerateCityUsingBSP(startX, startY + split, width, height - split);
            }
            else
            {
                GenerateCityUsingBSP(startX, startY, split, height);
                GenerateCityUsingBSP(startX + split, startY, width - split, height);
            }
        }

        void PlaceBuildingOrSpace(int row, int col, Vector3 position, float noBuildProbability)
        {
            if (col < columns / 4 && selectedAlgorithm == GenerationAlgorithm.MarchingSquares)
            {
                position.y += 0.1f;
                GameObject waterInstance = Instantiate(waterPrefab, position, Quaternion.identity, transform);
                waterInstance.transform.localScale = new Vector3(columnWidth / 10f, 1, rowWidth / 10f);
                return;
            }

            if (Random.value < noBuildProbability)
            {
                position.y += 0.1f;
                GameObject roadInstance = Instantiate(roadPrefab, position, Quaternion.identity, transform);
                roadInstance.transform.localScale = new Vector3(columnWidth / 10f, 1, rowWidth / 10f);
            }
            else
            {
                InstantiateRandomPrefab(GetPrefabArrayForPosition(row, col), position);
            }
        }

        GameObject[] GetPrefabArrayForPosition(int row, int col)
        {
            if (row < rows / 2)
                return col < columns / 2 ? westPrefabs : northPrefabs;
            else
                return col < columns / 2 ? southPrefabs : eastPrefabs;
        }

        void InstantiateRandomPrefab(GameObject[] prefabs, Vector3 position)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Instantiate(prefab, position, Quaternion.identity, transform);
        }

        bool IsExactCenterCell(int row, int col)
        {
            int centerRow1 = rows / 2;
            int centerCol1 = columns / 2;

            if (rows % 2 == 0 && columns % 2 == 0)
            {
                return (row == centerRow1 || row == centerRow1 - 1) && (col == centerCol1 || col == centerCol1 - 1);
            }
            else if (rows % 2 == 1 && columns % 2 == 1)
            {
                return row == centerRow1 && col == centerCol1;
            }
            else
            {
                int centerRow2 = (rows % 2 == 0) ? centerRow1 - 1 : centerRow1;
                int centerCol2 = (columns % 2 == 0) ? centerCol1 - 1 : centerCol1;
                return (row == centerRow1 || row == centerRow2) && (col == centerCol1 || col == centerCol2);
            }
        }

        bool IsCentralArea(int row, int col)
        {
            int centralRowStart = (rows - centralAreaHeight) / 2;
            int centralColStart = (columns - centralAreaWidth) / 2;
            return row >= centralRowStart && row < centralRowStart + centralAreaHeight &&
                   col >= centralColStart && col < centralColStart + centralAreaWidth;
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
