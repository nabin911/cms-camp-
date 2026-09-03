# Camp Management System

A simple **C# Windows Forms App (.NET 10)** for a college semester project.
Built with **Microsoft SQL Server LocalDB** and basic **ADO.NET** (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `DataTable`).
Designed to match the supplied **ER diagram** and **Level-1 DFD** exactly.

---

## ✨ Features
- Login for **Admin** and **Customer**
- Full **CRUD** (Add / View / Update / Delete / Search) for every entity
- **Parameterized SQL** (`@param`) throughout — safe from SQL injection
- Clean, single-file, code-behind WinForms — easy to explain in a viva
- Built on .NET 10 SDK (works with `dotnet run` from the CLI)

---

## 🗂 Project Structure
```
cms(camp)/
├── cms(camp).csproj               # .NET 10 (net10.0-windows) SDK project
├── Program.cs                     # Entry point → LoginForm
├── DatabaseHelper.cs              # SqlConnection + ADO.NET helpers
├── LoginForm.cs                   # DFD Process 1 — Authenticate User
├── MainForm.cs                    # Navigation dashboard
├── CampsiteForm.cs                # DFD Process 2 — Manage Campsites
├── BookingForm.cs                 # DFD Process 3 — Manage Bookings
├── PaymentForm.cs                 # DFD Process 4 — Manage Payments
├── EquipmentForm.cs               # DFD Process 5a — Manage Equipment
├── RentalForm.cs                  # DFD Process 5b — Manage Rentals
├── SQL/CampManagementDB.sql       # CREATE TABLE + sample INSERT data
├── README.md
├── brain.md                       # AI project memory
└── system design/                 # ER diagram + 0-level & Level-1 DFDs
```

> Modern .NET 10 SDK style — each form's designer code is inlined in the same `.cs` file (no `*.Designer.cs` or `*.resx` files).

---

## 🚀 How to Run

### 1. Make sure LocalDB is running
```powershell
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### 2. Create the database
**Option A — SSMS:** Open `SQL/CampManagementDB.sql` in SQL Server Management Studio, connect to `(localdb)\MSSQLLocalDB`, press F5.

**Option B — sqlcmd (no SSMS needed):**
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "C:\Users\sthan\source\repos\cms(camp)\SQL\CampManagementDB.sql"
```

This creates the database `CampManagementDB`, all 7 tables with PK/FK constraints, and seeds sample data.

### 3. Configure the connection string (if needed)
Open `DatabaseHelper.cs:18` and set the `connString` to match your local SQL Server:
```csharp
// LocalDB (default — already configured)
private static readonly string connString =
    @"Server=(localdb)\MSSQLLocalDB;Database=CampManagementDB;Integrated Security=True;";

// SQL Server Express
// @"Server=.\SQLEXPRESS;Database=CampManagementDB;Integrated Security=True;";

// SQL authentication
// @"Server=.\SQLEXPRESS;Database=CampManagementDB;User Id=sa;Password=yourpass;";
```

### 4. Build & run

**Visual Studio 2022 (17.12+):**
- File → Open → Project/Solution → `cms(camp).csproj`
- Press **F5**

**Command line:**
```powershell
dotnet build "C:\Users\sthan\source\repos\cms(camp)\cms(camp).csproj"
dotnet run   --project "C:\Users\sthan\source\repos\cms(camp)\cms(camp).csproj"
```

The Login window opens. Use the credentials below.

---

## 🔑 Sample Login Credentials
| Role | Username | Password |
|------|----------|----------|
| Admin | `admin` | `admin123` |
| Admin | `manager` | `manager123` |
| Customer | `rahul@gmail.com` | `rahul123` |
| Customer | `priya@gmail.com` | `priya123` |
| Customer | `amit@gmail.com` | `amit123` |

> Customers log in with their **Email** as the username. The **Register** button on the login form lets a new customer self-register (used during demo so extra users can be added on the fly).

---

## 🧩 ER Diagram → SQL Tables
The exact 7 entities from the ER diagram are mapped 1-to-1 to SQL tables:

