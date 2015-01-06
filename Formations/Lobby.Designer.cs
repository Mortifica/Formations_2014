namespace Formations
{
    partial class Lobby
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.connectionsList = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ChallengeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.ForeColor = System.Drawing.Color.White;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "Game 1",
            "Game 2",
            "Game 3",
            "Game 4",
            "Game 5",
            "Game 6",
            "Game 7",
            "Game 8"});
            this.listBox1.Location = new System.Drawing.Point(8, 8);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(603, 186);
            this.listBox1.TabIndex = 1;
            // 
            // connectionsList
            // 
            this.connectionsList.BackColor = System.Drawing.Color.Black;
            this.connectionsList.ForeColor = System.Drawing.Color.White;
            this.connectionsList.FormattingEnabled = true;
            this.connectionsList.Items.AddRange(new object[] {
            "Josh",
            "Maggie",
            "Dan",
            "Zach",
            "Jeff",
            "Bob",
            "Jim",
            "Bill",
            "Sandy",
            "Lisa",
            "Jenny"});
            this.connectionsList.Location = new System.Drawing.Point(620, 8);
            this.connectionsList.Margin = new System.Windows.Forms.Padding(2);
            this.connectionsList.Name = "connectionsList";
            this.connectionsList.Size = new System.Drawing.Size(215, 316);
            this.connectionsList.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Red;
            this.button2.Font = new System.Drawing.Font("MS Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(620, 326);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(213, 28);
            this.button2.TabIndex = 4;
            this.button2.Text = "Challenge";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox3
            // 
            this.listBox3.BackColor = System.Drawing.Color.Black;
            this.listBox3.ForeColor = System.Drawing.Color.White;
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Items.AddRange(new object[] {
            "<Zach> Sup guys",
            "<Josh> Yo",
            "<Maggie> Hey!",
            "<Dan> ...",
            "<Jeff> haaaay"});
            this.listBox3.Location = new System.Drawing.Point(8, 202);
            this.listBox3.Margin = new System.Windows.Forms.Padding(2);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(603, 121);
            this.listBox3.TabIndex = 5;
            this.listBox3.SelectedIndexChanged += new System.EventHandler(this.listBox3_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(9, 326);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(500, 30);
            this.textBox1.TabIndex = 6;
            // 
            // ChallengeButton
            // 
            this.ChallengeButton.BackColor = System.Drawing.Color.DarkCyan;
            this.ChallengeButton.Location = new System.Drawing.Point(517, 326);
            this.ChallengeButton.Margin = new System.Windows.Forms.Padding(2);
            this.ChallengeButton.Name = "ChallengeButton";
            this.ChallengeButton.Size = new System.Drawing.Size(93, 28);
            this.ChallengeButton.TabIndex = 7;
            this.ChallengeButton.Text = "Send";
            this.ChallengeButton.UseVisualStyleBackColor = false;
            // 
            // Lobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(841, 361);
            this.Controls.Add(this.ChallengeButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.connectionsList);
            this.Controls.Add(this.listBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Lobby";
            this.Text = "Lobby";
            this.Load += new System.EventHandler(this.Lobby_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox connectionsList;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ChallengeButton;

    }
}