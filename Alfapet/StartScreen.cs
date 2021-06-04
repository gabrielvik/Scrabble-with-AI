using Microsoft.Xna.Framework;

namespace Alfapet
{
    class StartScreen : Game
    {
        public static string LoadString = "";
        private static Button playBtn;
        private static readonly Button settingsBtn;

        private static async void Start()
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

        private static void BuildPlayBtn()
        {
            playBtn = new Button();
            playBtn.SetPos(Alfapet._graphics.GraphicsDevice.Viewport.Width / 2 - 100, Alfapet._graphics.GraphicsDevice.Viewport.Height / 2 + 75);
            playBtn.SetSize(200, 75);
            playBtn.SetText("Play");

            playBtn.ClickEvent = delegate ()
            {
                Start();
            };
        }

        new public static void Initialize()
        {
            Alfapet.DrawFunction = delegate ()
            {
                Button.Draw();
                UI.DrawCenterText(UI.Montserrat_Bold, "GameTitle", new Vector2(0, 0), Color.White, Alfapet._graphics.GraphicsDevice.Viewport.Width, Alfapet._graphics.GraphicsDevice.Viewport.Height / 2);
                UI.DrawCenterText(UI.Montserrat_Bold_Smaller, LoadString, new Vector2(0, 0), Color.White, Alfapet._graphics.GraphicsDevice.Viewport.Width, Alfapet._graphics.GraphicsDevice.Viewport.Height);
            };
            Alfapet.UpdateFunction = delegate ()
            {
                Button.Think();
            };

            BuildPlayBtn();
        }
    }
}
