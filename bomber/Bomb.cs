#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace bomber
{
    public enum BombTypes {Bomb, FloatingBomb, StickyBomb};

    public class Bomb : Sprite
    {
        protected float vy = 0.0f;
        protected float vx = 0.0f;
        protected float maxV = 10.0f;
        protected float gravity = 0.3f;
        protected float throwVelocity = 8.0f;
        protected int time;
        protected float restitution = 0.5f;
        protected float friction = 0.7f;

        protected SoundEffect bounceSound;
        protected int explosionSize = 16;

        public static List<Bomb> BombList = new List<Bomb>();

        public Bomb(Rectangle box, int explosionSize) :
            base(Globals.Content.Load<Texture2D>("Textures/bomb.png"), box)
        {
            this.explosionSize = explosionSize;
            time = 1500;
            bounceSound = Globals.Content.Load<SoundEffect>("Sounds/bounce.wav");
            BombList.Add(this);
        }

        protected void handleDeath(GameTime gameTime)
        {
            if (Dead)
                return;

            Globals.HandleTimer(ref time, gameTime, Die);
        }

        protected void handleYCollision()
        {
            if (Math.Abs(vy) > 2)
            {
                SoundEffectInstance b = bounceSound.CreateInstance();
                b.Volume = Math.Abs(vy) / maxV;
                b.Play();
            }

            if (vy > 0)
            {
                Box.Y = (Box.Y / Globals.TileHeight) * Globals.TileHeight + (Globals.TileHeight - Box.Height);
            }
            else
            {
                Box.Y = ((Box.Y / Globals.TileHeight) + 1) * Globals.TileHeight;
            }
            Box.Y = Globals.WrappedY(Box.Y);
            vy = -vy * restitution;
            vx = vx * friction;
            //vx = vx * restitution;
        }

        protected void handleXCollision()
        {
            if (Math.Abs(vx) > 2)
            {
                SoundEffectInstance b = bounceSound.CreateInstance();
                b.Volume = Math.Abs(vx) / (maxV / 2);
                b.Play();
            }

            if (vx > 0)
            {
                Box.X = (Box.X / Globals.TileWidth) * Globals.TileWidth + (Globals.TileWidth - Box.Width);
            }
            else
            {
                Box.X = ((Globals.WrappedX(Box.X) / Globals.TileWidth) + 1) * Globals.TileWidth;
            }
            Box.X = Globals.WrappedX(Box.X);
            vx = -vx * restitution;
            vy = vy * friction;
            //vy = vy * restitution;
        }

        protected void handleGravity()
        {
            vy += gravity;
            if (vy > maxV)
            {
                vy = maxV;
            }
        }

        protected void moveX()
        {
            Box.X = Globals.WrappedX(Box.X + (int)vx);
        }

        protected void moveY()
        {
            Box.Y = Globals.WrappedY(Box.Y + (int)vy);
        }

        public override void Update(GameTime gameTime)
        {
            handleDeath(gameTime);

            handleGravity();
            moveY();
            if (Globals.Map.Collide(this).Any())
            {
                handleYCollision();
            }

            moveX();
            if (Globals.Map.Collide(this).Any())
            {
                handleXCollision();
            }
        }

        /*
        public int Push(int dx)
        {
            int oldX = Box.X;
            Box.X += dx;

            if (Globals.Map.Collide(this).Any())
            {
                if (dx < 0)
                {
                    Box.X = ((Globals.WrappedX(Box.X) / Globals.TileWidth) + 1) * Globals.TileWidth;
                }
                else
                {
                    Box.X = (Box.X / Globals.TileWidth) * Globals.TileWidth + (Globals.TileWidth - Box.Width);
                }
            }
            Box.X = Globals.WrappedX(Box.X);
            int diff = Globals.WrappedX(Box.X - oldX);
            if (diff > Globals.Width / 2)
            {
                return diff - Globals.Width;
            }
            else
            {
                return diff;
            }
        }
        */

        public void Push(float pushPower, int direction)
        {
            float angle = 30;
            vx = throwVelocity * pushPower * (float)Math.Cos(Math.PI/180*angle) * direction;
            vy = -throwVelocity * pushPower * (float)Math.Sin(Math.PI/180*angle);
        }

        public void Throw(float throwPower, int direction)
        {
            float angle = 55;
            vx = throwVelocity * throwPower * (float)Math.Cos(Math.PI/180*angle) * direction;
            vy = -throwVelocity * throwPower * (float)Math.Sin(Math.PI/180*angle);
        }

        public void Die()
        {
            Dead = true;
        }

        public override void AfterDeath()
        {
            //new Explosion(new Rectangle(Box.Center.X - 16, Box.Center.Y - 16, 32, 32));
            new Explosion(new Rectangle(Box.Center.X - (explosionSize / 2), Box.Center.Y - (explosionSize / 2), explosionSize, explosionSize));
        }

        public static void CleanUp()
        {
            BombList.RemoveAll(b => b.Dead);
        }

    }

    public class StickyBomb : Bomb
    {
        private bool stuck = false;

        public StickyBomb(Rectangle box, int explosionSize) : base(box, explosionSize)
        {
            this.texture = Globals.Content.Load<Texture2D>("Textures/sticky_bomb.png");
        }

        public override void Update(GameTime gameTime)
        {
            handleDeath(gameTime);

            if (stuck)
                return;

            handleGravity();
            moveY();
            if (Globals.Map.Collide(this).Any())
            {
                handleYCollision();
                stuck = true;
            }

            moveX();
            if (Globals.Map.Collide(this).Any())
            {
                handleXCollision();
                stuck = true;
            }
        }
    }

    public class FloatingBomb : Bomb
    {

        public FloatingBomb(Rectangle box, int explosionSize) : base(box, explosionSize)
        {
            this.texture = Globals.Content.Load<Texture2D>("Textures/floating_bomb.png");
            this.gravity = 0.2f;
        }

        protected void handleGravityFloaty()
        {
            vy += gravity;
            if (vy > 0)
            {
                vx = 0;
                vy = 0;
            }
        }

        public override void Update(GameTime gameTime)
        {
            handleDeath(gameTime);

            handleGravityFloaty();
            moveY();
            if (Globals.Map.Collide(this).Any())
            {
                handleYCollision();
            }

            moveX();
            if (Globals.Map.Collide(this).Any())
            {
                handleXCollision();
            }
        }
    }
}

