using UnityEngine;

namespace Demo
{
    public class SimpleBuilding : Shape
    {
        public int buildingHeight = -1;
        public float stockHeight = 1;
        public int maxHeight = 5;
        public int minHeight = 1;

        public GameObject[] floorPrefabs;
        public GameObject[] stockPrefabs;
        public GameObject[] roofPrefabs;

        int stockNumber = 0;
        public float roofProbability = 20.0f;
        GameObject chosenStockPrefab = null;

        public void Initialize(int pBuildingHeight, float pStockHeight, int pStockNumber, GameObject[] pStockPrefabs, GameObject[] pRoofPrefabs, int pMinHeight, int pMaxHeight)
        {
            buildingHeight = pBuildingHeight;
            stockHeight = pStockHeight;
            stockNumber = pStockNumber;
            stockPrefabs = pStockPrefabs;
            roofPrefabs = pRoofPrefabs;
            minHeight = pMinHeight;
            maxHeight = pMaxHeight;
        }

        GameObject ChooseRandom(GameObject[] choices)
        {
            int index = Random.Range(0, choices.Length);
            return choices[index];
        }

        protected override void Execute()
        {
            if (buildingHeight < 0)
            {
                buildingHeight = Random.Range(minHeight, maxHeight + 1);
            }

            if (stockNumber < buildingHeight)
            {
                if ((stockNumber >= minHeight && Random.Range(0, 100) < roofProbability) || stockNumber == maxHeight - 1)
                {
                    GameObject newRoof = SpawnPrefab(ChooseRandom(roofPrefabs));
                    return;
                }

                // Determine stock prefab to use
                if (chosenStockPrefab == null)  // Check if this is the first stock
                {
                    chosenStockPrefab = ChooseRandom(stockPrefabs);  // Choose stock prefab for the first stock
                }

                GameObject newStock = SpawnPrefab(chosenStockPrefab);  // Use the chosen stock prefab for all stocks
                SimpleBuilding remainingBuilding = CreateSymbol<SimpleBuilding>("stock", new Vector3(0, stockHeight, 0));
                remainingBuilding.Initialize(buildingHeight, stockHeight, stockNumber + 1, stockPrefabs, roofPrefabs, minHeight, maxHeight);
                remainingBuilding.Generate(buildDelay);
            }
        }
    }
}
