using RTOS.Animator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace RTOS.Forms
{
     public partial class ViewForm : Form
    {
        RoboAnimator RA;
        List<PictureBox> List_PictureBox = new List<PictureBox>();

        public ViewForm()
        {
            RoboInitialize();
            InitializeComponent();
        }

        private void RoboInitialize()
        {
            Console.WriteLine("Инициализация аниматора");

            RA = new RoboAnimator(Program.comandText, this);

            Console.WriteLine("Инициализация роботов");
            Image I = Image.FromFile(@"..\..\Animation\Robo\RoboHand_Default.png");
            Robo R = new Robo(I, 1);
            RA.addRobo(R);
            Controls.Add(R.RoboBox);
        }

        delegate void AddImgInteraption_Delegate(Interaption I);
        public void Interrupt(Interaption I)
        {
            RA.Interrupt(I);

            AddImgInteraption_Delegate AddImgInteraption_D = delegate (Interaption Q)
            {
                AddImgInteraption(Q);
            };
            Invoke(AddImgInteraption_D, new object[] { I });
        }

        public void AddImgInteraption(Interaption I)
        {
            PictureBox P = new PictureBox();
            P.Size = new Size(50, 50);
            P.BackColor = Color.Transparent;
            P.SizeMode = PictureBoxSizeMode.Zoom;

            switch (I)
            {
                case Interaption.Energy:
                    P.Image = Image.FromFile(@"..\..\Animation\IconInterrupt_Energy.png");
                    break;
                case Interaption.Human:
                    P.Image = Image.FromFile(@"..\..\Animation\IconInterrupt_Human.png");
                    break;
                case Interaption.Fire:
                    P.Image = Image.FromFile(@"..\..\Animation\IconInterrupt_Fire.png");
                    break;
                case Interaption.Pause:
                    P.Image = Image.FromFile(@"..\..\Animation\IconInterrupt_Pause.png");
                    break;
            }
            
            P.Location = new Point(629, 12 + (List_PictureBox.Count * 50));

            List_PictureBox.Add(P);

            Controls.Add(P);
        }

        public void RemImgInteraption()
        {
            List_PictureBox[0].Dispose();
            List_PictureBox.RemoveAt(0);

            for (int i = 0; i < List_PictureBox.Count; i++)
            {
                List_PictureBox[i].Location = new Point(629, 12 + (i * 50));
            }
        }


        /*=== Thread task ===*/
        public void StartAnimation()
        {
            RA.RoboDo();
        }

        public void setLog(string L)
        {
            richTextBox_Log.AppendText(L);
        }

        public void Flag(char C)
        {
            if(C == 'M')
            {
                RA.reversM();
            }
            else
            {
                RA.reversP();
            }
        }


    }
}
