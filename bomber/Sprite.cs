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
        public static List<Sprite> SpriteList = new List<Sprite>();

        public Rectangle Box;
        public int Direction = 1;
        public bool Dead = false;

        protected Texture2D texture;
        protected Color color;

        public Sprite(Texture2D texture, Rectangle box, Color? maybeColor = null)
        {
            this.texture = texture;
            this.Box = box;
            if (maybeColor.HasValue)
            {
                this.color = maybeColor.Value;
            }
            else
            {
                this.color = Color.White;
            }
            SpriteList.Add(this);
        }

        public Sprite()
        {
            this.color = Color.White;
            SpriteList.Add(this);
        }

        public void Draw()
        {
            SpriteEffects effect = Direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Globals.Batch.Draw(texture, Box, null, color, 0.0f, new Vector2(0, 0), effect, 0.0f);
            Rectangle BoxWrapX = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
            Rectangle BoxWrapY = new Rectangle(Box.X, Box.Y, Box.Width, Box.Height);
            BoxWrapX.X -= Globals.Width;
            BoxWrapY.Y -= Globals.Height;
            Globals.Batch.Draw(texture, BoxWrapX, null, color, 0.0f, new Vector2(0, 0), effect, 0.0f);
            Globals.Batch.Draw(texture, BoxWrapY, null, color, 0.0f, new Vector2(0, 0), effect, 0.0f);
        }

        public bool Collides(Sprite that)
        {
            return Box.Intersects(that.Box);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void AfterDeath()
        {
        }

        public static void DrawAll()
        {
            SpriteList.ForEach(s => s.Draw());
        }

        public static void UpdateAll(GameTime gameTime)
        {
            SpriteList.ForEach(s => s.Update(gameTime));
            List<Sprite> deadSprites = SpriteList.Where(s => s.Dead).ToList();
            deadSprites.ForEach(s => s.AfterDeath());
            SpriteList.RemoveAll(s => s.Dead);
        }
    }
}
