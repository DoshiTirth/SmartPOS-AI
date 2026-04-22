
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SmartPOSDB')
BEGIN
    CREATE DATABASE SmartPOSDB;
END
GO

USE SmartPOSDB;
GO

-- USERS & AUTH
CREATE TABLE Roles (
    RoleId      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50) NOT NULL UNIQUE,  -- Admin, Manager, Cashier
    Description NVARCHAR(200) NULL,
    CreatedAt   DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE Users (
    UserId        INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(100) NOT NULL,
    Email         NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(512) NOT NULL,
    RoleId        INT NOT NULL REFERENCES Roles(RoleId),
    IsActive      BIT DEFAULT 1,
    CreatedAt     DATETIME2 DEFAULT GETUTCDATE(),
    LastLoginAt   DATETIME2 NULL
);

-- PRODUCTS & INVENTORY

CREATE TABLE Categories (
    CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description  NVARCHAR(200) NULL,
    CreatedAt    DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE Products (
    ProductId     INT IDENTITY(1,1) PRIMARY KEY,
    ProductName   NVARCHAR(150) NOT NULL,
    SKU           NVARCHAR(50) NOT NULL UNIQUE,
    Barcode       NVARCHAR(100) NULL UNIQUE,
    CategoryId    INT NOT NULL REFERENCES Categories(CategoryId),
    UnitPrice     DECIMAL(10,2) NOT NULL,
    CostPrice     DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    LowStockThreshold INT NOT NULL DEFAULT 10,
    IsActive      BIT DEFAULT 1,
    CreatedAt     DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt     DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE StockAdjustments (
    AdjustmentId   INT IDENTITY(1,1) PRIMARY KEY,
    ProductId      INT NOT NULL REFERENCES Products(ProductId),
    AdjustedByUserId INT NOT NULL REFERENCES Users(UserId),
    QuantityBefore INT NOT NULL,
    QuantityAfter  INT NOT NULL,
    Reason         NVARCHAR(200) NULL,
    AdjustedAt     DATETIME2 DEFAULT GETUTCDATE()
);

-- CUSTOMERS

CREATE TABLE Customers (
    CustomerId  INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(150) NULL UNIQUE,
    Phone       NVARCHAR(20) NULL,
    LoyaltyPoints INT DEFAULT 0,
    Segment     NVARCHAR(50) NULL,  -- Populated by Python K-Means
    CreatedAt   DATETIME2 DEFAULT GETUTCDATE()
);

-- TRANSACTIONS

CREATE TABLE Transactions (
    TransactionId   INT IDENTITY(1,1) PRIMARY KEY,
    TransactionCode NVARCHAR(50) NOT NULL UNIQUE,  -- e.g. TXN-20260421-0001
    CashierId       INT NOT NULL REFERENCES Users(UserId),
    CustomerId      INT NULL REFERENCES Customers(CustomerId),
    Subtotal        DECIMAL(10,2) NOT NULL,
    TaxAmount       DECIMAL(10,2) NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(10,2) NOT NULL DEFAULT 0,
    TotalAmount     DECIMAL(10,2) NOT NULL,
    PaymentMethod   NVARCHAR(50) NOT NULL,  -- Cash, Card, Other
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Completed',  -- Completed, Voided, Refunded
    IsAnomaly       BIT DEFAULT 0,          -- Flagged by Python Isolation Forest
    AnomalyScore    FLOAT NULL,             -- Raw score from anomaly model
    CreatedAt       DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE TransactionItems (
    TransactionItemId INT IDENTITY(1,1) PRIMARY KEY,
    TransactionId     INT NOT NULL REFERENCES Transactions(TransactionId),
    ProductId         INT NOT NULL REFERENCES Products(ProductId),
    Quantity          INT NOT NULL,
    UnitPrice         DECIMAL(10,2) NOT NULL,
    Discount          DECIMAL(10,2) NOT NULL DEFAULT 0,
    LineTotal         DECIMAL(10,2) NOT NULL
);

-- AI / FORECASTING

CREATE TABLE SalesForecasts (
    ForecastId    INT IDENTITY(1,1) PRIMARY KEY,
    ProductId     INT NULL REFERENCES Products(ProductId),  -- NULL = store-level forecast
    ForecastDate  DATE NOT NULL,
    PredictedSales DECIMAL(10,2) NOT NULL,
    ActualSales   DECIMAL(10,2) NULL,       -- Filled in after the date passes
    ModelVersion  NVARCHAR(50) NULL,
    GeneratedAt   DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE AnomalyLogs (
    AnomalyLogId    INT IDENTITY(1,1) PRIMARY KEY,
    TransactionId   INT NOT NULL REFERENCES Transactions(TransactionId),
    AnomalyScore    FLOAT NOT NULL,
    DetectedAt      DATETIME2 DEFAULT GETUTCDATE(),
    ReviewedBy      INT NULL REFERENCES Users(UserId),
    ReviewedAt      DATETIME2 NULL,
    IsConfirmed     BIT NULL,               -- Manager confirms or dismisses
    Notes           NVARCHAR(500) NULL
);

CREATE TABLE CustomerSegments (
    SegmentRunId  INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId    INT NOT NULL REFERENCES Customers(CustomerId),
    Segment       NVARCHAR(50) NOT NULL,    -- e.g. High Value, Occasional, At Risk
    RunDate       DATETIME2 DEFAULT GETUTCDATE()
);

-- AI ASSISTANT LOGS

CREATE TABLE AIAssistantLogs (
    LogId       INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT NOT NULL REFERENCES Users(UserId),
    Prompt      NVARCHAR(MAX) NOT NULL,
    Response    NVARCHAR(MAX) NOT NULL,
    CreatedAt   DATETIME2 DEFAULT GETUTCDATE()
);

-- INDEXES

CREATE INDEX IX_Transactions_CreatedAt    ON Transactions(CreatedAt);
CREATE INDEX IX_Transactions_CashierId    ON Transactions(CashierId);
CREATE INDEX IX_Transactions_CustomerId   ON Transactions(CustomerId);
CREATE INDEX IX_TransactionItems_TransactionId ON TransactionItems(TransactionId);
CREATE INDEX IX_TransactionItems_ProductId     ON TransactionItems(ProductId);
CREATE INDEX IX_Products_CategoryId       ON Products(CategoryId);
CREATE INDEX IX_Products_SKU              ON Products(SKU);
CREATE INDEX IX_SalesForecasts_ForecastDate    ON SalesForecasts(ForecastDate);
CREATE INDEX IX_AnomalyLogs_TransactionId ON AnomalyLogs(TransactionId);

GO