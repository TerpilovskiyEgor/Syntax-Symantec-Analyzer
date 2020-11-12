namespace RTOS.Forms
{
    partial class ViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewForm));
            this.pictureBox_Conteiner = new System.Windows.Forms.PictureBox();
            this.pictureBox_Conveyor = new System.Windows.Forms.PictureBox();
            this.pictureBox_Car = new System.Windows.Forms.PictureBox();
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Conteiner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Conveyor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Car)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Conteiner
            // 
            this.pictureBox_Conteiner.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Conteiner.Image")));
            this.pictureBox_Conteiner.Location = new System.Drawing.Point(5, 5);
            this.pictureBox_Conteiner.Name = "pictureBox_Conteiner";
            this.pictureBox_Conteiner.Size = new System.Drawing.Size(243, 72);
            this.pictureBox_Conteiner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Conteiner.TabIndex = 1;
            this.pictureBox_Conteiner.TabStop = false;
            // 
            // pictureBox_Conveyor
            // 
            this.pictureBox_Conveyor.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Conveyor.Image")));
            this.pictureBox_Conveyor.Location = new System.Drawing.Point(382, -13);
            this.pictureBox_Conveyor.Name = "pictureBox_Conveyor";
            this.pictureBox_Conveyor.Size = new System.Drawing.Size(215, 436);
            this.pictureBox_Conveyor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Conveyor.TabIndex = 3;
            this.pictureBox_Conveyor.TabStop = false;
            // 
            // pictureBox_Car
            // 
            this.pictureBox_Car.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Car.Image")));
            this.pictureBox_Car.Location = new System.Drawing.Point(-299, 272);
            this.pictureBox_Car.Name = "pictureBox_Car";
            this.pictureBox_Car.Size = new System.Drawing.Size(356, 131);
            this.pictureBox_Car.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox_Car.TabIndex = 4;
            this.pictureBox_Car.TabStop = false;
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox_Log.Location = new System.Drawing.Point(0, 452);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.ReadOnly = true;
            this.richTextBox_Log.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBox_Log.Size = new System.Drawing.Size(704, 96);
            this.richTextBox_Log.TabIndex = 5;
            this.richTextBox_Log.Text = "";
            // 
            // ViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(704, 548);
            this.ControlBox = false;
            this.Controls.Add(this.richTextBox_Log);
            this.Controls.Add(this.pictureBox_Car);
            this.Controls.Add(this.pictureBox_Conveyor);
            this.Controls.Add(this.pictureBox_Conteiner);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ViewForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Conteiner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Conveyor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Car)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox_Conteiner;
        private System.Windows.Forms.PictureBox pictureBox_Conveyor;
        private System.Windows.Forms.PictureBox pictureBox_Car;
        private System.Windows.Forms.RichTextBox richTextBox_Log;
    }
}