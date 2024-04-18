using _365.Core.Database;
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using Sungaila.ImmersiveDarkMode.WinForms;


namespace _365
{
    public partial class CreateEntry : Form
    {
        public AccountListEntry accEntry { get; private set; }

        public CreateEntry()
        {
            this.SetTitlebarTheme();
            InitializeComponent();
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CreateEntry_Load(object sender, EventArgs e)
        {
            resellerCheckBox.CheckedChanged += ResellerChecked;
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            WindowExtensions.CheckAppsThemeChanged(m);
        }

        void ResellerChecked(object sender, EventArgs e)
        {
            if (resellerCheckBox.Checked)
            {
                resellerPanel.Enabled = true;
                resellerPanel.Visible = true;
            }
            else
            {
                resellerPanel.Enabled = false;
                resellerPanel.Visible = false;
            }
        }
        private void TenantDomain_TextChanged(object sender, EventArgs e)
        {
            if (TenantDomain.Text.Contains("indirect"))
                resellerCheckBox.Checked = true;
        }

        private void Create_Click(object sender, EventArgs e)
        {
            if (resellerCheckBox.Checked == false && resellerCRMParent.Text != null)
                resellerCRMParent.Text = null;

            NewEntry entry = new NewEntry()
            {
                customerName = CRMCustomerName.Text,
                crmNumber = CRMNumber.Text,
                tenantDomain = TenantDomain.Text,
                reseller = resellerCheckBox.Checked,
                resellerCRMParent = resellerCRMParent.Text
            };



            if (DatabaseManager.SearchEntries(entry.crmNumber).Count() > 0)
            {
                var crmExist = MessageBox.Show("This CRM number exist in the databse, do you want to continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (crmExist == DialogResult.No)
                    return;
            }

            var choice = MessageBox.Show("You are about to create this account to the list, these fields are not changable. Continue?", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (choice == DialogResult.OK)
            {
                int resultId = DatabaseManager.CreateAccount(entry);


                accEntry = new AccountListEntry()
                {
                    id = resultId,
                    customerName = entry.customerName
                };

                this.Close();
            }
        }
    }
}
