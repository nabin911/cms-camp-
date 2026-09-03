using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CampManagementSystem
{
    public partial class RentalForm : Form
    {
        public RentalForm() { InitializeComponent(); }

        private void RentalForm_Load(object sender, EventArgs e)
        {
            LoadBookings();
            LoadEquipment();
            LoadData();
        }

        private void LoadBookings()
        {
            string sql = @"SELECT b.BookingID, c.FullName + ' - ' + cs.SiteName AS BookingInfo
                           FROM dbo.Booking b
                           INNER JOIN dbo.Customer c ON b.CustomerID = c.CustomerID
                           INNER JOIN dbo.Campsite cs ON b.CampsiteID = cs.CampsiteID
                           ORDER BY b.BookingID DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            cmbBooking.DataSource = dt;
            cmbBooking.DisplayMember = "BookingInfo";
            cmbBooking.ValueMember = "BookingID";
        }

        private void LoadEquipment()
        {
            string sql = "SELECT EquipmentID, EquipmentName FROM dbo.Equipment ORDER BY EquipmentName";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            cmbEquipment.DataSource = dt;
            cmbEquipment.DisplayMember = "EquipmentName";
            cmbEquipment.ValueMember = "EquipmentID";
        }

        private void LoadData()
        {
            string sql = @"SELECT er.RentalID, er.Quantity, er.ReturnStatus,
                                  b.BookingID, e.EquipmentName
                           FROM dbo.Equipment_Rental er
                           INNER JOIN dbo.Booking b    ON er.BookingID   = b.BookingID
                           INNER JOIN dbo.Equipment e  ON er.EquipmentID = e.EquipmentID
                           ORDER BY er.RentalID DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgvRentals.DataSource = dt;
        }

        private void dgvRentals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvRentals.Rows[e.RowIndex];
            txtRentalID.Text = row.Cells["RentalID"].Value.ToString();
            txtQuantity.Text = row.Cells["Quantity"].Value.ToString();
            cmbReturnStatus.Text = row.Cells["ReturnStatus"].Value.ToString();
            int bookingId = Convert.ToInt32(row.Cells["BookingID"].Value);
            if (cmbBooking.Items.Count > 0) cmbBooking.SelectedValue = bookingId;
            cmbEquipment.Text = row.Cells["EquipmentName"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtQuantity.Text, out _) || int.Parse(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("Quantity must be a positive number.");
                return;
            }
            try
            {
                string sql = @"INSERT INTO dbo.Equipment_Rental (Quantity, ReturnStatus, BookingID, EquipmentID)
                               VALUES (@q, @rs, @b, @e)";
                SqlParameter[] p = {
                    new SqlParameter("@q",  int.Parse(txtQuantity.Text)),
                    new SqlParameter("@rs", cmbReturnStatus.Text),
                    new SqlParameter("@b",  cmbBooking.SelectedValue),
                    new SqlParameter("@e",  cmbEquipment.SelectedValue)
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Rental added.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRentalID.Text))
            {
                MessageBox.Show("Please select a rental to update.");
                return;
            }
            try
            {
                string sql = @"UPDATE dbo.Equipment_Rental
                               SET Quantity = @q, ReturnStatus = @rs,
                                   BookingID = @b, EquipmentID = @e
                               WHERE RentalID = @id";
                SqlParameter[] p = {
                    new SqlParameter("@q",  int.Parse(txtQuantity.Text)),
                    new SqlParameter("@rs", cmbReturnStatus.Text),
                    new SqlParameter("@b",  cmbBooking.SelectedValue),
                    new SqlParameter("@e",  cmbEquipment.SelectedValue),
                    new SqlParameter("@id", int.Parse(txtRentalID.Text))
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Rental updated.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRentalID.Text))
            {
                MessageBox.Show("Please select a rental to delete.");
                return;
            }
            if (MessageBox.Show("Delete this rental?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                string sql = "DELETE FROM dbo.Equipment_Rental WHERE RentalID = @id";
                SqlParameter[] p = { new SqlParameter("@id", int.Parse(txtRentalID.Text)) };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Rental deleted.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT er.RentalID, er.Quantity, er.ReturnStatus, b.BookingID, e.EquipmentName
                           FROM dbo.Equipment_Rental er
                           INNER JOIN dbo.Booking b    ON er.BookingID   = b.BookingID
                           INNER JOIN dbo.Equipment e  ON er.EquipmentID = e.EquipmentID
                           WHERE e.EquipmentName LIKE @s OR er.ReturnStatus LIKE @s OR b.BookingID LIKE @s
                           ORDER BY er.RentalID DESC";
            SqlParameter[] p = { new SqlParameter("@s", "%" + txtSearch.Text.Trim() + "%") };
            dgvRentals.DataSource = DatabaseHelper.ExecuteQuery(sql, p);
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadData();
            txtSearch.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e) { Clear(); }

        private void Clear()
        {
            txtRentalID.Clear();
            txtQuantity.Clear();
            txtSearch.Clear();
            cmbReturnStatus.SelectedIndex = -1;
            if (cmbBooking.Items.Count > 0) cmbBooking.SelectedIndex = 0;
            if (cmbEquipment.Items.Count > 0) cmbEquipment.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblRentalID = new Label();
            this.lblQuantity = new Label();
            this.lblStatus = new Label();
            this.lblBooking = new Label();
            this.lblEquipment = new Label();
            this.txtRentalID = new TextBox();
            this.txtQuantity = new TextBox();
            this.cmbReturnStatus = new ComboBox();
            this.cmbBooking = new ComboBox();
            this.cmbEquipment = new ComboBox();
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();
            this.btnSearch = new Button();
            this.btnViewAll = new Button();
            this.btnClear = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.dgvRentals = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRentals)).BeginInit();
            SuspendLayout();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 15); lblTitle.Text = "Manage Equipment Rentals";
            lblRentalID.Location = new System.Drawing.Point(20, 55); lblRentalID.Text = "Rental ID:"; lblRentalID.AutoSize = true;
            txtRentalID.Location = new System.Drawing.Point(120, 52); txtRentalID.ReadOnly = true; txtRentalID.Size = new System.Drawing.Size(120, 23);
            lblQuantity.Location = new System.Drawing.Point(20, 90); lblQuantity.Text = "Quantity:"; lblQuantity.AutoSize = true;
            txtQuantity.Location = new System.Drawing.Point(120, 87); txtQuantity.Size = new System.Drawing.Size(100, 23);
            lblStatus.Location = new System.Drawing.Point(20, 125); lblStatus.Text = "Return Status:"; lblStatus.AutoSize = true;
            cmbReturnStatus.Items.AddRange(new object[] { "Returned", "Not Returned" });
            cmbReturnStatus.Location = new System.Drawing.Point(120, 122); cmbReturnStatus.Size = new System.Drawing.Size(150, 23);
            lblBooking.Location = new System.Drawing.Point(20, 160); lblBooking.Text = "Booking:"; lblBooking.AutoSize = true;
            cmbBooking.Location = new System.Drawing.Point(120, 157); cmbBooking.Size = new System.Drawing.Size(240, 23);
            lblEquipment.Location = new System.Drawing.Point(20, 195); lblEquipment.Text = "Equipment:"; lblEquipment.AutoSize = true;
            cmbEquipment.Location = new System.Drawing.Point(120, 192); cmbEquipment.Size = new System.Drawing.Size(220, 23);
            btnAdd.Location = new System.Drawing.Point(390, 55); btnAdd.Size = new System.Drawing.Size(90, 30); btnAdd.Text = "Add"; btnAdd.UseVisualStyleBackColor = true; btnAdd.Click += btnAdd_Click;
            btnUpdate.Location = new System.Drawing.Point(490, 55); btnUpdate.Size = new System.Drawing.Size(90, 30); btnUpdate.Text = "Update"; btnUpdate.UseVisualStyleBackColor = true; btnUpdate.Click += btnUpdate_Click;
            btnDelete.Location = new System.Drawing.Point(590, 55); btnDelete.Size = new System.Drawing.Size(90, 30); btnDelete.Text = "Delete"; btnDelete.UseVisualStyleBackColor = true; btnDelete.Click += btnDelete_Click;
            btnSearch.Location = new System.Drawing.Point(590, 190); btnSearch.Size = new System.Drawing.Size(90, 30); btnSearch.Text = "Search"; btnSearch.UseVisualStyleBackColor = true; btnSearch.Click += btnSearch_Click;
            btnViewAll.Location = new System.Drawing.Point(490, 190); btnViewAll.Size = new System.Drawing.Size(90, 30); btnViewAll.Text = "View All"; btnViewAll.UseVisualStyleBackColor = true; btnViewAll.Click += btnViewAll_Click;
            btnClear.Location = new System.Drawing.Point(390, 190); btnClear.Size = new System.Drawing.Size(90, 30); btnClear.Text = "Clear"; btnClear.UseVisualStyleBackColor = true; btnClear.Click += btnClear_Click;
            txtSearch.Location = new System.Drawing.Point(120, 193); txtSearch.Size = new System.Drawing.Size(260, 23);
            lblSearch.Location = new System.Drawing.Point(20, 196); lblSearch.Text = "Search:"; lblSearch.AutoSize = true;
            dgvRentals.AllowUserToAddRows = false; dgvRentals.AllowUserToDeleteRows = false;
            dgvRentals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRentals.Location = new System.Drawing.Point(20, 240); dgvRentals.ReadOnly = true;
            dgvRentals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRentals.Size = new System.Drawing.Size(720, 200);
            dgvRentals.CellClick += dgvRentals_CellClick;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(760, 460);
            Controls.Add(lblTitle); Controls.Add(lblRentalID); Controls.Add(txtRentalID);
            Controls.Add(lblQuantity); Controls.Add(txtQuantity);
            Controls.Add(lblStatus); Controls.Add(cmbReturnStatus);
            Controls.Add(lblBooking); Controls.Add(cmbBooking);
            Controls.Add(lblEquipment); Controls.Add(cmbEquipment);
            Controls.Add(btnAdd); Controls.Add(btnUpdate); Controls.Add(btnDelete);
            Controls.Add(btnSearch); Controls.Add(btnViewAll); Controls.Add(btnClear);
            Controls.Add(txtSearch); Controls.Add(lblSearch); Controls.Add(dgvRentals);
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            Name = "RentalForm"; StartPosition = FormStartPosition.CenterParent;
            Text = "Equipment Rentals - Camp Management System";
            Load += RentalForm_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvRentals)).EndInit();
            ResumeLayout(false); PerformLayout();
        }

        private Label lblTitle;
        private Label lblRentalID;
        private TextBox txtRentalID;
        private Label lblQuantity;
        private TextBox txtQuantity;
        private Label lblStatus;
        private ComboBox cmbReturnStatus;
        private Label lblBooking;
        private ComboBox cmbBooking;
        private Label lblEquipment;
        private ComboBox cmbEquipment;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnViewAll;
        private Button btnClear;
        private TextBox txtSearch;
        private Label lblSearch;
        private DataGridView dgvRentals;
    }
}