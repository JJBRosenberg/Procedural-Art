using UnityEngine;

namespace Demo
{
    public class SimpleStock : Shape
    {
        // grammar rule probabilities:
        const float stockContinueChance = 0.5f;

        // shape parameters:
        [SerializeField]
        int Width;
        [SerializeField]
        int Depth;

        [SerializeField]
        GameObject[] wallStyle;
        [SerializeField]
        GameObject doorPrefab; // The door prefab
        [SerializeField]
        GameObject[] roofStyle;

        private int doorWallIndex; // Index for the wall that will have the door
        private int currentHeightIndex = 0; // Tracks the current height of the building

        public void Initialize(int Width, int Depth, GameObject[] wallStyle, GameObject doorPrefab, GameObject[] roofStyle, int currentHeightIndex = 0)
        {
            this.Width = Width;
            this.Depth = Depth;
            this.wallStyle = wallStyle;
            this.doorPrefab = doorPrefab;
            this.roofStyle = roofStyle;
            this.currentHeightIndex = currentHeightIndex;

            // Only select a doorWallIndex if this is the ground floor
            if (currentHeightIndex == 0)
            {
                doorWallIndex = Random.Range(0, 4); // Randomly choose one wall to have the door on the ground floor
            }
            else
            {
                doorWallIndex = -1; // Ensure no door is placed on higher floors
            }
        }

        protected override void Execute()
        {
            // Create four walls:
            for (int i = 0; i < 4; i++)
            {
                Vector3 localPosition = new Vector3();
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
                SimpleRow newRow;
                if (i == doorWallIndex)
                {
                    newRow = CreateSymbol<SimpleRow>("wallWithDoor", localPosition, Quaternion.Euler(0, i * 90, 0));
                    newRow.Initialize(i % 2 == 1 ? Width : Depth, new GameObject[] { doorPrefab }); // Use the door prefab for this wall
                }
                else
                {
                    newRow = CreateSymbol<SimpleRow>("wall", localPosition, Quaternion.Euler(0, i * 90, 0));
                    newRow.Initialize(i % 2 == 1 ? Width : Depth, wallStyle);
                }
                newRow.Generate();
            }

            // Continue with a stock or with a roof (random choice):
            float randomValue = RandomFloat();
            if (randomValue < stockContinueChance)
            {
                SimpleStock nextStock = CreateSymbol<SimpleStock>("stock", new Vector3(0, 1, 0));
                nextStock.Initialize(Width, Depth, wallStyle, doorPrefab, roofStyle, currentHeightIndex + 1);
                nextStock.Generate(buildDelay);
            }
            else
            {
                SimpleRoof nextRoof = CreateSymbol<SimpleRoof>("roof", new Vector3(0, 1, 0));
                nextRoof.Initialize(Width, Depth, roofStyle, wallStyle);
                nextRoof.Generate(buildDelay);
            }
        }
    }
}
