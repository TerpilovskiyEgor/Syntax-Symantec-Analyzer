using System;
using System.Threading;
using System.Windows.Forms;

using RTOS.Forms;
using RTOS.Animator;
using RTOS.Compilation;
using System.Drawing;
using System.Collections.Generic;
using RTOS.Compilation.SemanticAnalyzer;

namespace RTOS
{
    public partial class MainForm : Form
    {
        ViewForm ActionForm;
        Thread ViewFormThread;
        Thread AnimationThread;

        public MainForm()
        {
            InitializeComponent();
        }

        #region File
        private void Button_Save_Click(object sender, EventArgs e)
        {
            if(SaveFileDialog.ShowDialog() != DialogResult.OK) return;
            System.IO.File.WriteAllText(SaveFileDialog.FileName, RichTextBox_ProgramCode.Text); 
        }

        private void Button_Load_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;
            RichTextBox_ProgramCode.Text = System.IO.File.ReadAllText(OpenFileDialog.FileName);
        }

        private void Button_Clean_Click(object sender, EventArgs e)
        {
            RichTextBox_ProgramCode.Text = null;
        }
        #endregion

        #region Compilation

        private void Button_SyntacticalAnalyzer_Click(object sender, EventArgs e)
        {
            SyntacticAnalyzer SA = new SyntacticAnalyzer(RichTextBox_ProgramCode.Text);
            SA.Analysis();
            List<string> AnalysisResult = SA.GetError();


            if (AnalysisResult.Count == 0)
            {
                RichTextBox_AssemblyStatus.Text = "ОШИБОК НЕТ";
                RichTextBox_ProgramCode.SelectAll();
                RichTextBox_ProgramCode.SelectionBackColor = Color.White;
            }
            else
            {
                ErrorSelect(SA.GetErrorNumberLine());
                RichTextBox_AssemblyStatus.Text = "НАЙДЕНЫ ОШИБКИ!\n----------------------------------------------------\n";
                foreach (string Line in AnalysisResult)
                {
                    RichTextBox_AssemblyStatus.AppendText(Line + "\n\n");
                }
            }

        }

        private void Button_SemanticAnalyzer_Click(object sender, EventArgs e)
        {
            SemanticAnalyzer SA = new SemanticAnalyzer(RichTextBox_ProgramCode.Text);
            SA.Analysis();
            List<string> AnalysisResult = SA.GetError();


            if (AnalysisResult.Count == 0)
            {
                RichTextBox_AssemblyStatus.Text = "ОШИБОК НЕТ";
                RichTextBox_ProgramCode.SelectAll();
                RichTextBox_ProgramCode.SelectionBackColor = Color.White;
            }
            else
            {
                ErrorSelect(SA.GetErrorNumberLine());
                RichTextBox_AssemblyStatus.Text = "НАЙДЕНЫ ОШИБКИ!\n----------------------------------------------------\n";
                foreach (string Line in AnalysisResult)
                {
                    RichTextBox_AssemblyStatus.AppendText(Line + "\n\n");
                }
            }


        }

        private void Button_Assembly_Click(object sender, EventArgs e)
        {
            Generator gen = new Generator(RichTextBox_ProgramCode.Text);
            gen.Generate();
        }

        public void ErrorSelect(List<int> ErrorList)
        {
            RichTextBox_ProgramCode.SelectAll();
            RichTextBox_ProgramCode.SelectionBackColor = Color.White;

            foreach (int LineNumber in ErrorList)
            {
                int startIndex = RichTextBox_ProgramCode.GetFirstCharIndexFromLine(LineNumber);
                if (startIndex == -1) { startIndex = RichTextBox_ProgramCode.Text.Length; }
                int endIndex = RichTextBox_ProgramCode.Text.IndexOf('\n', startIndex);
                if(endIndex == -1) { endIndex = RichTextBox_ProgramCode.Text.Length; }
                RichTextBox_ProgramCode.Select(startIndex, endIndex - startIndex);
                RichTextBox_ProgramCode.SelectionBackColor = Color.FromArgb(239, 123, 123);
            }
        }

