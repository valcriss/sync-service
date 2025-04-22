namespace sync_client
{
    partial class General
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(General));
            groupBox1 = new GroupBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            labelStatus = new Label();
            progressBarOverall = new ProgressBar();
            button1 = new Button();
            label5 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Transparent;
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(labelStatus);
            groupBox1.Controls.Add(progressBarOverall);
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(12, 302);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(696, 72);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Process";
            // 
            // label4
            // 
            label4.Location = new Point(6, 132);
            label4.Name = "label4";
            label4.Size = new Size(684, 22);
            label4.TabIndex = 6;
            label4.Text = "_";
            label4.TextAlign = ContentAlignment.TopCenter;
            // 
            // label3
            // 
            label3.Location = new Point(6, 110);
            label3.Name = "label3";
            label3.Size = new Size(684, 22);
            label3.TabIndex = 5;
            label3.Text = "_";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // label2
            // 
            label2.Location = new Point(6, 88);
            label2.Name = "label2";
            label2.Size = new Size(684, 22);
            label2.TabIndex = 4;
            label2.Text = "_";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // label1
            // 
            label1.Location = new Point(6, 66);
            label1.Name = "label1";
            label1.Size = new Size(684, 22);
            label1.TabIndex = 3;
            label1.Text = "_";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // labelStatus
            // 
            labelStatus.Location = new Point(6, 44);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(684, 22);
            labelStatus.TabIndex = 2;
            labelStatus.Text = "_";
            labelStatus.TextAlign = ContentAlignment.TopCenter;
            // 
            // progressBarOverall
            // 
            progressBarOverall.Location = new Point(6, 22);
            progressBarOverall.Name = "progressBarOverall";
            progressBarOverall.Size = new Size(684, 19);
            progressBarOverall.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackColor = Color.DarkRed;
            button1.Enabled = false;
            button1.FlatAppearance.BorderColor = Color.Brown;
            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.MouseDownBackColor = Color.IndianRed;
            button1.FlatAppearance.MouseOverBackColor = Color.Firebrick;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.White;
            button1.Location = new Point(280, 86);
            button1.Name = "button1";
            button1.Size = new Size(145, 67);
            button1.TabIndex = 1;
            button1.Text = "Lancer le jeu";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label5
            // 
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.White;
            label5.Location = new Point(12, 181);
            label5.Name = "label5";
            label5.Size = new Size(696, 40);
            label5.TabIndex = 2;
            label5.Text = "Game Name";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // General
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(720, 386);
            Controls.Add(label5);
            Controls.Add(button1);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "General";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sync Service";
            Load += General_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ProgressBar progressBarOverall;
        private Label labelStatus;
        private Button button1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label5;
    }
}
