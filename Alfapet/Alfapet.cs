using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Alfapet
{
    public class Alfapet : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public new static GameWindow Window;
        public new static bool IsActive;

        public static Texture2D TransparentBack;

        public static Action UpdateFunction;
        public static Action DrawFunction;

        public Alfapet()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public static async Task<int> Start()
        {

            var initFuncs = new Dictionary<Action, string>()
            {
                { AlfapetConfig.Initialize, "Loading config"},
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

                foreach (var func in initFuncs)
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

            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 800;
            IsFixedTimeStep = false; // tar bort FPS cap
            //Window.Position = new Point(-1500, 0);
            Graphics.ApplyChanges();

            Window = base.Window;

            StartScreen.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            TransparentBack = new Texture2D(GraphicsDevice, 1, 1);
            TransparentBack.SetData(new Color[] { Color.White * 0.5f });

            UI.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateFunction?.Invoke();
            
            IsActive = base.IsActive;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(47, 54, 64));

            SpriteBatch.Begin();
            DrawFunction?.Invoke();
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
