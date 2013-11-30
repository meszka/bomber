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
        private float throwVelocity = 5.0f;
        private int time;
        public bool Dead;

        public Bomb(Texture2D texture, Rectangle box) : base(texture, box)
        {
            time = 3000;
        }

        public void Update(GameTime gameTime)
        {
            if (Dead)
                return;

            time -= gameTime.ElapsedGameTime.Milliseconds;
            if (time < 0)
                time = 0;
            if (time == 0)
                Dead = true;

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
                }
                else
                {
                    Box.Y = ((Box.Y / Globals.TileHeight) + 1) * Globals.TileHeight;
                }
                Box.Y = Globals.WrappedY(Box.Y);
                vy = 0;
                vx = 0;
            }

            Box.X += (int)vx;
            Box.X = Globals.WrappedX(Box.X);

            if (Globals.Map.Collide(this))
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
                vx = 0;
            }
        }

        public void Throw(float throwPower, int direction)
        {
            vx = throwVelocity * throwPower * direction;
            vy = -throwVelocity * throwPower;
        }
    }
}

