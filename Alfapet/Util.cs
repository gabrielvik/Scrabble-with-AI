using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Alfapet
{
    class Util : Game
    {
        public static char GenerateRandomLetter()
        {
            // Random nummer från 65 till 90 (värden för stora bokstäver i ASCII), och gör om till karaktär
            return (char)new Random().Next(65, 90);
        }

        /*
         * Returnerar om muspekaren är över positionen och inom storleken 
        */
        public static bool IsHovering(Vector2 pos, Vector2 size)
        {
            // Gå inte vidare om man inte är inne i programmet
            if (!Alfapet.IsActive)
                return false;

            var mouse = Mouse.GetState(Alfapet.Window);
            return mouse.X >= pos.X && mouse.X <= pos.X + size.X && mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y;
        }
    }
}
