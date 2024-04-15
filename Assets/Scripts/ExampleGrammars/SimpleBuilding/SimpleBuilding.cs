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
                GameObject newStock = buildingHeight == 1 ? SpawnPrefab(floorPrefabs[0]) : SpawnPrefab(ChooseRandom(stockPrefabs));
                SimpleBuilding remainingBuilding = CreateSymbol<SimpleBuilding>("stock", new Vector3(0, stockHeight, 0));
                remainingBuilding.Initialize(buildingHeight, stockHeight, stockNumber + 1, stockPrefabs, roofPrefabs, minHeight, maxHeight);
                remainingBuilding.Generate(buildDelay);
            }
            else if (stockNumber >= buildingHeight)
            {
                GameObject newRoof = SpawnPrefab(ChooseRandom(roofPrefabs));
            }
        }
    }
}
