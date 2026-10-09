using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CampManagementSystem
{
    // DFD Process 1: Authenticate User
    public partial class LoginForm : Form
    {
        private string currentRole = "";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            rbCustomer.Checked = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter both username and password.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (rbAdmin.Checked)
                    currentRole = AuthenticateAdmin(user, pass);
                else
                    currentRole = AuthenticateCustomer(user, pass);

                if (!string.IsNullOrEmpty(currentRole))
                {
                    MessageBox.Show("Login successful. Welcome " + currentRole + "!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Close this login form and open the full-screen dashboard.
                    // Using Show() (not ShowDialog) + Close() ensures only one window
                    // exists at a time — clicking a menu in the dashboard does NOT
                    // touch the login form because it is already closed.
                    this.Hide();
                    MainForm main = new MainForm(currentRole);
                    main.WindowState = FormWindowState.Maximized;
                    main.FormClosed += (s, args) =>
                    {
                        // When the dashboard closes (Logout / X button), exit the app.
                        // The login form was hidden so the user never sees it again.
                        Application.Exit();
                    };
                    main.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string AuthenticateAdmin(string username, string password)
        {
            string sql = "SELECT FullName FROM dbo.Admin WHERE Username = @u AND Password = @p";
            SqlParameter[] p = {
                new SqlParameter("@u", username),
                new SqlParameter("@p", password)
            };
            object result = DatabaseHelper.ExecuteScalar(sql, p);
            return result != null ? result.ToString() : "";
        }

        private string AuthenticateCustomer(string username, string password)
        {
            string sql = "SELECT FullName FROM dbo.Customer WHERE Email = @u AND Password = @p";
            SqlParameter[] p = {
                new SqlParameter("@u", username),
                new SqlParameter("@p", password)
            };
            object result = DatabaseHelper.ExecuteScalar(sql, p);
            return result != null ? result.ToString() : "";
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = Prompt("Enter your Full Name:", "Customer Registration");
            if (string.IsNullOrWhiteSpace(fullName)) return;

            string email = Prompt("Enter your Email:", "Customer Registration");
            if (string.IsNullOrWhiteSpace(email)) return;

            string pass = Prompt("Enter a Password:", "Customer Registration");
            if (string.IsNullOrWhiteSpace(pass)) return;

            try
            {
                string sql = "INSERT INTO dbo.Customer (FullName, Email, Password) VALUES (@n, @e, @p)";
                SqlParameter[] p = {
                    new SqlParameter("@n", fullName),
                    new SqlParameter("@e", email),
                    new SqlParameter("@p", pass)
                };
                int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
                if (rows > 0)
                {
                    MessageBox.Show("Registration successful! You can now log in.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string Prompt(string text, string title)
        {
            Form prompt = new Form()
            {
                Width = 360,
                Height = 160,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };
            Label lbl = new Label() { Left = 10, Top = 10, Width = 320, Text = text };
            TextBox txt = new TextBox() { Left = 10, Top = 40, Width = 320 };
            Button ok = new Button() { Text = "OK", Left = 170, Top = 75, Width = 75, DialogResult = DialogResult.OK };
            Button ca = new Button() { Text = "Cancel", Left = 255, Top = 75, Width = 75, DialogResult = DialogResult.Cancel };
            prompt.Controls.Add(lbl);
            prompt.Controls.Add(txt);
            prompt.Controls.Add(ok);
            prompt.Controls.Add(ca);
            prompt.AcceptButton = ok;
            prompt.CancelButton = ca;
            return prompt.ShowDialog() == DialogResult.OK ? txt.Text : "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            pnlSidebar = new Panel();
            pnlContent = new Panel();
            label1 = new Label();
            rbCustomer = new RadioButton();
            rbAdmin = new RadioButton();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            btnRegister = new Button();
            btnClear = new Button();
            btnExit = new Button();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(45, 55, 72);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(4, 5, 4, 5);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1286, 100);
            pnlHeader.TabIndex = 2;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(21, 30);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(377, 38);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Camp Management System";
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(237, 240, 245);
            pnlSidebar.Dock = DockStyle.Fill;
            pnlSidebar.Location = new Point(0, 100);
            pnlSidebar.Margin = new Padding(4, 5, 4, 5);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(1286, 900);
            pnlSidebar.TabIndex = 1;
            pnlSidebar.Paint += pnlSidebar_Paint;
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(label1);
            pnlContent.Controls.Add(rbCustomer);
            pnlContent.Controls.Add(rbAdmin);
            pnlContent.Controls.Add(lblUsername);
            pnlContent.Controls.Add(txtUsername);
            pnlContent.Controls.Add(lblPassword);
            pnlContent.Controls.Add(txtPassword);
            pnlContent.Controls.Add(btnLogin);
            pnlContent.Controls.Add(btnRegister);
            pnlContent.Controls.Add(btnClear);
            pnlContent.Controls.Add(btnExit);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 100);
            pnlContent.Margin = new Padding(4, 5, 4, 5);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(1286, 900);
            pnlContent.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(343, 233);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(50, 25);
            label1.TabIndex = 0;
            label1.Text = "Role:";
            // 
            // rbCustomer
            // 
            rbCustomer.AutoSize = true;
            rbCustomer.Location = new Point(429, 230);
            rbCustomer.Margin = new Padding(4, 5, 4, 5);
            rbCustomer.Name = "rbCustomer";
            rbCustomer.Size = new Size(114, 29);
            rbCustomer.TabIndex = 1;
            rbCustomer.Text = "Customer";
            // 
            // rbAdmin
            // 
            rbAdmin.AutoSize = true;
            rbAdmin.Location = new Point(557, 230);
            rbAdmin.Margin = new Padding(4, 5, 4, 5);
            rbAdmin.Name = "rbAdmin";
            rbAdmin.Size = new Size(90, 29);
            rbAdmin.TabIndex = 2;
            rbAdmin.Text = "Admin";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(343, 300);
            lblUsername.Margin = new Padding(4, 0, 4, 0);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(95, 25);
            lblUsername.TabIndex = 3;
            lblUsername.Text = "Username:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(471, 293);
            txtUsername.Margin = new Padding(4, 5, 4, 5);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(313, 31);
            txtUsername.TabIndex = 4;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(343, 367);
            lblPassword.Margin = new Padding(4, 0, 4, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(91, 25);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(471, 360);
            txtPassword.Margin = new Padding(4, 5, 4, 5);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(313, 31);
            txtPassword.TabIndex = 6;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(471, 433);
            btnLogin.Margin = new Padding(4, 5, 4, 5);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(114, 50);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(600, 433);
            btnRegister.Margin = new Padding(4, 5, 4, 5);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(114, 50);
            btnRegister.TabIndex = 8;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(729, 433);
            btnClear.Margin = new Padding(4, 5, 4, 5);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(114, 50);
            btnClear.TabIndex = 9;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(600, 508);
            btnExit.Margin = new Padding(4, 5, 4, 5);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(114, 50);
            btnExit.TabIndex = 10;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1286, 1000);
            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login - Camp Management System";
            Load += LoginForm_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlContent;
        private Label lblTitle;
        private Label label1;
        private RadioButton rbCustomer;
        private RadioButton rbAdmin;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnClear;
        private Button btnExit;

        private void pnlSidebar_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}