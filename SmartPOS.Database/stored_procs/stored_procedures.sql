USE SmartPOSDB;
GO

CREATE OR ALTER PROCEDURE sp_GetDashboardSummary
AS
BEGIN
    SET NOCOUNT ON;

    -- Today's sales
    SELECT
        COUNT(*)                        AS TotalTransactionsToday,
        ISNULL(SUM(TotalAmount), 0)     AS TotalRevenueToday,
        ISNULL(AVG(TotalAmount), 0)     AS AverageTransactionValue
    FROM Transactions
    WHERE CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE)
      AND Status = 'Completed';

    -- This month's sales
    SELECT
        COUNT(*)                        AS TotalTransactionsMonth,
        ISNULL(SUM(TotalAmount), 0)     AS TotalRevenueMonth
    FROM Transactions
    WHERE MONTH(CreatedAt) = MONTH(GETUTCDATE())
      AND YEAR(CreatedAt)  = YEAR(GETUTCDATE())
      AND Status = 'Completed';

    -- Low stock count
    SELECT COUNT(*) AS LowStockProductCount
    FROM Products
    WHERE StockQuantity <= LowStockThreshold
      AND IsActive = 1;

    -- Anomaly count (unreviewed)
    SELECT COUNT(*) AS UnreviewedAnomalies
    FROM AnomalyLogs
    WHERE ReviewedBy IS NULL;
END;
GO


CREATE OR ALTER PROCEDURE sp_GetSalesLast30Days
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(CreatedAt AS DATE)     AS SaleDate,
        COUNT(*)                    AS TransactionCount,
        SUM(TotalAmount)            AS DailyRevenue
    FROM Transactions
    WHERE CreatedAt >= DATEADD(DAY, -30, GETUTCDATE())
      AND Status = 'Completed'
    GROUP BY CAST(CreatedAt AS DATE)
    ORDER BY SaleDate ASC;
END;
GO


