USE SmartPOSDB;
GO

-- Remove existing products and categories cleanly
DELETE FROM TransactionItems;
DELETE FROM Products;
DELETE FROM Categories;

-- Reset identity so IDs start fresh
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);

-- Categories (expanded to match Analytics page)
INSERT INTO Categories (CategoryName, Description) VALUES
('Beverages',       'Hot and cold drinks'),
('Snacks',          'Chips, cookies, and packaged snacks'),
('Dairy',           'Milk, cheese, yogurt, and dairy products'),
('Bakery',          'Bread, pastries, and baked goods'),
('Frozen Foods',    'Frozen meals, ice cream, and frozen items'),
('Personal Care',   'Hygiene and personal care products'),
('Household',       'Cleaning and household supplies'),
('Confectionery',   'Candy, chocolate, and sweets'),
('Electronics',     'Accessories, cables, and tech gadgets'),
('Clothing',        'Apparel and fashion items'),
('Home & Garden',   'Home decor, garden, and kitchen items'),
('Sports',          'Sports equipment and fitness accessories');

-- Products (100 total)
INSERT INTO Products (ProductName, SKU, Barcode, CategoryId, UnitPrice, CostPrice, StockQuantity, LowStockThreshold) VALUES

-- Beverages (CategoryId = 1) - 12 products
('Coca-Cola 355ml',           'BEV-001', '0049000000443', 1,  1.99,  0.85,  120, 20),
('Pepsi 355ml',               'BEV-002', '0012000001086', 1,  1.99,  0.85,  100, 20),
('Dasani Water 500ml',        'BEV-003', '0049000028997', 1,  1.49,  0.50,  200, 30),
('Red Bull 250ml',            'BEV-004', '9002490100070', 1,  3.49,  1.80,   80, 15),
('Orange Juice 1L',           'BEV-005', '0059000000018', 1,  4.99,  2.20,   60, 10),
('Monster Energy 473ml',      'BEV-006', '0070847011002', 1,  3.99,  1.90,   75, 15),
('Tropicana OJ 1.89L',        'BEV-007', '0048500000135', 1,  6.99,  3.20,   45, 10),
('Gatorade Blue 710ml',       'BEV-008', '0052000142655', 1,  2.49,  1.10,   90, 20),
('Nestea Iced Tea 500ml',     'BEV-009', '0055000123456', 1,  1.99,  0.80,   85, 15),
('Starbucks Frappuccino',     'BEV-010', '0012000810002', 1,  4.49,  2.10,   50, 10),
('Organic Cold Brew 12-pk',   'BEV-011', '0074609123456', 1,  24.99, 12.00,  30,  6),
('Sparkling Water 6-pk',      'BEV-012', '0075720123456', 1,  5.99,  2.80,   60, 12),

-- Snacks (CategoryId = 2) - 10 products
('Lays Classic 200g',         'SNK-001', '0028400090827', 2,  3.99,  1.60,   90, 15),
('Doritos Nacho 200g',        'SNK-002', '0028400090155', 2,  3.99,  1.60,   85, 15),
('Pringles Original 165g',    'SNK-003', '0038000845031', 2,  4.49,  1.90,   70, 10),
('Nature Valley Bar',         'SNK-004', '0016000275287', 2,  1.99,  0.80,  150, 25),
('Oreo Cookies 303g',         'SNK-005', '0044000030506', 2,  5.49,  2.30,   60, 10),
('Ritz Crackers 200g',        'SNK-006', '0066721100223', 2,  3.49,  1.40,   80, 15),
('Cheetos Crunchy 285g',      'SNK-007', '0028400090292', 2,  4.99,  2.00,   65, 12),
('Pop-Tarts 8-pack',          'SNK-008', '0038000910419', 2,  4.99,  2.10,   55, 10),
('Granola Bar Mixed 6-pk',    'SNK-009', '0016000324008', 2,  6.99,  3.10,   45, 10),
('Trail Mix 400g',            'SNK-010', '0074175123456', 2,  7.99,  3.80,   40,  8),

-- Dairy (CategoryId = 3) - 8 products
('Milk 2% 1L',                'DAI-001', '0011110865038', 3,  3.49,  1.80,   80, 15),
('Cheddar Cheese 400g',       'DAI-002', '0021000016570', 3,  6.99,  3.50,   45, 10),
('Greek Yogurt 750g',         'DAI-003', '0070470003160', 3,  5.99,  2.80,   50, 10),
('Butter 454g',               'DAI-004', '0011110001313', 3,  5.49,  2.60,   40,  8),
('Cream Cheese 250g',         'DAI-005', '0021000007479', 3,  4.99,  2.20,   55, 10),
('Sour Cream 500ml',          'DAI-006', '0011110034209', 3,  3.99,  1.80,   60, 12),
('Mozzarella 300g',           'DAI-007', '0025317123456', 3,  5.49,  2.50,   45, 10),
('Almond Milk 1.89L',         'DAI-008', '0052159123456', 3,  6.49,  3.10,   40,  8),

