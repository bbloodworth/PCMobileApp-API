/****** Object:  StoredProcedure [dbo].[p_DeleteExcludedMember]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_DeleteExcludedMember]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_DeleteExcludedMember]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Delete ExcludedMember
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.UserContentPreference
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_DeleteExcludedMember] (
	@CCHID int
)
as

BEGIN--proc
	DELETE dbo.ExcludedMember 
	WHERE CCHID = @CCHID
END--proc
 
GO
