using System;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class Alfapet_Util : Game
    {
        public static char GenerateRandomLetter()
        {
            return (char)new Random().Next(65, 90); // Random från 65-90 (värden för stora bokstäver i ASCII) och sedan gör om till char
        }
    }
}
