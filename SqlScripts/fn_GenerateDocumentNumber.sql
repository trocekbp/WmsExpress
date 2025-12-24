CREATE FUNCTION fn_GenerateDocumentNumber(@DocDate DATE)
RETURNS NVARCHAR(9)
AS
BEGIN
    DECLARE @LastNumber NVARCHAR(20);
    DECLARE @NextInt INT;
    DECLARE @NewNumber NVARCHAR(9);

    -- pobranie ostatniego numeru z tego samego roku
    SELECT TOP 1 @LastNumber = [Number]
    FROM dbo.Document
    WHERE YEAR([Date]) = YEAR(@DocDate)
      AND [Number] IS NOT NULL
    ORDER BY [Number] DESC;

    -- jeœli brak dokumentów w tym roku to start od 1
    SET @NextInt = ISNULL(CAST(RIGHT(@LastNumber, 4) AS INT), 0) + 1;

    IF @NextInt > 9999 
        RETURN 'LIMIT'  -- Przekroczono limit 9999 dokumentów w roku, skontaktuj siê z producentem systemu aby móc wystawiaæ nowe dokumenty.', 1;

    -- stworzenie nowego numeru
    SET @NewNumber = FORMAT(@DocDate, 'yyyy') + '/' + RIGHT('0000' + CAST(@NextInt AS NVARCHAR), 4);

    RETURN @NewNumber;
END;
GO
