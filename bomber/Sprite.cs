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
        public Rectangle Box;
        public int Direction = 1;

        protected Texture2D texture;

        public Sprite(Texture2D texture, Rectangle box)
        {
            this.texture = texture;
            this.Box = box;
        }

        public Sprite()
        {
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
    }
}
