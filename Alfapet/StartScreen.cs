using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Alfapet
{
    class StartScreen : Game
    {
        public static string LoadString = "";
        private static Button playBtn;
        async private static void Start()
        {
            await Alfapet.Start();
            Alfapet.UpdateFunction = delegate ()
            {
                DragDrop.Think();
                Button.Think();
            };

            Alfapet.DrawFunction = delegate ()
            {
                Board.Draw();
                Hand.Draw();
                Button.Draw();
            };

            Button.List.Remove(playBtn);
            playBtn = null;
        }

        static public void Initialize()
        {
            Alfapet.DrawFunction = delegate ()
            {
                Button.Draw();
                UI.DrawCenterText(Fonts.Montserrat_Bold_Smaller, LoadString, new Vector2(0, 0), Color.White, (int)Alfapet._graphics.GraphicsDevice.Viewport.Width, (int)Alfapet._graphics.GraphicsDevice.Viewport.Height);
            };
            Alfapet.UpdateFunction = delegate ()
            {
                Button.Think();
            };

            playBtn = new Button();
            playBtn.SetPos(Alfapet._graphics.GraphicsDevice.Viewport.Width / 2 - 100, Alfapet._graphics.GraphicsDevice.Viewport.Height / 2 + 75);
            playBtn.SetSize(200, 75);
            playBtn.SetText("Play");

            playBtn.ClickEvent = delegate ()
            {
                Start();
            };
        }
    }
}
