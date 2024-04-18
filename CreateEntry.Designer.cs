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
            Label label1;
            CRMCustomerName = new TextBox();
            label2 = new Label();
            label3 = new Label();
            CRMNumber = new TextBox();
            label4 = new Label();
            TenantDomain = new TextBox();
            label5 = new Label();
            resellerCheckBox = new CheckBox();
            resellerCRMParent = new TextBox();
            label6 = new Label();
            resellerPanel = new Panel();
            Create = new Button();
            label1 = new Label();
            resellerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 16F);
            label1.ForeColor = SystemColors.ControlLight;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(388, 30);
            label1.TabIndex = 0;
            label1.Text = "Add new 365 account to the list";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CRMCustomerName
            // 
            CRMCustomerName.Location = new Point(66, 77);
            CRMCustomerName.Name = "CRMCustomerName";
            CRMCustomerName.Size = new Size(246, 23);
            CRMCustomerName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(66, 59);
            label2.Name = "label2";
            label2.Size = new Size(121, 15);
            label2.TabIndex = 2;
            label2.Text = "CRM Customer name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(66, 113);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 3;
            label3.Text = "CRM number";
            // 
            // CRMNumber
            // 
            CRMNumber.Location = new Point(66, 131);
            CRMNumber.Name = "CRMNumber";
            CRMNumber.Size = new Size(246, 23);
            CRMNumber.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 11F);
            label4.ForeColor = Color.Maroon;
            label4.Location = new Point(12, 30);
            label4.Name = "label4";
            label4.Size = new Size(373, 20);
            label4.TabIndex = 5;
            label4.Text = "* These fields are permanent and cannot be changed.";
            // 
            // TenantDomain
            // 
            TenantDomain.Location = new Point(66, 188);
            TenantDomain.Name = "TenantDomain";
            TenantDomain.Size = new Size(246, 23);
            TenantDomain.TabIndex = 7;
            TenantDomain.TextChanged += TenantDomain_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.Control;
            label5.Location = new Point(66, 170);
            label5.Name = "label5";
            label5.Size = new Size(212, 15);
            label5.TabIndex = 6;
            label5.Text = "Tenant domain (OnMicrosoft/ Indirect)";
            // 
            // resellerCheckBox
            // 
            resellerCheckBox.AutoSize = true;
            resellerCheckBox.ForeColor = SystemColors.Control;
            resellerCheckBox.Location = new Point(68, 222);
            resellerCheckBox.Name = "resellerCheckBox";
            resellerCheckBox.Size = new Size(103, 19);
            resellerCheckBox.TabIndex = 8;
            resellerCheckBox.Text = "Reseller tenant";
            resellerCheckBox.UseVisualStyleBackColor = true;
            // 
            // resellerCRMParent
            // 
            resellerCRMParent.Location = new Point(3, 36);
            resellerCRMParent.Name = "resellerCRMParent";
            resellerCRMParent.Size = new Size(121, 23);
            resellerCRMParent.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.Control;
            label6.Location = new Point(3, 18);
            label6.Name = "label6";
            label6.Size = new Size(121, 15);
            label6.TabIndex = 9;
            label6.Text = "Reseller CRM number";
            // 
            // resellerPanel
            // 
            resellerPanel.BackColor = Color.Transparent;
            resellerPanel.Controls.Add(label6);
            resellerPanel.Controls.Add(resellerCRMParent);
            resellerPanel.Enabled = false;
            resellerPanel.Location = new Point(177, 196);
            resellerPanel.Name = "resellerPanel";
            resellerPanel.Size = new Size(141, 62);
            resellerPanel.TabIndex = 11;
            resellerPanel.Visible = false;
            // 
            // Create
            // 
            Create.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Create.Dock = DockStyle.Bottom;
            Create.FlatStyle = FlatStyle.System;
            Create.Location = new Point(0, 277);
            Create.Name = "Create";
            Create.Size = new Size(388, 39);
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
            ClientSize = new Size(388, 316);
            Controls.Add(Create);
            Controls.Add(resellerCheckBox);
            Controls.Add(TenantDomain);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(CRMNumber);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(CRMCustomerName);
            Controls.Add(label1);
            Controls.Add(resellerPanel);
            DoubleBuffered = true;
            ForeColor = SystemColors.ControlText;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateEntry";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add account";
            Load += CreateEntry_Load;
            resellerPanel.ResumeLayout(false);
            resellerPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox CRMCustomerName;
        private Label label2;
        private Label label3;
        private TextBox CRMNumber;
        private Label label4;
        private TextBox TenantDomain;
        private Label label5;
        private CheckBox resellerCheckBox;
        private TextBox resellerCRMParent;
        private Label label6;
        private Panel resellerPanel;
        private Button Create;
    }
}