        #endregion

        #region View
        private void Start_ViewFormThread()
        {
            ActionForm.ShowDialog();
        }

        private void Start_AnimationThread()
        {
            ActionForm.StartAnimation();
        }

        private void Button_Play_Click(object sender, EventArgs e)
        {
            Loger.addToLofFile("\nНачало новой программы");
            Program.comandText = RichTextBox_ProgramCode.Text;

            ActionForm = new ViewForm();

            ViewFormThread = new Thread(Start_ViewFormThread);
            ViewFormThread.Start();

            AnimationThread = new Thread(Start_AnimationThread);
            AnimationThread.Start();

            ButtonStatusRevers(true);
        }

        delegate void Form_Close_Delegat();
        private void Button_Stop_Click(object sender, EventArgs e)
        {
            Form_Close_Delegat FormCloseD = delegate ()
            {
                ActionForm.Close();
                ViewFormThread.Abort();
                AnimationThread.Abort();
            };
            if (ActionForm.InvokeRequired)
            {
                Invoke(FormCloseD);
            }
            else
            {
                ActionForm.Close();
                ViewFormThread.Abort();
                AnimationThread.Abort();
            }
            ButtonStatusRevers(false);
        }

        private void ButtonStatusRevers(bool Status)
        {
            if (Status)
            {
                Button_Play.BackgroundImage = Properties.Resources.Button_Play_G;
                Button_Play.Enabled = false;
                Button_Stop.BackgroundImage = Properties.Resources.Button_Stop;
                Button_Stop.Enabled = true;
            }
            else
            {
                Button_Play.BackgroundImage = Properties.Resources.Button_Play;
                Button_Play.Enabled = true;
                Button_Stop.BackgroundImage = Properties.Resources.Button_Stop_G;
                Button_Stop.Enabled = false;
            }
        }

        bool F = false;
        delegate void FlagSwitchM_Delegate();
        private void Button_InterruptHuman_Click(object sender, EventArgs e)
        {

            if(F == false)
            {
                InteraptSend(Animator.Interaption.Human);
                F = !F;

            }
            else
            {
                FlagSwitchM_Delegate m_Delegate = delegate ()
                {
                    ActionForm.Flag('M');
                };

                ActionForm.Invoke(m_Delegate);
                F = !F;
            }
            
        }

        private void Button_InterruptFire_Click(object sender, EventArgs e)
        {
            InteraptSend(Animator.Interaption.Fire);
        }

        private void Button_InterruptEnergy_Click(object sender, EventArgs e)
        {
            InteraptSend(Animator.Interaption.Energy);
        }


        bool G;
        delegate void FlagSwitchP_Delegate();
        private void Button_InterruptPause_Click(object sender, EventArgs e)
        {
            if (G == false)
            {
                InteraptSend(Animator.Interaption.Pause);
                G= !G;

            }
            else
            {
                FlagSwitchP_Delegate p_Delegate = delegate ()
                {
                    ActionForm.Flag('P');
                };

                ActionForm.Invoke(p_Delegate);
                G = !G;
            }
            
        }

        delegate void InteraptionFlagSwitch_Delegate();
        private void InteraptSend(Animator.Interaption I)
        {
            InteraptionFlagSwitch_Delegate InteraptionFlagSwitch_D = delegate ()
            {
                ActionForm.Interrupt(I);
            };

            if (ActionForm.InvokeRequired)
            {
                Invoke(InteraptionFlagSwitch_D);
            }
            else
            {
                ActionForm.Interrupt(I);
            }
        }

        

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.KeyCode);
            switch (e.KeyCode)
            {
                case Keys.M:
                    

                    break;
                case Keys.P:
                    break;

            }
        }
        #endregion
    }
}
