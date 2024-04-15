using UnityEngine;
using System.Collections.Generic;

namespace Demo
{
    public class CentralStock : Shape
    {
        const float stockContinueChance = 0.5f;

        [SerializeField] int Width;
        [SerializeField] int Depth;
        [SerializeField] GameObject[] wallStyle;
        [SerializeField] GameObject[] roofStyle;

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

        public void Initialize(int width, int depth, GameObject[] wallStyle, GameObject[] roofStyle, int currentHeightIndex, int minHeight = 1, int maxHeight = 10)
        {
            Width = width;
            Depth = depth;
            this.wallStyle = wallStyle;
            this.roofStyle = roofStyle;
            this.currentHeightIndex = currentHeightIndex;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        protected override void Execute()
        {
            GenerateStock();
        }

        private void GenerateStock()
        {
            if (currentHeightIndex >= maxHeight)
            {
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
                        localPosition = new Vector3(-(Width - 1) * 0.5f, 0, 0);
                        break;
                    case 1:
                        localPosition = new Vector3(0, 0, (Depth - 1) * 0.5f);
                        break;
                    case 2:
                        localPosition = new Vector3((Width - 1) * 0.5f, 0, 0);
                        break;
                    case 3:
                        localPosition = new Vector3(0, 0, -(Depth - 1) * 0.5f);
                        break;
                }

                SimpleRow newRow = CreateSymbol<SimpleRow>("wall", localPosition, Quaternion.Euler(0, i * 90, 0));
                newRow.Initialize(i % 2 == 1 ? Width : Depth, wallStyle);
                newRow.Generate();
                Renderer[] rowRenderers = newRow.GetComponentsInChildren<Renderer>();
                allRenderers.AddRange(rowRenderers);
            }

            AddRenderersToLODGroup(allRenderers);

            currentHeightIndex++;

            if (currentHeightIndex < minHeight || Random.value < stockContinueChance)
            {
                CentralStock nextStock = CreateSymbol<CentralStock>("stock", new Vector3(0, 1, 0));
                nextStock.Initialize(Width, Depth, wallStyle, roofStyle, currentHeightIndex, minHeight, maxHeight);
                nextStock.Generate();
            }
            else
            {
                GenerateRoof();
            }
        }

        private void GenerateRoof()
        {
            CentralRoof nextRoof = CreateSymbol<CentralRoof>("roof", new Vector3(0, 1, 0));
            nextRoof.Initialize(Width, Depth, roofStyle);
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
