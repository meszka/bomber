#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

#endregion

namespace bomber
{
    public interface GameState
    {
        void Update(GameTime gameTime);
        void Draw();
    }

    public class Menu : GameState
    {
        private List<string> maps;
        private int pointsToWin = 5;
        private const int maxPoints = 9;
        private const int minPoints = 1;
        private int cursor = 0;
        private KeyboardState oldKbs;

        public Menu()
        {
            maps = new List<string>();
            DirectoryInfo d = new DirectoryInfo("Content/Maps");
            FileInfo[] files = d.GetFiles("*.txt");
            foreach (FileInfo f in files)
            {
                maps.Add(f.Name.Split('.')[0]);
            }
        }

        public void Update(GameTime gameTime)
        {
            int pointMod = maxPoints - minPoints + 1;

            KeyboardState kbs = Keyboard.GetState();
            if (oldKbs.IsKeyDown(Keys.Right) && kbs.IsKeyUp(Keys.Right))
            {
                cursor = (cursor + 1) % maps.Count;
            }
            if (oldKbs.IsKeyDown(Keys.Left) && kbs.IsKeyUp(Keys.Left))
            {
                cursor = (cursor + maps.Count - 1) % maps.Count;
            }
            if (oldKbs.IsKeyDown(Keys.Up) && kbs.IsKeyUp(Keys.Up))
            {
                pointsToWin = (pointsToWin - minPoints + 1) % pointMod + minPoints;
            }
            if (oldKbs.IsKeyDown(Keys.Down) && kbs.IsKeyUp(Keys.Down))
            {
                pointsToWin = (pointsToWin - minPoints + pointMod - 1) % pointMod + minPoints;
            }
            if (oldKbs.IsKeyDown(Keys.Enter) && kbs.IsKeyUp(Keys.Enter))
            {
                Globals.SetState(new MainGame(maps[cursor], pointsToWin));
            }
            oldKbs = kbs;
        }

        public void Draw()
        {
            Globals.Font.DrawString("map: < " + maps[cursor] + " >", 10, 60, Color.White, 2);
            Globals.Font.DrawString("pts to win: \\ " + pointsToWin.ToString() + " ^", 10, 100, Color.White, 2);
            Globals.Font.DrawString("enter to start", 10, 140, Color.White, 2);
        }
    }

    public class MainGame : GameState
    {
        private Player player;
        private Player player2;
        private List<Player> playerList = new List<Player>();
        private int[] score = new int[4] {0, 0, 0, 0};
        private bool scored = false;
        private int scoreDelay = 0;
        private Player winner = null;
        private int pointsToWin = 3;
        private string mapName;

        public MainGame(string map, int pointsToWin)
        {
            mapName = map;
            this.pointsToWin = pointsToWin;
            Initialize();
        }

        public void Initialize()
        {
            winner = null;
            scored = false;

            Globals.Map = new TileMap(20, 15);

            string[] lines = System.IO.File.ReadAllLines(string.Format("Content/Maps/{0}.txt", mapName));
            int[,] mapdata = new int[15, 20];

            Dictionary<char, int> char2code = new Dictionary<char, int> {{' ', 0}, {'#', 1}, {'%', 2}, {'0', 3}, {'1', 4}};

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    mapdata[i, j] = char2code[lines[i][j]];
                }
            }

            Globals.Map.LoadMap(mapdata);
            /*
            Globals.Map.LoadMap(new int[,] {
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2},
                {0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 1},
                {1, 2, 1, 0, 1, 0, 0, 0, 0, 2, 0, 2, 2, 0, 2, 2, 2, 0, 2, 1},
                {1, 2, 0, 1, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 2, 2, 2, 2, 2, 1},
                {1, 1, 0, 0, 0, 1, 0, 2, 2, 0, 2, 2, 2, 0, 2, 2, 2, 2, 2, 1},
                {1, 0, 0, 1, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1},
            });
            */

            Dictionary<string, Keys> playerControls = new Dictionary<string, Keys> {
                {"left", Keys.Left},
                {"right", Keys.Right},
                {"jump", Keys.Up},
                {"bomb", Keys.Down},
            };

            player = new Player(0, Globals.Content.Load<Texture2D>("Textures/player_small.png"), Globals.Map.SpawnPoints[0], playerControls, "red", Color.Maroon, Color.PeachPuff);
            playerList.Add(player);

            Dictionary<string, Keys> player2Controls = new Dictionary<string, Keys> {
                {"left", Keys.A},
                {"right", Keys.D},
                {"jump", Keys.W},
                {"bomb", Keys.S},
            };

            player2 = new Player(1, Globals.Content.Load<Texture2D>("Textures/player_small.png"), Globals.Map.SpawnPoints[1], player2Controls, "blue", Color.Navy, Color.LightBlue);
            playerList.Add(player2);
        }

        public void Update(GameTime gameTime)
        {
            if (scored)
            {
                Globals.HandleTimer(ref scoreDelay, gameTime, Restart);
            }

            Sprite.UpdateAll(gameTime);
            Globals.Map.Update();
            Explosion.CleanUp();
            Bomb.CleanUp();
            Powerup.CleanUp();

            playerList.RemoveAll(p => p.Dead);
            if (playerList.Count == 1 && !scored)
            {
                winner = playerList[0];
                score[winner.Id] += 1;
                scoreDelay = 2000;
                scored = true;
            }

            if (playerList.Count == 0 && !scored)
            {
                scoreDelay = 2000;
                scored = true;
            }
        }

        private void Restart()
        {
            Sprite.SpriteList.Clear();
            Explosion.ExplosionList.Clear();
            playerList.Clear();
            if (winner != null && score[winner.Id] == pointsToWin)
            {
                Globals.SetState(new WinnerState(winner));
                return;
            }
            Initialize();
        }

        public void Draw()
        {
            Sprite.DrawAll();

            Globals.Font.DrawString(score[0].ToString(), 20, 10, Color.Maroon, 2);
            Globals.Font.DrawString(score[1].ToString(), 280, 10, Color.Navy, 2);

            if (scored)
            {
                if (winner != null)
                {
                    Globals.Font.DrawString(string.Format("{0} player scores!", winner.ColorName.PadLeft(4)), 12, 100, winner.TextColor, 2);
                }
                else
                {
                    Globals.Font.DrawString("   It's a draw...", 12, 100, Color.White, 2);
                }
            }
        }
    }

    public class WinnerState: GameState
    {
        private Player winner;

        public WinnerState(Player winner)
        {
            this.winner = winner;
        }

        public void Update(GameTime gameTime)
        {   
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Space))
            {
                Globals.SetState(new Menu());
            }
        }

        public void Draw()
        {
            //Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            Globals.Font.DrawString(string.Format("{0} player wins!", winner.ColorName.PadLeft(4)), 12, 80, winner.TextColor, 2);
            Globals.Font.DrawString(string.Format("Press space", winner.ColorName.PadLeft(4)), 12, 100, winner.TextColor, 2);
            Globals.Font.DrawString(string.Format("to play again", winner.ColorName.PadLeft(4)), 12, 120, winner.TextColor, 2);
        }
    }
}

