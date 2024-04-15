using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
    public class SkyscraperRoof : Shape
    {
        int Width;
        int Depth;

        GameObject[] roofStyle;

        private LODGroup lodGroup;

        private void Awake()
        {
            lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                Debug.LogError("LODGroup component not found on the GameObject or its parents.");
            }
        }

        public void Initialize(int width, int depth, GameObject[] roofStyle)
        {
            this.Width = width;
            this.Depth = depth;
            this.roofStyle = roofStyle;
        }

        protected override void Execute()
        {
            if (Width == 0 || Depth == 0)
                return;

            List<Renderer> allRenderers = new List<Renderer>();
            CreateFlatRoofPart(allRenderers);
            AddRenderersToLODGroup(allRenderers);
        }

        void CreateFlatRoofPart(List<Renderer> allRenderers)
        {
            SimpleRow flatRoof = CreateSymbol<SimpleRow>("roofStrip", new Vector3(0, 0, 0));
            flatRoof.Initialize(Mathf.Max(Width, Depth), roofStyle);
            flatRoof.Generate();
            Renderer[] rowRenderers = flatRoof.GetComponentsInChildren<Renderer>();
            allRenderers.AddRange(rowRenderers);
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
