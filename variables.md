# Variable & Control Reference

> Complete inventory of every field, control, and parameter used in the project. Extracted directly from the source files so the docs always match the code.

---

## 📋 Naming Convention

| Prefix | Control / Type | Example |
|---|---|---|
| `btn` | `Button` | `btnAdd` |
| `txt` | `TextBox` | `txtSiteName` |
| `lbl` | `Label` | `lblPrice` |
| `cmb` | `ComboBox` | `cmbStatus` |
| `dt` | `DateTimePicker` | `dtCheckIn` |
| `dgv` | `DataGridView` | `dgvCampsites` |

---

## 1. `Program.cs`

```csharp
internal static class Program
{
    [STAThread]
    private static void Main()    // entry point — Application.Run(new LoginForm())
}
```

| Member | Type | Notes |
|---|---|---|
| `Main()` | `static void` | Bootstraps WinForms and starts `LoginForm` |

---

## 2. `DatabaseHelper.cs`

| Member | Type | Notes |
|---|---|---|
| `connString` | `static readonly string` | LocalDB connection string |
| `GetConnection()` | `static SqlConnection` | Returns a new `SqlConnection` |
| `ExecuteNonQuery(sql, parameters)` | `static int` | INSERT/UPDATE/DELETE → returns rows affected |
| `ExecuteQuery(sql, parameters)` | `static DataTable` | SELECT → returns `DataTable` via `SqlDataAdapter.Fill` |
| `ExecuteScalar(sql, parameters)` | `static object` | SELECT single value (e.g. login check) |

**Connection string (default):**
```
Server=(localdb)\MSSQLLocalDB;Database=CampManagementDB;Integrated Security=True;
```

---

## 3. `LoginForm.cs` — DFD Process 1 (Authenticate User)

| Field | Type | Purpose |
|---|---|---|
| `currentRole` | `string` | Full name returned after successful login |

| Control | Type | Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Camp Booking System - Login" header |
| `label1` | `Label` | "Role:" label |
| `rbCustomer` | `RadioButton` | Select Customer role (default) |
| `rbAdmin` | `RadioButton` | Select Admin role |
| `lblUsername` | `Label` | "Username:" label |
| `txtUsername` | `TextBox` | Username or Email input |
| `lblPassword` | `Label` | "Password:" label |
| `txtPassword` | `TextBox` | Password input (masked `*`) |
| `btnLogin` | `Button` | Validates and authenticates |
| `btnRegister` | `Button` | Quick customer self-register |
| `btnClear` | `Button` | Reset username/password |
| `btnExit` | `Button` | Close the application |

**Event handlers**
- `LoginForm_Load` — selects Customer radio by default
- `btnLogin_Click` — calls `AuthenticateAdmin` or `AuthenticateCustomer`
- `btnRegister_Click` — opens 3 prompts (Name / Email / Password) and inserts a `Customer`
- `btnClear_Click`, `btnExit_Click`
- Helper: `AuthenticateAdmin(string, string) → string` / `AuthenticateCustomer(string, string) → string`
- Helper: `Prompt(string text, string title) → string` — replaces `InputBox`

---

## 4. `MainForm.cs` — Navigation dashboard

| Field | Type | Purpose |
|---|---|---|
| `role` | `string` | Full name of the logged-in user |

| Control | Type | Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Camp Management - Main Menu" |
| `lblWelcome` | `Label` | "Welcome, <name>" |
| `lblRole` | `Label` | "Role: Administrator" or "Role: Customer" |
| `btnCampsites` | `Button` | Opens `CampsiteForm` (DFD 2) |
| `btnBookings` | `Button` | Opens `BookingForm` (DFD 3) |
| `btnPayments` | `Button` | Opens `PaymentForm` (DFD 4) |
| `btnEquipment` | `Button` | Opens `EquipmentForm` (DFD 5a) |
| `btnRentals` | `Button` | Opens `RentalForm` (DFD 5b) |
| `btnLogout` | `Button` | Closes MainForm → back to LoginForm |
| `btnDeleteCampsite` | `Button` | Hidden placeholder (disabled for Customers) |
| `btnDeleteEquipment` | `Button` | Hidden placeholder (disabled for Customers) |

**Event handlers**
- `MainForm_Load` — sets welcome label and disables hidden delete buttons for non-admins
- `btnCampsites_Click`, `btnBookings_Click`, `btnPayments_Click`, `btnEquipment_Click`, `btnRentals_Click`, `btnLogout_Click`

---

## 5. `CampsiteForm.cs` — DFD Process 2 (Manage Campsites)

