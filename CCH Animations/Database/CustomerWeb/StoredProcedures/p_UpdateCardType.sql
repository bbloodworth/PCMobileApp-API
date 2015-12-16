
/****** Object:  StoredProcedure [dbo].[p_UpdateCardType]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_UpdateCardType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_UpdateCardType]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert CardType
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.CardType
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-11  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_UpdateCardType] (
	@CardTypeID int
	,@CardTypeName nvarchar(50) = NULL	
	,@CardTypeDesc nvarchar(250) = NULL
	,@LocaleCode nvarchar(10) = NULL
)
as

DECLARE 
		@LocaleID int
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode

--------------------------------------------------------------
--Update the CardType Record
---------------------------------------------------------------

BEGIN
	UPDATE dbo.CardType
	SET
		CardTypeDesc = ISNULL(@CardTypeDesc, CardTypeDesc)
	WHERE
		CardTypeID = @CardTypeID

------------------------------------------------------------------
--Update the translation fields if supplied
-------------------------------------------------------------------
	IF @CardTypeName IS NOT NULL 
	BEGIN
		UPDATE dbo.CardTypeTranslation
		SET
			CardTypeName = ISNULL(@CardTypeName, CardTypeName)
		WHERE
			CardTypeID = @CardTypeID
			AND LocaleID = @LocaleID
	END --if		
END

GO


 
