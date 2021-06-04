using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public static async Task<int> Start()
        {

            var asd = new Dictionary<Action, string>()
            {
                { Alfapet_Config.Initialize, "Loading config"},
                { Board.Initialize, "Creating Board" },
                { Hand.Initialize, "Creating Hand" },
                { () => Dictionaries.Initialize("english"), "Unpacking JSON" },
                { ButtonRig.Initialize, "Creating Buttons" }
            };

            await Task.Run(() =>
            {
                /*StartScreen.LoadString = "Loading Config";
                Alfapet_Config.Initialize();
                StartScreen.LoadString = "Creating Board";
                Board.Initialize();
                StartScreen.LoadString = "Creating Hand";
                Hand.Initialize();
                StartScreen.LoadString = "Unpacking JSON";
                Dictionaries.Initialize("english");
                StartScreen.LoadString = "Creating Button Rig";
                ButtonRig.Initialize();*/

                foreach (var func in asd)
                {
                    StartScreen.LoadString = func.Value;
                    func.Key.Invoke();
                }


                return 1;
            });
            return 0;
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

            UI.Load(Content);
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
