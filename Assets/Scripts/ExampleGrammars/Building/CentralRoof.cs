using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class CentralRoof : Shape
    {
        const float roofContinueChance = 0.5f; 

        int Width;
        int Depth;

        GameObject[] roofStyle;

        private int currentHeightIndex = 0;
        private LODGroup lodGroup;

        private void Awake()
        {
            lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                Debug.LogError("LODGroup component not found on the GameObject or its parents.");
            }
        }

        public void Initialize(int Width, int Depth, GameObject[] roofStyle, int currentHeightIndex = 0)
        {
            this.Width = Width;
            this.Depth = Depth;
            this.roofStyle = roofStyle;
            this.currentHeightIndex = currentHeightIndex;
        }

        protected override void Execute()
        {
            if (Width == 0 || Depth == 0)
                return;

            List<Renderer> allRenderers = new List<Renderer>();
            CreateFlatRoofPart(allRenderers);
            CreateNextPart(allRenderers);
            AddRenderersToLODGroup(allRenderers);
        }

        void CreateFlatRoofPart(List<Renderer> allRenderers)
        {
            int side = RandomInt(2);
            SimpleRow flatRoof;

            switch (side)
            {
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        flatRoof = CreateSymbol<SimpleRow>("roofStrip", new Vector3((Width - 1) * (i - 0.5f), 0, 0));
                        flatRoof.Initialize(Depth, roofStyle);
                        flatRoof.Generate();
                        Renderer[] rowRenderers = flatRoof.GetComponentsInChildren<Renderer>();
                        allRenderers.AddRange(rowRenderers);
                    }
                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        flatRoof = CreateSymbol<SimpleRow>("roofStrip", new Vector3(0, 0, (Depth - 1) * (i - 0.5f)));
                        flatRoof.Initialize(Width, roofStyle, new Vector3(1, 0, 0));
                        flatRoof.Generate();
                        Renderer[] rowRenderers = flatRoof.GetComponentsInChildren<Renderer>();
                        allRenderers.AddRange(rowRenderers);
                    }
                    break;
            }
        }

        void CreateNextPart(List<Renderer> allRenderers)
        {
            float randomValue = RandomFloat();
            if (randomValue < roofContinueChance)
            {
                CentralRoof nextRoof = CreateSymbol<CentralRoof>("roof");
                nextRoof.Initialize(Width, Depth, roofStyle);
                nextRoof.Generate();
                Renderer[] rowRenderers = nextRoof.GetComponentsInChildren<Renderer>();
                allRenderers.AddRange(rowRenderers);
            }
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
