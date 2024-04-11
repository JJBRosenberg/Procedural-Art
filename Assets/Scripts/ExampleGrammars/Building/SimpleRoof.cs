using UnityEngine;

namespace Demo
{
    public class SimpleRoof : Shape
    {
        // grammar rule probabilities:
        const float roofContinueChance = 0.5f;

        private int currentHeightIndex = 0;
        int Width;
        int Depth;

        GameObject[] roofStyle;
        GameObject[] wallStyle;
        GameObject doorStyle;
        GameObject balconyPrefab; // Change to single GameObject for balconyPrefab

        // (offset) values for the next layer:
        int newWidth;
        int newDepth;

        // Define the balcony chance variable
        const float balconyChanceAbove2 = 0.3f;

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

            CreateFlatRoofPart();
            CreateNextPart();
        }

        void CreateFlatRoofPart()
        {
            // Randomly create two roof strips in depth direction or in width direction:
            int side = RandomInt(2);
            SimpleRow flatRoof;

            switch (side)
            {
                // Add two roof strips in depth direction
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        flatRoof = CreateSymbol<SimpleRow>("roofStrip", new Vector3((Width - 1) * (i - 0.5f), 0, 0));
                        flatRoof.Initialize(Depth, roofStyle);
                        flatRoof.Generate();
                    }
                    newWidth -= 2;
                    break;
                // Add two roof strips in width direction
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        flatRoof = CreateSymbol<SimpleRow>("roofStrip", new Vector3(0, 0, (Depth - 1) * (i - 0.5f)));
                        flatRoof.Initialize(Width, roofStyle, new Vector3(1, 0, 0));
                        flatRoof.Generate();
                    }
                    newDepth -= 2;
                    break;
            }
        }

        void CreateNextPart()
        {
            // randomly continue with a roof or a stock:
            if (newWidth <= 0 || newDepth <= 0)
                return;

            // Check if the current height index is above a threshold for spawning balconies
            bool spawnBalcony = currentHeightIndex > 2 && Random.value < balconyChanceAbove2;

            if (spawnBalcony)
            { // Create a balcony
                SimpleRow balcony = CreateSymbol<SimpleRow>("balcony", Vector3.zero);
                GameObject[] balconyPrefabArray = { balconyPrefab }; // Convert balconyPrefab to an array
                balcony.Initialize(Width, balconyPrefabArray); // Corrected parameter here
                balcony.Generate();
            }
            else
            { // Continue with the roof or a stock
                float randomValue = RandomFloat();
                if (randomValue < roofContinueChance)
                { // continue with the roof
                    SimpleRoof nextRoof = CreateSymbol<SimpleRoof>("roof");
                    nextRoof.Initialize(newWidth, newDepth, roofStyle, wallStyle, balconyPrefab);
                    nextRoof.Generate(buildDelay);
                }
                else
                { // continue with a stock
                    SimpleStock nextStock = CreateSymbol<SimpleStock>("stock");
                    // Assuming doorPrefab is set to null as it's not managed or relevant for roofs
                    // Ensure the order and types of arguments match the expected signature
                    nextStock.Initialize(newWidth, newDepth, wallStyle, null, roofStyle, balconyPrefab, currentHeightIndex + 1);
                    nextStock.Generate(buildDelay);
                }
            }
        }

    }
}
