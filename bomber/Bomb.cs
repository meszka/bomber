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

        public Bomb(Rectangle box) :
            base(Globals.Content.Load<Texture2D>("Textures/bomb.png"), box)
        {
            time = 1500;
        }

        protected void handleDeath(GameTime gameTime)
        {
            if (Dead)
                return;

            time -= gameTime.ElapsedGameTime.Milliseconds;
            if (time < 0)
                time = 0;
            if (time == 0)
                Die();
        }

        protected void handleYCollision()
        {
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
            new Explosion(new Rectangle(Box.Center.X - 16, Box.Center.Y - 16, 32, 32));
        }

    }

    public class StickyBomb : Bomb
    {
        private bool stuck = false;

        public StickyBomb(Rectangle box) : base(box)
        {
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

        public FloatingBomb(Rectangle box) : base(box)
        {
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

