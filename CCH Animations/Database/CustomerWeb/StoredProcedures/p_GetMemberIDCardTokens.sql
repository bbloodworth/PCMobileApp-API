
/****** Object:  StoredProcedure [dbo].[p_GetMemberIDCardTokens]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetMemberIDCardTokens]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetMemberIDCardTokens]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-05-12
Description:
      Creates & Returns new valid security tokens for all Member ID Cards for a given CCHID / LocaleCode
      
Declarations:
            
Execute:
      exec p_GetMemberIDCardTokens
		@CCHID= 57020
		,@LocaleCode = 'en-us'

Objects Listing:

Tables- dbo.MemberIDCard
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-12  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetMemberIDCardTokens] (
	@CCHID int
	,@LocaleCode nvarchar(10) = NULL
)
as

BEGIN
-----------------------------------------------------------
--Set the User's Preferred Locale (default to en-us)
-----------------------------------------------------------
IF @LocaleCode is NULL
	SET @LocaleCode = ISNULL((SELECT l.LocaleCode from UserContentPreference u inner join Locale l on u.DefaultLocaleID = l.LocaleID WHERE CCHID = @CCHID), 'en-us')
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'en'
	SET @LocaleCode = 'en-us'
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'es'
	SET @LocaleCode = 'es-us'
ELSE 
	SET @LocaleCode = 'en-us'
	
-----------------------------------------------------------
--Get PreferredLocaleID and DefaultLocaleID
-----------------------------------------------------------
DECLARE
	@PreferredLocaleID int
	,@DefaultLocaleID int

SELECT @PreferredLocaleID = LocaleID	
FROM dbo.Locale
WHERE LocaleCode = @LocaleCode

SELECT @DefaultLocaleID = LocaleID
FROM dbo.Locale
WHERE LocaleCode = 'en-us'
-----------------------------------------------------------
--Update the MemberIDCard Security Tokens
-----------------------------------------------------------
	UPDATE
		dbo.MemberIDCard
	SET
		SecurityTokenGUID = NEWID()
		,SecurityTokenBeginDatetime = GETDATE()
		,SecurityTokenEndDatetime = DATEADD(MINUTE,30,GETDATE())
	WHERE
		CCHID = @CCHID
		AND LocaleID = ISNULL(@PreferredLocaleID, @DefaultLocaleID)
		
------------------------------------------------------------
--Return the Security Tokens
-------------------------------------------------------------
	SELECT
		ctt.CardTypeName
		,cvm.CardViewModeName
		,mc.SecurityTokenGUID	
	FROM
		dbo.MemberIDCard mc
		INNER JOIN dbo.CardTypeTranslation ctt
			on mc.CardTypeID = ctt.CardTypeID
			and mc.LocaleID = ctt.LocaleID
		INNER JOIN dbo.CardViewMode cvm
			on mc.CardViewModeID = cvm.CardViewModeID
	WHERE
		mc.CCHID = @CCHID
		AND mc.LocaleID = ISNULL(@PreferredLocaleID, @DefaultLocaleID)

END

GO

