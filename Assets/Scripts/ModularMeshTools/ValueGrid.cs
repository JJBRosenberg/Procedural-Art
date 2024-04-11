using UnityEngine;

namespace Demo
{
    public class ValueGrid : MonoBehaviour
    {
        public int Width => width;
        public int Depth => depth;

        [SerializeField] private int width = 10;
        [SerializeField] private int depth = 10;
        public float cellSize = 1;

        private float[,] grid = null;

        private void Start()
        {
            Debug.Log("ValueGrid.cs: Press G to re-initialize the grid. Press the buildTrigger key (Space?) to show the new grid using marching squares.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                InitializeGrid();
                Debug.Log("ValueGrid: newly initialized. Press the BuildTrigger key to regenerate game objects");
            }
        }

        private void InitializeGrid()
        {
            grid = new float[width, depth];

            // Create a simple grid with all cells set to 0
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    grid[i, j] = 0;
                }
            }
        }

        public bool GetRowCol(Vector3 worldPosition, out int row, out int col)
        {
            Vector3 localHit = transform.InverseTransformPoint(worldPosition);

            row = Mathf.RoundToInt(localHit.x / cellSize);
            col = Mathf.RoundToInt(localHit.z / cellSize);
            return InRange(row, col);
        }

        public bool InRange(int row, int col)
        {
            if (grid == null)
            {
                InitializeGrid();
            }
            return row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1);
        }

        public void SetCell(int row, int col, float value)
        {
            if (grid == null)
            {
                InitializeGrid();
            }
            if (InRange(row, col))
            {
                grid[row, col] = value;
            }
        }

        public float GetCell(int row, int col)
        {
            if (grid == null)
            {
                InitializeGrid();
            }
            if (InRange(row, col))
            {
                return grid[row, col];
            }
            return 0;
        }

        public void SetCell(Vector3 worldPosition, float value)
        {
            if (GetRowCol(worldPosition, out int row, out int col))
            {
                SetCell(row, col, value);
            }
        }

        public float GetCell(Vector3 worldPosition)
        {
            if (GetRowCol(worldPosition, out int row, out int col))
            {
                return GetCell(row, col);
            }
            return 0;
        }
    }
}
