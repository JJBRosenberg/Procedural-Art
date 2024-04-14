using UnityEngine;
using System.Collections.Generic;

namespace Demo
{
    public class StraightStock : Shape
    {
        const float stockContinueChance = 0.5f;
        const float balconyChanceAbove2 = 0.3f;

        [SerializeField] int Width;
        [SerializeField] int Depth;
        [SerializeField] GameObject[] wallStyle;
        [SerializeField] GameObject doorPrefab;
        [SerializeField] GameObject[] roofStyle;
        [SerializeField] GameObject balconyPrefab;

        private int doorWallIndex;
        private int currentHeightIndex = 0;
        public int minHeight;
        public int maxHeight;

        private LODGroup lodGroup;

        private void Awake()
        {
            lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                Debug.LogError("LODGroup component not found on the GameObject or its parents.");
            }
        }

        public void Initialize(int width, int depth, GameObject[] wallStyle, GameObject doorPrefab, GameObject[] roofStyle, GameObject balconyPrefab, int currentHeightIndex, int minHeight = 1, int maxHeight = 10)
        {
            Width = width;
            Depth = depth;
            this.wallStyle = wallStyle;
            this.doorPrefab = doorPrefab;
            this.roofStyle = roofStyle;
            this.balconyPrefab = balconyPrefab;
            this.currentHeightIndex = currentHeightIndex;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;

            if (currentHeightIndex == 0)
            {
                doorWallIndex = Random.Range(0, 4);
            }
            else
            {
                doorWallIndex = -1;
            }
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

            List<Renderer> allRenderers = new List<Renderer>();

            for (int i = 0; i < 4; i++)
            {
                Vector3 localPosition = Vector3.zero;
                switch (i)
                {
                    case 0:
                        localPosition = new Vector3(-(Width - 1) * 0.5f, 0, 0); // left
                        break;
                    case 1:
                        localPosition = new Vector3(0, 0, (Depth - 1) * 0.5f); // back
                        break;
                    case 2:
                        localPosition = new Vector3((Width - 1) * 0.5f, 0, 0); // right
                        break;
                    case 3:
                        localPosition = new Vector3(0, 0, -(Depth - 1) * 0.5f); // front
                        break;
                }

                int index = i; // Use wall index instead of height index
                StraightRow newRow = CreateSymbol<StraightRow>("StraightRow", localPosition, Quaternion.Euler(0, i * 90, 0));
                newRow.Initialize(i % 2 == 1 ? Width : Depth, wallStyle, index); // Use fixed index for alignment

                newRow.Generate();

                Renderer[] rowRenderers = newRow.GetComponentsInChildren<Renderer>();
                allRenderers.AddRange(rowRenderers);
            }

            AddRenderersToLODGroup(allRenderers);

            currentHeightIndex++;

            if (currentHeightIndex < minHeight || Random.value < stockContinueChance)
            {
                StraightStock nextStock = CreateSymbol<StraightStock>("StraightStock", new Vector3(0, 1, 0));
                nextStock.Initialize(Width, Depth, wallStyle, doorPrefab, roofStyle, balconyPrefab, currentHeightIndex, minHeight, maxHeight);
                nextStock.Generate();
            }
            else
            {
                GenerateRoof();
            }
        }

        private void GenerateRoof()
        {
            StraightRoof nextRoof = CreateSymbol<StraightRoof>("roof", new Vector3(0, 1, 0));
            nextRoof.Initialize(Width, Depth, roofStyle, wallStyle, balconyPrefab, currentHeightIndex = 0);
            nextRoof.Generate();
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
