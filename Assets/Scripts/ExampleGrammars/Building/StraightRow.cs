using UnityEngine;

namespace Demo
{
    public class StraightRow : Shape
    {
        int number;
        GameObject[] prefabs = null;
        Vector3 direction;
        int startPrefabIndex = 0;  // Index to start prefab selection, allows vertical alignment across rows

        public void Initialize(int number, GameObject[] prefabs, int startPrefabIndex = 0, Vector3 dir = new Vector3())
        {
            this.number = number;
            this.prefabs = prefabs;
            this.startPrefabIndex = startPrefabIndex;
            if (dir.magnitude != 0)
            {
                direction = dir;
            }
            else
            {
                direction = new Vector3(0, 0, 1);  // Default direction if none specified
            }
        }

        protected override void Execute()
        {
            if (number <= 0 || prefabs == null || prefabs.Length == 0)
                return;

            for (int i = 0; i < number; i++)
            {
                int index = (startPrefabIndex + i) % prefabs.Length;  // Compute prefab index based on start index and position
                Vector3 position = direction * i - direction * (number - 1) / 2.0f;  // Position prefabs in a straight line

                SpawnPrefab(prefabs[index], position, Quaternion.identity);  // Spawn prefab with no rotation
            }
        }
    }
}
