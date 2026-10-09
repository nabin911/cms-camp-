using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CampManagementSystem
{
    // DFD Process 4: Manage Payment (UserControl)
    public partial class PaymentControl : UserControl
    {
        public PaymentControl() { InitializeComponent(); }

        private void PaymentControl_Load(object sender, EventArgs e)
        {
            LoadBookings();
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

        private void LoadData()
        {
            string sql = @"SELECT p.PaymentID, p.Amount, p.PaymentDate, p.PaymentMode, p.PaymentStatus,
                                  p.BookingID
                           FROM dbo.Payment p
                           ORDER BY p.PaymentID DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgvPayments.DataSource = dt;
        }

        private void dgvPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvPayments.Rows[e.RowIndex];
            txtPaymentID.Text = row.Cells["PaymentID"].Value.ToString();
            txtAmount.Text = row.Cells["Amount"].Value.ToString();
            dtPaymentDate.Value = Convert.ToDateTime(row.Cells["PaymentDate"].Value);
            cmbPaymentMode.Text = row.Cells["PaymentMode"].Value.ToString();
            cmbPaymentStatus.Text = row.Cells["PaymentStatus"].Value.ToString();
            int bookingId = Convert.ToInt32(row.Cells["BookingID"].Value);
            if (cmbBooking.Items.Count > 0) cmbBooking.SelectedValue = bookingId;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out _))
            {
                MessageBox.Show("Amount must be numeric.");
                return;
            }
            try
            {
                string sql = @"INSERT INTO dbo.Payment (Amount, PaymentDate, PaymentMode, PaymentStatus, BookingID)
                               VALUES (@a, @d, @m, @s, @b)";
                SqlParameter[] p = {
                    new SqlParameter("@a", decimal.Parse(txtAmount.Text)),
                    new SqlParameter("@d", dtPaymentDate.Value.Date),
                    new SqlParameter("@m", cmbPaymentMode.Text),
                    new SqlParameter("@s", cmbPaymentStatus.Text),
                    new SqlParameter("@b", cmbBooking.SelectedValue)
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Payment recorded.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPaymentID.Text))
            {
                MessageBox.Show("Please select a payment first.");
                return;
            }
            try
            {
                string sql = @"UPDATE dbo.Payment
                               SET Amount = @a, PaymentDate = @d, PaymentMode = @m,
                                   PaymentStatus = @s, BookingID = @b
                               WHERE PaymentID = @id";
                SqlParameter[] p = {
                    new SqlParameter("@a", decimal.Parse(txtAmount.Text)),
                    new SqlParameter("@d", dtPaymentDate.Value.Date),
                    new SqlParameter("@m", cmbPaymentMode.Text),
                    new SqlParameter("@s", cmbPaymentStatus.Text),
                    new SqlParameter("@b", cmbBooking.SelectedValue),
                    new SqlParameter("@id", int.Parse(txtPaymentID.Text))
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Payment updated.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPaymentID.Text))
            {
                MessageBox.Show("Please select a payment first.");
                return;
            }
            if (MessageBox.Show("Delete this payment?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                string sql = "DELETE FROM dbo.Payment WHERE PaymentID = @id";
                SqlParameter[] p = { new SqlParameter("@id", int.Parse(txtPaymentID.Text)) };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Payment deleted.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT p.PaymentID, p.Amount, p.PaymentDate, p.PaymentMode, p.PaymentStatus, p.BookingID
                           FROM dbo.Payment p
                           WHERE p.PaymentMode LIKE @s OR p.PaymentStatus LIKE @s OR p.BookingID LIKE @s
                           ORDER BY p.PaymentID DESC";
            SqlParameter[] p = { new SqlParameter("@s", "%" + txtSearch.Text.Trim() + "%") };
            dgvPayments.DataSource = DatabaseHelper.ExecuteQuery(sql, p);
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadData();
            txtSearch.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e) { Clear(); }

        private void Clear()
        {
            txtPaymentID.Clear();
            txtAmount.Clear();
            txtSearch.Clear();
            dtPaymentDate.Value = DateTime.Today;
            cmbPaymentMode.SelectedIndex = -1;
            cmbPaymentStatus.SelectedIndex = -1;
            if (cmbBooking.Items.Count > 0) cmbBooking.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblPaymentID = new Label();
            this.lblAmount = new Label();
            this.lblDate = new Label();
            this.lblMode = new Label();
            this.lblStatus = new Label();
            this.lblBooking = new Label();
            this.txtPaymentID = new TextBox();
            this.txtAmount = new TextBox();
            this.dtPaymentDate = new DateTimePicker();
            this.cmbPaymentMode = new ComboBox();
            this.cmbPaymentStatus = new ComboBox();
            this.cmbBooking = new ComboBox();
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();
            this.btnSearch = new Button();
            this.btnViewAll = new Button();
            this.btnClear = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.dgvPayments = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
            this.SuspendLayout();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 15); lblTitle.Text = "Manage Payments";
            lblPaymentID.Location = new System.Drawing.Point(20, 55); lblPaymentID.Text = "Payment ID:"; lblPaymentID.AutoSize = true;
            txtPaymentID.Location = new System.Drawing.Point(120, 52); txtPaymentID.ReadOnly = true; txtPaymentID.Size = new System.Drawing.Size(120, 23);
            lblAmount.Location = new System.Drawing.Point(20, 90); lblAmount.Text = "Amount:"; lblAmount.AutoSize = true;
            txtAmount.Location = new System.Drawing.Point(120, 87); txtAmount.Size = new System.Drawing.Size(150, 23);
            lblDate.Location = new System.Drawing.Point(20, 125); lblDate.Text = "Date:"; lblDate.AutoSize = true;
            dtPaymentDate.Location = new System.Drawing.Point(120, 122); dtPaymentDate.Size = new System.Drawing.Size(180, 23);
            lblMode.Location = new System.Drawing.Point(20, 160); lblMode.Text = "Mode:"; lblMode.AutoSize = true;
            cmbPaymentMode.Items.AddRange(new object[] { "Cash", "Card", "UPI", "NetBanking" });
            cmbPaymentMode.Location = new System.Drawing.Point(120, 157); cmbPaymentMode.Size = new System.Drawing.Size(150, 23);
            lblStatus.Location = new System.Drawing.Point(20, 195); lblStatus.Text = "Status:"; lblStatus.AutoSize = true;
            cmbPaymentStatus.Items.AddRange(new object[] { "Paid", "Pending", "Refunded" });
            cmbPaymentStatus.Location = new System.Drawing.Point(120, 192); cmbPaymentStatus.Size = new System.Drawing.Size(150, 23);
            lblBooking.Location = new System.Drawing.Point(20, 230); lblBooking.Text = "Booking:"; lblBooking.AutoSize = true;
            cmbBooking.Location = new System.Drawing.Point(120, 227); cmbBooking.Size = new System.Drawing.Size(220, 23);
            btnAdd.Location = new System.Drawing.Point(360, 55); btnAdd.Size = new System.Drawing.Size(90, 30); btnAdd.Text = "Add"; btnAdd.UseVisualStyleBackColor = true; btnAdd.Click += btnAdd_Click;
            btnUpdate.Location = new System.Drawing.Point(460, 55); btnUpdate.Size = new System.Drawing.Size(90, 30); btnUpdate.Text = "Update"; btnUpdate.UseVisualStyleBackColor = true; btnUpdate.Click += btnUpdate_Click;
            btnDelete.Location = new System.Drawing.Point(560, 55); btnDelete.Size = new System.Drawing.Size(90, 30); btnDelete.Text = "Delete"; btnDelete.UseVisualStyleBackColor = true; btnDelete.Click += btnDelete_Click;
            btnSearch.Location = new System.Drawing.Point(560, 225); btnSearch.Size = new System.Drawing.Size(90, 30); btnSearch.Text = "Search"; btnSearch.UseVisualStyleBackColor = true; btnSearch.Click += btnSearch_Click;
            btnViewAll.Location = new System.Drawing.Point(460, 225); btnViewAll.Size = new System.Drawing.Size(90, 30); btnViewAll.Text = "View All"; btnViewAll.UseVisualStyleBackColor = true; btnViewAll.Click += btnViewAll_Click;
            btnClear.Location = new System.Drawing.Point(360, 225); btnClear.Size = new System.Drawing.Size(90, 30); btnClear.Text = "Clear"; btnClear.UseVisualStyleBackColor = true; btnClear.Click += btnClear_Click;
            txtSearch.Location = new System.Drawing.Point(120, 228); txtSearch.Size = new System.Drawing.Size(230, 23);
            lblSearch.Location = new System.Drawing.Point(20, 231); lblSearch.Text = "Search:"; lblSearch.AutoSize = true;
            dgvPayments.AllowUserToAddRows = false; dgvPayments.AllowUserToDeleteRows = false;
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayments.Location = new System.Drawing.Point(20, 270); dgvPayments.ReadOnly = true;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.Size = new System.Drawing.Size(720, 170);
            dgvPayments.CellClick += dgvPayments_CellClick;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(lblTitle); this.Controls.Add(lblPaymentID); this.Controls.Add(txtPaymentID);
            this.Controls.Add(lblAmount); this.Controls.Add(txtAmount);
            this.Controls.Add(lblDate); this.Controls.Add(dtPaymentDate);
            this.Controls.Add(lblMode); this.Controls.Add(cmbPaymentMode);
            this.Controls.Add(lblStatus); this.Controls.Add(cmbPaymentStatus);
            this.Controls.Add(lblBooking); this.Controls.Add(cmbBooking);
            this.Controls.Add(btnAdd); this.Controls.Add(btnUpdate); this.Controls.Add(btnDelete);
            this.Controls.Add(btnSearch); this.Controls.Add(btnViewAll); this.Controls.Add(btnClear);
            this.Controls.Add(txtSearch); this.Controls.Add(lblSearch); this.Controls.Add(dgvPayments);
            this.Name = "PaymentControl";
            this.Load += PaymentControl_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }

        private Label lblTitle;
        private Label lblPaymentID;
        private TextBox txtPaymentID;
        private Label lblAmount;
        private TextBox txtAmount;
        private Label lblDate;
        private DateTimePicker dtPaymentDate;
        private Label lblMode;
        private ComboBox cmbPaymentMode;
        private Label lblStatus;
        private ComboBox cmbPaymentStatus;
        private Label lblBooking;
        private ComboBox cmbBooking;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnViewAll;
        private Button btnClear;
        private TextBox txtSearch;
        private Label lblSearch;
        private DataGridView dgvPayments;
    }
}