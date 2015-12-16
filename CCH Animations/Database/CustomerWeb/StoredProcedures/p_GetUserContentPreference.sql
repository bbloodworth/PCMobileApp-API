
/****** Object:  StoredProcedure [dbo].[p_GetUserContentPreference]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserContentPreference]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserContentPreference]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-05-27
Description:
      Gets the user's preference governing the receipt & display of content
      
Declarations:
            
Execute:
      exec p_GetUserContentPreference
		@CCHID= 57020
		
Objects Listing:

Tables- dbo.UserContentPreference
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-27  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetUserContentPreference] (
	@CCHID int
)
as

BEGIN

------------------------------------------------------------
--Get Users Preferences
-------------------------------------------------------------
	SELECT
		uc.CCHID
		,uc.SMSInd
		,uc.EmailInd
		,uc.OSBasedAlertInd
		,l.LocaleCode as DefaultLocaleCode	
		,uc.PreferredContactPhoneNum
	FROM
		dbo.UserContentPreference uc
		INNER JOIN Locale l
			on uc.DefaultLocaleID = l.LocaleID
	WHERE
		uc.CCHID = @CCHID
	
END

GO


 
