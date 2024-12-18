USE [ServerDb]

DECLARE @counter INT = 1;
DECLARE @max_iterations INT = 5;
DECLARE @id UNIQUEIDENTIFIER;
DECLARE @strIndex NVARCHAR;

WHILE @counter <= @max_iterations
BEGIN
	SET @strIndex = CAST(@counter AS NVARCHAR)
	SET @id = NEWID()
	
	PRINT 'Iteration: ' + @strIndex + ' customer id: ' + CAST(@id AS NVARCHAR(36))

    INSERT INTO Customer (CustomerID, FirstName, LastName, rowguid)
    VALUES (@id, 'First name' + @strIndex, 'Last name' + @strIndex, NEWID())
	
	WAITFOR DELAY '00:00:01';
    
	INSERT INTO CustomerAddress (CustomerID, AddressId, AddressLine1, ValidUntil, rowguid)
	VALUES (@id, NEWID(), 'Address 1' + @strIndex, GETDATE(), NEWID())

    INSERT INTO CustomerAddress (CustomerID, AddressId, AddressLine1, ValidUntil, rowguid)
	VALUES (@id, NEWID(), 'Address 2' + @strIndex, GETDATE(), NEWID())

	WAITFOR DELAY '00:00:01';

    UPDATE CustomerAddress
    SET AddressLine1 = 'Address 1 updated' + @strIndex
    WHERE CustomerID = @id and AddressLine1 like 'Address 1%'

	WAITFOR DELAY '00:00:01';

    DELETE FROM CustomerAddress
    WHERE CustomerID = @id and AddressLine1 like 'Address 2%'

    SET @counter = @counter + 1;
END