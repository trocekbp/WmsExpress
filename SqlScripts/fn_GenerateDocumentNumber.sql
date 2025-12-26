CREATE FUNCTION fn_GenerateDocumentNumber(@Date DATE, @Type NVARCHAR(2))
RETURNS NVARCHAR(12)
AS
BEGIN
    DECLARE @LastNumber NVARCHAR(20);
    DECLARE @NextInt INT;
    DECLARE @NewNumber NVARCHAR(12);
    DECLARE @TypeInt INT;
    -- mapowanie liter na int zgodnie z enum w C#
    /*  w   public enum DocumentType
    {
        PZ, //Przyjêcie zewnêtrzne
        WZ, //Wydanie zewnêtrzne
        PW, //Przyjêcie zewnêtrzne
        RW, //Rozchód wewnêtrzny
    } */
    SET @TypeInt = CASE @Type
                        WHEN 'PZ' THEN 0
                        WHEN 'WZ' THEN 1
                        WHEN 'PW' THEN 2
                        WHEN 'RW' THEN 3
                        ELSE -1 -- nieznany typ
                   END;

    IF @TypeInt = -1
        RETURN 'INVALID';  -- niepoprawny typ dokumentu

    -- pobranie ostatniego numeru z tego samego roku
    SELECT TOP 1 @LastNumber = [Number]
    FROM dbo.Document
    WHERE YEAR([Date]) = YEAR(@Date)
      AND [Type] = @TypeInt
      AND [Number] IS NOT NULL
    ORDER BY [Number] DESC;

    -- jeœli brak dokumentów w tym roku to start od 1
    SET @NextInt = ISNULL(CAST(RIGHT(@LastNumber, 4) AS INT), 0) + 1;

    IF @NextInt > 9999 
        RETURN 'LIMIT'  -- Przekroczono limit 9999 dokumentów w roku, skontaktuj siê z producentem systemu aby móc wystawiaæ nowe dokumenty.', 1;

    -- stworzenie nowego numeru
    SET @NewNumber = @Type + '/' + FORMAT(@Date, 'yyyy') + '/' + RIGHT('0000' + CAST(@NextInt AS NVARCHAR), 4);

    RETURN @NewNumber;
END;
GO
