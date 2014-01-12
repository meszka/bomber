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
    public interface GameState
    {
        void Initialize();
        void Update(GameTime gameTime);
        void Draw();
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

        public MainGame()
        {
        }

        public void Initialize()
        {
            winner = null;
            scored = false;

            Dictionary<string, Keys> playerControls = new Dictionary<string, Keys> {
                {"left", Keys.Left},
                {"right", Keys.Right},
                {"jump", Keys.Up},
                {"bomb", Keys.Down},
            };

            player = new Player(0, Globals.Content.Load<Texture2D>("Textures/player_small.png"), new Rectangle(100, 5, 13, 13), playerControls, "red", Color.Red, Color.PeachPuff);
            playerList.Add(player);

            Dictionary<string, Keys> player2Controls = new Dictionary<string, Keys> {
                {"left", Keys.A},
                {"right", Keys.D},
                {"jump", Keys.W},
                {"bomb", Keys.S},
            };

            player2 = new Player(1, Globals.Content.Load<Texture2D>("Textures/player_small.png"), new Rectangle(200, 5, 13, 13), player2Controls, "blue", Color.Blue, Color.LightBlue);
            playerList.Add(player2);

            Globals.Map = new TileMap(20, 15);

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

            Globals.Font.DrawString(score[0].ToString(), 20, 10, Color.Red, 2);
            Globals.Font.DrawString(score[1].ToString(), 280, 10, Color.Blue, 2);

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

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {   
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Space))
            {
                Globals.SetState(new MainGame());
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

