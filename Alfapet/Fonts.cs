using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Alfapet
{
    class Fonts : Game
    {
        public static SpriteFont Montserrat_Bold;

        public static void Load(ContentManager Content)
        {
            Montserrat_Bold = Content.Load<SpriteFont>("Fonts/Montserrat-Bold");
        }
    }
}