-- Bakery (CategoryId = 4) - 7 products
('White Bread 675g',          'BAK-001', '0073130001100', 4,  3.99,  1.70,   55, 12),
('Croissant 4-pack',          'BAK-002', '0072250020070', 4,  4.99,  2.10,   40, 10),
('Bagels 6-pack',             'BAK-003', '0072250030253', 4,  4.49,  1.90,   35,  8),
('Whole Wheat Bread 675g',    'BAK-004', '0073130001117', 4,  4.49,  1.90,   50, 12),
('Blueberry Muffins 4-pk',    'BAK-005', '0072250040192', 4,  5.99,  2.60,   30,  8),
('Cinnamon Raisin Bagels',    'BAK-006', '0072250030260', 4,  4.99,  2.10,   30,  8),
('Sourdough Loaf',            'BAK-007', '0073130002107', 4,  5.49,  2.40,   25,  6),

-- Frozen Foods (CategoryId = 5) - 7 products
('Ben & Jerrys 473ml',        'FRZ-001', '0076840100255', 5,  7.99,  3.80,   30,  8),
('McCain Fries 650g',         'FRZ-002', '0063200015178', 5,  4.99,  2.20,   45, 10),
('Frozen Pizza 400g',         'FRZ-003', '0070272010055', 5,  6.99,  3.10,   35,  8),
('Häagen-Dazs 500ml',         'FRZ-004', '0074570123456', 5,  8.99,  4.20,   25,  6),
('Birds Eye Veggies 500g',    'FRZ-005', '0014500123456', 5,  3.99,  1.80,   50, 10),
('Stouffer Lasagna 340g',     'FRZ-006', '0043000123456', 5,  6.49,  3.00,   30,  8),
('Eggo Waffles 12-pk',        'FRZ-007', '0038000123456', 5,  5.49,  2.40,   40, 10),

-- Personal Care (CategoryId = 6) - 8 products
('Dove Soap 6-pack',          'PCA-001', '0011111015090', 6,  8.99,  4.20,   55, 10),
('Head Shoulders 400ml',      'PCA-002', '0037000756460', 6,  9.99,  4.80,   40,  8),
('Colgate Toothpaste 100ml',  'PCA-003', '0035000006301', 6,  4.99,  2.10,   65, 12),
('Gillette Razor 4-pk',       'PCA-004', '0047400123456', 6, 12.99,  6.20,   35,  8),
('Pantene Shampoo 400ml',     'PCA-005', '0037000123456', 6,  8.49,  4.00,   45, 10),
('Degree Deodorant 76g',      'PCA-006', '0079400123456', 6,  5.99,  2.70,   60, 12),
('Neutrogena Face Wash',      'PCA-007', '0070501123456', 6, 11.99,  5.60,   30,  6),
('Listerine 500ml',           'PCA-008', '0312547123456', 6,  7.99,  3.70,   50, 10),

-- Household (CategoryId = 7) - 7 products
('Tide Pods 35-count',        'HOU-001', '0037000754008', 7, 14.99,  7.50,   30,  6),
('Bounty Paper Towel 6pk',    'HOU-002', '0037000017882', 7,  9.99,  4.80,   40,  8),
('Glad Garbage Bags 20pk',    'HOU-003', '0012587002094', 7,  6.99,  3.20,   50, 10),
('Windex Glass Cleaner',      'HOU-004', '0025700123456', 7,  5.49,  2.50,   45, 10),
('Lysol Disinfectant 650ml',  'HOU-005', '0019200123456', 7,  7.99,  3.70,   40,  8),
('Swiffer Refills 16-pk',     'HOU-006', '0037000912345', 7, 12.99,  6.20,   25,  6),
('Ziploc Bags 50-pk',         'HOU-007', '0025700912345', 7,  4.99,  2.20,   55, 12),

