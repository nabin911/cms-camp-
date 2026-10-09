using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CampManagementSystem
{
    // DFD Process 5a: Manage Equipment (UserControl)
    public partial class EquipmentControl : UserControl
    {
        public EquipmentControl() { InitializeComponent(); }

        private void EquipmentControl_Load(object sender, EventArgs e) { LoadData(); }

        private void LoadData()
        {
            string sql = "SELECT EquipmentID, EquipmentName, RentalPrice FROM dbo.Equipment ORDER BY EquipmentID DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            dgvEquipment.DataSource = dt;
        }

        private void dgvEquipment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvEquipment.Rows[e.RowIndex];
            txtEquipmentID.Text = row.Cells["EquipmentID"].Value.ToString();
            txtEquipmentName.Text = row.Cells["EquipmentName"].Value.ToString();
            txtRentalPrice.Text = row.Cells["RentalPrice"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEquipmentName.Text) ||
                !decimal.TryParse(txtRentalPrice.Text, out _))
            {
                MessageBox.Show("Please enter a valid name and rental price.");
                return;
            }
            try
            {
                string sql = "INSERT INTO dbo.Equipment (EquipmentName, RentalPrice) VALUES (@n, @p)";
                SqlParameter[] p = {
                    new SqlParameter("@n", txtEquipmentName.Text.Trim()),
                    new SqlParameter("@p", decimal.Parse(txtRentalPrice.Text))
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Equipment added.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEquipmentID.Text))
            {
                MessageBox.Show("Please select equipment to update.");
                return;
            }
            try
            {
                string sql = "UPDATE dbo.Equipment SET EquipmentName = @n, RentalPrice = @p WHERE EquipmentID = @id";
                SqlParameter[] p = {
                    new SqlParameter("@n", txtEquipmentName.Text.Trim()),
                    new SqlParameter("@p", decimal.Parse(txtRentalPrice.Text)),
                    new SqlParameter("@id", int.Parse(txtEquipmentID.Text))
                };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Equipment updated.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEquipmentID.Text))
            {
                MessageBox.Show("Please select equipment to delete.");
                return;
            }
            if (MessageBox.Show("Delete this equipment?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                string sql = "DELETE FROM dbo.Equipment WHERE EquipmentID = @id";
                SqlParameter[] p = { new SqlParameter("@id", int.Parse(txtEquipmentID.Text)) };
                DatabaseHelper.ExecuteNonQuery(sql, p);
                MessageBox.Show("Equipment deleted.");
                LoadData();
                Clear();
            }
            catch (Exception ex) { MessageBox.Show("Cannot delete: " + ex.Message); }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = "SELECT EquipmentID, EquipmentName, RentalPrice FROM dbo.Equipment WHERE EquipmentName LIKE @s ORDER BY EquipmentID DESC";
            SqlParameter[] p = { new SqlParameter("@s", "%" + txtSearch.Text.Trim() + "%") };
            dgvEquipment.DataSource = DatabaseHelper.ExecuteQuery(sql, p);
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadData();
            txtSearch.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e) { Clear(); }

        private void Clear()
        {
            txtEquipmentID.Clear();
            txtEquipmentName.Clear();
            txtRentalPrice.Clear();
            txtSearch.Clear();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblEquipmentID = new Label();
            this.lblName = new Label();
            this.lblPrice = new Label();
            this.txtEquipmentID = new TextBox();
            this.txtEquipmentName = new TextBox();
            this.txtRentalPrice = new TextBox();
            this.btnAdd = new Button();
            this.btnUpdate = new Button();
            this.btnDelete = new Button();
            this.btnSearch = new Button();
            this.btnViewAll = new Button();
            this.btnClear = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.dgvEquipment = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipment)).BeginInit();
            this.SuspendLayout();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(20, 15); lblTitle.Text = "Manage Equipment";
            lblEquipmentID.Location = new System.Drawing.Point(20, 60); lblEquipmentID.Text = "Equipment ID:"; lblEquipmentID.AutoSize = true;
            txtEquipmentID.Location = new System.Drawing.Point(120, 57); txtEquipmentID.ReadOnly = true; txtEquipmentID.Size = new System.Drawing.Size(150, 23);
            lblName.Location = new System.Drawing.Point(20, 95); lblName.Text = "Name:"; lblName.AutoSize = true;
            txtEquipmentName.Location = new System.Drawing.Point(120, 92); txtEquipmentName.Size = new System.Drawing.Size(220, 23);
            lblPrice.Location = new System.Drawing.Point(20, 130); lblPrice.Text = "Rental Price:"; lblPrice.AutoSize = true;
            txtRentalPrice.Location = new System.Drawing.Point(120, 127); txtRentalPrice.Size = new System.Drawing.Size(150, 23);
            btnAdd.Location = new System.Drawing.Point(370, 55); btnAdd.Size = new System.Drawing.Size(90, 30); btnAdd.Text = "Add"; btnAdd.UseVisualStyleBackColor = true; btnAdd.Click += btnAdd_Click;
            btnUpdate.Location = new System.Drawing.Point(470, 55); btnUpdate.Size = new System.Drawing.Size(90, 30); btnUpdate.Text = "Update"; btnUpdate.UseVisualStyleBackColor = true; btnUpdate.Click += btnUpdate_Click;
            btnDelete.Location = new System.Drawing.Point(570, 55); btnDelete.Size = new System.Drawing.Size(90, 30); btnDelete.Text = "Delete"; btnDelete.UseVisualStyleBackColor = true; btnDelete.Click += btnDelete_Click;
            btnSearch.Location = new System.Drawing.Point(570, 125); btnSearch.Size = new System.Drawing.Size(90, 30); btnSearch.Text = "Search"; btnSearch.UseVisualStyleBackColor = true; btnSearch.Click += btnSearch_Click;
            btnViewAll.Location = new System.Drawing.Point(470, 125); btnViewAll.Size = new System.Drawing.Size(90, 30); btnViewAll.Text = "View All"; btnViewAll.UseVisualStyleBackColor = true; btnViewAll.Click += btnViewAll_Click;
            btnClear.Location = new System.Drawing.Point(370, 125); btnClear.Size = new System.Drawing.Size(90, 30); btnClear.Text = "Clear"; btnClear.UseVisualStyleBackColor = true; btnClear.Click += btnClear_Click;
            txtSearch.Location = new System.Drawing.Point(120, 128); txtSearch.Size = new System.Drawing.Size(240, 23);
            lblSearch.Location = new System.Drawing.Point(20, 131); lblSearch.Text = "Search:"; lblSearch.AutoSize = true;
            dgvEquipment.AllowUserToAddRows = false; dgvEquipment.AllowUserToDeleteRows = false;
            dgvEquipment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEquipment.Location = new System.Drawing.Point(20, 180); dgvEquipment.ReadOnly = true;
            dgvEquipment.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEquipment.Size = new System.Drawing.Size(720, 250);
            dgvEquipment.CellClick += dgvEquipment_CellClick;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(lblTitle); this.Controls.Add(lblEquipmentID); this.Controls.Add(txtEquipmentID);
            this.Controls.Add(lblName); this.Controls.Add(txtEquipmentName);
            this.Controls.Add(lblPrice); this.Controls.Add(txtRentalPrice);
            this.Controls.Add(btnAdd); this.Controls.Add(btnUpdate); this.Controls.Add(btnDelete);
            this.Controls.Add(btnSearch); this.Controls.Add(btnViewAll); this.Controls.Add(btnClear);
            this.Controls.Add(txtSearch); this.Controls.Add(lblSearch); this.Controls.Add(dgvEquipment);
            this.Name = "EquipmentControl";
            this.Load += EquipmentControl_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipment)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }

        private Label lblTitle;
        private Label lblEquipmentID;
        private TextBox txtEquipmentID;
        private Label lblName;
        private TextBox txtEquipmentName;
        private Label lblPrice;
        private TextBox txtRentalPrice;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnViewAll;
        private Button btnClear;
        private TextBox txtSearch;
        private Label lblSearch;
        private DataGridView dgvEquipment;
    }
}