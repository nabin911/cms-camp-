# Camp Management System

A **single-window dashboard-style** **C# Windows Forms App (.NET 10)** for a college semester project.
Built with **Microsoft SQL Server LocalDB** and basic **ADO.NET** (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `DataTable`).
Designed to match the supplied **ER diagram** and **Level-1 DFD** exactly.

---

## ✨ Features
- **Traditional login window** (fixed size, modal) → full-screen **dashboard window** opens after success.
- **One main window at a time** — sidebar navigation, no popup windows for CRUD pages.
- Login for **Admin** and **Customer**.
- Full **CRUD** (Add / View / Update / Delete / Search) for every entity.
- **Parameterized SQL** (`@param`) throughout.
- Clean, single-file WinForms — easy to explain in a viva.
- Built on .NET 10 SDK.
- **Back** and **Logout** buttons in the header.

---

## 🗂 Project Structure
```
cms(camp)/
├── cms(camp).csproj               # .NET 10 (net10.0-windows) SDK project
├── Program.cs                     # Entry point → LoginForm
├── DatabaseHelper.cs              # SqlConnection + ADO.NET helpers
│
├── LoginForm.cs                   # DFD Process 1 — Authenticate User (Form, fixed size)
├── MainForm.cs                    # ⭐ Full-screen dashboard
│                                   (sidebar + content panel)
│
├── CampsiteControl.cs             # DFD Process 2 — Manage Campsites  (UserControl)
├── BookingControl.cs              # DFD Process 3 — Manage Bookings   (UserControl)
├── PaymentControl.cs              # DFD Process 4 — Manage Payments   (UserControl)
├── EquipmentControl.cs            # DFD Process 5a — Manage Equipment (UserControl)
├── RentalControl.cs               # DFD Process 5b — Manage Rentals   (UserControl)
│
├── SQL/CampManagementDB.sql       # CREATE TABLE + sample INSERT data
├── README.md
├── brain.md                       # AI project memory
├── structure.md                   # File-by-file project map
├── variables.md                   # Every variable / control in the project
└── system design/                 # ER diagram + 0-level & Level-1 DFDs
```

---

## 🚀 How to Run

### 1. Make sure LocalDB is running
```powershell
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### 2. Create the database
**Option A — SSMS:** Open `SQL/CampManagementDB.sql` in SSMS, connect to `(localdb)\MSSQLLocalDB`, press F5.

**Option B — sqlcmd:**
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "C:\Users\sthan\source\repos\cms(camp)\SQL\CampManagementDB.sql"
```

### 3. Configure the connection string (if needed)
Open `DatabaseHelper.cs:18` and adjust the `connString`.

### 4. Build & run

**Visual Studio 2022 (17.12+):**
- File → Open → Project/Solution → `cms(camp).csproj`
- Press **F5**

**Command line:**
```powershell
dotnet build "C:\Users\sthan\source\repos\cms(camp)\cms(camp).csproj"
dotnet run   --project "C:\Users\sthan\source\repos\cms(camp)\cms(camp).csproj"
```

The **Login** window opens (small, centered). After login, the **dashboard** opens in **full-screen** (`WindowState = Maximized`). Only one window exists at a time.

---

## 🖥 UI Walk-through

### Login Window (traditional modal)
```
┌──────────────────────────────────────────┐
│   Camp Booking System - Login            │
│                                          │
│   Role:    (•) Customer  ( ) Admin       │
│   Username: [_______________________]    │
│   Password: [_______________________]    │
│                                          │
│        [ Login ]  [ Register ]  [ Clear ]│
│                  [ Exit ]                │
└──────────────────────────────────────────┘
```

### Dashboard Window (full-screen after login)
```
┌──────────────────────────────────────────────────────────────────────┐
│  Camp Management System   Welcome, Admin   Role: Admin  [Back] [Logout]│  pnlHeader
├──────────────┬───────────────────────────────────────────────────────┤
│              │                                                       │
│  🏕 Campsites│                                                       │
│  📅 Bookings │                                                       │
│  💳 Payments │            UserControl (current view)                 │
│  🎒 Equipment│            e.g. CampsiteControl                       │
│  🔄 Rentals  │            ↳ Grid + Add/Update/Delete/Search          │
│              │                                                       │
│  pnlSidebar  │                     pnlContent                         │
└──────────────┴───────────────────────────────────────────────────────┘
```

- Clicking a sidebar item replaces the content panel with the matching `UserControl` (no new window is opened).
- The active sidebar button is highlighted in **SteelBlue**.
- **Back** returns to the default Campsites view.
- **Logout** (or closing the dashboard window) exits the application. The hidden LoginForm never reappears.

---

## 🔑 Sample Login Credentials
| Role | Username | Password |
|------|----------|----------|
| Admin | `admin` | `admin123` |
| Admin | `manager` | `manager123` |
| Customer | `rahul@gmail.com` | `rahul123` |
| Customer | `priya@gmail.com` | `priya123` |
| Customer | `amit@gmail.com` | `amit123` |

> Customers log in with their **Email** as the username. The **Register** button on the login form lets a new customer self-register.

---

## 🧩 ER Diagram → SQL Tables
The exact 7 entities from the ER diagram are mapped 1-to-1 to SQL tables (see `SQL/CampManagementDB.sql`):

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

## 🔁 DFD Level-1 → Modules
| DFD Process | Module (File) | Operations |
|-------------|---------------|-----------|
| **1 – Authenticate User** | `LoginForm.cs` | Login (Admin / Customer), Register |
| **2 – Manage Campsites** | `CampsiteControl.cs` | Add / Update / Delete / Search / View All |
| **3 – Manage Bookings** | `BookingControl.cs` | Add / Update / Delete / Search / View All |
| **4 – Manage Payment** | `PaymentControl.cs` | Add / Update / Delete / Search / View All |
| **5 – Manage Equipment Rental** | `EquipmentControl.cs`, `RentalControl.cs` | Add / Update / Delete / Search / View All |

Every CRUD control exposes the same six button handlers:
`btnAdd_Click`, `btnUpdate_Click`, `btnDelete_Click`, `btnSearch_Click`, `btnViewAll_Click`, `btnClear_Click`, plus `dgvXxx_CellClick`.

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
`SqlDataAdapter.Fill(dt)` populates every `DataGridView`.

---

## 📚 Notes for the Demo
- Show **ER diagram** → open SQL script (`SQL/CampManagementDB.sql`).
- Show **Level-0 DFD** (system + customer + admin).
- Show **Level-1 DFD** → walk through each control and the matching button (all inside the same dashboard).
- Highlight **single-window architecture** — sidebar navigation, only one control visible at a time, dashboard takes full screen.
- Highlight **window-lifecycle discipline** — LoginForm is hidden before MainForm opens; MainForm's `FormClosed` event calls `Application.Exit()` so the hidden LoginForm never reappears.
- Highlight **parameterized queries** in `DatabaseHelper.cs` and explain how they prevent SQL injection.
- Login as **admin/admin123**, perform Add/Update/Delete on Campsite, then on a Booking, then record a Payment for that Booking, then attach a Rental — demonstrating the full process flow from the DFD.

---

## 🛠 Build Notes
- **Target framework:** `net10.0-windows`
- **NuGet:** `System.Data.SqlClient` 4.9.1
- **Latest build:** 0 errors. App launches cleanly.