#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

#endregion

namespace bomber
{
    public static class Globals
    {
        public static SpriteBatch Batch;
        public static ContentManager Content;
        public static GraphicsDeviceManager Graphics;
        public static TileMap Map;
        public const int TileWidth = 16;
        public const int TileHeight = 16;
        public const int Width = 320;
        public const int Height = 240;

        public static Random random;
        public static BitmapFont Font;

        public static GameState CurrentState;

        public static int WrappedY(int y)
        {
            return (y + Height) % Height;
        }

        public static int WrappedX(int x)
        {
            return (x + Width) % Width;
        }

        public static void HandleTimer(ref int counter, GameTime gameTime, Action action = null)
        {
            counter -= gameTime.ElapsedGameTime.Milliseconds;
            if (counter < 0)
            {
                counter = 0;
            }
            if (counter == 0)
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        public static void SetState(GameState state)
        {
            CurrentState = state;
            state.Initialize();
        }
    }
     
	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public const int Scale = 2;
        //private Boolean resized = false;

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Globals.Content = Content;
            Globals.Content.RootDirectory = "Content";

            //graphics.PreferredBackBufferWidth = 1680;
            //graphics.PreferredBackBufferHeight = 1050;
            //graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Globals.random = new Random();
            base.Initialize();
            Globals.SetState(new MainGame());
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Globals.Batch = new SpriteBatch(GraphicsDevice);
            Globals.Font = new BitmapFont(Content.Load<Texture2D>("Textures/c64_font.png"), 8, 10);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            /*
            if (gameTime.TotalGameTime.Milliseconds > 500 && !resized)
            {
                resized = true;
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 600;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }
            */

            Globals.CurrentState.Update(gameTime);	
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            Globals.Batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(Scale));
            Globals.CurrentState.Draw();
            Globals.Batch.End();
        }
    }
}

