using UnityEngine;

namespace Demo
{
    public class ValueGrid : MonoBehaviour
    {
        [SerializeField]
        int width = 10;
        [SerializeField]
        int depth = 10;
        public int Depth
        {
            get { return depth; }
        }
        public int Width
        {
            get { return width; }
        }
        public float cellSize = 1;

        float[,] grid;

        void Start()
        {
            InitializeGrid();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                InitializeGrid();
                Debug.Log("Grid re-initialized.");
            }
        }

        void InitializeGrid()
        {
            grid = new float[width, depth];
            float xOffset = Random.value;
            float yOffset = Random.value;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    grid[i, j] = Mathf.PerlinNoise(i * 0.1f + xOffset, j * 0.1f + yOffset);
                }
            }
            SetNoBuildZone();
        }

        void SetNoBuildZone()
        {
            // Example: setting an inner area in the middle of the grid to be a no-build zone.
            int innerAreaWidth = 4;
            int innerAreaLength = 4;
            int startCol = (width - innerAreaWidth) / 2;
            int startRow = (depth - innerAreaLength) / 2;

            for (int i = startRow; i < startRow + innerAreaLength; i++)
            {
                for (int j = startCol; j < startCol + innerAreaWidth; j++)
                {
                    grid[i, j] = 1;  // Using 1 to indicate a no-build zone
                }
            }
        }

        public bool GetRowCol(Vector3 worldPosition, out int row, out int col)
        {
            Vector3 localHit = transform.InverseTransformPoint(worldPosition);
            row = (int)(localHit.x / cellSize);
            col = (int)(localHit.z / cellSize);
            return InRange(row, col);
        }

        public bool InRange(int row, int col)
        {
            return grid != null && row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1);
        }

        public void SetCell(int row, int col, float value)
        {
            if (InRange(row, col))
            {
                grid[row, col] = value;
            }
        }

        public float GetCell(int row, int col)
        {
            if (InRange(row, col))
            {
                return grid[row, col];
            }
            return 0;
        }
    }
}
