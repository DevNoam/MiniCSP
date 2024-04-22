namespace _365
{
    partial class Logs
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
            id = new Label();
            SuspendLayout();
            // 
            // id
            // 
            id.AutoSize = true;
            id.ForeColor = Color.Cornsilk;
            id.Location = new Point(88, 243);
            id.Name = "id";
            id.Size = new Size(38, 15);
            id.TabIndex = 0;
            id.Text = "label1";
            // 
            // Logs
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(227, 504);
            Controls.Add(id);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Logs";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Logs";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label id;
    }
}