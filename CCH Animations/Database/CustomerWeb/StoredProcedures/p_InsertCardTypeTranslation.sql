
 /****** Object:  StoredProcedure [dbo].[p_InsertCardTypeTranslation]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertCardTypeTranslation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertCardTypeTranslation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-04-13
Description:
      Insert CardType Translation
      
Declarations:
      
Execute:
      exec p_InsertCardTypeTranslation
		@CardTypeID = 1,
		@LocaleCode = 'en-us',
		@CardTypeName = 'My healthplan'
	
Objects Listing:

Tables- dbo.Message
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-11  AS	     Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertCardTypeTranslation] (
	@CardTypeID int,
	@CardTypeName nvarchar(50),
	@LocaleCode nvarchar(10) = NULL,
	@LocaleID int = NULL
)
as

BEGIN

-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleID is NULL
	BEGIN
		IF @LocaleCode is NULL
			SET @LocaleCode = 'en-us'
	
		SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode
	END

------------------------------------------------------------------
--Insert the translation fields
-------------------------------------------------------------------
	INSERT INTO dbo.CardTypeTranslation(
		CardTypeID
		,LocaleID
		,CardTypeName
		,CreateDate )
	VALUES (
		@CardTypeID
		,@LocaleID
		,@CardTypeName
		,GETDATE()
	)
		
END

GO

