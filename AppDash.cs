using _365.Core;
using _365.Core.Database;
using _365.Core.Properties;
using Sungaila.ImmersiveDarkMode.WinForms;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace _365
{
    public partial class AppDash : Form
    {
        private string? mfaToken = null;
        private System.Windows.Forms.Timer? countdownTimer;
        private DateTime endTime;
        private int selectedId = -1;
        private bool editMode = false;
        private string AppTitle;

        #region contextbutton

        private const int WS_SYSMENU = 0x80000;
        private const int MF_SEPARATOR = 0x800;
        private const int MF_BYPOSITION = 0x400;
        private const int MF_STRING = 0x0;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        private const int CustomMenuItemID = 1002;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_SYSMENU;
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Add custom button to system menu
            IntPtr systemMenuHandle = GetSystemMenu(this.Handle, false);
            if (systemMenuHandle != IntPtr.Zero)
            {
                AppendMenu(systemMenuHandle, MF_STRING, CustomMenuItemID, "Set Database");
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            WindowExtensions.CheckAppsThemeChanged(m);


            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                int menuID = m.WParam.ToInt32();
                if (menuID == CustomMenuItemID)
                {
                    DatabaseManager.ReplaceDatabase();
                }
            }
        }
        #endregion
        public AppDash()
        {
            InitializeComponent();
            AppTitle = this.Text;
            this.SetTitlebarTheme();
        }
        private void AppDash_Shown(object sender, EventArgs e)
        {
            FetchAll(0);
        }

        private void AccountList_SelectedIndexChanged(object sender, EventArgs e)
        {
            mfaToken = null;
            Edit.Enabled = false;
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer = null;
            }
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
                    while (CustomerName.Width < System.Windows.Forms.TextRenderer.MeasureText(CustomerName.Text,
                    new Font(CustomerName.Font.FontFamily, CustomerName.Font.Size, CustomerName.Font.Style)).Width)
                    {
                        CustomerName.Font = new Font(CustomerName.Font.FontFamily, CustomerName.Font.Size - 0.5f, CustomerName.Font.Style);
                    }
                    Crm.Text = "CRM: " + account.crmNumber;
                    Email.Text = account.email;
                    Password.Text = account.password;
                    mfaToken = account.mfaToken;
                    OnMicrosoftDomain.Text = account.domainMicrosoft;
                    Phone.Text = account.phone;
                    Notes.Text = account.notes;
                    isArchived.Checked = (account.isArchived == 1) ? true : false;
                    ModifiedDate.Text = "Last modified: " + account.modifyDate;
                    SelectACustomer.Visible = false;
                    Edit.Enabled = true;

                    TokenGenerator(mfaToken);
                }
            }
        }

        void TokenGenerator(string mfa = "")
        {
            if (editMode == true)
                return;
            if (string.IsNullOrEmpty(mfa) || mfa == "ERROR")
            {
                MFA.Enabled = false;
                MFATimer.Enabled = false;
                MFA.Text = "";
                MFATimer.Visible = false;
                return;
            }
            else
            {
                MFA.Enabled = true;
                MFATimer.Enabled = true;
                MFATimer.Visible = true;
            }
            Token token = TokenDecrypt.GetNumbers(mfa);
            if (token.otp == "ERROR")
            {
                TokenGenerator("ERROR");
                return;
            }
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
                if (countdownTimer != null)
                    countdownTimer.Stop();
                if (Clipboard.GetText() == MFA.Text)
                    Clipboard.Clear();
                if (mfaToken != null)
                {
                    TokenGenerator(mfaToken);
                }
            }
        }
        private void Search(object sender, KeyEventArgs e)
        {
            string search = AccountSearcher.Text;
            e.SuppressKeyPress = true;
            if (!string.IsNullOrEmpty(search))
            {
                AccountListEntry[] results = DatabaseManager.SearchEntries(search);
                if (results.Length > 0)
                {
                    AccountList.Items.Clear();
                    foreach (var result in results)
                        AccountList.Items.Add(result);
                }
                UpdateEntriesNumber();
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
        private void FetchAll(int selectId = -1)
        {
            AccountList.Items.Clear();
            AccountListEntry[] accounts = DatabaseManager.FetchAllEntries();
            // Add buttons to the ListBox

            foreach (var account in accounts)
            {
                AccountList.Items.Add(account);
                if (selectedId != -1 && account.id == selectId)
                    AccountList.SelectedItem = account;
            }
            UpdateEntriesNumber();
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
            if (!editMode)
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

        private void UpdateEntriesNumber() => this.Text = AppTitle + " Found: (" + AccountList.Items.Count.ToString() + ") accounts";

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

                if (newAccountProp.domainMicrosoft == EditAccount.oldAccountProp.domainMicrosoft && newAccountProp.email == EditAccount.oldAccountProp.email &&
                    newAccountProp.isArchived == EditAccount.oldAccountProp.isArchived && newAccountProp.phone == EditAccount.oldAccountProp.phone &&
                    newAccountProp.password == EditAccount.oldAccountProp.password && newAccountProp.notes == EditAccount.oldAccountProp.notes && newAccountProp.mfaToken == EditAccount.oldAccountProp.mfaToken)
                {
                    CancelPuslish();
                    return;
                }

                var selection = MessageBox.Show("Save?", System.Windows.Forms.Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (selection == DialogResult.Yes)
                {
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
                        var errorMessage = MessageBox.Show("ERROR, Not published", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (selection == DialogResult.No)
                {
                    CancelPuslish();
                }
            }


            void CancelPuslish()
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
                    MFA.Enabled = true;
                    OnMicrosoftDomain.ReadOnly = false;
                    Phone.ReadOnly = false;
                    Notes.ReadOnly = false;
                    isArchived.Enabled = true;
                    MFATimer.Visible = false;
                    MFA.Text = mfaToken;
                    editMode = true;
                    Edit.BackgroundImage = Properties.Resources.Save;
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
                    Edit.BackgroundImage = Properties.Resources.Settings;

                    TokenGenerator(mfaToken);
                }
            }
        }

        private void BezeqClick(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://bezeqint.net/",
                UseShellExecute = true
            });
        }

        private void AddAccount_Click(object sender, EventArgs e)
        {
            CreateEntry entryForm = new CreateEntry();
            entryForm.ShowDialog();
            if (entryForm.accEntry != null)
                FetchAll(entryForm.accEntry.id);
        }
    }
}