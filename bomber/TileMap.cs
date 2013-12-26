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

        public int Width;
        public int Height;

        public TileMap(int width, int height)
        {
            Tiles = new Tile[width, height];
            Width = width;
            Height = height;
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

        public List<Tile> Collide(Sprite sprite)
        {
            int top = Globals.WrappedY(sprite.Box.Top) / Globals.TileHeight;
            int bottom = Globals.WrappedY(sprite.Box.Bottom - 1) / Globals.TileHeight;
            int left = Globals.WrappedX(sprite.Box.Left) / Globals.TileWidth;
            int right = Globals.WrappedX(sprite.Box.Right - 1) / Globals.TileWidth;

            List<Tile> tilesToCheck = new List<Tile>();
            for (int i = left; i != (right + 1) % Width; i = (i + 1) % Width)
            {
                for (int j = top; j != (bottom + 1) % Height; j = (j + 1) % Height)
                {
                    if (Tiles[i, j] != null && Tiles[i, j].Solid)
                    {
                        tilesToCheck.Add(Tiles[i, j]);
                    }
                }
            }
            return tilesToCheck;
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

        public void Update()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Tiles[i, j] != null)
                    {
                        if (Tiles[i, j].Dead)
                        {
                            Tiles[i, j] = null;
                        }
                    }
                }
            }
        }
    }
}

