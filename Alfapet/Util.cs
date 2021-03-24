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

        public static bool IsHovering(Vector2 pos)
        {
            MouseState mouse = new MouseState();

            Debug.WriteLine(mouse.X);

            return false;
        }
    }
}
