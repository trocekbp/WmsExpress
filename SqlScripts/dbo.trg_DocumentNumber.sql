CREATE TRIGGER dbo.trg_DocumentNumber
ON dbo.Document
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocId INT;
    DECLARE @DocDate DATE;
    DECLARE @LastNumber NVARCHAR(20);
    DECLARE @NextInt INT;
    DECLARE @NewNumber NVARCHAR(20);

    -- pobranie wstawionego rekordu
    SELECT 
        @DocId = DocumentId,
        @DocDate = [Date]
    FROM inserted;

    -- pobieranie ostatni numer dokumentu z tego samego roku i miesi¹ca
    SELECT TOP 1 @LastNumber = [Number]
    FROM dbo.Document
    WHERE YEAR([Date]) = YEAR(@DocDate)
      AND [Number] IS NOT NULL
    ORDER BY [Number] DESC;

    -- jeœli brak dokumentów w tym miesi¹cu to start od 1
    SET @NextInt = ISNULL(CAST(RIGHT(@LastNumber, 4) AS INT), 0) + 1;

    IF @NextInt > 9999 
    THROW 50001, 'Przekroczono limit 9999 dokumentów na rok, skontaktuj siê z producentem systemu aby móc wystawiaæ dokumenty', 1;
    -- od 500001 zaczynaj¹ siê wolne kody na w³asne b³êdy programisty

    -- budowanie nowego numeru
    SET @NewNumber =
        FORMAT(@DocDate, 'yyyy') + '/' +
        RIGHT('0000' + CAST(@NextInt AS NVARCHAR), 4);

    -- aktualizacja wstawionego rekordu
    UPDATE dbo.Document
    SET [Number] = @NewNumber,
    [CreationDate] = GETDATE()
    WHERE DocumentId = @DocId;
END;
GO