-- Confectionery (CategoryId = 8) - 8 products
('KitKat 4-finger',           'CON-001', '0028000173944', 8,  1.99,  0.75,  130, 25),
('Snickers 52g',              'CON-002', '0040000001522', 8,  1.99,  0.75,  120, 25),
('Reeses Peanut Butter Cup',  'CON-003', '0034000002597', 8,  1.99,  0.75,  110, 25),
('Lindt Dark 85% 100g',       'CON-004', '0037466017598', 8,  5.99,  2.80,   45, 10),
('Skittles 191g',             'CON-005', '0040000001539', 8,  2.49,  1.00,  100, 20),
('Twix 2-pack',               'CON-006', '0040000001560', 8,  1.99,  0.75,  115, 25),
('Ferrero Rocher 16-pk',      'CON-007', '0009800123456', 8, 12.99,  6.20,   30,  8),
('Haribo Goldbears 200g',     'CON-008', '0042238123456', 8,  3.49,  1.50,   80, 15),

-- Electronics (CategoryId = 9) - 10 products
('USB-C Cable 2m',            'EL-001', '0681493123456',  9,  9.99,  4.50,   60, 10),
('Phone Screen Protector',    'EL-002', '0681493123457',  9,  7.99,  3.50,   75, 15),
('Wireless Earbuds Basic',    'EL-003', '0681493123458',  9, 29.99, 14.00,   40,  8),
('Power Bank 10000mAh',       'EL-004', '0681493123459',  9, 34.99, 16.00,   30,  6),
('Phone Case Universal',      'EL-005', '0681493123460',  9,  9.99,  4.20,   80, 15),
('HDMI Cable 1.8m',           'EL-006', '0681493123461',  9, 12.99,  5.80,   50, 10),
('Wireless Mouse',            'EL-007', '0681493123462',  9, 24.99, 11.00,   35,  8),
('USB Hub 4-port',            'EL-008', '0681493123463',  9, 19.99,  9.00,   40,  8),
('Smart Plug WiFi',           'EL-009', '0681493123464',  9, 17.99,  8.00,   45, 10),
('AA Batteries 16-pk',        'EL-010', '0681493123465',  9,  9.99,  4.50,   90, 20),

-- Clothing (CategoryId = 10) - 8 products
('White T-Shirt S',           'CL-001', '0123456789001', 10,  9.99,  4.00,   50, 10),
('White T-Shirt M',           'CL-002', '0123456789002', 10,  9.99,  4.00,   60, 12),
('White T-Shirt L',           'CL-003', '0123456789003', 10,  9.99,  4.00,   55, 12),
('Black Hoodie M',            'CL-004', '0123456789004', 10, 34.99, 15.00,   25,  6),
('Black Hoodie L',            'CL-005', '0123456789005', 10, 34.99, 15.00,   20,  6),
('Slim Chino Pants 32',       'CL-006', '0123456789006', 10, 49.99, 22.00,   20,  5),
('Slim Chino Pants 34',       'CL-007', '0123456789007', 10, 49.99, 22.00,   18,  5),
('Baseball Cap',              'CL-008', '0123456789008', 10, 19.99,  8.00,   40,  8),

-- Home & Garden (CategoryId = 11) - 7 products
('Scented Candle 200g',       'HG-001', '0123456700001', 11, 14.99,  6.50,   35,  8),
('Picture Frame 4x6',         'HG-002', '0123456700002', 11,  7.99,  3.50,   50, 10),
('Succulent Plant Small',     'HG-003', '0123456700003', 11, 12.99,  5.50,   25,  6),
('Kitchen Sponge 3-pk',       'HG-004', '0123456700004', 11,  3.99,  1.60,   80, 15),
('Dish Soap 532ml',           'HG-005', '0123456700005', 11,  4.49,  1.90,   70, 15),
('Laundry Hamper',            'HG-006', '0123456700006', 11, 19.99,  8.50,   20,  5),
('LED Night Light',           'HG-007', '0123456700007', 11,  8.99,  3.80,   45, 10),

-- Sports (CategoryId = 12) - 8 products
('Yoga Mat Standard',         'SP-001', '0123456800001', 12, 29.99, 13.00,   30,  6),
('Resistance Bands Set',      'SP-002', '0123456800002', 12, 19.99,  8.50,   40,  8),
('Water Bottle 1L',           'SP-003', '0123456800003', 12, 14.99,  6.00,   55, 12),
('Jump Rope',                 'SP-004', '0123456800004', 12,  9.99,  4.00,   50, 10),
('Foam Roller',               'SP-005', '0123456800005', 12, 24.99, 10.50,   25,  6),
('Gym Gloves M',              'SP-006', '0123456800006', 12, 17.99,  7.50,   30,  8),
('Protein Shaker 700ml',      'SP-007', '0123456800007', 12, 12.99,  5.50,   45, 10),
('Ankle Weights 2kg',         'SP-008', '0123456800008', 12, 22.99,  9.50,   20,  5);

GO