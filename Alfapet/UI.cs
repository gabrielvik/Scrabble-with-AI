using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Alfapet
{
    class UI : Game
    {
        public static void StylishRectangle(Rectangle rec, Color? colorOverwrite = null)
        {
            int thickness = 2;
            int width = 10;
            int height = 10;

            Color border_color = Color.White;

            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, rec, (colorOverwrite == null) ? (Color.Black * 0.5f) : (Color)(colorOverwrite));

            // Vänster top
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y, width, thickness), border_color);
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + thickness, thickness, height - thickness), border_color);

            // Vänster nedre
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + rec.Height - thickness, width, thickness), border_color);
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X, rec.Y + rec.Height - height, thickness, height - thickness), border_color);

            // Högre top
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - width, rec.Y, width, thickness), border_color);
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - thickness, rec.Y + thickness, thickness, height - thickness), border_color);

            // Högre nedre
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - width, rec.Y + rec.Height - thickness, width, thickness), border_color);
            Alfapet._spriteBatch.Draw(Alfapet.TransparentBack, new Rectangle(rec.X + rec.Width - thickness, rec.Y + rec.Height - height, thickness, height - thickness), border_color);
        }
        public static void DrawCenterText(SpriteFont font, string text, Vector2 pos, Color color, int rec_width, int rec_height)
        {
            if (text == "\0")
                return;

            Vector2 font_size = font.MeasureString(text);
            int offset_x = (int)(rec_width / 2 - font_size.X / 2);
            int offset_y = (int)(rec_height / 2 - font_size.Y / 2);

            Alfapet._spriteBatch.DrawString(font, text, new Vector2(pos.X + offset_x, pos.Y + offset_y), color);
        }
    }
}
