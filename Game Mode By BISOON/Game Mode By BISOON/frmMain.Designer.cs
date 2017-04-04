namespace Game_Mode_By_BISOON
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.frm = new theme.FormSkin();
            this.label1 = new System.Windows.Forms.Label();
            this.startBtn = new theme.FlatButton();
            this.conBtn = new theme.FlatButton();
            this.cexCh = new System.Windows.Forms.RadioButton();
            this.dexCh = new System.Windows.Forms.RadioButton();
            this.flatButton1 = new theme.FlatButton();
            this.flatButton2 = new theme.FlatButton();
            this.closeBtn = new theme.FlatButton();
            this.flatMini1 = new theme.FlatMini();
            this.frm.SuspendLayout();
            this.SuspendLayout();
            // 
            // frm
            // 
            this.frm.BackColor = System.Drawing.Color.White;
            this.frm.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.frm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(58)))), ((int)(((byte)(60)))));
            this.frm.Controls.Add(this.flatMini1);
            this.frm.Controls.Add(this.closeBtn);
            this.frm.Controls.Add(this.flatButton2);
            this.frm.Controls.Add(this.flatButton1);
            this.frm.Controls.Add(this.dexCh);
            this.frm.Controls.Add(this.cexCh);
            this.frm.Controls.Add(this.label1);
            this.frm.Controls.Add(this.startBtn);
            this.frm.Controls.Add(this.conBtn);
            this.frm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frm.FlatColor = System.Drawing.Color.DimGray;
            this.frm.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.frm.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.frm.HeaderMaximize = false;
            this.frm.Location = new System.Drawing.Point(0, 0);
            this.frm.Name = "frm";
            this.frm.Size = new System.Drawing.Size(315, 235);
            this.frm.TabIndex = 0;
            this.frm.Text = "Projectile War";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.label1.Location = new System.Drawing.Point(12, 171);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "Map :";
            // 
            // startBtn
            // 
            this.startBtn.BackColor = System.Drawing.Color.Transparent;
            this.startBtn.BaseColor = System.Drawing.Color.DimGray;
            this.startBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.startBtn.Location = new System.Drawing.Point(60, 114);
            this.startBtn.Name = "startBtn";
            this.startBtn.Rounded = false;
            this.startBtn.Size = new System.Drawing.Size(189, 32);
            this.startBtn.TabIndex = 2;
            this.startBtn.Text = "Start Game Mode";
            this.startBtn.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // conBtn
            // 
            this.conBtn.BackColor = System.Drawing.Color.Transparent;
            this.conBtn.BaseColor = System.Drawing.Color.DimGray;
            this.conBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.conBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.conBtn.Location = new System.Drawing.Point(60, 76);
            this.conBtn.Name = "conBtn";
            this.conBtn.Rounded = false;
            this.conBtn.Size = new System.Drawing.Size(189, 32);
            this.conBtn.TabIndex = 0;
            this.conBtn.Text = "Connect / Attach";
            this.conBtn.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.conBtn.Click += new System.EventHandler(this.conBtn_Click);
            // 
            // cexCh
            // 
            this.cexCh.AutoSize = true;
            this.cexCh.BackColor = System.Drawing.Color.Transparent;
            this.cexCh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cexCh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.cexCh.Location = new System.Drawing.Point(162, 53);
            this.cexCh.Name = "cexCh";
            this.cexCh.Size = new System.Drawing.Size(46, 19);
            this.cexCh.TabIndex = 4;
            this.cexCh.TabStop = true;
            this.cexCh.Text = "CEX";
            this.cexCh.UseVisualStyleBackColor = false;
            // 
            // dexCh
            // 
            this.dexCh.AutoSize = true;
            this.dexCh.BackColor = System.Drawing.Color.Transparent;
            this.dexCh.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dexCh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dexCh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.dexCh.Location = new System.Drawing.Point(95, 53);
            this.dexCh.Name = "dexCh";
            this.dexCh.Size = new System.Drawing.Size(48, 19);
            this.dexCh.TabIndex = 5;
            this.dexCh.TabStop = true;
            this.dexCh.Text = "DEX";
            this.dexCh.UseVisualStyleBackColor = false;
            // 
            // flatButton1
            // 
            this.flatButton1.BackColor = System.Drawing.Color.Transparent;
            this.flatButton1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.flatButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flatButton1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.flatButton1.Location = new System.Drawing.Point(3, 212);
            this.flatButton1.Name = "flatButton1";
            this.flatButton1.Rounded = false;
            this.flatButton1.Size = new System.Drawing.Size(20, 20);
            this.flatButton1.TabIndex = 6;
            this.flatButton1.Text = " ?";
            this.flatButton1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.flatButton1.Click += new System.EventHandler(this.flatButton1_Click);
            // 
            // flatButton2
            // 
            this.flatButton2.BackColor = System.Drawing.Color.Transparent;
            this.flatButton2.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.flatButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flatButton2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.flatButton2.Location = new System.Drawing.Point(238, 212);
            this.flatButton2.Name = "flatButton2";
            this.flatButton2.Rounded = false;
            this.flatButton2.Size = new System.Drawing.Size(74, 20);
            this.flatButton2.TabIndex = 7;
            this.flatButton2.Text = "By BISOON";
            this.flatButton2.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.flatButton2.Click += new System.EventHandler(this.flatButton1_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.closeBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeBtn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.closeBtn.Location = new System.Drawing.Point(294, 3);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Rounded = false;
            this.closeBtn.Size = new System.Drawing.Size(18, 18);
            this.closeBtn.TabIndex = 9;
            this.closeBtn.Text = "X";
            this.closeBtn.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // flatMini1
            // 
            this.flatMini1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flatMini1.BackColor = System.Drawing.Color.White;
            this.flatMini1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.flatMini1.Font = new System.Drawing.Font("Marlett", 12F);
            this.flatMini1.Location = new System.Drawing.Point(270, 0);
            this.flatMini1.Name = "flatMini1";
            this.flatMini1.Size = new System.Drawing.Size(18, 18);
            this.flatMini1.TabIndex = 10;
            this.flatMini1.Text = "flatMini1";
            this.flatMini1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 235);
            this.Controls.Add(this.frm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GM | By BISOON";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.frm.ResumeLayout(false);
            this.frm.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private theme.FormSkin frm;
        private theme.FlatButton conBtn;
        private theme.FlatButton startBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton dexCh;
        private System.Windows.Forms.RadioButton cexCh;
        private theme.FlatButton flatButton2;
        private theme.FlatButton flatButton1;
        private theme.FlatMini flatMini1;
        private theme.FlatButton closeBtn;
    }
}

