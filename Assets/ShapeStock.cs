using UnityEngine;
using System.Collections.Generic;

namespace Demo
{
    public class ShapeStock : Shape
    {
        public float stockContinueChance = 0.25f;
        const float balconyChanceAbove2 = 0.3f;

        public int Width;
        public int Depth;

        [SerializeField] private GameObject[] wallStyle;
        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private GameObject[] roofStyle;
        [SerializeField] private GameObject balconyPrefab;

        private int doorWallIndex;
        private int currentHeightIndex = 0;
        public int minHeight;
        public int maxHeight;

        private LODGroup lodGroup;
        private Vector3 buildDirection;

        private void Awake()
        {
            lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                Debug.LogError("LODGroup component not found on the GameObject or its parents.");
            }
        }

        public void Initialize(int width, int depth, GameObject[] wallStyle, GameObject doorPrefab, GameObject[] roofStyle, GameObject balconyPrefab, int currentHeightIndex, Vector3 direction, int minHeight = 1, int maxHeight = 10)
        {
            Width = width;
            Depth = depth;
            this.wallStyle = wallStyle;
            this.doorPrefab = doorPrefab;
            this.roofStyle = roofStyle;
            this.balconyPrefab = balconyPrefab;
            this.currentHeightIndex = currentHeightIndex;
            this.buildDirection = direction;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            doorWallIndex = currentHeightIndex == 0 ? Random.Range(0, 4) : -1;
        }

        protected override void Execute()
        {
            GenerateStock();
        }

        private void GenerateStock()
        {
            if (currentHeightIndex >= maxHeight)
            {
                Debug.Log("Maximum height reached, skipping generation for this stock.");
                GenerateRoof();
                return;
            }
            GenerateWalls();
            UpdateLODGroup();
            DecideNextStep();
        }

        private void GenerateWalls()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject wall = Instantiate(SelectWallPrefab(i), transform.position + DetermineWallPosition(i), Quaternion.Euler(0, i * 90, 0), transform);
                Renderer[] renderers = wall.GetComponentsInChildren<Renderer>();
                AddRenderersToLODGroup(new List<Renderer>(renderers));
            }
        }

        private Vector3 DetermineWallPosition(int index)
        {
            float xOffset = (Width - 1) * 0.5f;
            float zOffset = (Depth - 1) * 0.5f;
            switch (index)
            {
                case 0: return new Vector3(-xOffset, 0, 0);
                case 1: return new Vector3(0, 0, zOffset);
                case 2: return new Vector3(xOffset, 0, 0);
                case 3: return new Vector3(0, 0, -zOffset);
                default: return Vector3.zero;
            }
        }

        private GameObject SelectWallPrefab(int index)
        {
            if (index == doorWallIndex)
                return doorPrefab;
            else if (index == 3 && currentHeightIndex > 0 && Random.value < balconyChanceAbove2)
                return balconyPrefab;
            else
                return wallStyle[Random.Range(0, wallStyle.Length)];
        }

        private void UpdateLODGroup()
        {
            if (lodGroup != null)
            {
                LOD[] lods = lodGroup.GetLODs();
                List<Renderer> updatedRenderers = new List<Renderer>();
                foreach (LOD lod in lods)
                {
                    updatedRenderers.AddRange(lod.renderers);
                }
                lodGroup.SetLODs(new LOD[] { new LOD(0.5f, updatedRenderers.ToArray()) });
                lodGroup.RecalculateBounds();
            }
        }

        private void DecideNextStep()
        {
            currentHeightIndex++;
            if (currentHeightIndex < minHeight || Random.value < stockContinueChance)
            {
                Vector3 newPosition = transform.position + buildDirection;
                ShapeStock newStock = Instantiate(this, newPosition, Quaternion.identity);
                newStock.Initialize(Width, Depth, wallStyle, doorPrefab, roofStyle, balconyPrefab, currentHeightIndex, buildDirection, minHeight, maxHeight);
                newStock.GenerateStock();
            }
            else
            {
                GenerateRoof();
            }
        }

        private void GenerateRoof()
        {
            GameObject roof = Instantiate(roofStyle[Random.Range(0, roofStyle.Length)], transform.position + Vector3.up, Quaternion.identity, transform);
        }

        private void AddRenderersToLODGroup(List<Renderer> renderers)
        {
            if (lodGroup == null)
            {
                Debug.LogWarning("LODGroup component not found on the GameObject.");
                return;
            }

            LOD[] lods = lodGroup.GetLODs();
            if (lods.Length == 0)
            {
                Debug.LogWarning("No LOD levels found on the LODGroup.");
                return;
            }

            List<Renderer> updatedRenderers = new List<Renderer>(lods[0].renderers);
            updatedRenderers.AddRange(renderers);
            lods[0].renderers = updatedRenderers.ToArray();
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
    }
}
