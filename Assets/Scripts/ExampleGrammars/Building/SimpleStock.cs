using UnityEngine;

namespace Demo
{
    public class SimpleStock : Shape
    {
        const float stockContinueChance = 0.5f;

        public int maxWidth = 10;
        public int maxLength = 5;

        public int Width;
        public int Depth;

        public GameObject[] wallStyle;
        public GameObject[] roofStyle;

        public void Initialize(int Width, int Depth, GameObject[] wallStyle, GameObject[] roofStyle)
        {
            this.Width = Mathf.Min(Width, maxWidth);
            this.Depth = Mathf.Min(Depth, maxLength);
            this.wallStyle = wallStyle;
            this.roofStyle = roofStyle;
        }

        protected override void Execute()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 localPosition = new Vector3();
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
            }

            float randomValue = RandomFloat();
            if (randomValue < stockContinueChance)
            {
                SimpleStock nextStock = CreateSymbol<SimpleStock>("stock", new Vector3(0, 1, 0));
                nextStock.Initialize(Width, Depth, wallStyle, roofStyle);
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
