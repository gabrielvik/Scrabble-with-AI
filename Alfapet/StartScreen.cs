using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Alfapet
{
    class StartScreen : Game
    {
        public static string LoadString = "";
        private static void Start()
        {
            Alfapet.Start();

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
        }

        static public void Initialize()
        {
            Alfapet.DrawFunction = delegate ()
            {
                Button.Draw();
                UI.DrawCenterText(Fonts.Montserrat_Bold_Smaller, LoadString, new Vector2(Alfapet._graphics.GraphicsDevice.Viewport.Width / 2, Alfapet._graphics.GraphicsDevice.Viewport.Height / 2), Color.White, (int)Alfapet._graphics.GraphicsDevice.Viewport.Width / 2, (int)Alfapet._graphics.GraphicsDevice.Viewport.Height / 2);
            };
            Alfapet.UpdateFunction = delegate ()
            {
                Button.Think();
            };

            Button playBtn = new Button();
            playBtn.SetPos(50, 50);
            playBtn.SetSize(100, 100);

            playBtn.ClickEvent = delegate ()
            {
                Button.List.Remove(playBtn);
                playBtn = null;

                Start();
            };
        }
    }
}
