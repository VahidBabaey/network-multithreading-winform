namespace clientapp
{
    partial class FormConf
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
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ListMessage = new System.Windows.Forms.ListBox();
            this.ListClients = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(153, 567);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(615, 20);
            this.txtMessage.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(776, 570);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "پیام:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(110, 566);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 25);
            this.button1.TabIndex = 10;
            this.button1.Text = ">";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Send_Click);
            // 
            // ListMessage
            // 
            this.ListMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ListMessage.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.ListMessage.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.ListMessage.FormattingEnabled = true;
            this.ListMessage.ItemHeight = 16;
            this.ListMessage.Location = new System.Drawing.Point(108, 24);
            this.ListMessage.Name = "ListMessage";
            this.ListMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ListMessage.Size = new System.Drawing.Size(660, 532);
            this.ListMessage.TabIndex = 7;
            // 
            // ListClients
            // 
            this.ListClients.FormattingEnabled = true;
            this.ListClients.Location = new System.Drawing.Point(776, 24);
            this.ListClients.Name = "ListClients";
            this.ListClients.Size = new System.Drawing.Size(215, 524);
            this.ListClients.TabIndex = 6;
            // 
            // FormConf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1099, 614);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ListMessage);
            this.Controls.Add(this.ListClients);
            this.Name = "FormConf";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "کنفرانس";
            this.Load += new System.EventHandler(this.FormConf_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox ListMessage;
        private System.Windows.Forms.ListBox ListClients;
    }
}