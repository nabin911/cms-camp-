# Project Brain — Camp Management System

> Persistent project notes for AI assistant sessions on this repo. Update this file whenever the project structure, naming, or decisions change.

---

## 1. Project Identity
- **Name:** Camp Management System (CMS)
- **Purpose:** College semester project — campsite booking & rental management
- **Stack:** C# Windows Forms App (.NET 10) + Microsoft SQL Server LocalDB + ADO.NET
- **Repo root:** `C:\Users\sthan\source\repos\cms(camp)`
- **Old reference (do NOT use, contains web version):** `C:\Users\sthan\source\repos\camp management system`

## 2. Build / Runtime
- **Target framework:** `net10.0-windows` (SDK `10.0.400` confirmed on this machine)
- **Output type:** WinExe
- **Namespace / Assembly:** `CampManagementSystem`
- **Entry point:** `Program.cs` → `Application.Run(new LoginForm())`
- **Build cmd:** `dotnet build cms(camp).csproj`
- **Run cmd:** `dotnet run --project cms(camp).csproj` (or `dotnet cms\(camp\).dll` from bin)
- **NuGet package:** `System.Data.SqlClient` 4.9.1 (added because .NET 10 doesn't ship it in the BCL)
- **Warnings during build:** ~101 deprecation warnings suggesting `Microsoft.Data.SqlClient` — harmless, code still works. Can be silenced later by switching packages.

## 3. Database (Microsoft SQL Server LocalDB)
- **DB name:** `CampManagementDB`
- **Server:** `(localdb)\MSSQLLocalDB` (confirmed running on this machine)
- **Connection string** (in `DatabaseHelper.cs:18`):
  `Server=(localdb)\MSSQLLocalDB;Database=CampManagementDB;Integrated Security=True;`
- **Alternatives:** `Server=.\SQLEXPRESS;` for SQL Server Express; add `User Id=...;Password=...;` for SQL auth.
- **Script:** `SQL/CampManagementDB.sql` — drops & recreates schema + seeds sample data. Already executed; DB and 7 tables exist with seeded rows.

### 3.1 ER Tables (column names must match exactly)
| # | Table | PK | Columns | FK → |
|---|---|---|---|---|
| 1 | Admin | AdminID | Username, Password, FullName | — |
| 2 | Customer | CustomerID | FullName, Email, Password | — |
| 3 | Campsite | CampsiteID | SiteName, PricePerNight, AvailabilityStatus, **AdminID** | Admin |
| 4 | Booking | BookingID | CheckInDate, CheckOutDate, Status, **CustomerID**, **CampsiteID** | Customer, Campsite |
| 5 | Payment | PaymentID | Amount, PaymentDate, PaymentMode, PaymentStatus, **BookingID** | Booking |
| 6 | Equipment | EquipmentID | EquipmentName, RentalPrice | — |
| 7 | Equipment_Rental | RentalID | Quantity, ReturnStatus, **BookingID**, **EquipmentID** | Booking, Equipment |

## 4. DFD → Form Mapping (Level 1)
| DFD Process | Form(s) | Notes |
|---|---|---|
| 1 Authenticate User | LoginForm | Customer uses Email + Password; Admin uses Username + Password |
| 2 Manage Campsites | CampsiteForm | Admin: full CRUD; Customer: view only (delete disabled) |
| 3 Manage Bookings | BookingForm | Full CRUD |
| 4 Manage Payment | PaymentForm | Full CRUD, tied to a Booking |
| 5 Manage Equipment Rental | EquipmentForm (master) + RentalForm (rentals per booking) | Full CRUD on each |

## 5. Source Files (in repo — modern .NET 10 SDK style, no .resx)
- `Program.cs` — entry point
- `DatabaseHelper.cs` — `SqlConnection` helpers (`ExecuteNonQuery`, `ExecuteQuery → DataTable`, `ExecuteScalar`)
- `LoginForm.cs` — login + register (designer code inlined in same file)
- `MainForm.cs` — navigation dashboard
- `CampsiteForm.cs` — CRUD campsites
- `BookingForm.cs` — CRUD bookings
- `PaymentForm.cs` — CRUD payments
- `EquipmentForm.cs` — CRUD equipment
- `RentalForm.cs` — CRUD rentals
- `SQL/CampManagementDB.sql` — schema + seed
- `system design/` — `er diagram.jpeg`, `0_level_dfd.png`, `level 1 diagram.jpeg`
- `README.md`, `brain.md`

> No `*.Designer.cs` or `*.resx` files in this version — the modern .NET 10 SDK auto-generates WinForms designer code inline.

## 7. Coding Conventions (enforce on every edit)
- **Buttons:** `btnAdd`, `btnUpdate`, `btnDelete`, `btnSearch`, `btnViewAll`, `btnClear`
- **TextBoxes:** `txtXxx`
- **Labels:** `lblXxx`
- **ComboBoxes:** `cmbXxx`
- **DateTimePickers:** `dtXxx`
- **DataGridViews:** `dgvXxx`
- **Load data method:** private `LoadData()` (and helper `LoadXxx()` for combo boxes)
- **All SQL is parameterized** using `@param` + `SqlParameter[]`
- **No ORMs, no third-party libs**, no MVVM patterns — keep code-behind flat and explainable
- **Customer role disables Delete on Campsite/Equipment** (logic in `MainForm.MainForm_Load`)
- **`InputBox` replacement:** custom WinForms prompt in `LoginForm.Prompt()` (no `Microsoft.VisualBasic` dependency)

## 8. Sample Login Credentials
- Admin: `admin` / `admin123` (also `manager` / `manager123`)
- Customers (use email as username): `rahul@gmail.com` / `rahul123`, `priya@gmail.com` / `priya123`, `amit@gmail.com` / `amit123`

## 9. Recent Decisions / History
- Started as a .NET 10 Windows Forms project (original repo scaffolding).
- Briefly switched to .NET Framework 4.7.2 per original user request — but the user only has .NET 10 SDK installed, so reverted back to .NET 10 (`net10.0-windows`).
- Merged each form's `*.Designer.cs` + `*.resx` into a single `.cs` file (modern SDK style).
- Replaced `Microsoft.VisualBasic.Interaction.InputBox` with a custom prompt dialog so the project has no extra assembly reference.
- Added `System.Data.SqlClient` 4.9.1 NuGet package (not in .NET 10 BCL by default).
- DB created and seeded successfully via `sqlcmd -S "(localdb)\MSSQLLocalDB" -i SQL\CampManagementDB.sql`.
- Verified all 7 tables contain sample rows (Admin=2, Customer=3, Campsite=4, Booking=3, Payment=3, Equipment=4, Equipment_Rental=3).
- App launched and login form opens successfully.

## 10. Future / TODO (only if user asks)
- Hash passwords (currently plain text — fine for college demo, not for production)
- Migrate to `Microsoft.Data.SqlClient` to silence the 101 deprecation warnings
- Role-based UI hiding per row instead of blanket delete disable
- Reports view (bookings/payments per date range)

## 11. Quick "How to Run" (for AI/handoff)
1. Ensure LocalDB is running: `sqllocaldb start MSSQLLocalDB`
2. If DB not yet created: `sqlcmd -S "(localdb)\MSSQLLocalDB" -i "C:\Users\sthan\source\repos\cms(camp)\SQL\CampManagementDB.sql"`
3. From VS: open `cms(camp).csproj` → F5.
4. From CLI: `dotnet run --project "C:\Users\sthan\source\repos\cms(camp)\cms(camp).csproj"`
5. Login: `admin` / `admin123` (admin) or `rahul@gmail.com` / `rahul123` (customer).