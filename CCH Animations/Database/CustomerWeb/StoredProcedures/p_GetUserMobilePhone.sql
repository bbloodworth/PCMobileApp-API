/****** Object:  StoredProcedure [dbo].[p_GetUserMobilePhone]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserMobilePhone]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserMobilePhone]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-04-03
Description:
      Returns the Mobile Phone stored in Enrollments for a CCHID
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.Enrollments
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-04-03  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetUserMobilePhone] (
	@CCHID int
	)
as

BEGIN--proc
	IF EXISTS (SELECT 1 FROM dbo.Enrollments where CCHID = @CCHID)
	BEGIN -- IF
		SELECT
			CCHID
			,MobilePhone
		FROM
			dbo.Enrollments
		WHERE
			CCHID = @CCHID
	END --IF
END--proc

GO


 