CREATE OR ALTER PROCEDURE sp_ProcessTransaction
    @CashierId       INT,
    @CustomerId      INT = NULL,
    @PaymentMethod   NVARCHAR(50),
    @TaxRate         DECIMAL(5,4) = 0.13,
    @DiscountAmount  DECIMAL(10,2) = 0,
    @ItemsXml        NVARCHAR(MAX)  -- XML: <items><item productId="1" quantity="2"/></items>
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Parse items from XML
        DECLARE @Items TABLE (
            ProductId INT,
            Quantity  INT
        );

        INSERT INTO @Items (ProductId, Quantity)
        SELECT
            Item.value('@productId', 'INT'),
            Item.value('@quantity',  'INT')
        FROM (SELECT CAST(@ItemsXml AS XML) AS XmlData) AS X
        CROSS APPLY X.XmlData.nodes('/items/item') AS T(Item);

        -- Validate stock
        IF EXISTS (
            SELECT 1 FROM @Items i
            JOIN Products p ON p.ProductId = i.ProductId
            WHERE p.StockQuantity < i.Quantity OR p.IsActive = 0
        )
        BEGIN
            RAISERROR('One or more products are out of stock or inactive.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Calculate subtotal
        DECLARE @Subtotal DECIMAL(10,2);
        SELECT @Subtotal = SUM(p.UnitPrice * i.Quantity)
        FROM @Items i
        JOIN Products p ON p.ProductId = i.ProductId;

        DECLARE @TaxAmount    DECIMAL(10,2) = ROUND(@Subtotal * @TaxRate, 2);
        DECLARE @TotalAmount  DECIMAL(10,2) = @Subtotal + @TaxAmount - @DiscountAmount;

        -- Generate transaction code
        DECLARE @TransactionCode NVARCHAR(50);
        SET @TransactionCode = 'TXN-' + FORMAT(GETUTCDATE(), 'yyyyMMdd') + '-' +
                               RIGHT('0000' + CAST((SELECT ISNULL(MAX(TransactionId),0)+1 FROM Transactions) AS NVARCHAR), 4);

        -- Insert transaction header
        DECLARE @TransactionId INT;
        INSERT INTO Transactions (TransactionCode, CashierId, CustomerId, Subtotal, TaxAmount, DiscountAmount, TotalAmount, PaymentMethod, Status)
        VALUES (@TransactionCode, @CashierId, @CustomerId, @Subtotal, @TaxAmount, @DiscountAmount, @TotalAmount, @PaymentMethod, 'Completed');

        SET @TransactionId = SCOPE_IDENTITY();

        -- Insert line items and deduct stock
        INSERT INTO TransactionItems (TransactionId, ProductId, Quantity, UnitPrice, Discount, LineTotal)
        SELECT
            @TransactionId,
            i.ProductId,
            i.Quantity,
            p.UnitPrice,
            0,
            p.UnitPrice * i.Quantity
        FROM @Items i
        JOIN Products p ON p.ProductId = i.ProductId;

        -- Deduct stock
        UPDATE p
        SET p.StockQuantity = p.StockQuantity - i.Quantity,
            p.UpdatedAt     = GETUTCDATE()
        FROM Products p
        JOIN @Items i ON p.ProductId = i.ProductId;

        -- Update loyalty points if customer provided
        IF @CustomerId IS NOT NULL
        BEGIN
            UPDATE Customers
            SET LoyaltyPoints = LoyaltyPoints + FLOOR(@TotalAmount)
            WHERE CustomerId = @CustomerId;
        END

        COMMIT TRANSACTION;

        -- Return transaction summary
        SELECT
            t.TransactionId,
            t.TransactionCode,
            t.Subtotal,
            t.TaxAmount,
            t.DiscountAmount,
            t.TotalAmount,
            t.PaymentMethod,
            t.Status,
            t.CreatedAt
        FROM Transactions t
        WHERE t.TransactionId = @TransactionId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO


CREATE OR ALTER PROCEDURE sp_GetLowStockProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.ProductId,
        p.ProductName,
        p.SKU,
        p.StockQuantity,
        p.LowStockThreshold,
        c.CategoryName
    FROM Products p
    JOIN Categories c ON c.CategoryId = p.CategoryId
    WHERE p.StockQuantity <= p.LowStockThreshold
      AND p.IsActive = 1
    ORDER BY p.StockQuantity ASC;
END;
GO


CREATE OR ALTER PROCEDURE sp_GetTopSellingProducts
    @TopN    INT = 10,
    @Days    INT = 30
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        p.ProductId,
        p.ProductName,
        p.SKU,
        c.CategoryName,
        SUM(ti.Quantity)    AS TotalUnitsSold,
        SUM(ti.LineTotal)   AS TotalRevenue
    FROM TransactionItems ti
    JOIN Transactions t ON t.TransactionId = ti.TransactionId
    JOIN Products p     ON p.ProductId     = ti.ProductId
    JOIN Categories c   ON c.CategoryId    = p.CategoryId
    WHERE t.CreatedAt >= DATEADD(DAY, -@Days, GETUTCDATE())
      AND t.Status = 'Completed'
    GROUP BY p.ProductId, p.ProductName, p.SKU, c.CategoryName
    ORDER BY TotalUnitsSold DESC;
END;
GO


CREATE OR ALTER PROCEDURE sp_GetTransactionDetail
    @TransactionId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Header
    SELECT
        t.TransactionId,
        t.TransactionCode,
        t.Subtotal,
        t.TaxAmount,
        t.DiscountAmount,
        t.TotalAmount,
        t.PaymentMethod,
        t.Status,
        t.IsAnomaly,
        t.CreatedAt,
        u.FullName      AS CashierName,
        c.FullName      AS CustomerName,
        c.LoyaltyPoints
    FROM Transactions t
    JOIN Users u         ON u.UserId     = t.CashierId
    LEFT JOIN Customers c ON c.CustomerId = t.CustomerId
    WHERE t.TransactionId = @TransactionId;

    -- Line items
    SELECT
        ti.TransactionItemId,
        p.ProductName,
        p.SKU,
        ti.Quantity,
        ti.UnitPrice,
        ti.Discount,
        ti.LineTotal
    FROM TransactionItems ti
    JOIN Products p ON p.ProductId = ti.ProductId
    WHERE ti.TransactionId = @TransactionId;
END;
GO