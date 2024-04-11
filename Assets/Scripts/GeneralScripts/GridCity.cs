using System.Collections;
using UnityEngine;

namespace Demo
{
    public class GridCity : MonoBehaviour
    {
        public int rows = 10;
        public int columns = 10;
        public int rowWidth = 10;
        public int columnWidth = 10;
        public int minBuildingHeight = 1;
        public int maxBuildingHeight = 2;
        public GameObject[] buildingPrefabs;
        public GameObject roadPrefab;
        public float buildingSpacing = 2.0f;
        public float buildDelaySeconds = 0.1f;
        public Vector2Int giantBuildingPosition;
        public float centerHeightInfluence = 1.0f;
        public float roadStartWidth = 5.0f;
        public int minStockWidth = 1;
        public int minStockHeight = 1;
        public int maxStockWidth = 5;
        public int maxStockHeight = 5;
        public GameObject giantBuildingPrefab;
        public float centerNodeWidth = 50;
        public float centerNodeHeight = 50;

        private bool isInitialGenerationComplete = false;
        private Coroutine regenerateCoroutine = null;

        private void Start()
        {
            if (ValidatePrefabs())
            {
                Generate();
                isInitialGenerationComplete = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && ValidatePrefabs())
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
            if (regenerateCoroutine != null)
            {
                StopCoroutine(regenerateCoroutine);
            }
            regenerateCoroutine = StartCoroutine(RegenerateCoroutine());
        }

        private IEnumerator RegenerateCoroutine()
        {
            DestroyChildren();
            yield return null;
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
            BSPNode rootNode = new BSPNode(0, 0, columns * columnWidth, rows * rowWidth);
            SplitNode(rootNode, 4);
            SpawnBuildingsInLeafNodes(rootNode);
            CreateCenterNode();
        }

        private void SplitNode(BSPNode node, int depth)
        {
            if (depth <= 0 || (node.width < 2 * columnWidth && node.height < 2 * rowWidth))
            {
                return;
            }
            bool splitHorizontally = node.width < node.height ? true : Random.Range(0, 2) == 0;
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
            return Mathf.Lerp(roadStartWidth / 2, roadStartWidth, nodeDimension / (columns * columnWidth));
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
                int plotsAlongWidth = Mathf.FloorToInt(node.width / columnWidth);
                int plotsAlongHeight = Mathf.FloorToInt(node.height / rowWidth);
                for (int w = 0; w < plotsAlongWidth; w++)
                {
                    for (int h = 0; h < plotsAlongHeight; h++)
                    {
                        GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                        GameObject newBuilding = Instantiate(buildingPrefab, transform);
                        Vector3 newPosition = new Vector3(node.x + w * (columnWidth + buildingSpacing) + columnWidth / 2.0f, 0, node.y + h * (rowWidth + buildingSpacing) + rowWidth / 2.0f);
                        newBuilding.transform.localPosition = newPosition;
                        newBuilding.transform.localRotation = Quaternion.Euler(0, Random.Range(-10, 10), 0);
                        SimpleStock stockScript = newBuilding.GetComponent<SimpleStock>();
                        if (stockScript != null)
                        {
                            int stockWidth = Random.Range(minStockWidth, Mathf.Min(maxStockWidth, plotsAlongWidth));
                            int stockHeight = Random.Range(minStockHeight, Mathf.Min(maxStockHeight, plotsAlongHeight));
                            stockScript.Initialize(stockWidth, stockHeight, stockScript.wallStyle, stockScript.roofStyle);
                            stockScript.Generate(buildDelaySeconds);
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

        private void CreateCenterNode()
        {
            float centerNodeX = (columns * columnWidth - centerNodeWidth) / 2;
            float centerNodeY = (rows * rowWidth - centerNodeHeight) / 2;
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
