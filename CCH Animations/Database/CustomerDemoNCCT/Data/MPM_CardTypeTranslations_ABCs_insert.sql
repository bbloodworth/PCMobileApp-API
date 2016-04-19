/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-04-18
-- Description:	For the Customer DEMO DB
--              Adds new ID Card Type Translations
-- =============================================
*/

USE CCH_CustomerDemoNCCT
GO
        
  IF (NOT EXISTS(SELECT * FROM [dbo].[CardTypeTranslation] WHERE CardTypeID = 7) )
	BEGIN 
	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (7, 1, 'Medical ID Card (PPO)', GETDATE())

	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (7, 2, 'Credencial Médica (PPO)', GETDATE())
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[CardTypeTranslation] WHERE CardTypeID = 8) )
	BEGIN 
	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (8, 1, 'Rx ID Card', GETDATE())

	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (8, 2, 'Credencial de Recetas', GETDATE())
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[CardTypeTranslation] WHERE CardTypeID = 9) )
	BEGIN 
	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (9, 1, 'Vision ID Card', GETDATE())

	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (9, 2, 'Credencial Óptica', GETDATE())
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[CardTypeTranslation] WHERE CardTypeID = 10) )
	BEGIN 
	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (10, 1, 'Dental ID Card', GETDATE())

	  INSERT INTO [dbo].[CardTypeTranslation] (CardTypeID, LocaleID, CardTypeName, CreateDate)
	  VALUES (10, 2, 'Credencial Dental', GETDATE())
	END
  GO
