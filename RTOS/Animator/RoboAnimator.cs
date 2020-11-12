using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTOS.Interpreter;
using RTOS.Forms;

namespace RTOS.Animator
{
    public enum Interaption
    {
        None,
        Human,
        Fire,
        Energy,
        Pause
    }

    class RoboAnimator
    {
        List<Robo> roboList = new List<Robo>();
        List<string> comandList;
        int AnimationPause = 200;
        List<Interaption> ListInteraption = new List<Interaption>();

        ViewForm VF;


        public RoboAnimator(string comands, ViewForm VF)
        {
            comandList = Parser.Parse(comands);
            this.VF = VF;
        }

        public void addRobo(Robo robo)
        {
            roboList.Add(robo);
        }

        public void RoboDo()
        {
            foreach (string comand in comandList)
            {
                Console.WriteLine("Действие: " + comand);
                Thread.Sleep(AnimationPause);
                if (ListInteraption.Count != 0)
                {
                    for (int i = 0; i < ListInteraption.Count; i++)
                    {
                        switch (ListInteraption[i])
                        {
                            case Interaption.Human:
                                setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Interaption]-Человек в погрузочной зоне");
                                InteraptHuman();
                                DelInterrupt(ref i);
                                break;
                            case Interaption.Fire:
                                setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Interaption]-Пожар на производстве");
                                InteraptFire();
                                DelInterrupt(ref i);
                                return;
                            case Interaption.Energy:
                                setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Interaption]-Проблемы с электропитанием");
                                InteraptEnergy();
                                DelInterrupt(ref i);
                                return;
                            case Interaption.Pause:
                                setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Interaption]-Пауза главного технолога");
                                InteraptPause();
                                DelInterrupt(ref i);
                                break;
                        }
                    }
                }
                switch (comand)
                {
                    case "TurnLeft":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот повернулся на лево");
                        roboList[0].TurnLeft();
                        break;
                    case "TurnRight":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот повернулся на право");
                        roboList[0].TurnRight();
                        break;
                    case "MotionForward":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот прошел вперед");
                        roboList[0].GoForvard();
                        break;
                    case "MotionBack":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот прошел назад");
                        roboList[0].GoBack();
                        break;
                    case "Take":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот взял деталь");
                        roboList[0].Take();
                        break;
                    case "Seal":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот упаковал деталь");
                        roboList[0].Pack();
                        break;
                    case "Put":
                        setLog("[" + DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss") + "][Robot]-Робот погрузил деталь");
                        roboList[0].Put();
                        break;
                }
            }
        }



        public void Interrupt(Interaption I)
        {
            ListInteraption.Add(I);
        }

        delegate void DelInterrupt_Delegate();
        public void DelInterrupt(ref int Index)
        {
            ListInteraption.RemoveAt(Index);
            Index--;

            DelInterrupt_Delegate DelInterrupt_D = delegate ()
            {
                VF.RemImgInteraption();
            };

            VF.Invoke(DelInterrupt_D);
        }

        bool maninzone = true;
        public void reversM() { maninzone = !maninzone; }

        bool pause = true;
        public void reversP() { pause = !pause; }

        private void InteraptHuman()
        {
            while (maninzone) ;
        }

        private void InteraptFire()
        {
            roboList[0].GoToBase();
        }

        private void InteraptEnergy()
        {
            roboList[0].SetIMG(Image.FromFile(@"..\..\Animation\Interaption\RoboHand_Error.png"));
        }

        private void InteraptPause()
        {
            while (pause) ;
        }


        private void setLog(string L)
        {
            Loger.addToLofFile(L);

            if (VF.InvokeRequired)
            {
                try
                {
                    VF.Invoke(new Action(() => { VF.setLog(L + "\n"); }));
                }
                catch { }
            }
            else
            {
                VF.setLog(L + "\n");
            }
        }
    }
}