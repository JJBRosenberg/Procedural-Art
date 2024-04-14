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
                GenerateCityUsingBSP();
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
                    Vector3 position = new Vector3(col * columnWidth, 0, row * rowWidth);
                    if (IsCentralArea(row, col))
                    {
                        InstantiateRandomPrefab(centralPrefabs, position);
                        continue;
                    }
                    PlaceBuildingOrSpace(row, col, position, noBuildProbability);
                }
            }
        }

        void GenerateCityUsingBSP()
        {
            PartitionArea(0, 0, columns, rows);
        }

        void PartitionArea(int startX, int startY, int width, int height)
        {
            if (width < 2 || height < 2) return;
            bool splitHorizontally = (width > height) ? Random.value > 0.5 : false;
            int split = Random.Range(1, (splitHorizontally ? height : width) - 1);
            if (splitHorizontally)
            {
                PartitionArea(startX, startY, width, split);
                PartitionArea(startX, startY + split, width, height - split);
            }
            else
            {
                PartitionArea(startX, startY, split, height);
                PartitionArea(startX + split, startY, width - split, height);
            }
        }

        void PlaceBuildingOrSpace(int row, int col, Vector3 position, float noBuildProbability)
        {
            if (Random.value < noBuildProbability)
            {
                Instantiate(noBuildZonePrefab, position, Quaternion.identity, transform);
                return;
            }
            if (!IsCentralArea(row, col))
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
