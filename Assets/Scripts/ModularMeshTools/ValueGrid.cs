using UnityEngine;

public class ValueGrid : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int depth = 10;
    public float cellSize = 1;

    private float[,] grid;

    private void Start()
    {
        Debug.Log("ValueGrid.cs: Press G to re-initialize the grid.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            InitializeGrid();
            Debug.Log("ValueGrid: newly initialized. Press the BuildTrigger key to regenerate game objects");
        }
    }

    public void InitializeGrid()
    {
        grid = new float[width, depth];
        float xOffset = Random.value;
        float yOffset = Random.value;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                grid[i, j] = Mathf.PerlinNoise(i * 0.1f + xOffset, j * 0.1f + yOffset) > 0.5 ? 1 : 0;
            }
        }
    }

    public bool GetRowCol(Vector3 worldPosition, out int row, out int col)
    {
        Vector3 localHit = transform.InverseTransformPoint(worldPosition);
        row = (int)Mathf.Round(localHit.x / cellSize);
        col = (int)Mathf.Round(localHit.z / cellSize);
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

    // Added methods
    public bool IsCellOccupied(int row, int col)
    {
        return GetCell(row, col) != 0;  // Assuming 0 means unoccupied
    }

    public void SetCellOccupied(int row, int col, bool occupied)
    {
        SetCell(row, col, occupied ? 1 : 0);
    }
}
