using _365.Core;
using _365.Core.Database;
using _365.Core.Properties;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Security.Principal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace _365
{
    public partial class Form1 : Form
    {
        private string? mfaToken = null;
        private System.Windows.Forms.Timer? countdownTimer;
        private DateTime endTime;
        private int selectedId = -1;
        private bool editMode = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Win_Load(object sender, EventArgs e)
        {
            AccountSearcher.PlaceholderText = "Search..";
            AccountListEntry[] accounts = DatabaseManager.FetchAllEntries();
            // Add buttons to the ListBox
            foreach (var account in accounts)
            {
                AccountList.Items.Add(account);
            }
        }

        private void AccountList_SelectedIndexChanged(object sender, EventArgs e)
        {
            mfaToken = null;
            Edit.Visible = false;
            countdownTimer = null;
            // Check if an item is selected
            if (AccountList.SelectedIndex != -1)
            {
                // Get the selected button ID
                AccountListEntry buttonId = (AccountListEntry)AccountList.SelectedItem;
                AccountProp account = DatabaseManager.FetchAccount(buttonId.id);
                if (account != null)
                {
                    //Set properties
                    selectedId = buttonId.id;
                    CustomerName.Text = account.customerName;
                    Password.Text = account.password;
                    Crm.Text = "CRM: " + account.crmNumber;
                    Email.Text = account.email;
                    isArchived.Checked = (account.isArchived == 1) ? true : false;
                    mfaToken = account.mfaToken;
                    ModifiedDate.Text = "Last modified: " + account.modifyDate;
                    TokenGenerator(mfaToken);
                    SelectACustomer.Visible = false;
                    Edit.Visible = true;
                }
            }
        }

        void TokenGenerator(string mfa)
        {
            if (editMode == true)
                return;
            Token token = TokenDecrypt.GetNumbers(mfa);
            MFA.Text = token.otp;
            UpdateMFA(TimeSpan.FromSeconds(token.remSeconds));
        }
        private void UpdateMFA(TimeSpan time)
        {
            endTime = DateTime.Now.Add(time);

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000; //1 second
            countdownTimer.Tick += MFA_Elapsed;
            MFA_Elapsed(null, null);
            countdownTimer.Start();
        }
        private void MFA_Elapsed(object? sender, EventArgs? e)
        {
            TimeSpan remainingTime = endTime - DateTime.Now;
            if (remainingTime.TotalSeconds >= 0)
                MFATimer.Value = (int)remainingTime.TotalSeconds;
            if (remainingTime <= TimeSpan.Zero)
            {
                countdownTimer.Stop();
                if (mfaToken != null)
                {
                    TokenGenerator(mfaToken);
                }
            }
        }

        private void Search(object sender, KeyEventArgs e)
        {
            string search = AccountSearcher.Text;
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (!string.IsNullOrEmpty(search))
                {
                    AccountListEntry[] results = DatabaseManager.SearchEntries(search);
                    if (results.Length > 0)
                    {
                        AccountList.Items.Clear();
                        foreach (var result in results)
                        {
                            AccountList.Items.Add(result);
                        }
                    }
                }
                else if (string.IsNullOrEmpty(search))
                {
                    if (DatabaseManager.GetAccountsCount() == AccountList.Items.Count)
                        return;
                    else
                    {
                        FetchAll();
                    }
                }
            }
        }

        private void FetchAll()
        {
            AccountList.Items.Clear();
            AccountListEntry[] accounts = DatabaseManager.FetchAllEntries();
            // Add buttons to the ListBox
            foreach (var account in accounts)
            {
                AccountList.Items.Add(account);
            }
        }


        private void Crm_Click(object sender, EventArgs e)
        {
            if (Crm.Text.Length > 5)
                Clipboard.SetText(Crm.Text.Remove(0, 5));
        }
        private void CopyPassword(object sender, EventArgs e)
        {
            if (!editMode)
            {
                Password.SelectAll();
                if (!string.IsNullOrEmpty(Password.Text))
                    Clipboard.SetText(Password.Text);
            }
        }
        private void CopyMail(object sender, EventArgs e)
        {
            if (!editMode)
            {
                Email.SelectAll();
                if (!string.IsNullOrEmpty(Email.Text))
                    Clipboard.SetText(Email.Text);
            }
        }
        private void CopyMFA(object sender, EventArgs e)
        {
            if (!editMode)
            {
                MFA.SelectAll();
                if (!string.IsNullOrEmpty(MFA.Text))
                    Clipboard.SetText(MFA.Text);
            }
        }
        private void CustomerName_Click(object sender, EventArgs e) => Clipboard.SetText(CustomerName.Text);

        private void PasswordFieldDisableKey(object sender, KeyPressEventArgs e)
        {
            if(!editMode)
                e.Handled = true;
        }

        private void RevealPassword(object sender, EventArgs e)
        {
            if (!editMode)
                Password.PasswordChar = '\0';
        }
        private void HidePassword(object sender, EventArgs e)
        {
            if (!editMode)
                Password.PasswordChar = '●';
        }

        AccountEdit EditAccount;
        private void Edit_Click(object sender, EventArgs e)
        {
            if (editMode is false)
            {
                //Start Edit mode

                EditAccount = new AccountEdit(); //Create EditAccount instance
                EditAccount.oldAccountProp = new AccountProp //Move current props to EditAccount
                {
                    id = selectedId,
                    domainMicrosoft = OnMicrosoftDomain.Text,
                    email = Email.Text,
                    isArchived = Convert.ToInt32(isArchived.Checked),
                    mfaToken = mfaToken,
                    phone = Phone.Text,
                    password = Password.Text,
                    notes = Notes.Text
                };


                //Change fields to editable
                ChangeEditState(true);
            }
            else if (editMode is true)
            {
                var selection = MessageBox.Show("Save?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (selection == DialogResult.Yes)
                {
                    AccountProp newAccountProp = new AccountProp
                    {
                        id = selectedId,

                        domainMicrosoft = OnMicrosoftDomain.Text,
                        email = Email.Text,
                        isArchived = Convert.ToInt32(isArchived.Checked),
                        mfaToken = MFA.Text,
                        phone = Phone.Text,
                        password = Password.Text,
                        notes = Notes.Text,
                        modifyDate = DateTime.Now
                    };


                    if (EditAccount.PublishEdit(newAccountProp) == true)
                    {
                        //Published
                        mfaToken = newAccountProp.mfaToken;
                        ChangeEditState(false);
                        ModifiedDate.Text = "Last modified: " + newAccountProp.modifyDate;
                        EditAccount = null;
                    }
                    else
                    {
                        var errorMessage = MessageBox.Show("ERROR, Not published", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (selection == DialogResult.No)
                {
                    CancelPuslich();
                }
            }


            void CancelPuslich()
            {
                OnMicrosoftDomain.Text = EditAccount.oldAccountProp.domainMicrosoft;
                Email.Text = EditAccount.oldAccountProp.email;
                isArchived.Checked = (EditAccount.oldAccountProp.isArchived == 1) ? true : false;
                Phone.Text = EditAccount.oldAccountProp.phone;
                Password.Text = EditAccount.oldAccountProp.password;
                Notes.Text = EditAccount.oldAccountProp.notes;
                mfaToken = EditAccount.oldAccountProp.mfaToken;

                ChangeEditState(false);
                EditAccount = null;
            }


            void ChangeEditState(bool edit)
            {
                if (edit is true)
                {
                    AccountList.Enabled = false;
                    AccountSearcher.Enabled = false;
                    AddAccount.Enabled = false;
                    Email.ReadOnly = false;
                    RevealPassword(sender, e);
                    Password.ReadOnly = false;
                    MFA.ReadOnly = false;
                    OnMicrosoftDomain.ReadOnly = false;
                    Phone.ReadOnly = false;
                    Notes.ReadOnly = false;
                    isArchived.Enabled = true;
                    MFATimer.Visible = false;
                    MFA.Text = mfaToken;
                    editMode = true;
                    Edit.Text = "SAVE?";
                }
                else if (edit is false)
                {
                    AccountList.Enabled = true;
                    AccountSearcher.Enabled = true;
                    AddAccount.Enabled = true;
                    Email.ReadOnly = true;
                    Password.ReadOnly = true;
                    HidePassword(sender, e);
                    MFA.ReadOnly = true;
                    OnMicrosoftDomain.ReadOnly = true;
                    Phone.ReadOnly = true;
                    Notes.ReadOnly = true;
                    isArchived.Enabled = false;
                    MFATimer.Visible = true;

                    editMode = false;
                    Edit.Text = "EDIT";
                    TokenGenerator(mfaToken);
                }
            }
        }

        private void BezeqClick(object sender, EventArgs e) {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://bezeqint.net/",
                UseShellExecute = true
            });
        }


    }
}