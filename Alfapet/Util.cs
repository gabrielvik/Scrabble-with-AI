using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


namespace Alfapet
{
    class Alfapet_Util : Game
    {
        public static char GenerateRandomLetter()
        {
            return (char)new Random().Next(65, 90); // Random från 65-90 (värden för stora bokstäver i ASCII) och sedan gör om till char
        }

        public static bool IsHovering(Vector2 pos, Vector2 size)
        {
            MouseState mouse = Mouse.GetState(Alfapet._window);

            return (mouse.X >= pos.X && mouse.X <= pos.X + size.X && mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y);
        }
    }
}
