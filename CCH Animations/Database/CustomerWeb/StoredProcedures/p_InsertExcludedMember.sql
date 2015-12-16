/****** Object:  StoredProcedure [dbo].[p_InsertExcludedMember]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertExcludedMember]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertExcludedMember]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert ExcludedMember
      
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

CREATE PROCEDURE [dbo].[p_InsertExcludedMember] (
	@CCHID int
	,@ExcludeReasonDesc nvarchar(100))
as

BEGIN--proc

	IF NOT EXISTS (SELECT 1 FROM ExcludedMember where CCHID = @CCHID)
	BEGIN
		INSERT INTO dbo.ExcludedMember (
			CCHID
			,ExcludeReasonDesc
			,CreateDate)
		VALUES (
			@CCHID
			,@ExcludeReasonDesc
			,GETDATE())
	END
END--proc

GO


 
