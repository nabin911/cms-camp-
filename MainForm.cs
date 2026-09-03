using System;
using System.Windows.Forms;

namespace CampManagementSystem
{
    public partial class MainForm : Form
    {
        private string role = "";

        public MainForm(string userRole)
        {
            InitializeComponent();
            role = userRole;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "Welcome, " + role;
            if (role.Equals("System Administrator", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("Camp Manager", StringComparison.OrdinalIgnoreCase))
            {
                lblRole.Text = "Role: Administrator";
            }
            else
            {
                lblRole.Text = "Role: Customer";
                btnDeleteCampsite.Enabled = false;
                btnDeleteEquipment.Enabled = false;
            }
        }

        private void btnCampsites_Click(object sender, EventArgs e) { new CampsiteForm().ShowDialog(); }
        private void btnBookings_Click(object sender, EventArgs e) { new BookingForm().ShowDialog(); }
        private void btnEquipment_Click(object sender, EventArgs e) { new EquipmentForm().ShowDialog(); }
        private void btnRentals_Click(object sender, EventArgs e) { new RentalForm().ShowDialog(); }
        private void btnPayments_Click(object sender, EventArgs e) { new PaymentForm().ShowDialog(); }
        private void btnLogout_Click(object sender, EventArgs e) { this.Close(); }

        private void btnDeleteCampsite_Click(object sender, EventArgs e) { }
        private void btnDeleteEquipment_Click(object sender, EventArgs e) { }
        private void lblWelcome_Click(object sender, EventArgs e) { }
        private void lblRole_Click(object sender, EventArgs e) { }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblWelcome = new Label();
            this.lblRole = new Label();
            this.btnCampsites = new Button();
            this.btnBookings = new Button();
            this.btnEquipment = new Button();
            this.btnRentals = new Button();
            this.btnPayments = new Button();
            this.btnLogout = new Button();
            this.btnDeleteCampsite = new Button();
            this.btnDeleteEquipment = new Button();
            SuspendLayout();
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(150, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(360, 30);
            lblTitle.Text = "Camp Management - Main Menu";
            //
            // lblWelcome
            //
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblWelcome.Location = new System.Drawing.Point(40, 65);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new System.Drawing.Size(120, 19);
            lblWelcome.Text = "Welcome, User";
            lblWelcome.Click += lblWelcome_Click;
            //
            // lblRole
            //
            lblRole.AutoSize = true;
            lblRole.Location = new System.Drawing.Point(500, 67);
            lblRole.Name = "lblRole";
            lblRole.Size = new System.Drawing.Size(80, 15);
            lblRole.Text = "Role: Customer";
            lblRole.Click += lblRole_Click;
            //
            // btnCampsites
            //
            btnCampsites.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnCampsites.Location = new System.Drawing.Point(40, 110);
            btnCampsites.Name = "btnCampsites";
            btnCampsites.Size = new System.Drawing.Size(170, 40);
            btnCampsites.Text = "Manage Campsites";
            btnCampsites.UseVisualStyleBackColor = true;
            btnCampsites.Click += btnCampsites_Click;
            //
            // btnBookings
            //
            btnBookings.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnBookings.Location = new System.Drawing.Point(230, 110);
            btnBookings.Name = "btnBookings";
            btnBookings.Size = new System.Drawing.Size(170, 40);
            btnBookings.Text = "Manage Bookings";
            btnBookings.UseVisualStyleBackColor = true;
            btnBookings.Click += btnBookings_Click;
            //
            // btnPayments
            //
            btnPayments.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnPayments.Location = new System.Drawing.Point(420, 110);
            btnPayments.Name = "btnPayments";
            btnPayments.Size = new System.Drawing.Size(170, 40);
            btnPayments.Text = "Manage Payments";
            btnPayments.UseVisualStyleBackColor = true;
            btnPayments.Click += btnPayments_Click;
            //
            // btnEquipment
            //
            btnEquipment.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnEquipment.Location = new System.Drawing.Point(40, 165);
            btnEquipment.Name = "btnEquipment";
            btnEquipment.Size = new System.Drawing.Size(170, 40);
            btnEquipment.Text = "Manage Equipment";
            btnEquipment.UseVisualStyleBackColor = true;
            btnEquipment.Click += btnEquipment_Click;
            //
            // btnRentals
            //
            btnRentals.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnRentals.Location = new System.Drawing.Point(230, 165);
            btnRentals.Name = "btnRentals";
            btnRentals.Size = new System.Drawing.Size(170, 40);
            btnRentals.Text = "Manage Rentals";
            btnRentals.UseVisualStyleBackColor = true;
            btnRentals.Click += btnRentals_Click;
            //
            // btnLogout
            //
            btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F);
            btnLogout.Location = new System.Drawing.Point(280, 250);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new System.Drawing.Size(100, 35);
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            //
            // btnDeleteCampsite
            btnDeleteCampsite.Visible = false;
            //
            // btnDeleteEquipment
            btnDeleteEquipment.Visible = false;
            //
            // MainForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(650, 320);
            Controls.Add(lblTitle);
            Controls.Add(lblWelcome);
            Controls.Add(lblRole);
            Controls.Add(btnCampsites);
            Controls.Add(btnBookings);
            Controls.Add(btnPayments);
            Controls.Add(btnEquipment);
            Controls.Add(btnRentals);
            Controls.Add(btnLogout);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main Menu - Camp Management System";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;
        private Label lblWelcome;
        private Label lblRole;
        private Button btnCampsites;
        private Button btnBookings;
        private Button btnPayments;
        private Button btnEquipment;
        private Button btnRentals;
        private Button btnLogout;
        private Button btnDeleteCampsite;
        private Button btnDeleteEquipment;
    }
}