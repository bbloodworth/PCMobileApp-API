
/****** Object:  StoredProcedure [dbo].[p_InsertCardType]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertCardType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertCardType]
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

CREATE PROCEDURE [dbo].[p_InsertCardType] (
	@CardTypeName nvarchar(50)
	,@CardTypeDesc nvarchar(250)
	,@LocaleCode nvarchar(10) = NULL
)
as

DECLARE 
		@CardTypeID int
		,@LocaleID int
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode

--------------------------------------------------------------
--Insert the CardType Record
---------------------------------------------------------------

BEGIN

	INSERT INTO dbo.CardType (
	    CardTypeDesc
		,CreateDate)
	VALUES (
		@CardTypeDesc
		,getdate())
		
	SELECT @CardTypeID = SCOPE_IDENTITY()
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @CardTypeName IS NOT NULL 
	BEGIN
	
		exec p_InsertCardTypeTranslation
			@CardTypeID = @CardTypeID
			,@LocaleID = @LocaleID
			,@CardTypeName = @CardTypeName
	END --if		
END

GO


 
