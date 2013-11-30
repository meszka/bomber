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
}
