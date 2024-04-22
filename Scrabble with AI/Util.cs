using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Alfapet
{
    class Util : Game
    {
        public static char GenerateRandomLetter()
        {
            // Random number from 65 to 90 (values for uppercase letters in ASCII), and convert to character
            return (char)new Random().Next(65, 90);
        }

        /*
         * Returns whether the mouse pointer is over the position and within the size 
         */
        public static bool IsHovering(Vector2 pos, Vector2 size)
        {
            // Do not proceed if the program is not active
            if (!Alfapet.IsActive)
                return false;

            var mouse = Mouse.GetState(Alfapet.Window);
            return mouse.X >= pos.X && mouse.X <= pos.X + size.X && mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y;
        }
    }
}
