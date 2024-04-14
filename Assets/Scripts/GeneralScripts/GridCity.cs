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

        public GameObject[] northPrefabs;
        public GameObject[] eastPrefabs;
        public GameObject[] southPrefabs;
        public GameObject[] westPrefabs;
        public GameObject[] centralPrefabs;

        public GameObject debugPrefab;
        public GameObject noBuildZonePrefab;

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
            if (width < 1 || height < 1) return;

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
                Instantiate(noBuildZonePrefab, position, Quaternion.identity, transform).transform.localScale = scale;
            }

            if (width == 1 && height == 1) return; // Base case for recursion

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
                Instantiate(noBuildZonePrefab, position, Quaternion.identity, transform);
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
