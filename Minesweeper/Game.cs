using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public class Game
    {
        public List<Tile> Tiles { get; set; }
        internal static int ButtonWidth = 25;
        internal static int ButtonHeight = 25;
        internal static int ButtonsPerRow = Properties.Settings.Default.GridSizeX;
        internal static int ButtonsPerColumn = Properties.Settings.Default.GridSizeY;
        private int BombPercentage = Properties.Settings.Default.BombPercentage;
        internal static int Time { get; set; }

        public delegate void GameStateChangedDelegate(GameStateChangedEventArgs e);

        public event GameStateChangedDelegate GameStateChanged;

        public Game()
        {

        }

        public void OnGameStateChanged(GameStateChangedEventArgs e)
        {
            GameStateChanged?.Invoke (e);
        }

        public delegate void FaceChangedDelegate(FaceChangedEventArgs e);

        public event FaceChangedDelegate FaceChanged;
        
        public void OnFaceChanged(FaceChangedEventArgs e)
        {
            FaceChanged?.Invoke (e);
        }

        public void NewGame()
        {
            Time = 0;
            OnGameStateChanged (new GameStateChangedEventArgs(GameStates.PLAYING));

            Tiles = new List<Tile> (ButtonsPerRow * ButtonsPerColumn);
            var rand = new Random ();

            for (var i = 0; i < ButtonsPerRow; i++)
            {
                for (var j = 0; j < ButtonsPerColumn; j++)
                {
                    var isBomb = rand.Next(0, 101);
                    Tiles.Add(new Tile
                    {
                        Margin = new Padding(0, 0, 0, 0),
                        Size = new Size(ButtonWidth, ButtonHeight),
                        X = i,
                        Y = j,
                        IsBomb = isBomb <= BombPercentage,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        TileState = TileStates.None,
                        BackColor = Color.DarkGray,
                    });
                }
            }

            foreach (var tile in Tiles)
            {
                if (!tile.IsBomb)
                    continue;

                foreach (var surroundingTile in tile.GetSurroundingTiles ())
                {
                    if (surroundingTile != null && !surroundingTile.IsBomb)
                        surroundingTile.SurroundingBombs++;
                }
            }
        }
    }

    public class GameStateChangedEventArgs : EventArgs
    {
        public GameStates GameState { get; }

        public GameStateChangedEventArgs(GameStates gameState) { GameState = gameState; }
    }

    public class FaceChangedEventArgs : EventArgs
    {
        public FaceStates FaceState { get; }

        public FaceChangedEventArgs(FaceStates faceState) { FaceState = faceState; }
    }

    public enum GameStates
    {
        PLAYING,
        WON,
        LOST,
    }

    public enum FaceStates
    {
        HAPPY,
        WORRIED,
        SAD
    }
}
