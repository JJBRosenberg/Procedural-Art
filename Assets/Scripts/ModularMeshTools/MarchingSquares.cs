using UnityEngine;

namespace Demo
{
    [RequireComponent(typeof(ValueGrid))]
    public class MarchingSquares : Shape
    {
        [Tooltip("Add 6 corner prefabs here, in this order: 0 corners / 1 corner / 2 adjacent corners / 2 opposite corners / 3 corners / 4 corners")]
        public GameObject[] cornerPrefabs;
        public GameObject debugPrefab;
        [Tooltip("Give 6 prefab rotations here, as a number between 0 and 3 (1 = 90 degrees, etc.)")]
        public int[] prefabRotations;

        ValueGrid grid;

        private void Awake()
        {
            grid = GetComponent<ValueGrid>();
            if (cornerPrefabs.Length < 6 || prefabRotations.Length < 6)
            {
                throw new System.Exception("Marching Squares: the Corner Prefabs and Prefab Rotations arrays need to have length at least 6!");
            }
        }

        public int GetBuildingCount(int i, int j)
        {
            int count = 0;
            // Check neighboring cells for buildings
            if (IsBuildingAt(i, j)) count++;
            if (IsBuildingAt(i + 1, j)) count++;
            if (IsBuildingAt(i, j + 1)) count++;
            if (IsBuildingAt(i + 1, j + 1)) count++;
            return count;
        }

        bool IsBuildingAt(int i, int j)
        {
            // Define the position to check based on the grid cell indices
            Vector3 positionToCheck = new Vector3(i * grid.cellSize, 0f, j * grid.cellSize);

            // Cast a downward ray from a point above the grid cell to detect any colliders
            Ray ray = new Ray(positionToCheck + Vector3.up * 10f, Vector3.down);
            RaycastHit hitInfo;

            // Perform the raycast
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                // Check if the collider belongs to a building
                if (hitInfo.collider.CompareTag("Building"))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Execute()
        {
            // Loop over all square cells between the grid sample points:
            for (int i = 0; i < grid.Width - 1; i++)
            {
                for (int j = 0; j < grid.Depth - 1; j++)
                {
                    int buildingCount = GetBuildingCount(i, j);

                    // Use the building count to determine the bitmask
                    int bitMask = buildingCount;

                    // Use the lookup tables to match this to a prefab and rotation:
                    SpawnPrefab(i, j, bitMask);
                }
            }
        }

        GameObject SpawnPrefab(int i, int j, int bitMask)
        {
            Vector3 spawnOffset = new Vector3(0.5f, 0, 0.5f);

            if (cornerPrefabs[bitMask] != null)
            {
                return SpawnPrefab(
                    cornerPrefabs[bitMask],
                    (new Vector3(i, 0, j) + spawnOffset) * grid.cellSize,
                    Quaternion.Euler(0, 90 * prefabRotations[bitMask], 0),
                    transform
                );
            }
            else
            {
                return null;
            }
        }

        GameObject SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject newObj = Instantiate(prefab, position, rotation, parent);
            return newObj;
        }
    }
}
