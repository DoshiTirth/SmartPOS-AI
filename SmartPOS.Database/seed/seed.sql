USE SmartPOSDB;
GO

INSERT INTO Roles (RoleName, Description) VALUES
('Admin',   'Full system access - manage users, settings, reports'),
('Manager', 'Access to dashboard, analytics, AI insights, inventory'),
('Cashier', 'Access to POS terminal only');


INSERT INTO Users (FullName, Email, PasswordHash, RoleId) VALUES
('System Admin',   'admin@smartpos.com',   '$2a$11$KQpFbHzDJcx1e1B3sLhqZuH9L3zP8mN2oR4vT6wX0yA5bC7dE9fG1', 1),
('Sarah Manager',  'manager@smartpos.com', '$2a$11$KQpFbHzDJcx1e1B3sLhqZuH9L3zP8mN2oR4vT6wX0yA5bC7dE9fG1', 2),
('John Cashier',   'cashier1@smartpos.com','$2a$11$KQpFbHzDJcx1e1B3sLhqZuH9L3zP8mN2oR4vT6wX0yA5bC7dE9fG1', 3),
('Emily Cashier',  'cashier2@smartpos.com','$2a$11$KQpFbHzDJcx1e1B3sLhqZuH9L3zP8mN2oR4vT6wX0yA5bC7dE9fG1', 3);

-- SEED: Categories

INSERT INTO Categories (CategoryName, Description) VALUES
('Beverages',       'Hot and cold drinks'),
('Snacks',          'Chips, cookies, and packaged snacks'),
('Dairy',           'Milk, cheese, yogurt, and dairy products'),
('Bakery',          'Bread, pastries, and baked goods'),
('Frozen Foods',    'Frozen meals, ice cream, and frozen items'),
('Personal Care',   'Hygiene and personal care products'),
('Household',       'Cleaning and household supplies'),
('Confectionery',   'Candy, chocolate, and sweets');

-- SEED: Products

INSERT INTO Products (ProductName, SKU, Barcode, CategoryId, UnitPrice, CostPrice, StockQuantity, LowStockThreshold) VALUES
-- Beverages
('Coca-Cola 355ml',         'BEV-001', '0049000000443', 1,  1.99,  0.85,  120, 20),
('Pepsi 355ml',             'BEV-002', '0012000001086', 1,  1.99,  0.85,  100, 20),
('Dasani Water 500ml',      'BEV-003', '0049000028997', 1,  1.49,  0.50,  200, 30),
('Red Bull 250ml',          'BEV-004', '9002490100070', 1,  3.49,  1.80,   80, 15),
('Orange Juice 1L',         'BEV-005', '0059000000018', 1,  4.99,  2.20,   60, 10),

-- Snacks
('Lays Classic 200g',       'SNK-001', '0028400090827', 2,  3.99,  1.60,   90, 15),
('Doritos Nacho 200g',      'SNK-002', '0028400090155', 2,  3.99,  1.60,   85, 15),
('Pringles Original 165g',  'SNK-003', '0038000845031', 2,  4.49,  1.90,   70, 10),
('Nature Valley Bar',       'SNK-004', '0016000275287', 2,  1.99,  0.80,  150, 25),
('Oreo Cookies 303g',       'SNK-005', '0044000030506', 2,  5.49,  2.30,   60, 10),

-- Dairy
('Milk 2% 1L',              'DAI-001', '0011110865038', 3,  3.49,  1.80,   80, 15),
('Cheddar Cheese 400g',     'DAI-002', '0021000016570', 3,  6.99,  3.50,   45, 10),
('Greek Yogurt 750g',       'DAI-003', '0070470003160', 3,  5.99,  2.80,   50, 10),
('Butter 454g',             'DAI-004', '0011110001313', 3,  5.49,  2.60,   40,  8),

-- Bakery
('White Bread 675g',        'BAK-001', '0073130001100', 4,  3.99,  1.70,   55, 12),
('Croissant 4-pack',        'BAK-002', '0072250020070', 4,  4.99,  2.10,   40, 10),
('Bagels 6-pack',           'BAK-003', '0072250030253', 4,  4.49,  1.90,   35,  8),

-- Frozen Foods
('Ben & Jerrys 473ml',      'FRZ-001', '0076840100255', 5,  7.99,  3.80,   30,  8),
('McCain Fries 650g',       'FRZ-002', '0063200015178', 5,  4.99,  2.20,   45, 10),
('Frozen Pizza 400g',       'FRZ-003', '0070272010055', 5,  6.99,  3.10,   35,  8),

-- Personal Care
('Dove Soap 6-pack',        'PCA-001', '0011111015090', 6,  8.99,  4.20,   55, 10),
('Head Shoulders 400ml',    'PCA-002', '0037000756460', 6,  9.99,  4.80,   40,  8),
('Colgate Toothpaste 100ml','PCA-003', '0035000006301', 6,  4.99,  2.10,   65, 12),

-- Household
('Tide Pods 35-count',      'HOU-001', '0037000754008', 7, 14.99,  7.50,   30,  6),
('Bounty Paper Towel 6pk',  'HOU-002', '0037000017882', 7,  9.99,  4.80,   40,  8),
('Glad Garbage Bags 20pk',  'HOU-003', '0012587002094', 7,  6.99,  3.20,   50, 10),

-- Confectionery
('KitKat 4-finger',         'CON-001', '0028000173944', 8,  1.99,  0.75,  130, 25),
('Snickers 52g',            'CON-002', '0040000001522', 8,  1.99,  0.75,  120, 25),
('Reeses Peanut Butter Cup','CON-003', '0034000002597', 8,  1.99,  0.75,  110, 25),
('Lindt Dark 85% 100g',     'CON-004', '0037466017598', 8,  5.99,  2.80,   45, 10);

-- SEED: Sample Customers

INSERT INTO Customers (FullName, Email, Phone, LoyaltyPoints, Segment) VALUES
('Alice Johnson',   'alice.j@email.com',   '416-555-0101', 250,  'High Value'),
('Bob Williams',    'bob.w@email.com',     '416-555-0102', 80,   'Regular'),
('Carol Smith',     'carol.s@email.com',   '647-555-0103', 15,   'Occasional'),
('David Brown',     'david.b@email.com',   '905-555-0104', 420,  'High Value'),
('Emma Davis',      'emma.d@email.com',    '416-555-0105', 5,    'New'),
('Frank Miller',    'frank.m@email.com',   '647-555-0106', 190,  'Regular'),
('Grace Wilson',    'grace.w@email.com',   '905-555-0107', 0,    'At Risk'),
('Henry Moore',     'henry.m@email.com',   '416-555-0108', 310,  'High Value');

GO