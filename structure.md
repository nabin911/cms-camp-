# Project Structure

> Complete map of every file in the Camp Management System project. Use this as a quick reference during the viva or when navigating the code.

---

## 📁 Root — `C:\Users\sthan\source\repos\cms(camp)`

```
cms(camp)/
├── cms(camp).csproj               # .NET 10 (net10.0-windows) SDK project
├── cms(camp).csproj.user          # VS user-specific settings
├── cms(camp).sln / .slnx          # Solution files
├── Program.cs                     # ⭐ Entry point (starts LoginForm)
│
├── DatabaseHelper.cs              # ⭐ ADO.NET helper (SqlConnection wrapper)
│
├── LoginForm.cs                   # DFD Process 1 — Authenticate User
├── MainForm.cs                    # Navigation dashboard (opens each module)
├── CampsiteForm.cs                # DFD Process 2 — Manage Campsites
├── BookingForm.cs                 # DFD Process 3 — Manage Bookings
├── PaymentForm.cs                 # DFD Process 4 — Manage Payments
├── EquipmentForm.cs               # DFD Process 5a — Manage Equipment
├── RentalForm.cs                  # DFD Process 5b — Manage Rentals
│
├── SQL/
│   └── CampManagementDB.sql       # ⭐ CREATE TABLE + sample INSERT data
│
├── bin/                           # Build output (generated)
├── obj/                           # Build intermediates (generated)
│
├── README.md                      # Project overview + how to run
├── brain.md                       # AI project memory
├── structure.md                   # ⭐ This file
├── variables.md                   # ⭐ Every variable/control in the project
│
└── system design/                 # Documentation diagrams
    ├── er diagram.jpeg
    ├── 0_level_dfd.png
    └── level 1 diagram.jpeg
```

> ⭐ = file most often referenced during the demo.

---

## 📄 File-by-File Responsibilities

| File | Lines | Purpose |
|---|---|---|
| `Program.cs` | ~17 | Entry point. Calls `ApplicationConfiguration.Initialize()` then `Application.Run(new LoginForm())`. |
| `DatabaseHelper.cs` | ~70 | Static helpers wrapping `SqlConnection`: `ExecuteNonQuery`, `ExecuteQuery` (returns `DataTable` via `SqlDataAdapter`), `ExecuteScalar`. Holds the connection string. |
| `LoginForm.cs` | ~310 | Login screen. Two roles (Admin / Customer), Register button, Exit button. Has a custom `Prompt()` helper to replace `InputBox`. |
| `MainForm.cs` | ~190 | Menu with 5 navigation buttons (one per DFD process) + Logout. Hides Delete buttons when a Customer logs in. |
| `CampsiteForm.cs` | ~265 | CRUD on `Campsite` table. Loads admins into a combo, joins admin username in the grid. |
| `BookingForm.cs` | ~270 | CRUD on `Booking` table. Loads customers + campsites into combos. Validates check-out > check-in. |
| `PaymentForm.cs` | ~260 | CRUD on `Payment` table. Booking combo shows `"Customer - Campsite"`. |
| `EquipmentForm.cs` | ~195 | CRUD on `Equipment` table (master list). |
| `RentalForm.cs` | ~265 | CRUD on `Equipment_Rental` table (rentals per booking). Combo shows booking info + equipment name. |
| `SQL/CampManagementDB.sql` | ~110 | Creates 7 tables with PK/FK constraints, then seeds sample data (2 admins, 3 customers, 4 campsites, 3 bookings, 3 payments, 4 equipment, 3 rentals). |

---

## 🔗 Dependency Map

```
Program.cs
   └── runs → LoginForm.cs
                  └── on login → MainForm.cs
                                   ├── btnCampsites   → CampsiteForm.cs
                                   ├── btnBookings    → BookingForm.cs
                                   ├── btnPayments    → PaymentForm.cs
                                   ├── btnEquipment   → EquipmentForm.cs
                                   └── btnRentals     → RentalForm.cs

Every form  ──→  DatabaseHelper.cs  ──→  SQL Server LocalDB
                                        (Server=(localdb)\MSSQLLocalDB
                                         Database=CampManagementDB)
```

---

## 🗄️ Database Tables (created by `SQL/CampManagementDB.sql`)

```
Admin (AdminID, Username, Password, FullName)
   ↑ AdminID
   │  referenced by
Campsite (CampsiteID, SiteName, PricePerNight, AvailabilityStatus, AdminID FK→Admin)
   ↑ CampsiteID            ↑ CustomerID
   │  referenced by        │  referenced by
Booking (BookingID, CheckInDate, CheckOutDate, Status, CustomerID FK→Customer, CampsiteID FK→Campsite)
   ↑ BookingID
   │  referenced by
Payment (PaymentID, Amount, PaymentDate, PaymentMode, PaymentStatus, BookingID FK→Booking)

Equipment (EquipmentID, EquipmentName, RentalPrice)
   ↑ EquipmentID          ↑ BookingID (also → Booking)
   │  referenced by       │  referenced by
Equipment_Rental (RentalID, Quantity, ReturnStatus, BookingID FK→Booking, EquipmentID FK→Equipment)
```

---

## 📐 DFD Process → File Mapping (Level-1)

| DFD Process | Handled by |
|---|---|
| 1. Authenticate User | `LoginForm.cs` |
| 2. Manage Campsites | `CampsiteForm.cs` |
| 3. Manage Bookings | `BookingForm.cs` |
| 4. Manage Payment | `PaymentForm.cs` |
| 5a. Manage Equipment | `EquipmentForm.cs` |
| 5b. Manage Rentals | `RentalForm.cs` |
| Navigation | `MainForm.cs` |
| Entry / DB access | `Program.cs`, `DatabaseHelper.cs` |

---

## 🧩 Class Layout (per form file)

Each form file follows the same structure (modern .NET 10 SDK style — no separate Designer file):

```csharp
namespace CampManagementSystem
{
    public partial class <FormName> : Form
    {
        public <FormName>() { InitializeComponent(); }   // ctor

        // ----- Event handlers (Load, CellClick, Button clicks) -----
        private void <Form>_Load(...)        { ... }      // runs on form load
        private void dgv<Xxx>_CellClick(...) { ... }      // row click → fills inputs
        private void btnAdd_Click(...)       { ... }      // INSERT
        private void btnUpdate_Click(...)    { ... }      // UPDATE
        private void btnDelete_Click(...)    { ... }      // DELETE
        private void btnSearch_Click(...)    { ... }      // LIKE search
        private void btnViewAll_Click(...)   { ... }      // reload all
        private void btnClear_Click(...)     { ... }      // reset inputs

        // ----- Helper methods -----
        private void LoadData()      { ... }              // populates DataGridView
        private void LoadXxx()       { ... }              // populates a ComboBox
        private bool ValidateInputs(){ ... }              // returns false on bad input
        private void Clear()         { ... }              // empties inputs

        // ----- Designer-generated UI code (inline) -----
        private void InitializeComponent() { ... }

        // ----- Private control fields -----
        private Label lblXxx;
        private TextBox txtXxx;
        // ... etc
    }
}
```

> For the complete list of every control field per file, see **`variables.md`**.