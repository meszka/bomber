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
    public class Player : Sprite
    {
        private float vy = 0.0f;
        private float vx = 1.0f;
        //private float maxV = 16.0f;
        private float maxV = 10.0f;
        private float gravity = 0.3f;
        private float jumpVelocity = 5.0f;
        private Boolean jumping = false;

        private int bombCooldown = 0;
        private int bombHold = 0;
        private int bombHoldMax = 500;

        private BombTypes bombType = BombTypes.Bomb;

        public int Id;

        private Dictionary<string, Keys> controls;

        public Player(int id, Texture2D texture, Rectangle box, Dictionary<string, Keys> controls, Color? color = null) : base(texture, box, color)
        {
            this.Id = id;
            this.controls = controls;
        }

        public void WalkLeft()
        {
            Direction = -1;
            Box.X -= (int) vx;
            if (Globals.Map.Collide(this).Any())
            {
                Box.X = ((Globals.WrappedX(Box.X) / Globals.TileWidth) + 1) * Globals.TileWidth;
            }
            Box.X = Globals.WrappedX(Box.X);
        }

        public void WalkRight()
        {
            Direction = 1;
            Box.X += (int)vx;
            if (Globals.Map.Collide(this).Any())
            {
                Box.X = (Box.X / Globals.TileWidth) * Globals.TileWidth + (Globals.TileWidth - Box.Width);
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

        public int PushBombs()
        {
            int pushed = 0;
            foreach (Bomb b in Bomb.BombList)
            {
                Rectangle intersection = Rectangle.Intersect(Box, b.Box);
                //int sign = intersection.Location.X > Box.X ? 1 : -1;
                if (intersection.Width > 0)
                {
                    b.Push(0.4f, Direction);
                    pushed += 1;
                }
            }
            return pushed;
        }

        public override void Update(GameTime gameTime)
        {
            Box.Y = Globals.WrappedY(Box.Y + (int)vy);
            vy += gravity;

            if (vy > maxV)
            {
                vy = maxV;
            }

            if (Globals.Map.Collide(this).Any())
            {
                if (vy > 0)
                {
                    Box.Y = (Box.Y / Globals.TileHeight) * Globals.TileHeight + (Globals.TileHeight - Box.Height);
                    jumping = false;
                }
                else
                {
                    Box.Y = ((Box.Y / Globals.TileHeight) + 1) * Globals.TileHeight;
                }
                Box.Y = Globals.WrappedY(Box.Y);
                vy = 0;
            }

            foreach (Explosion e in Explosion.ExplosionList)
            {
                if (Collides(e))
                {
                    Dead = true;
                }
            }

            bombCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (bombCooldown < 0)
                bombCooldown = 0;
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(controls["right"]))
            {
                WalkRight();
            }
            if (kbs.IsKeyDown(controls["left"]))
            {
                WalkLeft();
            }
            if (kbs.IsKeyDown(controls["jump"]))
            {
                Jump();
            }
            if (kbs.IsKeyDown(Keys.Q))
            {
                bombType = (BombTypes)(((int)bombType + 1) % Enum.GetValues(typeof(BombTypes)).GetLength(0));
            }
            if (kbs.IsKeyDown(controls["bomb"]) && bombCooldown == 0)
            {
                bombHold += gameTime.ElapsedGameTime.Milliseconds;
                if (bombHold > bombHoldMax)
                    bombHold = bombHoldMax;
                //Console.WriteLine(bombHold);
            }
            if (kbs.IsKeyUp(controls["bomb"]) && bombHold > 0)
            {
                if (PushBombs() == 0)
                {
                    Bomb b;
                    float throwPower = (float)bombHold / (float)bombHoldMax;
                    if (bombType == BombTypes.Bomb)
                    {
                        b = new Bomb(Box);
                        b.Throw(throwPower, Direction);
                    }
                    else if (bombType == BombTypes.FloatingBomb)
                    {
                       b = new FloatingBomb(Box);
                       b.Throw(throwPower, Direction);
                    }
                    else if (bombType == BombTypes.StickyBomb)
                    {
                        b = new StickyBomb(Box);
                        b.Throw(throwPower, Direction);
                    }
                }
                //Console.WriteLine(throwPower);
                bombCooldown = 200;
                bombHold = 0;
            }
        }
    }

}