| Control | Type | Bound to / Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Manage Campsites" |
| `lblCampsiteID` | `Label` | "Campsite ID:" |
| `txtCampsiteID` | `TextBox` (ReadOnly) | Display the CampsiteID of the selected row |
| `lblSiteName` | `Label` | "Site Name:" |
| `txtSiteName` | `TextBox` | Input → `Campsite.SiteName` |
| `lblPrice` | `Label` | "Price/Night:" |
| `txtPrice` | `TextBox` | Input → `Campsite.PricePerNight` |
| `lblStatus` | `Label` | "Status:" |
| `cmbStatus` | `ComboBox` | "Available" / "Booked" / "Maintenance" |
| `lblAdmin` | `Label` | "Managed By:" |
| `cmbAdmin` | `ComboBox` | Bound to `Admin(AdminID, Username)` |
| `btnAdd` | `Button` | INSERT |
| `btnUpdate` | `Button` | UPDATE |
| `btnDelete` | `Button` | DELETE |
| `btnSearch` | `Button` | Search by Site Name / Admin Username |
| `btnViewAll` | `Button` | Reload all campsites |
| `btnClear` | `Button` | Reset form |
| `txtSearch` | `TextBox` | Search keyword |
| `lblSearch` | `Label` | "Search:" |
| `dgvCampsites` | `DataGridView` | Shows `CampsiteID, SiteName, PricePerNight, AvailabilityStatus, Admin` |

**Methods**
- `LoadData()` — `SELECT ... FROM Campsite JOIN Admin ORDER BY CampsiteID DESC`
- `LoadAdmins()` — populates `cmbAdmin`
- `dgvCampsites_CellClick` — fills inputs from selected row
- `btnAdd_Click / btnUpdate_Click / btnDelete_Click / btnSearch_Click / btnViewAll_Click / btnClear_Click`
- `ValidateInputs()` — non-empty + numeric price

---

## 6. `BookingForm.cs` — DFD Process 3 (Manage Bookings)

| Control | Type | Bound to / Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Manage Bookings" |
| `lblBookingID` | `Label` | "Booking ID:" |
| `txtBookingID` | `TextBox` (ReadOnly) | Selected `BookingID` |
| `lblCheckIn` | `Label` | "Check-In:" |
| `dtCheckIn` | `DateTimePicker` | `Booking.CheckInDate` |
| `lblCheckOut` | `Label` | "Check-Out:" |
| `dtCheckOut` | `DateTimePicker` | `Booking.CheckOutDate` |
| `lblStatus` | `Label` | "Status:" |
| `cmbStatus` | `ComboBox` | "Pending" / "Confirmed" / "Cancelled" |
| `lblCustomer` | `Label` | "Customer:" |
| `cmbCustomer` | `ComboBox` | Bound to `Customer(CustomerID, FullName)` |
| `lblCampsite` | `Label` | "Campsite:" |
| `cmbCampsite` | `ComboBox` | Bound to `Campsite(CampsiteID, SiteName)` |
| `btnAdd` / `btnUpdate` / `btnDelete` / `btnSearch` / `btnViewAll` / `btnClear` | `Button` | Standard CRUD |
| `txtSearch` | `TextBox` | Search keyword |
| `lblSearch` | `Label` | "Search:" |
| `dgvBookings` | `DataGridView` | Shows `BookingID, CheckInDate, CheckOutDate, Status, Customer, Campsite` |

**Methods**
- `LoadData()` — `SELECT ... FROM Booking JOIN Customer JOIN Campsite`
- `LoadCustomers()`, `LoadCampsites()` — populate combos
- Validates `dtCheckOut > dtCheckIn` in `btnAdd_Click`

---

## 7. `PaymentForm.cs` — DFD Process 4 (Manage Payments)

| Control | Type | Bound to / Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Manage Payments" |
| `lblPaymentID` | `Label` | "Payment ID:" |
| `txtPaymentID` | `TextBox` (ReadOnly) | Selected `PaymentID` |
| `lblAmount` | `Label` | "Amount:" |
| `txtAmount` | `TextBox` | Numeric → `Payment.Amount` |
| `lblDate` | `Label` | "Date:" |
| `dtPaymentDate` | `DateTimePicker` | `Payment.PaymentDate` |
| `lblMode` | `Label` | "Mode:" |
| `cmbPaymentMode` | `ComboBox` | "Cash" / "Card" / "UPI" / "NetBanking" |
| `lblStatus` | `Label` | "Status:" |
| `cmbPaymentStatus` | `ComboBox` | "Paid" / "Pending" / "Refunded" |
| `lblBooking` | `Label` | "Booking:" |
| `cmbBooking` | `ComboBox` | Shows `"<Customer> - <Campsite>"`, value = `BookingID` |
| `btnAdd` / `btnUpdate` / `btnDelete` / `btnSearch` / `btnViewAll` / `btnClear` | `Button` | Standard CRUD |
| `txtSearch` | `TextBox` | Search keyword |
| `lblSearch` | `Label` | "Search:" |
| `dgvPayments` | `DataGridView` | Shows `PaymentID, Amount, PaymentDate, PaymentMode, PaymentStatus, BookingID` |

