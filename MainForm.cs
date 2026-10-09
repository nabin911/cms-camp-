using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampManagementSystem
{
    // Single-window dashboard: sidebar buttons load UserControls into pnlContent.
    // Opens MAXIMIZED so it occupies the full screen. LoginForm is closed (not hidden)
    // before this opens, so clicking a menu item never affects the login window.
    public partial class MainForm : Form
    {
        private string role = "";
        private UserControl currentView = null;
        private Button activeButton = null;

        public MainForm(string userRole)
        {
            InitializeComponent();
            role = userRole;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "Welcome, " + role;
            if (IsAdmin())
                lblRole.Text = "Role: Administrator";
            else
                lblRole.Text = "Role: Customer";

            // Open the default page
            ShowView(new CampsiteControl(), btnCampsites);
            PositionHeaderButtons();
        }

        private bool IsAdmin()
        {
            return role.Equals("System Administrator", StringComparison.OrdinalIgnoreCase) ||
                   role.Equals("Camp Manager", StringComparison.OrdinalIgnoreCase);
        }

        // ----- Sidebar navigation -----
        private void btnCampsites_Click(object sender, EventArgs e) { ShowView(new CampsiteControl(), (Button)sender); }
        private void btnBookings_Click(object sender, EventArgs e) { ShowView(new BookingControl(), (Button)sender); }
        private void btnPayments_Click(object sender, EventArgs e) { ShowView(new PaymentControl(), (Button)sender); }
        private void btnEquipment_Click(object sender, EventArgs e) { ShowView(new EquipmentControl(), (Button)sender); }
        private void btnRentals_Click(object sender, EventArgs e) { ShowView(new RentalControl(), (Button)sender); }

        // ----- Single content-panel loader -----
        private void ShowView(UserControl view, Button clickedButton)
        {
            if (currentView != null)
            {
                pnlContent.Controls.Remove(currentView);
                currentView.Dispose();
                currentView = null;
            }
            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);
            currentView = view;

            if (activeButton != null) activeButton.BackColor = SystemColors.Control;
            activeButton = clickedButton;
            if (activeButton != null) activeButton.BackColor = Color.SteelBlue;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ShowView(new CampsiteControl(), btnCampsites);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close(); // FormClosed handler in LoginForm calls Application.Exit()
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            PositionHeaderButtons();
        }

        private void PositionHeaderButtons()
        {
            // Pin Back / Logout to the top-right corner of the header.
            int rightMargin = 15;
            int gap = 8;
            btnLogout.Left = pnlHeader.Width - btnLogout.Width - rightMargin;
            btnLogout.Top  = (pnlHeader.Height - btnLogout.Height) / 2;
            btnBack.Left   = btnLogout.Left - btnBack.Width - gap;
            btnBack.Top    = (pnlHeader.Height - btnBack.Height) / 2;
        }

        private void lblWelcome_Click(object sender, EventArgs e) { }
        private void lblRole_Click(object sender, EventArgs e) { }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.pnlSidebar = new Panel();
            this.pnlContent = new Panel();

            this.lblAppTitle = new Label();
            this.lblWelcome = new Label();
            this.lblRole = new Label();
            this.btnBack = new Button();
            this.btnLogout = new Button();

            this.btnCampsites = new Button();
            this.btnBookings = new Button();
            this.btnPayments = new Button();
            this.btnEquipment = new Button();
            this.btnRentals = new Button();

            pnlSidebar.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();

            // ===== Header =====
            pnlHeader.BackColor = Color.FromArgb(45, 55, 72);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 60;
            pnlHeader.Controls.Add(lblAppTitle);
            pnlHeader.Controls.Add(lblWelcome);
            pnlHeader.Controls.Add(lblRole);
            pnlHeader.Controls.Add(btnBack);
            pnlHeader.Controls.Add(btnLogout);

            lblAppTitle.AutoSize = true;
            lblAppTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.White;
            lblAppTitle.Location = new Point(15, 18);
            lblAppTitle.Text = "Camp Management System";

            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(380, 20);
            lblWelcome.Text = "Welcome, User";
            lblWelcome.Click += lblWelcome_Click;

            lblRole.AutoSize = true;
            lblRole.ForeColor = Color.White;
            lblRole.Location = new Point(620, 22);
            lblRole.Text = "Role: Customer";
            lblRole.Click += lblRole_Click;

            btnBack.Size = new Size(90, 32);
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;

            btnLogout.Size = new Size(90, 32);
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;

            // ===== Sidebar =====
            pnlSidebar.BackColor = Color.FromArgb(237, 240, 245);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 200;
            pnlSidebar.Controls.Add(btnCampsites);
            pnlSidebar.Controls.Add(btnBookings);
            pnlSidebar.Controls.Add(btnPayments);
            pnlSidebar.Controls.Add(btnEquipment);
            pnlSidebar.Controls.Add(btnRentals);

            int btnY = 20;
            int btnH = 50;
            int btnGap = 12;
            StyleNavButton(btnCampsites, "🏕  Campsites", btnY, btnCampsites_Click);   btnY += btnH + btnGap;
            StyleNavButton(btnBookings,  "📅  Bookings",  btnY, btnBookings_Click);    btnY += btnH + btnGap;
            StyleNavButton(btnPayments,  "💳  Payments",  btnY, btnPayments_Click);    btnY += btnH + btnGap;
            StyleNavButton(btnEquipment, "🎒  Equipment", btnY, btnEquipment_Click);   btnY += btnH + btnGap;
            StyleNavButton(btnRentals,   "🔄  Rentals",   btnY, btnRentals_Click);

            // ===== Content panel =====
            pnlContent.BackColor = Color.White;
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(15);

            // ===== MainForm =====
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1100, 700);
            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(900, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Camp Management System - Dashboard";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            Resize += MainForm_Resize;

            pnlSidebar.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private void StyleNavButton(Button b, string text, int y, EventHandler onClick)
        {
            b.Location = new Point(10, y);
            b.Size = new Size(180, 50);
            b.Text = text;
            b.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            b.TextAlign = ContentAlignment.MiddleLeft;
            b.Padding = new Padding(12, 0, 0, 0);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = SystemColors.Control;
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
            b.Click += onClick;
        }

        private Panel pnlHeader;
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblAppTitle;
        private Label lblWelcome;
        private Label lblRole;
        private Button btnBack;
        private Button btnLogout;
        private Button btnCampsites;
        private Button btnBookings;
        private Button btnPayments;
        private Button btnEquipment;
        private Button btnRentals;
    }
}