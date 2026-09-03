-- ============================================================
-- Camp Management System - Database Script
-- Matches the ER Diagram exactly (table & column names).
-- ============================================================

-- Create the database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CampManagementDB')
BEGIN
    CREATE DATABASE CampManagementDB;
END
GO

USE CampManagementDB;
GO

-- Drop in dependency order so script can be re-run
IF OBJECT_ID('dbo.Payment', 'U') IS NOT NULL DROP TABLE dbo.Payment;
IF OBJECT_ID('dbo.Equipment_Rental', 'U') IS NOT NULL DROP TABLE dbo.Equipment_Rental;
IF OBJECT_ID('dbo.Booking', 'U') IS NOT NULL DROP TABLE dbo.Booking;
IF OBJECT_ID('dbo.Equipment', 'U') IS NOT NULL DROP TABLE dbo.Equipment;
IF OBJECT_ID('dbo.Campsite', 'U') IS NOT NULL DROP TABLE dbo.Campsite;
IF OBJECT_ID('dbo.Customer', 'U') IS NOT NULL DROP TABLE dbo.Customer;
IF OBJECT_ID('dbo.Admin', 'U') IS NOT NULL DROP TABLE dbo.Admin;
GO

-- =========================
-- Admin
-- =========================
CREATE TABLE dbo.Admin
(
    AdminID    INT IDENTITY(1,1) NOT NULL,
    Username   VARCHAR(50)  NOT NULL,
    Password   VARCHAR(50)  NOT NULL,
    FullName   VARCHAR(100) NOT NULL,
    CONSTRAINT PK_Admin PRIMARY KEY (AdminID),
    CONSTRAINT UQ_Admin_Username UNIQUE (Username)
);
GO

-- =========================
-- Customer
-- =========================
CREATE TABLE dbo.Customer
(
    CustomerID INT IDENTITY(1,1) NOT NULL,
    FullName   VARCHAR(100) NOT NULL,
    Email      VARCHAR(100) NOT NULL,
    Password   VARCHAR(50)  NOT NULL,
    CONSTRAINT PK_Customer PRIMARY KEY (CustomerID),
    CONSTRAINT UQ_Customer_Email UNIQUE (Email)
);
GO

-- =========================
-- Campsite
-- =========================
CREATE TABLE dbo.Campsite
(
    CampsiteID         INT IDENTITY(1,1) NOT NULL,
    SiteName           VARCHAR(100) NOT NULL,
    PricePerNight      DECIMAL(10,2) NOT NULL,
    AvailabilityStatus VARCHAR(20)  NOT NULL,
    AdminID            INT NOT NULL,
    CONSTRAINT PK_Campsite PRIMARY KEY (CampsiteID),
    CONSTRAINT FK_Campsite_Admin FOREIGN KEY (AdminID) REFERENCES dbo.Admin(AdminID)
);
GO

-- =========================
-- Booking
-- =========================
CREATE TABLE dbo.Booking
(
    BookingID    INT IDENTITY(1,1) NOT NULL,
    CheckInDate  DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    Status       VARCHAR(20) NOT NULL,
    CustomerID   INT NOT NULL,
    CampsiteID   INT NOT NULL,
    CONSTRAINT PK_Booking PRIMARY KEY (BookingID),
    CONSTRAINT FK_Booking_Customer FOREIGN KEY (CustomerID) REFERENCES dbo.Customer(CustomerID),
    CONSTRAINT FK_Booking_Campsite FOREIGN KEY (CampsiteID) REFERENCES dbo.Campsite(CampsiteID)
);
GO

-- =========================
-- Payment
-- =========================
CREATE TABLE dbo.Payment
(
    PaymentID     INT IDENTITY(1,1) NOT NULL,
    Amount        DECIMAL(10,2) NOT NULL,
    PaymentDate   DATE NOT NULL,
    PaymentMode   VARCHAR(20) NOT NULL,
    PaymentStatus VARCHAR(20) NOT NULL,
    BookingID     INT NOT NULL,
    CONSTRAINT PK_Payment PRIMARY KEY (PaymentID),
    CONSTRAINT FK_Payment_Booking FOREIGN KEY (BookingID) REFERENCES dbo.Booking(BookingID)
);
GO

-- =========================
-- Equipment
-- =========================
CREATE TABLE dbo.Equipment
(
    EquipmentID   INT IDENTITY(1,1) NOT NULL,
    EquipmentName VARCHAR(100) NOT NULL,
    RentalPrice   DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_Equipment PRIMARY KEY (EquipmentID)
);
GO

-- =========================
-- Equipment_Rental
-- =========================
CREATE TABLE dbo.Equipment_Rental
(
    RentalID     INT IDENTITY(1,1) NOT NULL,
    Quantity     INT NOT NULL,
    ReturnStatus VARCHAR(20) NOT NULL,
    BookingID    INT NOT NULL,
    EquipmentID  INT NOT NULL,
    CONSTRAINT PK_EquipmentRental PRIMARY KEY (RentalID),
    CONSTRAINT FK_Rental_Booking   FOREIGN KEY (BookingID)   REFERENCES dbo.Booking(BookingID),
    CONSTRAINT FK_Rental_Equipment FOREIGN KEY (EquipmentID) REFERENCES dbo.Equipment(EquipmentID)
);
GO

-- ============================================================
-- Sample Data
-- ============================================================

INSERT INTO dbo.Admin (Username, Password, FullName) VALUES
('admin', 'admin123', 'System Administrator'),
('manager', 'manager123', 'Camp Manager');
GO

INSERT INTO dbo.Customer (FullName, Email, Password) VALUES
('Rahul Sharma', 'rahul@gmail.com', 'rahul123'),
('Priya Verma',  'priya@gmail.com', 'priya123'),
('Amit Singh',   'amit@gmail.com',  'amit123');
GO

INSERT INTO dbo.Campsite (SiteName, PricePerNight, AvailabilityStatus, AdminID) VALUES
('Pine Valley',   1200.00, 'Available', 1),
('Lake Side',     1500.00, 'Available', 1),
('Mountain View', 1800.00, 'Booked',    2),
('River Bend',    1000.00, 'Available', 1);
GO

INSERT INTO dbo.Booking (CheckInDate, CheckOutDate, Status, CustomerID, CampsiteID) VALUES
('2026-03-10', '2026-03-13', 'Confirmed', 1, 1),
('2026-03-15', '2026-03-18', 'Pending',   2, 2),
('2026-04-01', '2026-04-05', 'Confirmed', 3, 4);
GO

INSERT INTO dbo.Payment (Amount, PaymentDate, PaymentMode, PaymentStatus, BookingID) VALUES
(3600.00, '2026-03-09', 'Card',    'Paid',    1),
(4500.00, '2026-03-14', 'Cash',    'Pending', 2),
(4000.00, '2026-03-30', 'UPI',     'Paid',    3);
GO

INSERT INTO dbo.Equipment (EquipmentName, RentalPrice) VALUES
('Tent',        300.00),
('Sleeping Bag', 150.00),
('Camp Stove',   200.00),
('Lantern',       50.00);
GO

INSERT INTO dbo.Equipment_Rental (Quantity, ReturnStatus, BookingID, EquipmentID) VALUES
(2, 'Returned', 1, 1),
(1, 'Not Returned', 2, 2),
(3, 'Returned', 3, 3);
GO

SELECT 'CampManagementDB created and seeded successfully.' AS Result;
GO