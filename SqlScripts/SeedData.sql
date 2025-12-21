USE [WmsCoreExpress]
GO
INSERT INTO dbo.Category ([Name])
      VALUES ('Inne'),
             ('Kat dwa');

INSERT INTO [dbo].[Item]
           ([Code]
           ,[Acronym]
           ,[Name]
           ,[Price]
           ,[Description]
           ,[EAN]
           ,[CategoryId])
     VALUES
           ('1','jeden','towar1',20.00,'opis jedynki','12345678910',1),
           ('2','dwa','towar2',30.00,'opis dwójki','12345678910',1),
           ('3','trzy','towar3',40.00,'opis trójki','12345678910',2),
           ('4','cztery','towar4',50.00,'opis czwórki','12345678910',2);

INSERT INTO Contractor ([Name],[Email],[IsContractor],[IsCustomer],[NIP])
      VALUES ('Dostawca 1', 'dostawca@sed.pl', 1, 0, '123456789101'),
             ('Odbiorca 1', 'odbiorca@inf.pl', 0, 1, '123456789102'),
             ('Kontrahent', 'kontrahent@viv.pl', 1, 1, '123456789103');

INSERT INTO dbo.Address ([Street], [City], [PostalCode], [ContractorId])
      VALUES ('Ulica1', 'Miasto1', '08-110',1),
             ('Ulica2', 'Miasto2', '02-110',2),
             ('Ulica3', 'Miasto3', '03-110',3);

GO


