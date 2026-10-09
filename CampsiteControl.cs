using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CampManagementSystem
{
    // DFD Process 2: Manage Campsites (UserControl — loaded into MainForm's content panel)
    public partial class CampsiteControl : UserControl
    {
        public CampsiteControl()
        {
            InitializeComponent();
        }

        private void CampsiteControl_Load(object sender, EventArgs e)
        {
            LoadAdmins();
            LoadData();
        }

        private void LoadAdmins()
        {
            string sql = "SELECT AdminID, Username FROM dbo.Admin ORDER BY AdminID";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            cmbAdmin.DataSource = dt;
            cmbAdmin.DisplayMember = "Username";
            cmbAdmin.ValueMember = "AdminID";
        }

        private void LoadData()
        {
            string sql = @"SELECT c.CampsiteID, c.SiteName, c.PricePerNight, c.AvailabilityStatus,
                                  a.Username AS Admin
                           FROM dbo.Campsite c
                           INNER JOIN dbo.Admin a ON c.AdminID = a.AdminID
                           ORDER BY c.CampsiteID DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgvCampsites.DataSource = dt;
        }

        private void dgvCampsites_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvCampsites.Rows[e.RowIndex];
            txtCampsiteID.Text = row.Cells["CampsiteID"].Value.ToString();
            txtSiteName.Text = row.Cells["SiteName"].Value.ToString();
            txtPrice.Text = row.Cells["PricePerNight"].Value.ToString();
            cmbStatus.Text = row.Cells["AvailabilityStatus"].Value.ToString();
            cmbAdmin.Text = row.Cells["Admin"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            try
            {
                string sql = @"INSERT INTO dbo.Campsite (SiteName, PricePerNight, AvailabilityStatus, AdminID)
                               VALUES (@n, @p, @s, @a)";
                SqlParameter[] p = {
                    new SqlParameter("@n", txtSiteName.Text.Trim()),
                    new SqlParameter("@p", decimal.Parse(txtPrice.Text.Trim())),
                    new SqlParameter("@s", cmbStatus.Text),
                    new SqlParameter("@a", cmbAdmin.SelectedValue)
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Campsite added successfully.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCampsiteID.Text))
            {
                MessageBox.Show("Please select a campsite to update.");
                return;
            }
            if (!ValidateInputs()) return;
            try
            {
                string sql = @"UPDATE dbo.Campsite
                               SET SiteName = @n, PricePerNight = @p,
                                   AvailabilityStatus = @s, AdminID = @a
                               WHERE CampsiteID = @id";
                SqlParameter[] p = {
                    new SqlParameter("@n", txtSiteName.Text.Trim()),
                    new SqlParameter("@p", decimal.Parse(txtPrice.Text.Trim())),
                    new SqlParameter("@s", cmbStatus.Text),
                    new SqlParameter("@a", cmbAdmin.SelectedValue),
                    new SqlParameter("@id", int.Parse(txtCampsiteID.Text))
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Campsite updated successfully.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCampsiteID.Text))
            {
                MessageBox.Show("Please select a campsite to delete.");
                return;
            }
            if (MessageBox.Show("Delete this campsite?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                string sql = "DELETE FROM dbo.Campsite WHERE CampsiteID = @id";
                SqlParameter[] p = { new SqlParameter("@id", int.Parse(txtCampsiteID.Text)) };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Campsite deleted.");
                LoadData();
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot delete: " + ex.Message +
                    "\n(Tip: It may be referenced by bookings.)");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT c.CampsiteID, c.SiteName, c.PricePerNight, c.AvailabilityStatus,
                                  a.Username AS Admin
                           FROM dbo.Campsite c
                           INNER JOIN dbo.Admin a ON c.AdminID = a.AdminID
                           WHERE c.SiteName LIKE @s OR a.Username LIKE @s
                           ORDER BY c.CampsiteID DESC";
            SqlParameter[] p = { new SqlParameter("@s", "%" + txtSearch.Text.Trim() + "%") };
            DataTable dt = DatabaseHelper.ExecuteQuery(sql, p);
            dgvCampsites.DataSource = dt;
        }

        private void btnClear_Click(object sender, EventArgs e) { Clear(); }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadData();
            txtSearch.Clear();
        }

        private void Clear()
        {
            txtCampsiteID.Clear();
            txtSiteName.Clear();
            txtPrice.Clear();
            cmbStatus.SelectedIndex = -1;
            if (cmbAdmin.Items.Count > 0) cmbAdmin.SelectedIndex = 0;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtSiteName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(cmbStatus.Text))
            {
                MessageBox.Show("Please fill in Site Name, Price and Status.");
                return false;
            }
            if (!decimal.TryParse(txtPrice.Text, out _))
            {
                MessageBox.Show("Price must be a valid number.");
                return false;
            }
            return true;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblCampsiteID = new Label();
            this.lblSiteName = new Label();
            this.lblPrice = new Label();
            this.lblStatus = new Label();
            this.lblAdmin = new Label();
            this.txtCampsiteID = new TextBox();
            this.txtSiteName = new TextBox();
            this.txtPrice = new TextBox();
            this.cmbStatus = new ComboBox();
            this.cmbAdmin = new ComboBox();
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();
            this.btnSearch = new Button();
            this.btnViewAll = new Button();
            this.btnClear = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.dgvCampsites = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCampsites)).BeginInit();
            this.SuspendLayout();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 15);
            lblTitle.Text = "Manage Campsites";
            lblCampsiteID.Location = new System.Drawing.Point(20, 60); lblCampsiteID.Text = "Campsite ID:"; lblCampsiteID.AutoSize = true;
            txtCampsiteID.Location = new System.Drawing.Point(120, 57); txtCampsiteID.ReadOnly = true; txtCampsiteID.Size = new System.Drawing.Size(150, 23);
            lblSiteName.Location = new System.Drawing.Point(20, 95); lblSiteName.Text = "Site Name:"; lblSiteName.AutoSize = true;
            txtSiteName.Location = new System.Drawing.Point(120, 92); txtSiteName.Size = new System.Drawing.Size(200, 23);
            lblPrice.Location = new System.Drawing.Point(20, 130); lblPrice.Text = "Price/Night:"; lblPrice.AutoSize = true;
            txtPrice.Location = new System.Drawing.Point(120, 127); txtPrice.Size = new System.Drawing.Size(150, 23);
            lblStatus.Location = new System.Drawing.Point(20, 165); lblStatus.Text = "Status:"; lblStatus.AutoSize = true;
            cmbStatus.Items.AddRange(new object[] { "Available", "Booked", "Maintenance" });
            cmbStatus.Location = new System.Drawing.Point(120, 162); cmbStatus.Size = new System.Drawing.Size(150, 23);
            lblAdmin.Location = new System.Drawing.Point(20, 200); lblAdmin.Text = "Managed By:"; lblAdmin.AutoSize = true;
            cmbAdmin.Location = new System.Drawing.Point(120, 197); cmbAdmin.Size = new System.Drawing.Size(150, 23);
            btnAdd.Location = new System.Drawing.Point(350, 55); btnAdd.Size = new System.Drawing.Size(90, 30); btnAdd.Text = "Add"; btnAdd.UseVisualStyleBackColor = true; btnAdd.Click += btnAdd_Click;
            btnUpdate.Location = new System.Drawing.Point(450, 55); btnUpdate.Size = new System.Drawing.Size(90, 30); btnUpdate.Text = "Update"; btnUpdate.UseVisualStyleBackColor = true; btnUpdate.Click += btnUpdate_Click;
            btnDelete.Location = new System.Drawing.Point(550, 55); btnDelete.Size = new System.Drawing.Size(90, 30); btnDelete.Text = "Delete"; btnDelete.UseVisualStyleBackColor = true; btnDelete.Click += btnDelete_Click;
            btnSearch.Location = new System.Drawing.Point(550, 195); btnSearch.Size = new System.Drawing.Size(90, 30); btnSearch.Text = "Search"; btnSearch.UseVisualStyleBackColor = true; btnSearch.Click += btnSearch_Click;
            btnViewAll.Location = new System.Drawing.Point(450, 195); btnViewAll.Size = new System.Drawing.Size(90, 30); btnViewAll.Text = "View All"; btnViewAll.UseVisualStyleBackColor = true; btnViewAll.Click += btnViewAll_Click;
            btnClear.Location = new System.Drawing.Point(350, 195); btnClear.Size = new System.Drawing.Size(90, 30); btnClear.Text = "Clear"; btnClear.UseVisualStyleBackColor = true; btnClear.Click += btnClear_Click;
            txtSearch.Location = new System.Drawing.Point(120, 198); txtSearch.Size = new System.Drawing.Size(320, 23);
            lblSearch.Location = new System.Drawing.Point(20, 201); lblSearch.Text = "Search:"; lblSearch.AutoSize = true;
            dgvCampsites.AllowUserToAddRows = false; dgvCampsites.AllowUserToDeleteRows = false;
            dgvCampsites.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCampsites.Location = new System.Drawing.Point(20, 240); dgvCampsites.ReadOnly = true;
            dgvCampsites.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCampsites.Size = new System.Drawing.Size(720, 200);
            dgvCampsites.CellClick += dgvCampsites_CellClick;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(lblTitle); this.Controls.Add(lblCampsiteID); this.Controls.Add(txtCampsiteID);
            this.Controls.Add(lblSiteName); this.Controls.Add(txtSiteName);
            this.Controls.Add(lblPrice); this.Controls.Add(txtPrice);
            this.Controls.Add(lblStatus); this.Controls.Add(cmbStatus);
            this.Controls.Add(lblAdmin); this.Controls.Add(cmbAdmin);
            this.Controls.Add(btnAdd); this.Controls.Add(btnUpdate); this.Controls.Add(btnDelete);
            this.Controls.Add(btnSearch); this.Controls.Add(btnViewAll); this.Controls.Add(btnClear);
            this.Controls.Add(txtSearch); this.Controls.Add(lblSearch); this.Controls.Add(dgvCampsites);
            this.Name = "CampsiteControl";
            this.Load += CampsiteControl_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvCampsites)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }

        private Label lblTitle;
        private Label lblCampsiteID;
        private TextBox txtCampsiteID;
        private Label lblSiteName;
        private TextBox txtSiteName;
        private Label lblPrice;
        private TextBox txtPrice;
        private Label lblStatus;
        private ComboBox cmbStatus;
        private Label lblAdmin;
        private ComboBox cmbAdmin;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnViewAll;
        private Button btnClear;
        private TextBox txtSearch;
        private Label lblSearch;
        private DataGridView dgvCampsites;
    }
}