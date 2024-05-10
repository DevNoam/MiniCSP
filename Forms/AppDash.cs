using _365.Core;
using _365.Core.Database;
using _365.Core.Properties;
using Sungaila.ImmersiveDarkMode.WinForms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;


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
        private System.Windows.Forms.Timer? searchTimer;

        #region contextbutton

        private const int WS_SYSMENU = 0x80000;
        private const int MF_SEPARATOR = 0x800;
        private const int MF_BYPOSITION = 0x400;
        private const int MF_STRING = 0x0;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        private const int ChangeDBContextId = 1002;
        private const int ExportToJsonContextId = 1003;
        private const int ImportFromJsonContextId = 1004;
        private const int DeveloperInfoContextId = 1005;
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
                AppendMenu(systemMenuHandle, MF_STRING, ChangeDBContextId, "Set Database");
                AppendMenu(systemMenuHandle, MF_SEPARATOR, 0, string.Empty);
                AppendMenu(systemMenuHandle, MF_STRING, ExportToJsonContextId, "Export to json");
                AppendMenu(systemMenuHandle, MF_STRING, ImportFromJsonContextId, "Import from json");
                AppendMenu(systemMenuHandle, MF_SEPARATOR, 0, string.Empty);
                AppendMenu(systemMenuHandle, MF_STRING, DeveloperInfoContextId, "About & Developer Info");
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            WindowExtensions.CheckAppsThemeChanged(m);


            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                int menuID = m.WParam.ToInt32();
                if (menuID == ChangeDBContextId)
                {
                    DatabaseManager.ReplaceDatabase();
                }
                if (menuID == ExportToJsonContextId)
                {
                    ExportToJSON ExportInstance = new ExportToJSON();
                    ExportInstance.Export();
                }
                if (menuID == ImportFromJsonContextId)
                {
                    this.Text = AppTitle + " - Importing..";
                    ImportFromJson importInstance = new ImportFromJson();
                    importInstance.ImportJson(AccountList.Items.Count);
                    FetchAll();
                }
                if (menuID == DeveloperInfoContextId)
                {
                    MessageBox.Show($"MiniCSP is an app developed by DevNoam (Noam Sapir) for Microsoft 365 resellers. Report bugs to: contact@noamsapir.me", "Developer and app info");
                }

            }
        }
        #endregion
        public AppDash()
        {
            InitializeComponent();
            AppTitle = this.Text;
            this.SetTitlebarTheme();
            searchTimer = new System.Windows.Forms.Timer();
            searchTimer.Interval = 450; // Set delay time to 500 milliseconds
            searchTimer.Tick += SearchTimer_Tick;
        }
        private void AppDash_Shown(object sender, EventArgs e)
        {
            FetchAll(0);
        }

        private async void AccountList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editMode is true)
                return;
            mfaToken = null;
            Edit.Enabled = false;
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer = null;
            }
            // Check if an item is selected
            AccountListEntry buttonId = (AccountListEntry)AccountList.SelectedItem;
            int selectIndex = AccountList.SelectedIndex;
            await Task.Delay(350);
            if (selectIndex != AccountList.SelectedIndex)
                return;
            if (buttonId != null && buttonId.id != -1)
            {
                // Get the selected button ID
                GC.Collect();
                AccountProp account = DatabaseManager.FetchAccount(buttonId.id);
                if (account != null)
                {
                    //Set properties
                    selectedId = buttonId.id;
                    CustomerName.Text = HttpUtility.HtmlDecode(account.customerName);
                    while (CustomerName.Width < System.Windows.Forms.TextRenderer.MeasureText(CustomerName.Text,
                    new Font(CustomerName.Font.FontFamily, CustomerName.Font.Size, CustomerName.Font.Style)).Width)
                    {
                        CustomerName.Font = new Font(CustomerName.Font.FontFamily, CustomerName.Font.Size - 0.5f, CustomerName.Font.Style);
                    }
                    Crm.Text = "CRM: " + account.crmNumber;
                    Email.Text = account.email;
                    Password.Text = account.password;
                    mfaToken = account.mfaToken;
                    Domain.Text = account.domain;
                    Phone.Text = account.phone;
                    Notes.Text = HttpUtility.HtmlDecode(account.notes);
                    isArchived.Checked = (account.isArchived == 1) ? true : false;
                    ModifiedDate.Text = "Last modified: " + account.modifyDate;
                    SelectACustomer.Visible = false;
                    Edit.Enabled = true;

                    TokenGenerator(mfaToken);
                }
            }
            else
            {
                SelectACustomer.Visible = true;
                SelectACustomer.Enabled = true;
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

        private void Search(object sender, EventArgs e)
        {
            searchTimer.Stop();
            searchTimer.Start();
        }
        private async void SearchTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            searchTimer.Stop();
            // Perform the search operation after the delay
            await SearchAsync(AccountSearcher.Text);
        }


        private async Task SearchAsync(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                AccountListEntry[] accounts = DatabaseManager.SearchEntries(searchText);
                List<AccountListEntry> archivedEntries = new List<AccountListEntry>();
                if (accounts.Length > 0)
                {
                    AccountList.Items.Clear();
                    foreach (var account in accounts)
                    {
                        if (account.isArchived == true)
                        {
                            archivedEntries.Add(account);
                            continue;
                        }
                        AccountList.Items.Add(account);
                    }
                    //Create Archive section
                    if (archivedEntries.Count() > 0)
                    {
                        AccountList.Items.Add(new AccountListEntry { customerName = "---- Archives ----", id = -1 });
                        foreach (var account in archivedEntries)
                        {
                            AccountList.Items.Add(account);
                        }
                    }
                }
                UpdateEntriesNumber();
            }
            else if (string.IsNullOrEmpty(searchText))
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
            List<AccountListEntry> archivedEntries = new List<AccountListEntry>();

            foreach (var account in accounts)
            {
                if (account.isArchived == true)
                {
                    archivedEntries.Add(account);
                }
                else
                {
                    AccountList.Items.Add(account);
                    SelectEntry(selectId, account);
                }
            }
            if (archivedEntries.Count() > 0)
            {
                AccountList.Items.Add(new AccountListEntry { customerName = "---- Archives ----", id = -1 });
                foreach (var account in archivedEntries)
                {
                    AccountList.Items.Add(account);
                    SelectEntry(selectId, account);
                }

            }

            void SelectEntry(int selectId, AccountListEntry item)
            {
                if (selectId != -1 && item.id == selectId)
                    AccountList.SelectedItem = item;
            }


            UpdateEntriesNumber();
        }
        private void Crm_Click(object sender, EventArgs e)
        {
            if (editMode == false)
            {
                if (Crm.Text.Length > 5)
                    Clipboard.SetText(Crm.Text.Remove(0, 5));
            }
            else if (editMode == true)
                EditCoreAccount();
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
        private void CopyDomain(object sender, EventArgs e)
        {
            if (!editMode)
            {
                Domain.SelectAll();
                if (!string.IsNullOrEmpty(Domain.Text))
                    Clipboard.SetText(Domain.Text);
            }
        }
        private void CopyPhone(object sender, EventArgs e)
        {
            if (!editMode)
            {
                Phone.SelectAll();
                if (!string.IsNullOrEmpty(Phone.Text))
                    Clipboard.SetText(Phone.Text);
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
        private void CustomerName_Click(object sender, EventArgs e)
        {

            if (editMode == false)
                Clipboard.SetText(CustomerName.Text);
            else if (editMode == true)
                EditCoreAccount();

        }

        private void EditCoreAccount()
        {
            CreateEntry editEntry = new CreateEntry();
            NewEntry entry = new NewEntry() { customerName = CustomerName.Text, crmNumber = Crm.Text.Remove(0, 5) };
            editEntry.InitEditMode(selectedId, entry);
            editEntry.ShowDialog();

            if (editEntry.accEntry != null)
            {
                if (editEntry.accEntry.crmNumber != Crm.Text)
                    Crm.Text = "CRM: " + editEntry.accEntry.crmNumber;
                if (editEntry.accEntry.customerName != CustomerName.Text)
                { 
                    CustomerName.Text = editEntry.accEntry.customerName;
                    FetchAll(selectedId);
                }

                ModifiedDate.Text = DateTime.Now.ToString();
            }
        }


        private void PasswordFieldDisableKey(object sender, KeyPressEventArgs e)
        {
            if (!editMode)
                e.Handled = true;
        }

        private void RevealPassword(object sender, EventArgs e)
        {
            Password.PasswordChar = '\0';
        }
        private void HidePassword(object sender, EventArgs e)
        {
            if (!editMode)
                Password.PasswordChar = '●';
        }

        private void UpdateEntriesNumber() => this.Text = AppTitle + " - Found: (" + AccountList.Items.Count.ToString() + ") accounts";

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
                    domain = Domain.Text,
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
                    domain = Domain.Text,
                    email = Email.Text,
                    isArchived = Convert.ToInt32(isArchived.Checked),
                    mfaToken = MFA.Text,
                    phone = Phone.Text,
                    password = Password.Text,
                    notes = Notes.Text,
                    modifyDate = DateTime.Now
                };

                if (newAccountProp.domain == EditAccount.oldAccountProp.domain && newAccountProp.email == EditAccount.oldAccountProp.email &&
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
                        ChangeEditState(false);
                        EditAccount = null;
                        FetchAll(newAccountProp.id);
                        //mfaToken = newAccountProp.mfaToken;
                        //ModifiedDate.Text = "Last modified: " + newAccountProp.modifyDate;
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
                Domain.Text = EditAccount.oldAccountProp.domain;
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
                    Password.ReadOnly = false;
                    MFA.ReadOnly = false;
                    MFA.Enabled = true;
                    Domain.ReadOnly = false;
                    Phone.ReadOnly = false;
                    Notes.ReadOnly = false;
                    isArchived.Enabled = true;
                    MFATimer.Visible = false;
                    MFA.Text = mfaToken;
                    UploadQR.Visible = true;
                    UploadQR.Enabled = true;
                    editMode = true;
                    RevealPassword(sender, e);
                    Edit.BackgroundImage = Properties.Resources.Save;
                }
                else if (edit is false)
                {
                    AccountList.Enabled = true;
                    AccountSearcher.Enabled = true;
                    AddAccount.Enabled = true;
                    Email.ReadOnly = true;
                    Password.ReadOnly = true;
                    MFA.ReadOnly = true;
                    Domain.ReadOnly = true;
                    Phone.ReadOnly = true;
                    Notes.ReadOnly = true;
                    isArchived.Enabled = false;
                    MFATimer.Visible = true;
                    UploadQR.Visible = false;
                    UploadQR.Enabled = false;
                    editMode = false;
                    HidePassword(sender, e);
                    Edit.BackgroundImage = Properties.Resources.Settings;

                    TokenGenerator(mfaToken);
                }
            }
        }


        private void AddAccount_Click(object sender, EventArgs e)
        {
            CreateEntry entryForm = new CreateEntry();
            entryForm.ShowDialog();
            if (entryForm.accEntry != null)
            {
                FetchAll(entryForm.accEntry.Id);
            }
        }

        private void UploadQR_Click(object sender, EventArgs e)
        {
            ExtractQR extractQR = new ExtractQR();
            QRTotp TOTP = extractQR.GetTOTP();

            if (TOTP.secret == "error")
            {
                MessageBox.Show("Reading QR failed", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!string.IsNullOrEmpty(TOTP.secret))
            {
                if (TOTP.mail != null && Email.Text.Contains(TOTP.mail))
                {
                    //Do nothing, thats okay.
                }
                else if (string.IsNullOrWhiteSpace(Email.Text))
                    Email.Text = TOTP.mail;
                else
                {
                    MessageBox.Show("Mail does not match this MFA key!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                MFA.Text = TOTP.secret;
            }
        }

        private void OpenAdminCenter_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://admin.microsoft.com/",
                UseShellExecute = true
            });
        }
    }
}