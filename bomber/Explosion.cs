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
    public class Explosion : Sprite
    {
        private int time;

        public static List<Explosion> ExplosionList = new List<Explosion>();

        public Explosion(Rectangle box) :
            base (Globals.Content.Load<Texture2D>("Textures/explosion.png"), box)
        {
            time = 500;
            ExplosionList.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
                return;

            Globals.HandleTimer(ref time, gameTime, Die);

            List<Tile> tiles = Globals.Map.Collide(this);
            foreach (Tile t in tiles.Where(t => t.Destroyable))
            {
                t.Die();
            }
        }

        public void Die()
        {
            Dead = true;
        }

        public static void CleanUp()
        {
            ExplosionList.RemoveAll(s => s.Dead);
        }
    }
}

