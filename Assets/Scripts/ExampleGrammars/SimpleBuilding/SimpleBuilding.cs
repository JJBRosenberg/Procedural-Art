using UnityEngine;

namespace Demo
{
    public class SimpleBuilding : MonoBehaviour
    {
        public int buildingHeight = -1;
        public float stockHeight = 1;
        public int maxHeight = 5;
        public int minHeight = 1;

        public GameObject[] floorPrefabs;
        public GameObject[] stockPrefabs;
        public GameObject[] roofPrefabs;

        private int stockNumber = 0;

        public void Initialize(int pBuildingHeight, float pStockHeight, int pStockNumber, GameObject[] pStockPrefabs, GameObject[] pRoofPrefabs)
        {
            buildingHeight = pBuildingHeight;
            stockHeight = pStockHeight;
            stockNumber = pStockNumber;
            stockPrefabs = pStockPrefabs;
            roofPrefabs = pRoofPrefabs;
        }

        public void Generate(float delaySeconds)
        {
            StartCoroutine(BuildCoroutine(delaySeconds));
        }

        private System.Collections.IEnumerator BuildCoroutine(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            Execute();
        }

        private GameObject ChooseRandom(GameObject[] choices)
        {
            int index = Random.Range(0, choices.Length);
            return choices[index];
        }

        protected void Execute()
        {
            buildingHeight = Random.Range(minHeight, maxHeight + 1);

            stockNumber = 0;

            while (stockNumber < buildingHeight)
            {
                GameObject newStock = SpawnPrefab(ChooseRandom(stockPrefabs));
                newStock.transform.position = this.transform.position + new Vector3(0, stockHeight * stockNumber, 0);

                stockNumber++;
                if (stockNumber >= buildingHeight)
                {
                    GameObject newRoof = SpawnPrefab(ChooseRandom(roofPrefabs));
                    newRoof.transform.position = this.transform.position + new Vector3(0, stockHeight * stockNumber, 0);
                }
            }
        }

        private GameObject SpawnPrefab(GameObject prefab)
        {
            return Instantiate(prefab, this.transform.position, Quaternion.identity, this.transform);
        }

        public void ResetAndExecute()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            Execute();
        }
    }
}
