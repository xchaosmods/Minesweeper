using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public enum TileStates
    {
        None,
        Flagged,
        Questioned
    }

    public class Tile : Button
    {
        public bool IsBomb { get; set; }

        public bool IsShown { get; set; }

        public TileStates TileState { get; set; }

        public int SurroundingBombs { get; set; }

        public int X;
        public int Y;
        
        public Tile()
        {
            MouseDown += OnMouseClick;
            MouseDown += OnFaceChanged(new FaceChangedEventArgs(FaceStates.WORRIED));
            MouseUp += OnFaceChanged (new FaceChangedEventArgs(FaceStates.HAPPY));
        }

        private MouseEventHandler OnFaceChanged(FaceChangedEventArgs e)
        {
            Minesweeper.Game.OnFaceChanged(e);
            return null;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                //Reveal
                case MouseButtons.Left when IsBomb && TileState != TileStates.Flagged:
                {
                    //GameOver
                    OnClick (e);
                    IsShown = true;
                    Image = Properties.Resources.Bomb;
                    BackgroundImageLayout = ImageLayout.Stretch;
                    Minesweeper.Game.OnGameStateChanged(new GameStateChangedEventArgs(GameStates.LOST));
                    break;
                }
                case MouseButtons.Left when TileState != TileStates.Flagged:
                {
                    //Show necessary value
                    OnClick (e);
                    GetValue ();
                    break;
                }
                case MouseButtons.Right:
                {
                    if(IsShown)
                        return;

                    //Cycle flags
                    switch (TileState)
                    {
                        case TileStates.Flagged:
                            {
                                TileState = TileStates.Questioned;
                                Image = null;
                                BackgroundImageLayout = ImageLayout.Stretch;
                                Text = "?";
                                break;
                            }
                        case TileStates.Questioned:
                            {
                                TileState = TileStates.None;
                                Image = null;
                                BackgroundImageLayout = ImageLayout.Stretch;
                                Text = "";
                                break;
                            }
                        case TileStates.None:
                            {
                                TileState = TileStates.Flagged;
                                Image = Properties.Resources.Flag;
                                BackgroundImageLayout = ImageLayout.Stretch;
                                Text = "";
                                break;
                            }
                    }
                    break;
                }
            }
        }

        private void GetValue()
        {
            ShowValue();

            //If zero - reveal all surrounding tiles
            if (SurroundingBombs == 0)
            {
                ShowValue();
                ShowSurroundingTiles ();
            }

            CheckIfGameWon();
        }

        private void CheckIfGameWon()
        {
            Tile tile = Minesweeper.Game.Tiles.Find(t => t.IsBomb == false && t.IsShown == false);
            if (tile == null)
                Minesweeper.Game.OnGameStateChanged (new GameStateChangedEventArgs (GameStates.WON));
        }

        internal void ShowValue()
        {
            if (IsBomb)
                Image = Properties.Resources.Bomb;
            else if (SurroundingBombs == 0)
            {
                Image = null;
                Text = "";
                BackColor = Color.LightGray;
            }
            else
            {
                Image = null;
                //Text = SurroundingBombs.ToString();

                switch(SurroundingBombs.ToString())
                {
                    case "1":
                    {
                        Image = Properties.Resources._1;
                        break;
                    }
                    case "2":
                    {
                        Image = Properties.Resources._2;
                        break;
                    }
                    case "3":
                    {
                        Image = Properties.Resources._3;
                        break;
                    }
                    case "4":
                    {
                        Image = Properties.Resources._4;
                        break;
                    }
                    case "5":
                    {
                        Image = Properties.Resources._5;
                        break;
                    }
                    case "6":
                    {
                        Image = Properties.Resources._6;
                        break;
                    }
                    case "7":
                    {
                        Image = Properties.Resources._7;
                        break;
                    }
                    case "8":
                    {
                        Image = Properties.Resources._8;
                        break;
                    }
                }
            }

            IsShown = true;
        }

        private void ShowSurroundingTiles()
        {
            foreach (var tile in GetSurroundingTiles())
                if (tile != null && !tile.IsShown)
                    tile.GetValue();
        }
        
        internal List<Tile> GetSurroundingTiles()
        {
            var tiles = Minesweeper.Game.Tiles;
            var index = tiles.IndexOf(this);

            var surroundingTiles = new List<Tile>();
            if(index % Game.ButtonsPerRow != 0)
            {
                surroundingTiles.Add(GetSurroundingTile(tiles, index + Game.ButtonsPerRow - 1));
                surroundingTiles.Add(GetSurroundingTile(tiles, index - Game.ButtonsPerRow - 1));
                surroundingTiles.Add(GetSurroundingTile (tiles, index - 1));
            }

            surroundingTiles.Add(GetSurroundingTile(tiles, index - Game.ButtonsPerRow));
            surroundingTiles.Add(GetSurroundingTile(tiles, index + Game.ButtonsPerRow));

            if(index % Game.ButtonsPerRow != Game.ButtonsPerRow - 1)
            {
                surroundingTiles.Add(GetSurroundingTile(tiles, index + Game.ButtonsPerRow + 1));
                surroundingTiles.Add(GetSurroundingTile(tiles, index - Game.ButtonsPerRow + 1));
                surroundingTiles.Add(GetSurroundingTile(tiles, index + 1));
            }
            return surroundingTiles;
        }

        private static Tile GetSurroundingTile(IReadOnlyList<Tile> tiles, int index)
        {
            return index < 0 || index > tiles.Count - 1 ? null : tiles[index];
        }
    }
    
    
}
