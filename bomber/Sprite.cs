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
    public class Sprite
    {
        public static List<Sprite> spriteList = new List<Sprite>();

        public Rectangle Box;
        public int Direction = 1;
        public bool Dead = false;

        protected Texture2D texture;

        public Sprite(Texture2D texture, Rectangle box)
        {
            this.texture = texture;
            this.Box = box;
            spriteList.Add(this);
        }

        public Sprite()
        {
            spriteList.Add(this);
        }

        public void Draw()
        {
            SpriteEffects effect = Direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Globals.Batch.Draw(texture, Box, null, Color.White, 0.0f, new Vector2(0, 0), effect, 0.0f);
            Rectangle BoxWrapX = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
            Rectangle BoxWrapY = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
            BoxWrapX.X -= Globals.Width;
            BoxWrapY.Y -= Globals.Height;
            Globals.Batch.Draw(texture, BoxWrapX, null, Color.White, 0.0f, new Vector2(0, 0), effect, 0.0f);
            Globals.Batch.Draw(texture, BoxWrapY, null, Color.White, 0.0f, new Vector2(0, 0), effect, 0.0f);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void AfterDeath()
        {
        }

        public static void DrawAll()
        {
            spriteList.ForEach(s => s.Draw());
        }

        public static void UpdateAll(GameTime gameTime)
        {
            spriteList.ForEach(s => s.Update(gameTime));
            List<Sprite> deadSprites = spriteList.Where(s => s.Dead).ToList();
            deadSprites.ForEach(s => s.AfterDeath());
            spriteList.RemoveAll(s => s.Dead);
        }
    }
}
