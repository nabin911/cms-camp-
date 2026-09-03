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

                    MainForm main = new MainForm(currentRole);
                    this.Hide();
                    main.ShowDialog();
                    this.Show();
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
            this.lblTitle = new Label();
            this.label1 = new Label();
            this.rbCustomer = new RadioButton();
            this.rbAdmin = new RadioButton();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.btnClear = new Button();
            this.btnExit = new Button();
            SuspendLayout();
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(80, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(280, 30);
            lblTitle.Text = "Camp Booking System - Login";
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(80, 65);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(60, 15);
            label1.Text = "Role:";
            //
            // rbCustomer
            //
            rbCustomer.AutoSize = true;
            rbCustomer.Location = new System.Drawing.Point(140, 63);
            rbCustomer.Name = "rbCustomer";
            rbCustomer.Size = new System.Drawing.Size(74, 19);
            rbCustomer.TabStop = true;
            rbCustomer.Text = "Customer";
            //
            // rbAdmin
            //
            rbAdmin.AutoSize = true;
            rbAdmin.Location = new System.Drawing.Point(230, 63);
            rbAdmin.Name = "rbAdmin";
            rbAdmin.Size = new System.Drawing.Size(60, 19);
            rbAdmin.Text = "Admin";
            //
            // lblUsername
            //
            lblUsername.AutoSize = true;
            lblUsername.Location = new System.Drawing.Point(80, 110);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(78, 15);
            lblUsername.Text = "Username:";
            //
            // txtUsername
            //
            txtUsername.Location = new System.Drawing.Point(170, 107);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(180, 23);
            //
            // lblPassword
            //
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(80, 150);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(69, 15);
            lblPassword.Text = "Password:";
            //
            // txtPassword
            //
            txtPassword.Location = new System.Drawing.Point(170, 147);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new System.Drawing.Size(180, 23);
            //
            // btnLogin
            //
            btnLogin.Location = new System.Drawing.Point(80, 195);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(80, 30);
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            //
            // btnRegister
            //
            btnRegister.Location = new System.Drawing.Point(170, 195);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new System.Drawing.Size(85, 30);
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            //
            // btnClear
            //
            btnClear.Location = new System.Drawing.Point(265, 195);
            btnClear.Name = "btnClear";
            btnClear.Size = new System.Drawing.Size(80, 30);
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            //
            // btnExit
            //
            btnExit.Location = new System.Drawing.Point(170, 235);
            btnExit.Name = "btnExit";
            btnExit.Size = new System.Drawing.Size(80, 30);
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            //
            // LoginForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(400, 290);
            Controls.Add(lblTitle);
            Controls.Add(label1);
            Controls.Add(rbCustomer);
            Controls.Add(rbAdmin);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
            Controls.Add(btnClear);
            Controls.Add(btnExit);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login - Camp Management System";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

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
    }
}