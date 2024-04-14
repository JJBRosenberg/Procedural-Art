using Demo;
using UnityEngine;

public class ResidentalRoof : Shape
{
    private int Width;
    private int Depth;
    private GameObject[] roofStyle;
    private int currentHeightIndex = 0;

    private LODGroup lodGroup;

    private void Awake()
    {
        lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            Debug.LogError("LODGroup component not found on the GameObject or its parents.");
        }
    }

    public void Initialize(int width, int depth, GameObject[] roofStyle)
    {
        Width = width;
        Depth = depth;
        this.roofStyle = roofStyle;
    }

    protected override void Execute()
    {
        CreateFlatRoof();
    }

    private void CreateFlatRoof()
    {
        for (int i = 0; i < 1; i++) // Ensuring only one row of roofs
        {
            ResidentalRoof roofRow = CreateSymbol<ResidentalRoof>("roofStrip", new Vector3(0, 0, i * Depth));
            roofRow.Initialize(Width,Depth, roofStyle);
            roofRow.Generate();
        }
    }
}
