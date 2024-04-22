using _365.Core.Database;
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using Sungaila.ImmersiveDarkMode.WinForms;
using ZXing;


namespace _365
{
    public partial class CreateEntry : Form
    {
        public NewEntry accEntry { get; private set; }
        private bool editMode = false;
        private int selectedId;
        public CreateEntry()
        {
            this.SetTitlebarTheme();
            InitializeComponent();
        }
        public void InitEditMode(int id, NewEntry entry)
        {
            editMode = true;
            selectedId = id;
            Title1.Text = "Edit core account";
            Title2.Visible = false;
            this.Text = "Edit core account";
            CRMCustomerName.Text = entry.customerName;
            CRMNumber.Text = entry.crmNumber;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            WindowExtensions.CheckAppsThemeChanged(m);
        }

        private void Create_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CRMCustomerName.Text) || string.IsNullOrWhiteSpace(CRMNumber.Text))
            {
                MessageBox.Show("Cannot leave blank fields", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            NewEntry entry = new NewEntry()
            {
                customerName = CRMCustomerName.Text,
                crmNumber = CRMNumber.Text,
            };

            if (DatabaseManager.SearchEntries(entry.crmNumber).Count() > 0)
            {
                var crmExist = MessageBox.Show("This CRM number exist in the databse, do you wish to continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (crmExist == DialogResult.No)
                    return;
            }


            if (editMode == false)
                CreateAccountSubmit(entry);
            else if (editMode == true)
                EditAccountSubmit(entry);

        }


        void CreateAccountSubmit(NewEntry tmpEntry)
        {
            var choice = MessageBox.Show("You are about to create this account to the list, YOU CANNOT delete new accounts. Continue?", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (choice == DialogResult.OK)
            {
                int resultId = DatabaseManager.CreateAccount(tmpEntry);

                accEntry = new NewEntry()
                {
                    Id = resultId,
                    customerName = tmpEntry.customerName
                };
                this.Close();
            }
        }

        void EditAccountSubmit(NewEntry tmpEntry)
        {
            var choice = MessageBox.Show("You are about to edit this core account fields. Continue?", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (choice == DialogResult.OK)
            {
                bool updated = DatabaseManager.EditCoreAccount(selectedId, tmpEntry);
                if (updated)
                {
                    accEntry = new NewEntry()
                    {
                        customerName = tmpEntry.customerName,
                        crmNumber = tmpEntry.crmNumber
                    };
                }
                else
                { 
                    var selection = MessageBox.Show("Saving failed, try again?", System.Windows.Forms.Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if(selection == DialogResult.Yes)
                        EditAccountSubmit(tmpEntry);
                }
                this.Close();
            }
        }

    }
}
