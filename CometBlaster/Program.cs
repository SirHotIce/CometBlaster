using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CometBlaster
{
    class Program
    {
        static void Main(string[] args)
        {
            Screen screen = new Screen();
        }
        private class Screen: Form
        {

            //rocket
            PictureBox rocket = new PictureBox();
            Point rocketPos;

            //comet
            Comet comet;
            List<PictureBox> cometList = new List<PictureBox>();

            //bullet
            List<PictureBox> bulletList = new List<PictureBox>();

            //events
            public delegate void MyEventHandler();
            public event MyEventHandler CometSpawned;
            public event MyEventHandler CometMoving;
            public event MyEventHandler BulletMoving;
            public event MyEventHandler CometReachedEnd;

            //others
            Form theForm;
            Control.ControlCollection controller;
            int health;
            int score;

            public Screen()
            {
                this.SetBounds(500, 500, 500, 500);
                this.BackgroundImage = Image.FromFile(@"../../Sprites/Space.png");
                this.KeyPreview = true;
                theForm = this;
                controller = this.Controls;

                score = 0;
                health = 1;

                RocketInit();
                this.KeyDown += new KeyEventHandler(RocketActions);

                CometMoving += CometEndChecker;
                CometReachedEnd += GameStatusChecker;
                CometInit();

                BulletMoving += CometKiller;

                this.Show();
                Application.Run(this);
            }

            private void RocketActions(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.A)
                {
                    rocketPos.X -= 10;
                    rocket.Location = rocketPos;
                }
                if (e.KeyCode == Keys.D)
                {
                    rocketPos.X += 10;
                    rocket.Location = rocketPos;
                }
                if (e.KeyCode == Keys.E)
                {
                    BulletStuff();
                }

            }

            private void RocketInit()
            {
                rocket.Location = new Point(200, 400);
                rocket.Height = 40;
                rocket.Width = 50;
                rocket.Image = Image.FromFile(@"../../Sprites/rocket.png");
                rocket.BackColor = Color.Transparent;
                rocketPos = rocket.Location;
                controller.Add(rocket);
            }

            private async void CometInit()
            {
                while (true)
                {
                    comet = new Comet();
                    PictureBox theComet = comet.CometInit();
                    controller.Add(theComet);
                    cometList.Add(theComet);
                    Point cometPos = theComet.Location;

                    CometSpawned += async () =>
                    {
                        while (theComet.Location.Y <= 420)
                        {
                            cometPos.Y += 10;
                            theComet.Location = cometPos;
                            CometMoving();
                            await Task.Delay(100);
                        }
                    };
                    CometSpawned();
                    await Task.Delay(2000);
                }
            }

            private async void BulletStuff()
            {
                PictureBox bullet = new PictureBox();
                bullet.Location = new Point(rocket.Location.X+10, 400);
                bullet.Height = 4;
                bullet.Width = 3;
                bullet.Image = Image.FromFile(@"../../Sprites/bullet.png");
                bullet.BackColor = Color.Transparent;
                Point bulletPos = bullet.Location;
                controller.Add(bullet);
                bulletList.Add(bullet);

                while (bullet.Location.Y >= 0)
                {
                    bulletPos.Y -= 10;
                    bullet.Location = bulletPos;
                    BulletMoving();

                    await Task.Delay(20);
                    if (bullet.Location.Y == 0)
                    {
                        controller.Remove(bullet);
                        bulletList.Remove(bullet);
                        break;
                    }

                }

            }

            private void CometEndChecker()
            {
                foreach(PictureBox comet in cometList)
                {
                    if(comet.Location.Y == 420)
                    {
                        controller.Remove(comet);
                        cometList.Remove(comet);
                        health--;
                        CometReachedEnd();
                        break;
                    }
                }
            }

            private void CometKiller()
            {
                foreach (PictureBox bullet in bulletList) { 
                    foreach (PictureBox comet in cometList) {

                        if (bullet.Bounds.IntersectsWith(comet.Bounds))
                        {

                            controller.Remove(comet);
                            cometList.Remove(comet);
                            controller.Remove(bullet);
                            bulletList.Remove(bullet);
                            score++;
                            Console.WriteLine($"Score Increased to {score}");
                            break;
                        }
                    }
                    break;
                }
            }

            private void GameStatusChecker()
            {
                if (health == 0)
                {
                    Console.WriteLine($"Game Over, Your Score was: {score}");
                    theForm.Hide();
                    Thread.Sleep(3000);
                    Application.Restart();
                }
            }
            private class Comet
            {
                PictureBox comet = new PictureBox();
                Random rng = new Random();

                public Comet()
                {
                }

                public PictureBox CometInit()
                {
                    int x = rng.Next(10, 420);
                    int y = -50;
                    comet.Location = new Point(x, y);
                    comet.Width = 50;
                    comet.Height = 50;
                    comet.Image= Image.FromFile(@"../../Sprites/Comet.png");
                    comet.BackColor = Color.Transparent;
                    return comet;
                }
            }
        }
    }
}
