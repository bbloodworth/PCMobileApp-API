/****** Object:  StoredProcedure [dbo].[p_InsertUpdateUserContentPreference]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertUpdateUserContentPreference]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertUpdateUserContentPreference]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert UserContentPreference
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.UserContentPreference
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-06-25  AS       Change Default LocaleID to DefaultLocaleCode
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertUpdateUserContentPreference] (
	@CCHID int
	,@SMSInd bit = NULL	
	,@EmailInd bit = NULL
	,@OSBasedAlertInd bit = NULL
	,@DefaultLocaleCode nvarchar(5) = NULL
	,@PreferredContactPhone nvarchar(50) = NULL)
as

BEGIN--proc

	DECLARE
		@DefaultLocaleID int = NULL
		
	IF @DefaultLocaleCode IS NOT NULL
	BEGIN
		IF SUBSTRING(@DefaultLocaleCode,1,2) = 'es'
			SET @DefaultLocaleCode = 'es-us'
		ELSE 
			SET @DefaultLocaleCode = 'en-us'
	end
		
	SELECT @DefaultLocaleID = LocaleID from dbo.Locale where LocaleCode = @DefaultLocaleCode
	
	
	IF EXISTS (SELECT 1 FROM dbo.UserContentPreference where CCHID = @CCHID)
	BEGIN -- IF
		UPDATE dbo.UserContentPreference
		SET 
			SMSInd = ISNULL(@SMSInd,SMSInd),
			EmailInd = ISNULL(@EmailInd,EmailInd),
			OSBasedAlertInd = ISNULL(@OSBasedAlertInd, OSBasedAlertInd),
			DefaultLocaleID = ISNULL(@DefaultLocaleID, DefaultLocaleID),
			PreferredContactPhoneNum = ISNULL(@PreferredContactPhone, PreferredContactPhoneNum)
		WHERE CCHID = @CCHID
	END --IF
	ELSE
	BEGIN --ELSE
		INSERT INTO dbo.UserContentPreference (
			CCHID
			,SMSInd
			,EmailInd
			,OSBasedAlertInd
			,DefaultLocaleID
			,PreferredContactPhoneNum
			,LastUpdateDate
			,CreateDate)
		VALUES (
			@CCHID
			,ISNULL(@SMSInd,0)
			,ISNULL(@EmailInd,0)
			,ISNULL(@OSBasedAlertInd,0)
			,ISNULL(@DefaultLocaleID,1)
			,ISNULL(@PreferredContactPhone,'')
			,GETDATE()
			,GETDATE())
	END--ELSE
END--proc
 
 
 GO
 
 
