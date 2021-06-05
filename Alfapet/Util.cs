using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace Alfapet
{
    class AlfapetUtil : Game
    {
        public static char GenerateRandomLetter()
        {
            return (char)new Random().Next(65, 90); // Random från 65-90 (värden för stora bokstäver i ASCII) och sedan gör om till char
        }

        public static bool IsHovering(Vector2 pos, Vector2 size)
        {
            if (!Alfapet.IsActive)
                return false;

            var mouse = Mouse.GetState(Alfapet.Window);

            return (mouse.X >= pos.X && mouse.X <= pos.X + size.X && mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y);
        }

        private static int Partition(int left, int right)
        {
            int pivot = Hand.Tiles[left].Letter; // pivot börjar som längst till vänster som man inte sorterat
            while (true)
            {
                while (Hand.Tiles[left].Letter < pivot) // hitta vänster som ska bli höger
                    left++;
                while (Hand.Tiles[right].Letter > pivot) // hitta höger som ska bli vänster
                    right--;

                if (left < right)
                {
                    if (Hand.Tiles[right].Letter == Hand.Tiles[left].Letter) // om det är samma bokstav, returna
                        return right;

                    var temp = Hand.Tiles[left]; // lagra en kopia av Tile
                    Hand.Tiles[left] = Hand.Tiles[right]; // vänster blir höger
                    Hand.Tiles[right] = temp; // höger blir kopian
                }
                else
                    return right;
            }
        }

        private static void SortHand(int left, int right)
        {
            if (left < right)
            {
                var pivot = Partition(left, right);

                if (pivot > 1)
                    SortHand(left, pivot - 1);
                if (pivot + 1 < right)
                    SortHand(pivot + 1, right);
            }
        }
        public static void SortHand()
        {
            SortHand(0, AlfapetConfig.HandAmount - 1);
            Hand.SetPositions();
        }
    }
}
