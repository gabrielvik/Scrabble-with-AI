using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Alfapet
{
    public class Alfapet : Game
    {
        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        public static GameWindow _window;

        public static Texture2D TransparentBack;

        public static Action UpdateFunction;
        public static Action DrawFunction;

        public Alfapet()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public static void Start()
        {
            Alfapet_Config.Initialize();
            StartScreen.LoadString = "1";
            Board.Initialize();
            System.Diagnostics.Debug.WriteLine("2");
            Hand.Initialize();
            System.Diagnostics.Debug.WriteLine("3");
            Dictionaries.Initialize("english");
            System.Diagnostics.Debug.WriteLine("4");
            ButtonRig.Initialize();
            System.Diagnostics.Debug.WriteLine("5");
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            IsFixedTimeStep = false; // tar bort FPS cap
            //Window.Position = new Point(-1500, 0);
            _graphics.ApplyChanges();

            _window = Window;

            StartScreen.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TransparentBack = new Texture2D(GraphicsDevice, 1, 1);
            TransparentBack.SetData(new Color[] { Color.White * 0.5f });

            Fonts.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateFunction?.Invoke();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(47, 54, 64));

            _spriteBatch.Begin();
                DrawFunction?.Invoke();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
