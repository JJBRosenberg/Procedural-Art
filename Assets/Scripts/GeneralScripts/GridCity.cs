using System.Collections;
using UnityEngine;

namespace Demo
{
    public class GridCity : MonoBehaviour
    {
        public ValueGrid valueGrid;
        public GameObject[] buildingPrefabs;
        public GameObject roadPrefab;
        public float buildingSpacing = 2.0f;
        public float buildDelaySeconds = 0.1f;
        public GameObject giantBuildingPrefab;
        public float centerNodeWidth = 50;
        public float centerNodeHeight = 50;
        private float roadStartWidth = 5.0f;

        private void Start()
        {
            if (ValidatePrefabs() && valueGrid != null)
            {
                Generate();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && ValidatePrefabs() && valueGrid != null)
            {
                Regenerate();
            }
        }

        private bool ValidatePrefabs()
        {
            return buildingPrefabs != null && buildingPrefabs.Length > 0 && roadPrefab != null && giantBuildingPrefab != null;
        }

        private void Regenerate()
        {
            DestroyChildren();
            Generate();
        }

        private void DestroyChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void Generate()
        {
            // Ensure the ValueGrid component is available
            if (valueGrid == null)
            {
                Debug.LogError("ValueGrid reference is missing!");
                return;
            }

            int rowWidth = (int)valueGrid.cellSize;
            int columnWidth = (int)valueGrid.cellSize;

            // Start city generation process using ValueGrid dimensions
            BSPNode rootNode = new BSPNode(0, 0, valueGrid.Width * columnWidth, valueGrid.Depth * rowWidth);
            CreatePerimeterRoad(rootNode); // Create a road along the perimeter first
            SplitNode(rootNode, 4);
            SpawnBuildingsInLeafNodes(rootNode);
            CreateCenterNode();
        }

        private void CreatePerimeterRoad(BSPNode node)
        {
            // Create a road along the entire perimeter of the grid
            SpawnRoad(node.x, node.y, node.width, roadStartWidth, true);
            SpawnRoad(node.x, node.y, roadStartWidth, node.height, false);
            SpawnRoad(node.x, node.y + node.height - roadStartWidth, node.width, roadStartWidth, true);
            SpawnRoad(node.x + node.width - roadStartWidth, node.y, roadStartWidth, node.height, false);
        }

        private void SplitNode(BSPNode node, int depth)
        {
            if (depth <= 0 || (node.width < 2 * (int)valueGrid.cellSize && node.height < 2 * (int)valueGrid.cellSize))
            {
                return;
            }

            // Check if the node is already smaller than the cell size
            if (node.width < (int)valueGrid.cellSize || node.height < (int)valueGrid.cellSize)
            {
                return;
            }

            bool splitHorizontally = node.width < node.height ? true : Random.Range(0, 2) == 0;

            // Calculate the minimum dimension after the split
            float minDimensionAfterSplit = splitHorizontally ? (int)valueGrid.cellSize : (int)valueGrid.cellSize;

            // Check if either child node would be smaller than the cell size after the split
            if (node.width - minDimensionAfterSplit < minDimensionAfterSplit || node.height - minDimensionAfterSplit < minDimensionAfterSplit)
            {
                return;
            }

            float splitPos = splitHorizontally ? Random.Range(1, node.height - 1) : Random.Range(1, node.width - 1);
            float roadWidth = CalculateRoadWidth(splitHorizontally ? node.height : node.width);

            if (splitHorizontally)
            {
                node.child1 = new BSPNode(node.x, node.y, node.width, splitPos - roadWidth / 2);
                node.child2 = new BSPNode(node.x, node.y + splitPos + roadWidth / 2, node.width, node.height - splitPos - roadWidth / 2);
                SpawnRoad(node.x, node.y + splitPos - roadWidth / 2, node.width, roadWidth, true);
            }
            else
            {
                node.child1 = new BSPNode(node.x, node.y, splitPos - roadWidth / 2, node.height);
                node.child2 = new BSPNode(node.x + splitPos + roadWidth / 2, node.y, node.width - splitPos - roadWidth / 2, node.height);
                SpawnRoad(node.x + splitPos - roadWidth / 2, node.y, roadWidth, node.height, false);
            }

            SplitNode(node.child1, depth - 1);
            SplitNode(node.child2, depth - 1);
        }

