using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging; //add this for the jpg compressor

namespace SnakeGame_TanzeelProject
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        int maxWidth;
        int maxHeight;

        int score;
        int topScore;

        Random rand = new Random();
        bool goLeft, goRight, goUp, goDown;
        public Form1()
        {
            InitializeComponent();
            new Settings();
        }

        //EventsInForm

        private void keyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }

        }
        private void keyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }
        //EventInStartButton
        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }
        //EventInSnapButton
        private void TakeSnapShort(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "Hurray! I Scored: " + score + " and my top score is: " + topScore;
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.Black;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake Game SnapeShot";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }
        }
        //EventInGameTimer
        private void GameTimerEvent(object sender, EventArgs e)
        {
            //settings the directions
            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            //end of directions


            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].x--;
                            break;
                        case "right":
                            Snake[i].x++;
                            break;
                        case "down":
                            Snake[i].y++;
                            break;
                        case "up":
                            Snake[i].y--;
                            break;
                    }
                    if (Snake[i].x < 0)
                    {
                        Snake[i].x = maxWidth;
                    }
                    if (Snake[i].x > maxWidth)
                    {
                        Snake[i].x = 0;
                    }
                    if (Snake[i].y < 0)
                    {
                        Snake[i].y = maxHeight;
                    }
                    if (Snake[i].y > maxHeight)
                    {
                        Snake[i].y = 0;
                    }
                    //snakehead
                    if (Snake[i].x == food.x && Snake[i].y == food.y)
                    {
                        EatFood();
                    }
                    //snakebody
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
                        {
                         GameOver();
                        }
                    }

                }
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }
            picCanvas.Invalidate();
        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            Brush snakeColour;

            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkGreen;
                }
                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].x * Settings.width,
                    Snake[i].y * Settings.height,
                    Settings.width, Settings.height
                    ));
            }
            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
                   (
                   food.x * Settings.width,
                   food.y * Settings.height,
                   Settings.width, Settings.height
                   ));
        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.width - 1;
            maxHeight = picCanvas.Height / Settings.height - 1;

            Snake.Clear();
            startBtn.Enabled = false;
            snapBtn.Enabled = false;

            score = 0;
            scoreLabel.Text = "Score: " + score;

            //BodyPart Of Snake
            Circle head = new Circle { x = 10, y = 5 };
            Snake.Add(head); //adding the head part of the snake to the list

            for (int i = 0; i < 10; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }
            food = new Circle() { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };

            gameTimer.Start();
        }
        private void EatFood()
        {
            score += 5;
            scoreLabel.Text = "Score: " + score;
            Circle body = new Circle
            {
                x = Snake[Snake.Count - 1].x,
                y = Snake[Snake.Count - 1].y
            };
            Snake.Add(body);
            food = new Circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };
        }
        private void GameOver()
        {
            
            if (score > topScore)
            {
                gameTimer.Stop();
                startBtn.Enabled = true;
                snapBtn.Enabled = true;
                topScore = score;

                topScoreLabel.Text = "Top Score: " + Environment.NewLine + topScore;
                topScoreLabel.ForeColor = Color.Maroon;
                topScoreLabel.TextAlign = ContentAlignment.MiddleCenter;

            }
            
        }
    }
    }


