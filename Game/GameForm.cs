using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    class GameForm : Form
    {
        public Dictionary<string, Bitmap> Bitmaps = new Dictionary<string, Bitmap>();
        private Game game;
        private bool isUpDown;
        private bool isDownDown;
        private bool isLeftDown;
        private bool isRightDown;
        private bool isShootDown;
        private Point cloudPos;
        private Button retryButton;
        private Button startButton;
        private Button exitButton;
        private string backgroungName = "background.jpg";
        private bool isMenu;
        private readonly Image gameArea;
        private int iteration;
        private SolidBrush backgroundColor;

        private readonly List<SolidBrush> backgroundColorsBrushes;
        private bool backgroundForward = true;
        private Queue<SolidBrush> backgroundColorsQueue;

        public GameForm(Game game)
        {
            var imagesDirectory = new DirectoryInfo("Images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                Bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
            foreach (var e in imagesDirectory.GetFiles("*.jpg"))
                Bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
            gameArea = new Bitmap(Bitmaps[backgroungName], Game.GameFeatures.FieldSize);
            isMenu = true;
            InitializeUI();
            backgroundColorsBrushes = FillSkyColors();
            backgroundColor = backgroundColorsBrushes[0];
            backgroundColorsQueue = new Queue<SolidBrush>(backgroundColorsBrushes);
            this.game = game;
            var timer = new Timer { Interval = 10 };

            timer.Tick += (sender, args) =>
            {
                //феечки спавнятся, там снаряды летят
                ChangeBackgroundColor(sender, args);
                UpdateGameState(sender, args);
                Invalidate();
            };
            timer.Start();
        }

        private List<SolidBrush> FillSkyColors()
        {
            var brushes = new List<SolidBrush>();
            var i = 0;
            var currentRed = 220; //30
            var currentGreen = 230;//20
            var currentBlue = 240;//10
            while (currentBlue >= 130)
            {
                brushes.Add(new SolidBrush(
                    Color.FromArgb(currentRed > 0 ? currentRed : 0, 
                        currentGreen > 0 ? currentGreen : 0, 
                        currentBlue > 0 ? currentBlue : 0)));
                currentRed--;
                if (i % 2 == 0)
                    currentGreen--;
                if (i % 3 == 0)
                    currentBlue--;
                i++;
            }

            return brushes;
            //backgroundColorsBrushes = brushes;

        }

        public void InitializeUI()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            AddButtons();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackgroundImage = Bitmaps[backgroungName];
            KeyPreview = true;
            cloudPos = new Point(0, -ClientSize.Height);
        }

        private void AddButtons()
        {
            retryButton = Buttons.RetryButton;
            retryButton.Click += (sender, args) =>
            {
                game = new Game(new Size(500, 600), new Point(250, 600), 5);
                Controls.Remove(retryButton);
                BackgroundImage = Bitmaps[backgroungName];
            };
            retryButton.MouseEnter += (sender, args) =>
            {
                (sender as Button).ForeColor = Buttons.PushColor;
            };
            retryButton.MouseLeave += (sender, args) =>
            {
                (sender as Button).ForeColor = Buttons.RestColor;
            };
            startButton = Buttons.StartButton;
            startButton.Click += (sender, args) =>
            {
                isMenu = false;
                game = new Game(new Size(500, 600), new Point(250, 600), 5);
                Controls.Remove(startButton);
                Controls.Remove(exitButton);
                BackgroundImage = Bitmaps[backgroungName];
            };
            startButton.MouseEnter += (sender, args) =>
            {
                (sender as Button).ForeColor = Buttons.PushColor;
            };
            startButton.MouseLeave += (sender, args) =>
            {
                (sender as Button).ForeColor = Buttons.RestColor;
            };
            exitButton = Buttons.ExitButton;
            exitButton.Click += (sender, args) =>
            {
                Close();
            };
            exitButton.MouseEnter += (sender, args) =>
            {
                (sender as Button).ForeColor = Buttons.PushColor;
            };
            exitButton.MouseLeave += (sender, args) =>
            {
                (sender as Button).ForeColor = Buttons.RestColor;
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //            Text = "феминизм победил";
            Text = "аттака геев-нигеров из далёкого космоса";
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (isMenu)
            {
                DrawMenu();
                return;
            }
            ClientSize = new Size(Game.GameFeatures.FieldSize.Width + 200, Game.GameFeatures.FieldSize.Height + 20);
            e.Graphics.FillRectangle(Brushes.AliceBlue, ClientRectangle);

            if (game.CurrentGameState.IsOver)
            {
                var back = Bitmaps["play-skip.png"];
                var back1 = Bitmaps["play-skip1.png"];
                var back2 = Bitmaps["play-skip1-rev.png"];
                e.Graphics.DrawImage(back, new Rectangle(new Point(-10, -4), back.Size));
                e.Graphics.DrawImage(back1, new Rectangle(new Point(-49, 300), back1.Size));
                e.Graphics.DrawImage(back2, new Rectangle(new Point(250, 300), back1.Size));
                if (!Controls.Contains(retryButton))
                    Controls.Add(retryButton);
                e.Graphics.DrawString($"Score: {game.Score.ToString()}", new Font("Arial", 16), Brushes.DarkBlue, 550, 20);
            }
            else
            {
                var g = Graphics.FromImage(gameArea);
                DrawTo(g);
                e.Graphics.DrawImage(gameArea, 10, 10);
                e.Graphics.DrawString($"Score: {game.Score.ToString()}", new Font("Arial", 16), Brushes.DarkBlue, 550, 20);
                e.Graphics.DrawString($"HP: {game.Player.HP.ToString()}", new Font("Arial", 16), Brushes.DarkBlue, 550, 40);
                e.Graphics.DrawString($"Power: {game.Player.Power.ToString()}", new Font("Arial", 16), Brushes.DarkBlue, 550, 60);
            }
        }

        private void DrawMenu()
        {
            ClientSize = new Size(500, 600);
            BackgroundImage = Bitmaps["background1.png"];
            if (!Controls.Contains(startButton))
                Controls.Add(startButton);
            if (!Controls.Contains(exitButton))
                Controls.Add(exitButton);
        }


        private void DrawTo(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.FillRectangle(backgroundColor, ClientRectangle);

            if (cloudPos.Y > ClientSize.Height)
                cloudPos = new Point(0, -ClientSize.Height);
            g.DrawImage(Bitmaps["cloud.png"],
                new Point(cloudPos.X, cloudPos.Y));
            cloudPos = new Point(cloudPos.X, cloudPos.Y + 2);

            foreach (var character in game.CurrentGameState.GetAllCharacters())
            {
                var image = Bitmaps[character.GetImage()];
                g.DrawImage(image, new PointF(
                    (float)character.GetLocation().X - image.Width / 1.5f,
                    (float)character.GetLocation().Y - image.Height / 1.5f));
            }

            foreach (var deadCharacter in game.CurrentGameState.DeadCharacters)
            {
                var image = Bitmaps[$"explosion-{deadCharacter.Item2}.png"];
                var explosionRectangle = new RectangleF((float)deadCharacter.Item1.X - 25, (float)deadCharacter.Item1.Y - 25, 50, 50);
                g.DrawImage(image, explosionRectangle);
            }

            if (game.EnemyPattern is BossFight)
            {
                var step = (float)(Game.GameFeatures.FieldSize.Width - 50) / Game.GameFeatures.BossHP;
                g.FillRectangle(Brushes.Goldenrod, new RectangleF(20, 5, step * game.EnemyPattern.GetHP(), 5));
            }

            if (game.EnemyPattern is Dialog)
            {
                DrawDialog(g);
            }

        }

        private void DrawDialog(Graphics g)
        {
            var brush = new SolidBrush(Color.FromArgb(50 * 255 / 100, Color.Black));
            var textBrush = Brushes.LightGoldenrodYellow;
            var bossNameBrush = Brushes.YellowGreen;
            var playerNameBrush = Brushes.Goldenrod;
            var dialogRect = new Rectangle(50, 460, 400, 120);
            var textRect = new Rectangle(60, 470, 380, 110);
            var phrase = (game.EnemyPattern as Dialog).CurrentPhrase;
            if (phrase == null) return;
            if (phrase.Item1 is Player)
            {
                var image = Bitmaps["player-dialog.png"];
                var imageRect = new Rectangle(0, 350, 128, 250);
                g.DrawImage(image, imageRect);
                g.FillRectangle(brush, dialogRect);
                var nameRect = new Rectangle(50, 430, 200, 30);
                g.FillRectangle(brush, nameRect);
                g.DrawString(phrase.Item2, new Font("Playtime with Hot Toddies", 16), playerNameBrush, nameRect);
                g.DrawString(phrase.Item3,
                    new Font("Playtime with Hot Toddies", 16), textBrush, textRect);
            }
            else
            {
                var image = Bitmaps["boss-dialog.png"];
                var imageRect = new Rectangle(300, 300, 250, 300);
                g.DrawImage(image, imageRect);
                g.FillRectangle(brush, dialogRect);
                var nameRect = new Rectangle(250, 430, 200, 30);
                g.FillRectangle(brush, nameRect);
                g.DrawString(phrase.Item2, new Font("Playtime with Hot Toddies", 16), bossNameBrush, nameRect);
                g.DrawString(phrase.Item3,
                    new Font("Playtime with Hot Toddies", 16), textBrush, textRect);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                isShootDown = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                isUpDown = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                isDownDown = false;
            }
            if (e.KeyCode == Keys.Left)
            {
                isLeftDown = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                isRightDown = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                isShootDown = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                isUpDown = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                isDownDown = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                isLeftDown = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                isRightDown = true;
            }

        }

        void ChangeBackgroundColor(object sender, EventArgs e)
        {
            iteration++;
            if (backgroundColorsQueue.Count == 0)
            {
                backgroundForward = !backgroundForward;
                backgroundColorsQueue =
                    new Queue<SolidBrush>(backgroundForward ? backgroundColorsBrushes : backgroundColorsBrushes.AsEnumerable().Reverse());
            }

            if (iteration == 10)
            {
                backgroundColor = backgroundColorsQueue.Dequeue();
                iteration = 0;
            }
        }

        void UpdateGameState(object sender, EventArgs e)
        {
            if (isShootDown && !(game.EnemyPattern is Dialog)) game.Player.Shoot();
            if (isUpDown) game.Player.Move(Direction.Up);
            else if (isDownDown) game.Player.Move(Direction.Down);
            if (isRightDown) game.Player.Move(Direction.Right);
            else if (isLeftDown) game.Player.Move(Direction.Left);

            game.UpdateState();


            //Invalidate();
        }
    }
}
