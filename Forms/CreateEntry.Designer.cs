namespace _365
{
    partial class CreateEntry
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
            Title1 = new Label();
            Title2 = new Label();
            CRMCustomerName = new TextBox();
            label2 = new Label();
            label3 = new Label();
            CRMNumber = new TextBox();
            Create = new Button();
            SuspendLayout();
            // 
            // Title1
            // 
            Title1.Dock = DockStyle.Top;
            Title1.Font = new Font("Segoe UI", 16F);
            Title1.ForeColor = SystemColors.ControlLight;
            Title1.Location = new Point(0, 0);
            Title1.Name = "Title1";
            Title1.Size = new Size(306, 30);
            Title1.TabIndex = 0;
            Title1.Text = "New 365 account to the list";
            Title1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Title2
            // 
            Title2.BackColor = Color.Transparent;
            Title2.Font = new Font("Segoe UI", 10F);
            Title2.ForeColor = Color.Crimson;
            Title2.Location = new Point(0, 17);
            Title2.Name = "Title2";
            Title2.Size = new Size(306, 34);
            Title2.TabIndex = 13;
            Title2.Text = "New account listing are permanent!";
            Title2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CRMCustomerName
            // 
            CRMCustomerName.Location = new Point(32, 67);
            CRMCustomerName.Name = "CRMCustomerName";
            CRMCustomerName.Size = new Size(246, 23);
            CRMCustomerName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(32, 49);
            label2.Name = "label2";
            label2.Size = new Size(137, 15);
            label2.TabIndex = 2;
            label2.Text = "* Customer name (CRM)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(32, 96);
            label3.Name = "label3";
            label3.Size = new Size(86, 15);
            label3.TabIndex = 3;
            label3.Text = "* CRM number";
            // 
            // CRMNumber
            // 
            CRMNumber.Location = new Point(32, 114);
            CRMNumber.Name = "CRMNumber";
            CRMNumber.Size = new Size(246, 23);
            CRMNumber.TabIndex = 4;
            // 
            // Create
            // 
            Create.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Create.Dock = DockStyle.Bottom;
            Create.FlatStyle = FlatStyle.System;
            Create.Location = new Point(0, 156);
            Create.Name = "Create";
            Create.Size = new Size(306, 39);
            Create.TabIndex = 12;
            Create.Text = "Create";
            Create.UseVisualStyleBackColor = true;
            Create.Click += Create_Click;
            // 
            // CreateEntry
            // 
            AcceptButton = Create;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            BackgroundImage = Properties.Resources.grid_elements_01;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(306, 195);
            Controls.Add(Create);
            Controls.Add(CRMNumber);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(CRMCustomerName);
            Controls.Add(Title1);
            Controls.Add(Title2);
            DoubleBuffered = true;
            ForeColor = SystemColors.ControlText;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateEntry";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add account";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title1;
        private TextBox CRMCustomerName;
        private Label label2;
        private Label label3;
        private TextBox CRMNumber;
        private Button Create;
        private Label Title2;
    }
}