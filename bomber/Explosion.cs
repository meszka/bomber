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

        public Explosion(Rectangle box) :
            base (Globals.Content.Load<Texture2D>("Textures/explosion.png"), box)
        {
            time = 500;
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
        }

        public void Die()
        {
            Dead = true;
        }
    }
}