        private float CalculateRoadWidth(float nodeDimension)
        {
            return Mathf.Lerp(roadStartWidth / 2, roadStartWidth, nodeDimension / (valueGrid.Width * (int)valueGrid.cellSize));
        }

        private void SpawnRoad(float x, float y, float width, float height, bool isHorizontal)
        {
            GameObject newRoad = Instantiate(roadPrefab, transform);
            newRoad.transform.localPosition = new Vector3(x + width / 2, 0, y + height / 2);
            if (!isHorizontal)
            {
                newRoad.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            newRoad.transform.localScale = isHorizontal ? new Vector3(width, 1, height) : new Vector3(height, 1, width);
        }

        private void SpawnBuildingsInLeafNodes(BSPNode node)
        {
            if (node.child1 == null && node.child2 == null)
            {
                int plotsAlongWidth = Mathf.FloorToInt(node.width / (int)valueGrid.cellSize);
                int plotsAlongHeight = Mathf.FloorToInt(node.height / (int)valueGrid.cellSize);
                float offsetX = (node.width - plotsAlongWidth * ((int)valueGrid.cellSize + buildingSpacing)) / 2.0f;
                float offsetY = (node.height - plotsAlongHeight * ((int)valueGrid.cellSize + buildingSpacing)) / 2.0f;
                for (int w = 0; w < plotsAlongWidth; w++)
                {
                    for (int h = 0; h < plotsAlongHeight; h++)
                    {
                        // Calculate the position of the building at the center of the plot
                        float buildingX = node.x + offsetX + w * ((int)valueGrid.cellSize + buildingSpacing) + (int)valueGrid.cellSize / 2.0f;
                        float buildingY = node.y + offsetY + h * ((int)valueGrid.cellSize + buildingSpacing) + (int)valueGrid.cellSize / 2.0f;
                        // Check if the building position intersects with any road
                        if (!IntersectsRoad(buildingX, buildingY))
                        {
                            GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                            GameObject newBuilding = Instantiate(buildingPrefab, transform);
                            newBuilding.transform.localPosition = new Vector3(buildingX, 0, buildingY);
                            newBuilding.transform.localRotation = Quaternion.Euler(0, Random.Range(-10, 10), 0);
                            SimpleStock stockScript = newBuilding.GetComponent<SimpleStock>();
                            if (stockScript != null)
                            {
                                int stockWidth = Random.Range(1, Mathf.Min(5, plotsAlongWidth));
                                int stockHeight = Random.Range(1, Mathf.Min(5, plotsAlongHeight));
                                stockScript.Initialize(stockWidth, stockHeight, stockScript.wallStyle, stockScript.roofStyle);
                                stockScript.Generate(buildDelaySeconds);
                            }
                        }
                    }
                }
            }
            else
            {
                SpawnBuildingsInLeafNodes(node.child1);
                SpawnBuildingsInLeafNodes(node.child2);
            }
        }

        private bool IntersectsRoad(float x, float y)
        {
            // Check if the position intersects with any road
            Collider[] colliders = Physics.OverlapBox(new Vector3(x, 0, y), new Vector3((int)valueGrid.cellSize / 2.0f, 1, (int)valueGrid.cellSize / 2.0f));
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Road"))
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateCenterNode()
        {
            float centerNodeX = (valueGrid.Width * (int)valueGrid.cellSize - centerNodeWidth) / 2;
            float centerNodeY = (valueGrid.Depth * (int)valueGrid.cellSize - centerNodeHeight) / 2;
            GameObject centerBuilding = Instantiate(giantBuildingPrefab, new Vector3(centerNodeX + centerNodeWidth / 2, 0, centerNodeY + centerNodeHeight / 2), Quaternion.identity, transform);
            centerBuilding.transform.localScale = new Vector3(centerNodeWidth, giantBuildingPrefab.transform.localScale.y, centerNodeHeight);
        }

        private class BSPNode
        {
            public float x, y, width, height;
            public BSPNode child1, child2;

            public BSPNode(float x, float y, float width, float height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }
        }
    }
}