| Table | Columns |
|-------|---------|
| **Admin** | `AdminID` (PK), `Username`, `Password`, `FullName` |
| **Customer** | `CustomerID` (PK), `FullName`, `Email`, `Password` |
| **Campsite** | `CampsiteID` (PK), `SiteName`, `PricePerNight`, `AvailabilityStatus`, `AdminID` (FK → Admin) |
| **Booking** | `BookingID` (PK), `CheckInDate`, `CheckOutDate`, `Status`, `CustomerID` (FK → Customer), `CampsiteID` (FK → Campsite) |
| **Payment** | `PaymentID` (PK), `Amount`, `PaymentDate`, `PaymentMode`, `PaymentStatus`, `BookingID` (FK → Booking) |
| **Equipment** | `EquipmentID` (PK), `EquipmentName`, `RentalPrice` |
| **Equipment_Rental** | `RentalID` (PK), `Quantity`, `ReturnStatus`, `BookingID` (FK → Booking), `EquipmentID` (FK → Equipment) |

---

## 🔁 DFD Level-1 → Forms
| DFD Process | Form(s) | Operations |
|-------------|---------|-----------|
| **1 – Authenticate User** | `LoginForm` | Login (Admin / Customer), Register new customer |
| **2 – Manage Campsites** | `CampsiteForm` | Add / Update / Delete / Search / View All |
| **3 – Manage Bookings** | `BookingForm` | Add / Update / Delete / Search / View All |
| **4 – Manage Payment** | `PaymentForm` | Add / Update / Delete / Search / View All |
| **5 – Manage Equipment Rental** | `EquipmentForm`, `RentalForm` | Add / Update / Delete / Search / View All |

Every form exposes the same five button handlers used by the DFD processes:
`btnAdd_Click`, `btnUpdate_Click`, `btnDelete_Click`, `btnSearch_Click`, plus `btnViewAll_Click` / `btnClear_Click` and a `dgvXxx_CellClick` to load a row into the inputs.

---

## 🧪 ADO.NET at a Glance
All data access goes through `DatabaseHelper.cs`:
```csharp
// SELECT → DataTable
DataTable dt = DatabaseHelper.ExecuteQuery(
    "SELECT * FROM dbo.Campsite WHERE SiteName LIKE @n",
    new[] { new SqlParameter("@n", "%" + txtSearch.Text + "%") });

// INSERT / UPDATE / DELETE
DatabaseHelper.ExecuteNonQuery(
    "INSERT INTO dbo.Campsite (SiteName, PricePerNight, AvailabilityStatus, AdminID) VALUES (@n,@p,@s,@a)",
    new[] { new SqlParameter("@n", "..."), ... });
```
`SqlDataAdapter.Fill(dt)` is used to populate every `DataGridView`.

---

## 📂 Naming Conventions
| Control | Prefix | Example |
|---------|--------|---------|
| Button | `btn` | `btnAdd`, `btnSearch` |
| TextBox | `txt` | `txtSiteName` |
| ComboBox | `cmb` | `cmbStatus` |
| DateTimePicker | `dt` | `dtCheckIn` |
| DataGridView | `dgv` | `dgvCampsites` |
| Label | `lbl` | `lblPrice` |

---

## 📚 Notes for the Demo
- Show **ER diagram** → open SQL script (`SQL/CampManagementDB.sql`).
- Show **Level-0 DFD** (system + customer + admin).
- Show **Level-1 DFD** → walk through each form and the matching button.
- Highlight **parameterized queries** in `DatabaseHelper.cs` and explain how they prevent SQL injection.
- Login as **admin/admin123**, perform Add/Update/Delete on Campsite, then on a Booking, then record a Payment for that Booking, then attach a Rental — demonstrating the full process flow from the DFD.

---

## 🛠 Build Notes
- **Target framework:** `net10.0-windows`
- **NuGet:** `System.Data.SqlClient` 4.9.1 (added because .NET 10 no longer ships it in the BCL)
- **Build warnings:** ~101 deprecation notices from `System.Data.SqlClient` recommending `Microsoft.Data.SqlClient`. They are harmless — code runs fine. Migrate later if desired.