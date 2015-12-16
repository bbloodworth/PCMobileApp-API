/****** Object:  StoredProcedure [dbo].[p_GetUserPreferredContactPhone]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserPreferredContactPhone]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserPreferredContactPhone]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-06-04
Description:
      Returns the Preferred Contact Phone stored on the UserContentPreference for a CCHID
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.UserContentPreference
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-06-04 AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetUserPreferredContactPhone] (
	@CCHID int
	)
as

BEGIN--proc
	IF EXISTS (SELECT 1 FROM dbo.UserContentPreference where CCHID = @CCHID)
	BEGIN -- IF
		SELECT
			CCHID
			,PreferredContactPhoneNum
		FROM
			dbo.UserContentPreference
		WHERE
			CCHID = @CCHID
	END --IF
END--proc
 

GO


