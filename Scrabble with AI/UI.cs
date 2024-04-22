using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Linq;
using System;

namespace Alfapet
{
    class Ui : Game
    {
        public static SpriteFont MontserratBold;
        public static SpriteFont MontserratBoldSmaller;
        public static SpriteFont MontserratBoldTiny;

        /*
         * Loads all fonts 
         */
        public static void Load(ContentManager content)
        {
            MontserratBold = content.Load<SpriteFont>("Fonts/Montserrat-Bold");
            MontserratBoldSmaller = content.Load<SpriteFont>("Fonts/Montserrat-Bold-Smaller");
            MontserratBoldTiny = content.Load<SpriteFont>("Fonts/Montserrat-Bold-Tiny");
        }

        /*
         * Draws a rectangle with an outline around the edges
         */
        public static void OutlinedRectangle(Rectangle rec, Color? colorOverwrite = null)
        {
            const int thickness = 2;
            const int width = 10;
            const int height = 10;

            var borderColor = Color.White;

            // Background
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, rec, colorOverwrite ?? Color.Black * 0.5f);

            // Top left
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + thickness, thickness, height - thickness), borderColor);

            // Bottom left
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + rec.Height - thickness, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + rec.Height - height, thickness, height - thickness), borderColor);

            // Top right
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - width, rec.Y, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - thickness, rec.Y + thickness, thickness, height - thickness), borderColor);

            // Bottom right
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - width, rec.Y + rec.Height - thickness, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - thickness, rec.Y + rec.Height - height, thickness, height - thickness), borderColor);
        }

        /*
         * Draws text centered at the position depending on the size
         */
        public static void DrawCenterText(SpriteFont font, string text, Vector2 pos, Vector2 size, Color color)
        {
            var fontSize = font.MeasureString(text); // Size of the font in X and Y
            var offsetX = (int)(size.X / 2 - fontSize.X / 2);
            var offsetY = (int)(size.Y / 2 - fontSize.Y / 2);

            Alfapet.SpriteBatch.DrawString(font, text, new Vector2(pos.X + offsetX, pos.Y + offsetY), color);
        }
    }
}
