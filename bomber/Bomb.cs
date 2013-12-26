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
        private float vy = 0.0f;
        private float vx = 0.0f;
        private float maxV = 10.0f;
        private float gravity = 0.3f;
        private float throwVelocity = 8.0f;
        private int time;
        private float restitution = 0.5f;
        private float friction = 0.7f;

        public Bomb(Texture2D texture, Rectangle box) : base(texture, box)
        {
            time = 1500;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
                return;

            time -= gameTime.ElapsedGameTime.Milliseconds;
            if (time < 0)
                time = 0;
            if (time == 0)
                Die();

            Box.Y += (int)vy;
            Box.Y = Globals.WrappedY(Box.Y);

            if (vy > maxV)
            {
                vy = maxV;
            }

            if (Globals.Map.Collide(this).Any())
            {
                if (vy > 0)
                {
                    Box.Y = (Box.Y / Globals.TileHeight) * Globals.TileHeight;
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
            else
            {
                vy += gravity;
            }

            Box.X += (int)vx;
            Box.X = Globals.WrappedX(Box.X);

            if (Globals.Map.Collide(this).Any())
            {
                if (vx > 0)
                {
                    Box.X = (Box.X / Globals.TileHeight) * Globals.TileHeight;
                }
                else
                {
                    Box.X = ((Box.X / Globals.TileHeight) + 1) * Globals.TileHeight;
                }
                Box.X = Globals.WrappedX(Box.X);
                vx = -vx * restitution;
                vy = vy * friction;
                //vy = vy * restitution;
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
}

