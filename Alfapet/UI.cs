using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Alfapet
{
    class UI : Game
    {
        public static SpriteFont MontserratBold;
        public static SpriteFont MontserratBoldSmaller;

        public static void Load(ContentManager Content)
        {
            MontserratBold = Content.Load<SpriteFont>("Fonts/Montserrat-Bold");
            MontserratBoldSmaller = Content.Load<SpriteFont>("Fonts/Montserrat-Bold-Smaller");
        }

        public static void StylishRectangle(Rectangle rec, Color? colorOverwrite = null)
        {
            const int thickness = 2;
            const int width = 10;
            const int height = 10;

            var borderColor = Color.White;

            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, rec, colorOverwrite ?? (Color.Black * 0.5f));

            // Vänster top
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + thickness, thickness, height - thickness), borderColor);

            // Vänster nedre
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + rec.Height - thickness, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + rec.Height - height, thickness, height - thickness), borderColor);

            // Högre top
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - width, rec.Y, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - thickness, rec.Y + thickness, thickness, height - thickness), borderColor);

            // Högre nedre
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - width, rec.Y + rec.Height - thickness, width, thickness), borderColor);
            Alfapet.SpriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - thickness, rec.Y + rec.Height - height, thickness, height - thickness), borderColor);
        }
        
        public static void DrawCenterText(SpriteFont font, string text, Vector2 pos, Color color, int recWidth, int recHeight)
        {
            if (text == "\0")
                return;

            var fontSize = font.MeasureString(text);
            var offsetX = (int)(recWidth / 2 - fontSize.X / 2);
            var offsetY = (int)(recHeight / 2 - fontSize.Y / 2);

            Alfapet.SpriteBatch.DrawString(font, text, new Vector2(pos.X + offsetX, pos.Y + offsetY), color);
        }
    }
}
