using UnityEngine;

public class ValueGrid : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int depth = 10;
    public float cellSize = 1;

    private float[,] grid;

    private void Start()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        grid = new float[width, depth];
        float xOffset = Random.value * 1000;  // Large range to ensure variability
        float yOffset = Random.value * 1000;  // Large range to ensure variability

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                grid[i, j] = Mathf.PerlinNoise(i * 0.1f + xOffset, j * 0.1f + yOffset) > 0.5 ? 1 : 0;
            }
        }
    }

    public float GetCell(int row, int col)
    {
        if (InRange(row, col))
        {
            return grid[row, col];
        }
        return 0;  // Return 0 if out of range, could also throw an exception or handle differently
    }

    public bool IsCellOccupied(int row, int col)
    {
        return GetCell(row, col) != 0;
    }

    public int GetMarchingSquareIndex(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width - 1 || y >= depth - 1) return 0;

        int index = 0;
        if (IsCellOccupied(x, y + 1)) index |= 1;
        if (IsCellOccupied(x + 1, y + 1)) index |= 2;
        if (IsCellOccupied(x + 1, y)) index |= 4;
        if (IsCellOccupied(x, y)) index |= 8;

        return index;
    }

    private bool InRange(int row, int col)
    {
        return row >= 0 && row < width && col >= 0 && col < depth;
    }
}
