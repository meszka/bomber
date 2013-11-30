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
    public class TileMap
    {
        public Tile[,] Tiles { get; set; }

        public TileMap(int width, int height)
        {
            Tiles = new Tile[width, height];
        }

        public void LoadMap(int [,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Rectangle rect = new Rectangle(j * Globals.TileWidth,
                                                   i * Globals.TileHeight,
                                                   Globals.TileWidth,
                                                   Globals.TileHeight);
                    if (map[i, j] == 1)
                    {
                        Tiles[j, i] = new Metal(rect);
                    }
                    else if (map[i, j] == 2)
                    {
                        Tiles[j, i] = new Brick(rect);
                    }
                }
            }
        }

        public Boolean Collide(Sprite sprite)
        {
            int top = Globals.WrappedY(sprite.Box.Top) / Globals.TileHeight;
            int bottom = Globals.WrappedY(sprite.Box.Bottom - 1) / Globals.TileHeight;
            int left = Globals.WrappedX(sprite.Box.Left) / Globals.TileWidth;
            int right = Globals.WrappedX(sprite.Box.Right - 1) / Globals.TileWidth;

            Tile[] tilesToCheck = new Tile[] { Tiles[left, top], Tiles[right, top], Tiles[left, bottom], Tiles[right, bottom] };
            return tilesToCheck.Where(t => t != null).Any(t => t.Solid);
        }

        public void Draw()
        {
            foreach (Tile t in Tiles)
            {
                if (t != null)
                {
                    t.Draw();
                }
            }
        }
    }
}

