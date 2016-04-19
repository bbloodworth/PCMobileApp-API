/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-04-18
-- Description:	For the Customer DEMO DB
--              Adds new ID Card Types
-- =============================================
*/

USE CCH_CustomerDemoNCCT
GO
    
  IF (NOT EXISTS(SELECT * FROM [dbo].[CardType] WHERE CardTypeFileName = 'Med_Cigna_ABCState') )
	BEGIN 
	  INSERT INTO [dbo].[CardType] (CardTypeID, CardTypeDesc, CreateDate, InsuranceTypeID, CardTypeFileName)
	  VALUES (7, 'A medical card for the customer demo account', 
	          GETDATE(), 1, 'Med_Cigna_ABCState')
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[CardType] WHERE CardTypeFileName = 'RX_ESI_ABCState') )
	BEGIN 
	  INSERT INTO [dbo].[CardType] (CardTypeID, CardTypeDesc, CreateDate, InsuranceTypeID, CardTypeFileName)
	  VALUES (8, 'An RX ID card for the customer demo account', 
	          GETDATE(), 1, 'RX_ESI_ABCState')
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[CardType] WHERE CardTypeFileName = 'Vision_EyeMed_ABCState') )
	BEGIN 
	  INSERT INTO [dbo].[CardType] (CardTypeID, CardTypeDesc, CreateDate, InsuranceTypeID, CardTypeFileName)
	  VALUES (9, 'A Vision ID card for the customer demo account', 
	          GETDATE(), 1, 'Vision_EyeMed_ABCState')
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[CardType] WHERE CardTypeFileName = 'Dental_MetLife_ABCState') )
	BEGIN 
	  INSERT INTO [dbo].[CardType] (CardTypeID, CardTypeDesc, CreateDate, InsuranceTypeID, CardTypeFileName)
	  VALUES (10, 'A Dental ID card for the customer demo account', 
	          GETDATE(), 1, 'Dental_MetLife_ABCState')
	END
  GO

