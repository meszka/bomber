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

        public Player(Texture2D texture, Rectangle box) : base(texture, box)
        {
        }

        public void WalkLeft()
        {
            Direction = -1;
            Box.X -= (int) vx;
            if (Globals.Map.Collide(this).Any())
            {
                Box.X = ((Box.X / Globals.TileWidth) + 1) * Globals.TileWidth;
            }
            Box.X = Globals.WrappedX(Box.X);
        }

        public void WalkRight()
        {
            Direction = 1;
            Box.X += (int)vx;
            if (Globals.Map.Collide(this).Any())
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

        public override void Update(GameTime gameTime)
        {
            Box.Y += (int)vy;
            Box.Y = Globals.WrappedY(Box.Y);
            vy += gravity;

            if (vy > maxV)
            {
                vy = maxV;
            }

            if (Globals.Map.Collide(this).Any())
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

}
