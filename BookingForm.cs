using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CampManagementSystem
{
    public partial class BookingForm : Form
    {
        public BookingForm() { InitializeComponent(); }

        private void BookingForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadCampsites();
            LoadData();
        }

        private void LoadCustomers()
        {
            string sql = "SELECT CustomerID, FullName FROM dbo.Customer ORDER BY FullName";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            cmbCustomer.DataSource = dt;
            cmbCustomer.DisplayMember = "FullName";
            cmbCustomer.ValueMember = "CustomerID";
        }

        private void LoadCampsites()
        {
            string sql = "SELECT CampsiteID, SiteName FROM dbo.Campsite ORDER BY SiteName";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            cmbCampsite.DataSource = dt;
            cmbCampsite.DisplayMember = "SiteName";
            cmbCampsite.ValueMember = "CampsiteID";
        }

        private void LoadData()
        {
            string sql = @"SELECT b.BookingID, b.CheckInDate, b.CheckOutDate, b.Status,
                                  c.FullName AS Customer, cs.SiteName AS Campsite
                           FROM dbo.Booking b
                           INNER JOIN dbo.Customer c ON b.CustomerID = c.CustomerID
                           INNER JOIN dbo.Campsite cs ON b.CampsiteID = cs.CampsiteID
                           ORDER BY b.BookingID DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgvBookings.DataSource = dt;
        }

        private void dgvBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvBookings.Rows[e.RowIndex];
            txtBookingID.Text = row.Cells["BookingID"].Value.ToString();
            dtCheckIn.Value = Convert.ToDateTime(row.Cells["CheckInDate"].Value);
            dtCheckOut.Value = Convert.ToDateTime(row.Cells["CheckOutDate"].Value);
            cmbStatus.Text = row.Cells["Status"].Value.ToString();
            cmbCustomer.Text = row.Cells["Customer"].Value.ToString();
            cmbCampsite.Text = row.Cells["Campsite"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dtCheckOut.Value <= dtCheckIn.Value)
            {
                MessageBox.Show("Check-out date must be after Check-in date.");
                return;
            }
            try
            {
                string sql = @"INSERT INTO dbo.Booking (CheckInDate, CheckOutDate, Status, CustomerID, CampsiteID)
                               VALUES (@ci, @co, @st, @c, @cs)";
                SqlParameter[] p = {
                    new SqlParameter("@ci", dtCheckIn.Value.Date),
                    new SqlParameter("@co", dtCheckOut.Value.Date),
                    new SqlParameter("@st", cmbStatus.Text),
                    new SqlParameter("@c",  cmbCustomer.SelectedValue),
                    new SqlParameter("@cs", cmbCampsite.SelectedValue)
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Booking added successfully.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBookingID.Text))
            {
                MessageBox.Show("Please select a booking first.");
                return;
            }
            try
            {
                string sql = @"UPDATE dbo.Booking
                               SET CheckInDate = @ci, CheckOutDate = @co,
                                   Status = @st, CustomerID = @c, CampsiteID = @cs
                               WHERE BookingID = @id";
                SqlParameter[] p = {
                    new SqlParameter("@ci", dtCheckIn.Value.Date),
                    new SqlParameter("@co", dtCheckOut.Value.Date),
                    new SqlParameter("@st", cmbStatus.Text),
                    new SqlParameter("@c",  cmbCustomer.SelectedValue),
                    new SqlParameter("@cs", cmbCampsite.SelectedValue),
                    new SqlParameter("@id", int.Parse(txtBookingID.Text))
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Booking updated.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBookingID.Text))
            {
                MessageBox.Show("Please select a booking first.");
                return;
            }
            if (MessageBox.Show("Delete this booking?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                string sql = "DELETE FROM dbo.Booking WHERE BookingID = @id";
                SqlParameter[] p = { new SqlParameter("@id", int.Parse(txtBookingID.Text)) };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Booking deleted.");
                LoadData();
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot delete (may have related payments or rentals): " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT b.BookingID, b.CheckInDate, b.CheckOutDate, b.Status,
                                  c.FullName AS Customer, cs.SiteName AS Campsite
                           FROM dbo.Booking b
                           INNER JOIN dbo.Customer c ON b.CustomerID = c.CustomerID
                           INNER JOIN dbo.Campsite cs ON b.CampsiteID = cs.CampsiteID
                           WHERE c.FullName LIKE @s OR cs.SiteName LIKE @s OR b.Status LIKE @s
                           ORDER BY b.BookingID DESC";
            SqlParameter[] p = { new SqlParameter("@s", "%" + txtSearch.Text.Trim() + "%") };
            dgvBookings.DataSource = DatabaseHelper.ExecuteQuery(sql, p);
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadData();
            txtSearch.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e) { Clear(); }

        private void Clear()
        {
            txtBookingID.Clear();
            txtSearch.Clear();
            dtCheckIn.Value = DateTime.Today;
            dtCheckOut.Value = DateTime.Today.AddDays(1);
            cmbStatus.SelectedIndex = -1;
            if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;
            if (cmbCampsite.Items.Count > 0) cmbCampsite.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblBookingID = new Label();
            this.lblCheckIn = new Label();
            this.lblCheckOut = new Label();
            this.lblStatus = new Label();
            this.lblCustomer = new Label();
            this.lblCampsite = new Label();
            this.txtBookingID = new TextBox();
            this.dtCheckIn = new DateTimePicker();
            this.dtCheckOut = new DateTimePicker();
            this.cmbStatus = new ComboBox();
            this.cmbCustomer = new ComboBox();
            this.cmbCampsite = new ComboBox();
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();
            this.btnSearch = new Button();
            this.btnViewAll = new Button();
            this.btnClear = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.dgvBookings = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).BeginInit();
            SuspendLayout();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 15); lblTitle.Text = "Manage Bookings";
            lblBookingID.Location = new System.Drawing.Point(20, 55); lblBookingID.Text = "Booking ID:"; lblBookingID.AutoSize = true;
            txtBookingID.Location = new System.Drawing.Point(120, 52); txtBookingID.ReadOnly = true; txtBookingID.Size = new System.Drawing.Size(120, 23);
            lblCheckIn.Location = new System.Drawing.Point(20, 90); lblCheckIn.Text = "Check-In:"; lblCheckIn.AutoSize = true;
            dtCheckIn.Location = new System.Drawing.Point(120, 87); dtCheckIn.Size = new System.Drawing.Size(180, 23);
            lblCheckOut.Location = new System.Drawing.Point(20, 125); lblCheckOut.Text = "Check-Out:"; lblCheckOut.AutoSize = true;
            dtCheckOut.Location = new System.Drawing.Point(120, 122); dtCheckOut.Size = new System.Drawing.Size(180, 23);
            lblStatus.Location = new System.Drawing.Point(20, 160); lblStatus.Text = "Status:"; lblStatus.AutoSize = true;
            cmbStatus.Items.AddRange(new object[] { "Pending", "Confirmed", "Cancelled" });
            cmbStatus.Location = new System.Drawing.Point(120, 157); cmbStatus.Size = new System.Drawing.Size(180, 23);
            lblCustomer.Location = new System.Drawing.Point(20, 195); lblCustomer.Text = "Customer:"; lblCustomer.AutoSize = true;
            cmbCustomer.Location = new System.Drawing.Point(120, 192); cmbCustomer.Size = new System.Drawing.Size(220, 23);
            lblCampsite.Location = new System.Drawing.Point(20, 230); lblCampsite.Text = "Campsite:"; lblCampsite.AutoSize = true;
            cmbCampsite.Location = new System.Drawing.Point(120, 227); cmbCampsite.Size = new System.Drawing.Size(220, 23);
            btnAdd.Location = new System.Drawing.Point(360, 55); btnAdd.Size = new System.Drawing.Size(90, 30); btnAdd.Text = "Add"; btnAdd.UseVisualStyleBackColor = true; btnAdd.Click += btnAdd_Click;
            btnUpdate.Location = new System.Drawing.Point(460, 55); btnUpdate.Size = new System.Drawing.Size(90, 30); btnUpdate.Text = "Update"; btnUpdate.UseVisualStyleBackColor = true; btnUpdate.Click += btnUpdate_Click;
            btnDelete.Location = new System.Drawing.Point(560, 55); btnDelete.Size = new System.Drawing.Size(90, 30); btnDelete.Text = "Delete"; btnDelete.UseVisualStyleBackColor = true; btnDelete.Click += btnDelete_Click;
            btnSearch.Location = new System.Drawing.Point(560, 225); btnSearch.Size = new System.Drawing.Size(90, 30); btnSearch.Text = "Search"; btnSearch.UseVisualStyleBackColor = true; btnSearch.Click += btnSearch_Click;
            btnViewAll.Location = new System.Drawing.Point(460, 225); btnViewAll.Size = new System.Drawing.Size(90, 30); btnViewAll.Text = "View All"; btnViewAll.UseVisualStyleBackColor = true; btnViewAll.Click += btnViewAll_Click;
            btnClear.Location = new System.Drawing.Point(360, 225); btnClear.Size = new System.Drawing.Size(90, 30); btnClear.Text = "Clear"; btnClear.UseVisualStyleBackColor = true; btnClear.Click += btnClear_Click;
            txtSearch.Location = new System.Drawing.Point(120, 228); txtSearch.Size = new System.Drawing.Size(230, 23);
            lblSearch.Location = new System.Drawing.Point(20, 231); lblSearch.Text = "Search:"; lblSearch.AutoSize = true;
            dgvBookings.AllowUserToAddRows = false; dgvBookings.AllowUserToDeleteRows = false;
            dgvBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBookings.Location = new System.Drawing.Point(20, 270); dgvBookings.ReadOnly = true;
            dgvBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBookings.Size = new System.Drawing.Size(720, 170);
            dgvBookings.CellClick += dgvBookings_CellClick;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(760, 460);
            Controls.Add(lblTitle); Controls.Add(lblBookingID); Controls.Add(txtBookingID);
            Controls.Add(lblCheckIn); Controls.Add(dtCheckIn);
            Controls.Add(lblCheckOut); Controls.Add(dtCheckOut);
            Controls.Add(lblStatus); Controls.Add(cmbStatus);
            Controls.Add(lblCustomer); Controls.Add(cmbCustomer);
            Controls.Add(lblCampsite); Controls.Add(cmbCampsite);
            Controls.Add(btnAdd); Controls.Add(btnUpdate); Controls.Add(btnDelete);
            Controls.Add(btnSearch); Controls.Add(btnViewAll); Controls.Add(btnClear);
            Controls.Add(txtSearch); Controls.Add(lblSearch); Controls.Add(dgvBookings);
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            Name = "BookingForm"; StartPosition = FormStartPosition.CenterParent;
            Text = "Bookings - Camp Management System";
            Load += BookingForm_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookings)).EndInit();
            ResumeLayout(false); PerformLayout();
        }

        private Label lblTitle;
        private Label lblBookingID;
        private TextBox txtBookingID;
        private Label lblCheckIn;
        private DateTimePicker dtCheckIn;
        private Label lblCheckOut;
        private DateTimePicker dtCheckOut;
        private Label lblStatus;
        private ComboBox cmbStatus;
        private Label lblCustomer;
        private ComboBox cmbCustomer;
        private Label lblCampsite;
        private ComboBox cmbCampsite;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnViewAll;
        private Button btnClear;
        private TextBox txtSearch;
        private Label lblSearch;
        private DataGridView dgvBookings;
    }
}