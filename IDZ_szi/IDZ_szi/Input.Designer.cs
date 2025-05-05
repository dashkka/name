namespace IDZ_szi
{
    partial class Input
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Input));
            this.gradientPanel1 = new GradientPanelDemo.GradientPanel();
            this.tb_mail = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.entry = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.l_mail = new System.Windows.Forms.Label();
            this.l_password = new System.Windows.Forms.Label();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.Color.MediumPurple;
            this.gradientPanel1.ColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gradientPanel1.ColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(64)))), ((int)(((byte)(130)))));
            this.gradientPanel1.Controls.Add(this.tb_mail);
            this.gradientPanel1.Controls.Add(this.label2);
            this.gradientPanel1.Controls.Add(this.entry);
            this.gradientPanel1.Controls.Add(this.checkBox1);
            this.gradientPanel1.Controls.Add(this.label1);
            this.gradientPanel1.Controls.Add(this.l_mail);
            this.gradientPanel1.Controls.Add(this.l_password);
            this.gradientPanel1.Controls.Add(this.tb_password);
            this.gradientPanel1.Location = new System.Drawing.Point(-9, -1);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Size = new System.Drawing.Size(421, 324);
            this.gradientPanel1.TabIndex = 22;
            // 
            // tb_mail
            // 
            this.tb_mail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_mail.Location = new System.Drawing.Point(125, 111);
            this.tb_mail.Name = "tb_mail";
            this.tb_mail.Size = new System.Drawing.Size(256, 27);
            this.tb_mail.TabIndex = 21;
            this.tb_mail.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(-3, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(544, 22);
            this.label2.TabIndex = 21;
            this.label2.Text = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" +
    " - - - - - - - -";
            // 
            // entry
            // 
            this.entry.BackColor = System.Drawing.Color.White;
            this.entry.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.entry.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.entry.Location = new System.Drawing.Point(34, 220);
            this.entry.Name = "entry";
            this.entry.Size = new System.Drawing.Size(347, 52);
            this.entry.TabIndex = 16;
            this.entry.Text = "войти в учетную запись!";
            this.entry.UseVisualStyleBackColor = false;
            this.entry.UseWaitCursor = true;
            this.entry.Click += new System.EventHandler(this.entry_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox1.Location = new System.Drawing.Point(345, 158);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(14, 13);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.UseWaitCursor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(156, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 32);
            this.label1.TabIndex = 13;
            this.label1.Text = "ВХОД";
            // 
            // l_mail
            // 
            this.l_mail.AutoSize = true;
            this.l_mail.BackColor = System.Drawing.Color.Transparent;
            this.l_mail.Cursor = System.Windows.Forms.Cursors.Default;
            this.l_mail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.l_mail.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.l_mail.Location = new System.Drawing.Point(30, 114);
            this.l_mail.Name = "l_mail";
            this.l_mail.Size = new System.Drawing.Size(68, 22);
            this.l_mail.TabIndex = 20;
            this.l_mail.Text = "почта: ";
            // 
            // l_password
            // 
            this.l_password.AutoSize = true;
            this.l_password.BackColor = System.Drawing.Color.Transparent;
            this.l_password.Cursor = System.Windows.Forms.Cursors.Default;
            this.l_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.l_password.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.l_password.Location = new System.Drawing.Point(30, 155);
            this.l_password.Name = "l_password";
            this.l_password.Size = new System.Drawing.Size(79, 22);
            this.l_password.TabIndex = 18;
            this.l_password.Text = "пароль: ";
            // 
            // tb_password
            // 
            this.tb_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_password.Location = new System.Drawing.Point(125, 152);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(201, 27);
            this.tb_password.TabIndex = 17;
            this.tb_password.UseSystemPasswordChar = true;
            this.tb_password.UseWaitCursor = true;
            // 
            // Input
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(399, 301);
            this.Controls.Add(this.gradientPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Input";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "вход";
            this.Load += new System.EventHandler(this.Input_Load);
            this.gradientPanel1.ResumeLayout(false);
            this.gradientPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox tb_mail;
        private System.Windows.Forms.Label l_mail;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label l_password;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.Button entry;
        private System.Windows.Forms.Label label1;
        private GradientPanelDemo.GradientPanel gradientPanel1;
        private System.Windows.Forms.Label label2;
    }
}

