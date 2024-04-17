using UnityEngine;
using System.Collections.Generic;

namespace Demo
{
    public class ResidentialStock : Shape
    {
        const float stockContinueChance = 0.5f;
        const float balconyChanceAbove2 = 0f; // No balcony generation

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
        private List<Renderer> allRenderers = new List<Renderer>();

        private void Awake()
        {
            lodGroup = GetComponentInParent<LODGroup>() ?? GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                Debug.LogError("LODGroup component not found on the GameObject or its parents.");
            }
            else
            {
                PrepareLODLevels();
            }
        }

        private void PrepareLODLevels()
        {
            if (lodGroup.lodCount == 0) // If no LODs are set up, set up a default one
            {
                LOD[] lods = new LOD[2];
                lods[0] = new LOD(0.5f, new Renderer[0]); // High detail LOD
                lods[1] = new LOD(0.15f, new Renderer[0]); // Low detail LOD
                lodGroup.SetLODs(lods);
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

                SimpleRow newRow = CreateSymbol<SimpleRow>(i == doorWallIndex ? "wallWithDoor" : "wall", localPosition, Quaternion.Euler(0, i * 90, 0));
                newRow.Initialize(i % 2 == 1 ? Width : Depth, i == doorWallIndex ? new GameObject[] { doorPrefab } : wallStyle);
                newRow.Generate();
                allRenderers.AddRange(newRow.GetComponentsInChildren<Renderer>());
            }

            AddRenderersToLODGroup();

            currentHeightIndex++;

            if (currentHeightIndex < minHeight || Random.value < stockContinueChance)
            {
                ResidentialStock nextStock = CreateSymbol<ResidentialStock>("stock", new Vector3(0, 1, 0));
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
            SimpleRoof nextRoof = CreateSymbol<SimpleRoof>("roof", new Vector3(0, 1, 0));
            nextRoof.Initialize(Width, Depth, roofStyle, wallStyle, balconyPrefab, 0);
            nextRoof.Generate();
        }

        private void AddRenderersToLODGroup()
        {
            if (lodGroup == null || lodGroup.lodCount == 0)
            {
                Debug.LogWarning("LODGroup component not found or not set up properly.");
                return;
            }

            // Assume LOD[0] is for close detail and LOD[1] is for far detail
            LOD[] lods = lodGroup.GetLODs();
            foreach (Renderer renderer in allRenderers)
            {
                if (renderer.bounds.size.sqrMagnitude > 1) // Example condition for sorting into LOD levels
                {
                    List<Renderer> highDetailRenderers = new List<Renderer>(lods[0].renderers);
                    highDetailRenderers.Add(renderer);
                    lods[0].renderers = highDetailRenderers.ToArray();
                }
                else
                {
                    List<Renderer> lowDetailRenderers = new List<Renderer>(lods[1].renderers);
                    lowDetailRenderers.Add(renderer);
                    lods[1].renderers = lowDetailRenderers.ToArray();
                }
            }

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
    }
}
