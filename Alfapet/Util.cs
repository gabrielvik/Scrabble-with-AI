using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using SharpDX.Direct2D1.Effects;


namespace Alfapet
{
    class Util : Game
    {
        public static char GenerateRandomLetter()
        {
            return (char)new Random().Next(65, 90); // 65-90 är värden för stora bokstäver i ASCII
        }

        public static bool IsHovering(Vector2 pos, Vector2 size)
        {
            if (!Alfapet.IsActive) // Gå inte vidare om man inte är inne i programmet
                return false;

            var mouse = Mouse.GetState(Alfapet.Window);

            return mouse.X >= pos.X && mouse.X <= pos.X + size.X && mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y;
        }
    }
}
