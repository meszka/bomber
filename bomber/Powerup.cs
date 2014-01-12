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
    public abstract class Powerup : Sprite
    {
        protected float vy = 0;
        protected float maxV = 10.0f;
        protected float gravity = 0.3f;

        public static List<Powerup> PowerupList = new List<Powerup>();

        public Powerup()
        {
            PowerupList.Add(this);
        }

        public virtual void Effect(Player p)
        {
        }

        protected void handleGravity()
        {
            vy += gravity;
            if (vy > maxV)
            {
                vy = maxV;
            }
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
            vy = 0;
        }

        protected void moveY()
        {
            Box.Y = Globals.WrappedY(Box.Y + (int)vy);
        }

        public override void Update(GameTime gameTime)
        {
            handleGravity();
            moveY();
            if (Globals.Map.Collide(this).Any())
            {
                handleYCollision();
            }
        }
        
        public static void CleanUp()
        {
            PowerupList.RemoveAll(p => p.Dead);
        }

        public static  void SpawnRandom(Rectangle box)
        {
            int type = Globals.random.Next(2);

            if (type == 0)
            {
                new StickyPowerup(box);
            }
            else if (type == 1)
            {
                new FloatingPowerup(box);
            }
        }
    }

    public class StickyPowerup : Powerup
    {
        public StickyPowerup(Rectangle box): base()
        {
            this.Box = box;
            this.texture = Globals.Content.Load<Texture2D>("Textures/sticky_powerup.png");
        }

        public override void Effect(Player p)
        {
            p.BombType = BombTypes.StickyBomb;
        }
    }

    public class FloatingPowerup : Powerup
    {
        public FloatingPowerup(Rectangle box): base()
        {
            this.Box = box;
            this.texture = Globals.Content.Load<Texture2D>("Textures/floating_powerup.png");
        }

        public override void Effect(Player p)
        {
            p.BombType = BombTypes.FloatingBomb;
        }
    }
}

