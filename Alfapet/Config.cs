using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Alfapet
{
    class Alfapet_Config : Game
    {
        public static int HandAmount = 9;

        public static Dictionary<char, int> CharactherPoints;

        public async static void Initialize()
        {
            CharactherPoints = new Dictionary<char, int>()
            {
                { 'A', 2 },
                { 'B', 2 },
                { 'C', 4 },
                { 'D', 3 },
                { 'E', 3 },
                { 'F', 3 },
                { 'G', 6 },
                { 'H', 8 },
                { 'I', 3 },
                { 'J', 7 },
                { 'K', 4 },
                { 'L', 4 },
                { 'M', 5 },
                { 'N', 3 },
                { 'O', 4 },
                { 'P', 6 },
                { 'Q', 12 },
                { 'R', 3 },
                { 'S', 5 },
                { 'T', 7 },
                { 'U', 7 },
                { 'V', 9 },
                { 'W', 11 },
                { 'X', 13 },
                { 'Y', 9 },
                { 'Z', 10 },
            };
        }
    }
}
