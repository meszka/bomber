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
    public abstract class Tile : Sprite
    {
        protected Boolean solid;
        protected Boolean destroyable;

        public Boolean Solid { get { return solid; } }
        public Boolean Destroyable { get { return destroyable; } }
        public virtual void Die()
        {
        }
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

        public override void Die()
        {
            Dead = true;
        }

        public override void AfterDeath()
        {
            double r = Globals.random.NextDouble();
            //Console.WriteLine(r);
            if (r < 0.2)
            {
                Powerup.SpawnRandom(new Rectangle(Box.X + 1, Box.Y + 1, 13, 13));
            }
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
}
