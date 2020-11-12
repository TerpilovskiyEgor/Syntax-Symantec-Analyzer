namespace RTOS
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.Button_Load = new System.Windows.Forms.Button();
            this.Button_Save = new System.Windows.Forms.Button();
            this.Button_Clean = new System.Windows.Forms.Button();
            this.RichTextBox_ProgramCode = new System.Windows.Forms.RichTextBox();
            this.GroupBox_File = new System.Windows.Forms.GroupBox();
            this.GroupBox_Assembly = new System.Windows.Forms.GroupBox();
            this.Button_Assembly = new System.Windows.Forms.Button();
            this.Button_SemanticAnalyzer = new System.Windows.Forms.Button();
            this.Button_SyntacticalAnalyzer = new System.Windows.Forms.Button();
            this.RichTextBox_AssemblyStatus = new System.Windows.Forms.RichTextBox();
            this.GroupBox_View = new System.Windows.Forms.GroupBox();
            this.GroupBox_Interupt = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Button_InterruptPause = new System.Windows.Forms.Button();
            this.Label_InterruptEnergy = new System.Windows.Forms.Label();
            this.Label_InterruptFire = new System.Windows.Forms.Label();
            this.Label_InterruptHuman = new System.Windows.Forms.Label();
            this.Button_InterruptEnergy = new System.Windows.Forms.Button();
            this.Button_InterruptFire = new System.Windows.Forms.Button();
            this.Button_InterruptHuman = new System.Windows.Forms.Button();
            this.Button_Stop = new System.Windows.Forms.Button();
            this.Button_Play = new System.Windows.Forms.Button();
            this.GroupBox_File.SuspendLayout();
            this.GroupBox_Assembly.SuspendLayout();
            this.GroupBox_View.SuspendLayout();
            this.GroupBox_Interupt.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.Filter = "Текстовый файл|*.txt";
            this.OpenFileDialog.Title = "Open";
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.FileName = "Script";
            this.SaveFileDialog.Filter = "Текстовый файл|*.txt";
            this.SaveFileDialog.OverwritePrompt = false;
            this.SaveFileDialog.Title = "Save";
            // 
            // Button_Load
            // 
            this.Button_Load.Location = new System.Drawing.Point(87, 19);
            this.Button_Load.Name = "Button_Load";
            this.Button_Load.Size = new System.Drawing.Size(75, 23);
            this.Button_Load.TabIndex = 0;
            this.Button_Load.Text = "Загрузить";
            this.Button_Load.UseVisualStyleBackColor = true;
            this.Button_Load.Click += new System.EventHandler(this.Button_Load_Click);
            // 
            // Button_Save
            // 
            this.Button_Save.Location = new System.Drawing.Point(6, 19);
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.Size = new System.Drawing.Size(75, 23);
            this.Button_Save.TabIndex = 1;
            this.Button_Save.Text = "Сохранить";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // Button_Clean
            // 
            this.Button_Clean.Location = new System.Drawing.Point(6, 48);
            this.Button_Clean.Name = "Button_Clean";
            this.Button_Clean.Size = new System.Drawing.Size(75, 23);
            this.Button_Clean.TabIndex = 2;
            this.Button_Clean.Text = "Очистить";
            this.Button_Clean.UseVisualStyleBackColor = true;
            this.Button_Clean.Click += new System.EventHandler(this.Button_Clean_Click);
            // 
            // RichTextBox_ProgramCode
            // 
            this.RichTextBox_ProgramCode.Location = new System.Drawing.Point(6, 74);
            this.RichTextBox_ProgramCode.Name = "RichTextBox_ProgramCode";
            this.RichTextBox_ProgramCode.Size = new System.Drawing.Size(265, 350);
            this.RichTextBox_ProgramCode.TabIndex = 3;
            this.RichTextBox_ProgramCode.Text = resources.GetString("RichTextBox_ProgramCode.Text");
            // 
            // GroupBox_File
            // 
            this.GroupBox_File.Controls.Add(this.Button_Clean);
            this.GroupBox_File.Controls.Add(this.RichTextBox_ProgramCode);
            this.GroupBox_File.Controls.Add(this.Button_Load);
            this.GroupBox_File.Controls.Add(this.Button_Save);
            this.GroupBox_File.Location = new System.Drawing.Point(11, 11);
            this.GroupBox_File.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBox_File.Name = "GroupBox_File";
            this.GroupBox_File.Size = new System.Drawing.Size(277, 430);
            this.GroupBox_File.TabIndex = 4;
            this.GroupBox_File.TabStop = false;
            this.GroupBox_File.Text = "Файл";
            // 
            // GroupBox_Assembly
            // 
            this.GroupBox_Assembly.Controls.Add(this.Button_Assembly);
            this.GroupBox_Assembly.Controls.Add(this.Button_SemanticAnalyzer);
            this.GroupBox_Assembly.Controls.Add(this.Button_SyntacticalAnalyzer);
            this.GroupBox_Assembly.Controls.Add(this.RichTextBox_AssemblyStatus);
            this.GroupBox_Assembly.Location = new System.Drawing.Point(293, 12);
            this.GroupBox_Assembly.Name = "GroupBox_Assembly";
            this.GroupBox_Assembly.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBox_Assembly.Size = new System.Drawing.Size(174, 429);
            this.GroupBox_Assembly.TabIndex = 5;
            this.GroupBox_Assembly.TabStop = false;
            this.GroupBox_Assembly.Text = "Сборка";
            // 
            // Button_Assembly
            // 
            this.Button_Assembly.Location = new System.Drawing.Point(41, 85);
            this.Button_Assembly.Name = "Button_Assembly";
            this.Button_Assembly.Size = new System.Drawing.Size(93, 23);
            this.Button_Assembly.TabIndex = 3;
            this.Button_Assembly.Text = "Сборка";
            this.Button_Assembly.UseVisualStyleBackColor = true;
            this.Button_Assembly.Click += new System.EventHandler(this.Button_Assembly_Click);
            // 
            // Button_SemanticAnalyzer
            // 
            this.Button_SemanticAnalyzer.Location = new System.Drawing.Point(36, 55);
            this.Button_SemanticAnalyzer.Name = "Button_SemanticAnalyzer";
            this.Button_SemanticAnalyzer.Size = new System.Drawing.Size(102, 23);
            this.Button_SemanticAnalyzer.TabIndex = 2;
            this.Button_SemanticAnalyzer.Text = "Семантический";
            this.Button_SemanticAnalyzer.UseVisualStyleBackColor = true;
            this.Button_SemanticAnalyzer.Click += new System.EventHandler(this.Button_SemanticAnalyzer_Click);
            // 
            // Button_SyntacticalAnalyzer
            // 
            this.Button_SyntacticalAnalyzer.Location = new System.Drawing.Point(36, 25);
            this.Button_SyntacticalAnalyzer.Name = "Button_SyntacticalAnalyzer";
            this.Button_SyntacticalAnalyzer.Size = new System.Drawing.Size(102, 23);
            this.Button_SyntacticalAnalyzer.TabIndex = 1;
            this.Button_SyntacticalAnalyzer.Text = "Синтаксический";
            this.Button_SyntacticalAnalyzer.UseVisualStyleBackColor = true;
            this.Button_SyntacticalAnalyzer.Click += new System.EventHandler(this.Button_SyntacticalAnalyzer_Click);
            // 
            // RichTextBox_AssemblyStatus
            // 
            this.RichTextBox_AssemblyStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RichTextBox_AssemblyStatus.Location = new System.Drawing.Point(5, 114);
            this.RichTextBox_AssemblyStatus.Name = "RichTextBox_AssemblyStatus";
            this.RichTextBox_AssemblyStatus.ReadOnly = true;
            this.RichTextBox_AssemblyStatus.Size = new System.Drawing.Size(164, 310);
            this.RichTextBox_AssemblyStatus.TabIndex = 0;
            this.RichTextBox_AssemblyStatus.Text = "";
            // 
            // GroupBox_View
            // 
            this.GroupBox_View.Controls.Add(this.GroupBox_Interupt);
            this.GroupBox_View.Controls.Add(this.Button_Stop);
            this.GroupBox_View.Controls.Add(this.Button_Play);
            this.GroupBox_View.Location = new System.Drawing.Point(473, 12);
            this.GroupBox_View.Name = "GroupBox_View";
            this.GroupBox_View.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GroupBox_View.Size = new System.Drawing.Size(174, 429);
            this.GroupBox_View.TabIndex = 6;
            this.GroupBox_View.TabStop = false;
            this.GroupBox_View.Text = "Визуализация";
            // 
            // GroupBox_Interupt
            // 
            this.GroupBox_Interupt.Controls.Add(this.label1);
            this.GroupBox_Interupt.Controls.Add(this.Button_InterruptPause);
            this.GroupBox_Interupt.Controls.Add(this.Label_InterruptEnergy);
            this.GroupBox_Interupt.Controls.Add(this.Label_InterruptFire);
            this.GroupBox_Interupt.Controls.Add(this.Label_InterruptHuman);
            this.GroupBox_Interupt.Controls.Add(this.Button_InterruptEnergy);
            this.GroupBox_Interupt.Controls.Add(this.Button_InterruptFire);
            this.GroupBox_Interupt.Controls.Add(this.Button_InterruptHuman);
            this.GroupBox_Interupt.Location = new System.Drawing.Point(5, 173);
            this.GroupBox_Interupt.Name = "GroupBox_Interupt";
            this.GroupBox_Interupt.Size = new System.Drawing.Size(164, 250);
            this.GroupBox_Interupt.TabIndex = 2;
            this.GroupBox_Interupt.TabStop = false;
            this.GroupBox_Interupt.Text = "Прерывания";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(62, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 50);
            this.label1.TabIndex = 7;
            this.label1.Text = "Аварийная остановка роботов";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Button_InterruptPause
            // 
            this.Button_InterruptPause.BackgroundImage = global::RTOS.Properties.Resources.IconInterrupt_Pause;
            this.Button_InterruptPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_InterruptPause.Location = new System.Drawing.Point(6, 187);
            this.Button_InterruptPause.Name = "Button_InterruptPause";
            this.Button_InterruptPause.Size = new System.Drawing.Size(50, 50);
            this.Button_InterruptPause.TabIndex = 6;
            this.Button_InterruptPause.UseVisualStyleBackColor = true;
            this.Button_InterruptPause.Click += new System.EventHandler(this.Button_InterruptPause_Click);
            // 
            // Label_InterruptEnergy
            // 
            this.Label_InterruptEnergy.Location = new System.Drawing.Point(62, 131);
            this.Label_InterruptEnergy.Name = "Label_InterruptEnergy";
            this.Label_InterruptEnergy.Size = new System.Drawing.Size(96, 50);
            this.Label_InterruptEnergy.TabIndex = 5;
            this.Label_InterruptEnergy.Text = "Рабочая зона обезточена";
            this.Label_InterruptEnergy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label_InterruptFire
            // 
            this.Label_InterruptFire.Location = new System.Drawing.Point(62, 75);
            this.Label_InterruptFire.Name = "Label_InterruptFire";
            this.Label_InterruptFire.Size = new System.Drawing.Size(96, 50);
            this.Label_InterruptFire.TabIndex = 4;
            this.Label_InterruptFire.Text = "Пожар на производстве";
            this.Label_InterruptFire.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label_InterruptHuman
            // 
            this.Label_InterruptHuman.Location = new System.Drawing.Point(62, 19);
            this.Label_InterruptHuman.Name = "Label_InterruptHuman";
            this.Label_InterruptHuman.Size = new System.Drawing.Size(96, 50);
            this.Label_InterruptHuman.TabIndex = 3;
            this.Label_InterruptHuman.Text = "Человек в рабочей зоне";
            this.Label_InterruptHuman.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Button_InterruptEnergy
            // 
            this.Button_InterruptEnergy.BackgroundImage = global::RTOS.Properties.Resources.IconInterrupt_Energy;
            this.Button_InterruptEnergy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_InterruptEnergy.Location = new System.Drawing.Point(6, 131);
            this.Button_InterruptEnergy.Name = "Button_InterruptEnergy";
            this.Button_InterruptEnergy.Size = new System.Drawing.Size(50, 50);
            this.Button_InterruptEnergy.TabIndex = 2;
            this.Button_InterruptEnergy.UseVisualStyleBackColor = true;
            this.Button_InterruptEnergy.Click += new System.EventHandler(this.Button_InterruptEnergy_Click);
            // 
            // Button_InterruptFire
            // 
            this.Button_InterruptFire.BackgroundImage = global::RTOS.Properties.Resources.IconInterrupt_Fire;
            this.Button_InterruptFire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_InterruptFire.Location = new System.Drawing.Point(6, 75);
            this.Button_InterruptFire.Name = "Button_InterruptFire";
            this.Button_InterruptFire.Size = new System.Drawing.Size(50, 50);
            this.Button_InterruptFire.TabIndex = 1;
            this.Button_InterruptFire.UseVisualStyleBackColor = true;
            this.Button_InterruptFire.Click += new System.EventHandler(this.Button_InterruptFire_Click);
            // 
            // Button_InterruptHuman
            // 
            this.Button_InterruptHuman.BackgroundImage = global::RTOS.Properties.Resources.IconInterrupt_Human;
            this.Button_InterruptHuman.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_InterruptHuman.Location = new System.Drawing.Point(6, 19);
            this.Button_InterruptHuman.Name = "Button_InterruptHuman";
            this.Button_InterruptHuman.Size = new System.Drawing.Size(50, 50);
            this.Button_InterruptHuman.TabIndex = 0;
            this.Button_InterruptHuman.UseVisualStyleBackColor = true;
            this.Button_InterruptHuman.Click += new System.EventHandler(this.Button_InterruptHuman_Click);
            // 
            // Button_Stop
            // 
            this.Button_Stop.BackgroundImage = global::RTOS.Properties.Resources.Button_Stop_G;
            this.Button_Stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_Stop.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Button_Stop.Enabled = false;
            this.Button_Stop.Location = new System.Drawing.Point(104, 26);
            this.Button_Stop.Name = "Button_Stop";
            this.Button_Stop.Size = new System.Drawing.Size(65, 65);
            this.Button_Stop.TabIndex = 1;
            this.Button_Stop.UseVisualStyleBackColor = true;
            this.Button_Stop.Click += new System.EventHandler(this.Button_Stop_Click);
            // 
            // Button_Play
            // 
            this.Button_Play.BackgroundImage = global::RTOS.Properties.Resources.Button_Play;
            this.Button_Play.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_Play.Location = new System.Drawing.Point(5, 25);
            this.Button_Play.Name = "Button_Play";
            this.Button_Play.Size = new System.Drawing.Size(65, 65);
            this.Button_Play.TabIndex = 0;
            this.Button_Play.UseVisualStyleBackColor = true;
            this.Button_Play.Click += new System.EventHandler(this.Button_Play_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 452);
            this.Controls.Add(this.GroupBox_View);
            this.Controls.Add(this.GroupBox_Assembly);
            this.Controls.Add(this.GroupBox_File);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Система уроавления техническим процесом";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.GroupBox_File.ResumeLayout(false);
            this.GroupBox_Assembly.ResumeLayout(false);
            this.GroupBox_View.ResumeLayout(false);
            this.GroupBox_Interupt.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.Button Button_Load;
        private System.Windows.Forms.Button Button_Save;
        private System.Windows.Forms.Button Button_Clean;
        private System.Windows.Forms.GroupBox GroupBox_File;
        private System.Windows.Forms.GroupBox GroupBox_Assembly;
        private System.Windows.Forms.Button Button_Assembly;
        private System.Windows.Forms.Button Button_SemanticAnalyzer;
        private System.Windows.Forms.Button Button_SyntacticalAnalyzer;
        private System.Windows.Forms.RichTextBox RichTextBox_AssemblyStatus;
        private System.Windows.Forms.GroupBox GroupBox_View;
        private System.Windows.Forms.Button Button_Play;
        private System.Windows.Forms.Button Button_Stop;
        private System.Windows.Forms.GroupBox GroupBox_Interupt;
        private System.Windows.Forms.Label Label_InterruptEnergy;
        private System.Windows.Forms.Label Label_InterruptFire;
        private System.Windows.Forms.Label Label_InterruptHuman;
        private System.Windows.Forms.Button Button_InterruptEnergy;
        private System.Windows.Forms.Button Button_InterruptFire;
        private System.Windows.Forms.Button Button_InterruptHuman;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Button_InterruptPause;
        public System.Windows.Forms.RichTextBox RichTextBox_ProgramCode;
        public System.Windows.Forms.SaveFileDialog SaveFileDialog;
    }
}

