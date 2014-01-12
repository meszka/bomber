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
        public static TileMap Map;
        public const int TileWidth = 16;
        public const int TileHeight = 16;
        public const int Width = 320;
        public const int Height = 240;

        public static Random random;

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
    }

	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public const int Scale = 2;

        private GraphicsDeviceManager graphics;
        private Player player;
        private Player player2;
        //private Boolean resized = false;

        private List<Player> playerList = new List<Player>();
        private int[] score = new int[4] {0, 0, 0, 0};
        private bool scored = false;
        private int scoreDelay = 0;
        private Player winner = null;
        private int pointsToWin = 5;

        private BitmapFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Globals.Batch = new SpriteBatch(GraphicsDevice);

            font = new BitmapFont(Content.Load<Texture2D>("Textures/c64_font.png"), 8, 10);

            Dictionary<string, Keys> playerControls = new Dictionary<string, Keys> {
                {"left", Keys.Left},
                {"right", Keys.Right},
                {"jump", Keys.Up},
                {"bomb", Keys.Down},
            };

            player = new Player(0, Content.Load<Texture2D>("Textures/player_small.png"), new Rectangle(100, 5, 13, 13), playerControls, "red", Color.Red, Color.PeachPuff);
            playerList.Add(player);

            Dictionary<string, Keys> player2Controls = new Dictionary<string, Keys> {
                {"left", Keys.A},
                {"right", Keys.D},
                {"jump", Keys.W},
                {"bomb", Keys.S},
            };

            player2 = new Player(1, Content.Load<Texture2D>("Textures/player_small.png"), new Rectangle(200, 5, 13, 13), player2Controls, "blue", Color.Blue, Color.LightBlue);
            playerList.Add(player2);

            Globals.Map = new TileMap(20, 15);

            Globals.Map.LoadMap(new int[,] {
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2},
                {0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 1},
                {1, 2, 1, 0, 1, 0, 0, 0, 0, 2, 0, 2, 2, 0, 2, 2, 2, 0, 2, 1},
                {1, 2, 0, 1, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 1},
                {1, 1, 0, 0, 0, 1, 0, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 2, 1},
                {1, 0, 0, 1, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1},
            });

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

            if (scored)
            {
                Globals.HandleTimer(ref scoreDelay, gameTime, Restart);
            }

            Sprite.UpdateAll(gameTime);
            Globals.Map.Update();
            Explosion.CleanUp();
            Bomb.CleanUp();
            Powerup.CleanUp();

            playerList.RemoveAll(p => p.Dead);
            if (playerList.Count == 1 && !scored)
            {
                winner = playerList[0];
                score[winner.Id] += 1;
                scoreDelay = 2000;
                scored = true;
            }

            // TODO: Add your update logic here			
            base.Update(gameTime);
        }

        private void Restart()
        {
            winner = null;
            scored = false;
            Sprite.SpriteList.Clear();
            Explosion.ExplosionList.Clear();
            playerList.Clear();
            LoadContent();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            Globals.Batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(Scale));
            Sprite.DrawAll();
            //Globals.Map.Draw();

            font.DrawString(score[0].ToString(), 20, 10, Color.Red, 2);
            font.DrawString(score[1].ToString(), 280, 10, Color.Blue, 2);

            if (scored)
            {
                font.DrawString(string.Format("{0} player scores!", winner.ColorName), 10, 200, winner.TextColor, 2);
            }

            Globals.Batch.End();
        }
    }
}

