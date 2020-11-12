using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTOS.Animator
{
    class Robo
    {
        int Xcord = 150, Ycord = 288;
        int width = 100, height = 100;
        PictureBox roboBox;

        int RoboNumber;
        int roboDirection = 1;

        int AnimationPause = 180;

        public Robo(Image I, int RN)
        {
            RoboNumber = RN;

            RoboBox = new PictureBox();
            RoboBox.Image = I;
            RoboBox.Size = new Size(width, height);
            RoboBox.Location = new Point(Xcord, Ycord);
            RoboBox.BackColor = Color.Transparent;
            RoboBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public void TurnLeft()
        {
            roboBox.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
            roboBox.Image = roboBox.Image;
            roboDirection = (--roboDirection + 4) % 4;
        }

        public void TurnRight()
        {
            roboBox.Image.RotateFlip(RotateFlipType.Rotate270FlipXY);
            roboBox.Image = roboBox.Image;
            roboDirection = (++roboDirection) % 4;
        }

        public void TurnAround()
        {
            roboBox.Image.RotateFlip(RotateFlipType.Rotate180FlipY);
            roboBox.Image = roboBox.Image;
            roboDirection = (roboDirection + 2) % 4;
        }

        public void GoForvard()
        {
            roboGo(6, 0);
        }

        public void GoBack()
        {
            roboGo(-6, 0);
        }

        public void Take()
        {
            string path = @"..\..\Animation\Robo\Take\";
            for (int i = 1; i <= 3; i++)
            {
                roboBox.Image = PreRotate(Image.FromFile(path + "RoboHand_Take_" + i + ".png"));
                Thread.Sleep(AnimationPause);
            }
        }

        public void Pack()
        {
            string path = @"..\..\Animation\Robo\Pack\";
            for (int i = 1; i <= 11; i++)
            {
                roboBox.Image = PreRotate(Image.FromFile(path + "RoboHand_Pack_" + i + ".png"));
                Thread.Sleep(AnimationPause);
            }
        }

        public void Put()
        {
            string path = @"..\..\Animation\Robo\Put\";
            for (int i = 1; i <= 3; i++)
            {
                roboBox.Image = PreRotate(Image.FromFile(path + "RoboHand_Put_" + i + ".png"));
                Thread.Sleep(AnimationPause);
            }
        }

        public void GoToBase()
        {
            Point basePosition = new Point(270, 6);
            Point curentPoint = roboBox.Location;

            if (curentPoint.X < basePosition.X)
            {
                while(roboDirection != 1)
                {
                    TurnRight();
                    Thread.Sleep(200);
                }
            }
            else if (curentPoint.X > basePosition.X)
            {
                while (roboDirection != 3)
                {
                    TurnLeft();
                    Thread.Sleep(200);
                }
            }

            int colStep = Math.Abs(basePosition.X- curentPoint.X) / 6;
            for (int i = 0; i < colStep; i++)
            {
                GoForvard();
                Thread.Sleep(200);
            }
            if(roboDirection == 1)
            {
                TurnLeft();
                Thread.Sleep(200);
            }
            else
            {
                TurnRight();
                Thread.Sleep(200);
            }

            colStep = Math.Abs(basePosition.Y - curentPoint.Y) / 6;
            for (int i = 0; i < colStep; i++)
            {
                GoForvard();
                Thread.Sleep(200);
            }
            TurnRight();
            Thread.Sleep(200);
            TurnRight();
            Thread.Sleep(200);
        }


        delegate void roboGoDelegate();
        public void roboGo(int X, int Y)
        {
            int C;
            switch (roboDirection)
            {
                case 0:
                    C = X;
                    X = Y;
                    Y = -C;
                    break;
                case 1:
                    break;
                case 2:
                    C = X;
                    X = -Y;
                    Y = C;
                    break;
                case 3:
                    X = -X;
                    Y = -Y;
                    break;
            }

            roboGoDelegate roboGoD = delegate ()
            {
                roboBox.Location = new Point(roboBox.Location.X + X, roboBox.Location.Y + Y);
            };

            roboBox.Invoke(roboGoD);
        }

        private Image PreRotate(Image I)
        {
            switch(roboDirection)
            {
                case 0:
                    I.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 2:
                    I.RotateFlip(RotateFlipType.Rotate90FlipXY);
                    break;
                case 3:
                    I.RotateFlip(RotateFlipType.Rotate180FlipY);
                    break;
            }
            return I;
        }

        public PictureBox RoboBox
        {
            get => roboBox;
            set => roboBox = value;
        }

        public Point GetCoordinate()
        {
            return roboBox.Location;
        }

        public void SetIMG(Image I)
        {
            roboBox.Image = PreRotate(I);
        }
    }
}