---

## 8. `EquipmentForm.cs` — DFD Process 5a (Manage Equipment)

| Control | Type | Bound to / Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Manage Equipment" |
| `lblEquipmentID` | `Label` | "Equipment ID:" |
| `txtEquipmentID` | `TextBox` (ReadOnly) | Selected `EquipmentID` |
| `lblName` | `Label` | "Name:" |
| `txtEquipmentName` | `TextBox` | → `Equipment.EquipmentName` |
| `lblPrice` | `Label` | "Rental Price:" |
| `txtRentalPrice` | `TextBox` | Numeric → `Equipment.RentalPrice` |
| `btnAdd` / `btnUpdate` / `btnDelete` / `btnSearch` / `btnViewAll` / `btnClear` | `Button` | Standard CRUD |
| `txtSearch` | `TextBox` | Search by name |
| `lblSearch` | `Label` | "Search:" |
| `dgvEquipment` | `DataGridView` | Shows `EquipmentID, EquipmentName, RentalPrice` |

---

## 9. `RentalForm.cs` — DFD Process 5b (Manage Equipment Rentals)

| Control | Type | Bound to / Purpose |
|---|---|---|
| `lblTitle` | `Label` | "Manage Equipment Rentals" |
| `lblRentalID` | `Label` | "Rental ID:" |
| `txtRentalID` | `TextBox` (ReadOnly) | Selected `RentalID` |
| `lblQuantity` | `Label` | "Quantity:" |
| `txtQuantity` | `TextBox` | Positive integer → `Equipment_Rental.Quantity` |
| `lblStatus` | `Label` | "Return Status:" |
| `cmbReturnStatus` | `ComboBox` | "Returned" / "Not Returned" |
| `lblBooking` | `Label` | "Booking:" |
| `cmbBooking` | `ComboBox` | Shows `"<Customer> - <Campsite>"`, value = `BookingID` |
| `lblEquipment` | `Label` | "Equipment:" |
| `cmbEquipment` | `ComboBox` | Bound to `Equipment(EquipmentID, EquipmentName)` |
| `btnAdd` / `btnUpdate` / `btnDelete` / `btnSearch` / `btnViewAll` / `btnClear` | `Button` | Standard CRUD |
| `txtSearch` | `TextBox` | Search by equipment/booking/status |
| `lblSearch` | `Label` | "Search:" |
| `dgvRentals` | `DataGridView` | Shows `RentalID, Quantity, ReturnStatus, BookingID, EquipmentName` |

**Methods**
- `LoadData()` — joins `Equipment_Rental`, `Booking`, `Equipment`
- `LoadBookings()`, `LoadEquipment()` — populate combos

---

## 🔁 Common Helper Method Signatures

Found in every CRUD form:

```csharp
private void LoadData();                       // populates DataGridView
private void dgv<Xxx>_CellClick(...);          // loads selected row into inputs
private void btnAdd_Click(...);                // INSERT
private void btnUpdate_Click(...);             // UPDATE
private void btnDelete_Click(...);             // DELETE
private void btnSearch_Click(...);             // LIKE search via @s param
private void btnViewAll_Click(...);            // LoadData() + clear search
private void btnClear_Click(...);              // Clear() inputs
private void Clear();                          // resets all inputs + defaults combos
```

---

## 🧮 SQL Parameters Used (everywhere)

| Param name | Where | Sample |
|---|---|---|
| `@u`, `@p` | `LoginForm` | username / password |
| `@n`, `@e`, `@p` | `LoginForm.btnRegister` | customer name / email / password |
| `@n`, `@p`, `@s`, `@a` | `CampsiteForm` | site name / price / status / admin id |
| `@id` | all DELETE/UPDATE | primary key |
| `@ci`, `@co`, `@st`, `@c`, `@cs` | `BookingForm` | check-in / check-out / status / customer id / campsite id |
| `@a`, `@d`, `@m`, `@s`, `@b` | `PaymentForm` | amount / date / mode / status / booking id |
| `@q`, `@rs`, `@b`, `@e` | `RentalForm` | quantity / return status / booking id / equipment id |
| `@s` | all `btnSearch_Click` | LIKE search keyword (`%" + txtSearch.Text + "%"`) |

---

## ✅ Quick Counts

| Category | Count |
|---|---|
| Source `.cs` files | 9 |
| Forms (Form classes) | 7 |
| Standard CRUD buttons per form | 6 (`btnAdd`, `btnUpdate`, `btnDelete`, `btnSearch`, `btnViewAll`, `btnClear`) |
| Database tables | 7 |
| CRUD forms with DataGridView | 6 |
| Connection-string locations | 1 (`DatabaseHelper.cs:18`) |