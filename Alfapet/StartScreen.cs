using Microsoft.Xna.Framework;

namespace Alfapet
{
    class StartScreen : Game
    {
        public static string LoadString = "";
        private static Button PlayBtn;
        private static readonly Button SettingsBtn;

        private static async void Start()
        {
            await Alfapet.Start();

            Alfapet.UpdateFunction = delegate ()
            {
                DragDrop.Think();
                Button.ListenForPresses();
            };

            Alfapet.DrawFunction = delegate ()
            {
                Board.Draw();
                Hand.Draw();
                Button.Draw();
            };

            Button.List.Remove(PlayBtn);
            PlayBtn = null;
        }

        private static void BuildPlayBtn()
        {
            PlayBtn = new Button();
            PlayBtn.SetPos(Alfapet.Graphics.GraphicsDevice.Viewport.Width / 2 - 100, Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2 + 75);
            PlayBtn.SetSize(200, 75);
            PlayBtn.SetText("Play");

            PlayBtn.ClickEvent = Start;
        }

        public new static void Initialize()
        {
            Alfapet.DrawFunction = delegate ()
            {
                Button.Draw();
                UI.DrawCenterText(UI.MontserratBold, "GameTitle", new Vector2(0, 0), Color.White, Alfapet.Graphics.GraphicsDevice.Viewport.Width, Alfapet.Graphics.GraphicsDevice.Viewport.Height / 2);
                UI.DrawCenterText(UI.MontserratBoldSmaller, LoadString, new Vector2(0, 0), Color.White, Alfapet.Graphics.GraphicsDevice.Viewport.Width, Alfapet.Graphics.GraphicsDevice.Viewport.Height);
            };
            Alfapet.UpdateFunction = Button.ListenForPresses;

            BuildPlayBtn();
        }
    }
}
