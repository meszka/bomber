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

        public static int WrappedY(int y)
        {
            return (y + Height) % Height;
        }

        public static int WrappedX(int x)
        {
            return (x + Width) % Width;
        }
    }

    public class Sprite
    {
        public Rectangle Box;
        public int Direction = 1;

        protected Texture2D texture;

        public Sprite(Texture2D texture, Rectangle box)
        {
            this.texture = texture;
            this.Box = box;
        }

        public Sprite()
        {
        }

        public void Draw()
        {
            SpriteEffects effect = Direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Globals.Batch.Draw(texture, Box, null, Color.White, 0.0f, new Vector2(0, 0), effect, 0.0f);
            Rectangle BoxWrapX = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
            Rectangle BoxWrapY = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
            BoxWrapX.X -= Globals.Width;
            BoxWrapY.Y -= Globals.Height;
            Globals.Batch.Draw(texture, BoxWrapX, null, Color.White, 0.0f, new Vector2(0, 0), effect, 0.0f);
            Globals.Batch.Draw(texture, BoxWrapY, null, Color.White, 0.0f, new Vector2(0, 0), effect, 0.0f);
        }
    }

    public class Player : Sprite
    {
        private float vy = 0.0f;
        private float vx = 1.0f;
        //private float maxV = 16.0f;
        private float maxV = 10.0f;
        private float gravity = 0.3f;
        private float jumpVelocity = 5.0f;
        private Boolean jumping = false;

        public Player(Texture2D texture, Rectangle box) : base(texture, box)
        {
        }

        public void WalkLeft()
        {
            Direction = -1;
            Box.X -= (int) vx;
            if (Globals.Map.Collide(this))
            {
                Box.X = ((Box.X / Globals.TileWidth) + 1) * Globals.TileWidth;
            }
            Box.X = Globals.WrappedX(Box.X);
        }

        public void WalkRight()
        {
            Direction = 1;
            Box.X += (int)vx;
            if (Globals.Map.Collide(this))
            {
                Box.X = (Box.X / Globals.TileWidth) * Globals.TileWidth;
            }
            Box.X = Globals.WrappedX(Box.X);
        }

        public void Jump()
        {
            if (!jumping)
            {
                vy = -jumpVelocity;
                jumping = true;
            }
        }

        public void Update()
        {
            Box.Y += (int)vy;
            Box.Y = Globals.WrappedY(Box.Y);
            vy += gravity;

            if (vy > maxV)
            {
                vy = maxV;
            }

            if (Globals.Map.Collide(this))
            {
                if (vy > 0)
                {
                    Box.Y = (Box.Y / Globals.TileHeight) * Globals.TileHeight;
                    jumping = false;
                }
                else
                {
                    Box.Y = ((Box.Y / Globals.TileHeight) + 1) * Globals.TileHeight;
                }
                Box.Y = Globals.WrappedY(Box.Y);
                vy = 0;
            }
        }
    }

    public abstract class Tile : Sprite
    {
        protected Boolean solid;
        protected Boolean destroyable;

        public Boolean Solid { get { return solid; } }
        public Boolean Destroyable { get { return destroyable; } }
    }

    public class Brick : Tile
    {
        public Brick(Rectangle box)
        {
            solid = true;
            destroyable = true;
            texture = Globals.Content.Load<Texture2D>("Textures/brick.png");
            Box = box;
        }
    }

    public class Metal : Tile
    {
        public Metal(Rectangle box)
        {
            solid = true;
            destroyable = false;
            texture = Globals.Content.Load<Texture2D>("Textures/metal.png");
            Box = box;
        }
    }

    public class TileMap
    {
        public Tile[,] Tiles { get; set; }

        public TileMap(int width, int height)
        {
            Tiles = new Tile[width, height];
        }

        public void LoadMap(int [,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Rectangle rect = new Rectangle(j * Globals.TileWidth,
                                                   i * Globals.TileHeight,
                                                   Globals.TileWidth,
                                                   Globals.TileHeight);
                    if (map[i, j] == 1)
                    {
                        Tiles[j, i] = new Metal(rect);
                    }
                    else if (map[i, j] == 2)
                    {
                        Tiles[j, i] = new Brick(rect);
                    }
                }
            }
        }

        public Boolean Collide(Sprite sprite)
        {
            int top = Globals.WrappedY(sprite.Box.Top) / Globals.TileHeight;
            int bottom = Globals.WrappedY(sprite.Box.Bottom - 1) / Globals.TileHeight;
            int left = Globals.WrappedX(sprite.Box.Left) / Globals.TileWidth;
            int right = Globals.WrappedX(sprite.Box.Right - 1) / Globals.TileWidth;
 
            Tile[] tilesToCheck = new Tile[] { Tiles[left, top], Tiles[right, top], Tiles[left, bottom], Tiles[right, bottom] };
            return tilesToCheck.Where(t => t != null).Any(t => t.Solid);
        }

        public void Draw()
        {
            foreach (Tile t in Tiles)
            {
                if (t != null)
                {
                    t.Draw();
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
        //private Boolean resized = false;

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
            // TODO: Add your initialization logic here
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

            player = new Player(Content.Load<Texture2D>("Textures/player.png"), new Rectangle(100, 5, 16, 16));
            Globals.Map = new TileMap(20, 16);
            Globals.Map.LoadMap(new int[,] {
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {1, 0, 0, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
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


            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Right))
            {
                player.WalkRight();
            }
            if (kbs.IsKeyDown(Keys.Left))
            {
                player.WalkLeft();
            }
            if (kbs.IsKeyDown(Keys.Z))
            {
                player.Jump();
            }

            player.Update();

            // TODO: Add your update logic here			
            base.Update(gameTime);
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
            player.Draw();
            Globals.Map.Draw();
            Globals.Batch.End();
        }
    }
}

