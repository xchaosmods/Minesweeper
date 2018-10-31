using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Minesweeper : Form
    {
        internal static Game Game = new Game();

        public Minesweeper()
        {
            InitializeComponent ();
            Game.GameStateChanged += OnGameStateChanged;
            Game.FaceChanged += OnFaceChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.Width = Game.ButtonWidth * Game.ButtonsPerRow;
            flowLayoutPanel1.Height = Game.ButtonHeight * Game.ButtonsPerColumn;
            btnNewGame.Location = new Point((Width / 2) - (btnNewGame.Width / 2) - 7, btnNewGame.Location.Y);
            btnNewGame_Click(sender, e);
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            Game.NewGame ();
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.AddRange (Game.Tiles.ToArray<Control> ());
            btnNewGame.Image = Properties.Resources.Smiley;
            timer1.Enabled = !Properties.Settings.Default.UnlimitedTime;

            //FOR TESTING
            //===========
            //foreach(var tile in Game.Tiles)
            //    tile.ShowValue();
        }
        
        internal void OnGameStateChanged(GameStateChangedEventArgs e)
        {
            switch (e.GameState)
            {
                case GameStates.LOST:
                {
                    timer1.Enabled = false;
                    Console.WriteLine ("Game Over");
                    btnNewGame.Image = Properties.Resources.NotSmiley;
                    foreach (var tile in Game.Tiles)
                        tile.ShowValue ();
                    MessageBox.Show ("You lost!");
                    break;
                }
                case GameStates.WON:
                {
                    timer1.Enabled = false;
                    btnNewGame.Image = Properties.Resources.VerySmiley;
                    Console.WriteLine ("Won the game");
                    MessageBox.Show ("You won!");
                    //Add time to highscore
                    break;
                }
                case GameStates.PLAYING:
                {
                    btnNewGame.Image = Properties.Resources.Smiley;
                    break;
                }
            }
        }

        internal void OnFaceChanged(FaceChangedEventArgs e)
        {
            switch (e.FaceState)
            {
                case FaceStates.HAPPY:
                    btnNewGame.Image = Properties.Resources.Smiley;
                    break;
                case FaceStates.SAD:
                    btnNewGame.Image = Properties.Resources.NotSmiley;
                    break;
                case FaceStates.WORRIED:
                    btnNewGame.Image = Properties.Resources.Duckface;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Game.Time++;
            if (Game.Time == 1000)
                OnGameStateChanged(new GameStateChangedEventArgs(GameStates.LOST));
            lblTime.Text = Game.Time.ToString("D3");
        }
    }
}
