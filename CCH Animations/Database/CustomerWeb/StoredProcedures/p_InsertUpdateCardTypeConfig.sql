

/****** Object:  StoredProcedure [dbo].[p_InsertUpdateCardTypeConfig]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertUpdateCardTypeConfig]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertUpdateCardTypeConfig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-06-23
Description:
      Insert or Update CardTypeConfig Values
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.CardTypeConfig
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-06-23  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertUpdateCardTypeConfig] (
	@CardTypeID int
--	,@CardTypeName nvarchar(250) = NULL
	,@CardTypeConfigKey nvarchar(250)
	,@CardTypeConfigValue nvarchar(250)
)
as

BEGIN--proc

	IF EXISTS (SELECT 1 FROM dbo.CardTypeConfig WHERE CardTypeID = @CardTypeID AND CardTypeConfigKey = @CardTypeConfigKey)
	BEGIN -- IF
		UPDATE dbo.CardTypeConfig
		SET 
			CardTypeConfigValuePrior = CardTypeConfigValue,
			CardTypeConfigValue = @CardTypeConfigValue,
			ModifiedDate = GETDATE()
		WHERE 
			CardTypeID = @CardTypeID
			AND CardTypeConfigKey = @CardTypeConfigKey
	END --IF
	ELSE
	BEGIN --ELSE
		INSERT INTO dbo.CardTypeConfig (
			CardTypeID
			,CardTypeConfigKey
			,CardTypeConfigValue
			,CardTypeConfigValuePrior
			,CreateDate
			,ModifiedDate)
		VALUES (
			@CardTypeID
			,@CardTypeConfigKey
			,@CardTypeConfigValue
			,''
			,GETDATE()
			,GETDATE())
	END--ELSE
END--proc
 
 
 GO
 
 
