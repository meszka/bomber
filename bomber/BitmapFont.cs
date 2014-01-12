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
    public class BitmapFont
    {
        private Texture2D texture;
        private int width;
        private int height;

        public BitmapFont(Texture2D texture, int width, int height)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;
        }

        public void DrawString(string s, int x, int y, Color color, int scale = 1)
        {
            s = s.ToUpper();
            int destinationOffset = 0;
            foreach (char c in s)
            {
                int sourceOffset = (int)(c - ' ') * width;
                Globals.Batch.Draw(texture,
                                   new Rectangle(x + destinationOffset, y, width * scale, height * scale),
                                   new Rectangle(sourceOffset, 0, width, height),
                                   color);
                destinationOffset += width * scale;
            }
        }
    }
}

