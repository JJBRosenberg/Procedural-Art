using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class StraightRoof : Shape
    {
        const float roofContinueChance = 0.5f;
        const float balconyChanceAbove2 = 0.3f;

        private int currentHeightIndex = 0;
        [SerializeField] int Width;
        [SerializeField] int Depth;
        [SerializeField] GameObject[] roofStyle;
        [SerializeField] GameObject[] wallStyle;
        [SerializeField] GameObject balconyPrefab;

        int newWidth;
        int newDepth;

        private LODGroup lodGroup;

        private void Awake()
        {
            lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                Debug.LogError("LODGroup component not found on the GameObject or its parents.");
            }
        }

        public void Initialize(int Width, int Depth, GameObject[] roofStyle, GameObject[] wallStyle, GameObject balconyPrefab, int currentHeightIndex = 0)
        {
            this.Width = Width;
            this.Depth = Depth;
            this.roofStyle = roofStyle;
            this.wallStyle = wallStyle;
            this.balconyPrefab = balconyPrefab;
            this.currentHeightIndex = currentHeightIndex;
        }

        protected override void Execute()
        {
            if (Width == 0 || Depth == 0)
                return;

            newWidth = Width;
            newDepth = Depth;
            List<Renderer> allRenderers = new List<Renderer>();
            CreateFlatRoofPart(allRenderers);
            CreateNextPart(allRenderers);
            AddRenderersToLODGroup(allRenderers);
        }

        void CreateFlatRoofPart(List<Renderer> allRenderers)
        {
            int side = Random.Range(0, 2);
            StraightRow flatRoof;

            switch (side)
            {
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        flatRoof = CreateSymbol<StraightRow>("roofStrip", new Vector3((Width - 1) * (i - 0.5f), 0, 0));
                        flatRoof.Initialize(Depth, roofStyle, currentHeightIndex);
                        flatRoof.Generate();
                        Renderer[] rowRenderers = flatRoof.GetComponentsInChildren<Renderer>();
                        allRenderers.AddRange(rowRenderers);
                    }
                    newWidth -= 2;
                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        flatRoof = CreateSymbol<StraightRow>("roofStrip", new Vector3(0, 0, (Depth - 1) * (i - 0.5f)));
                        flatRoof.Initialize(Width, roofStyle, currentHeightIndex, new Vector3(1, 0, 0));
                        flatRoof.Generate();
                        Renderer[] rowRenderers = flatRoof.GetComponentsInChildren<Renderer>();
                        allRenderers.AddRange(rowRenderers);
                    }
                    newDepth -= 2;
                    break;
            }
        }

        void CreateNextPart(List<Renderer> allRenderers)
        {
            if (newWidth <= 0 || newDepth <= 0)
                return;

            bool spawnBalcony = currentHeightIndex > 2 && Random.value < balconyChanceAbove2;

            if (spawnBalcony)
            {
                StraightRow balcony = CreateSymbol<StraightRow>("balcony", Vector3.zero);
                GameObject[] balconyPrefabArray = { balconyPrefab };
                balcony.Initialize(Width, balconyPrefabArray, currentHeightIndex);
                balcony.Generate();
                Renderer[] rowRenderers = balcony.GetComponentsInChildren<Renderer>();
                allRenderers.AddRange(rowRenderers);
            }
            else
            {
                if (Random.value < roofContinueChance)
                {
                    StraightRoof nextRoof = CreateSymbol<StraightRoof>("roof");
                    nextRoof.Initialize(newWidth, newDepth, roofStyle, wallStyle, balconyPrefab, currentHeightIndex);
                    nextRoof.Generate();
                    Renderer[] rowRenderers = nextRoof.GetComponentsInChildren<Renderer>();
                    allRenderers.AddRange(rowRenderers);
                }
                else
                {
                    StraightStock nextStock = CreateSymbol<StraightStock>("stock");
                    nextStock.Initialize(newWidth, newDepth, wallStyle, null, roofStyle, balconyPrefab, currentHeightIndex + 1);
                    nextStock.Generate();
                    Renderer[] rowRenderers = nextStock.GetComponentsInChildren<Renderer>();
                    allRenderers.AddRange(rowRenderers);
                }
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
