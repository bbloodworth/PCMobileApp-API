/****** Object:  StoredProcedure [dbo].[p_InsertContentStatus]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertContentStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertContentStatus]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Content Status
      
Declarations:
      
Execute:
      exec p_InsertContentStatus @ContentStatusDesc = 'Viewed'

Objects Listing:

Tables- dbo.ContentStatus
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertContentStatus] (
	@ContentStatusDesc	nvarchar(100) 
)
as

BEGIN
	INSERT INTO dbo.ContentStatus (
		ContentStatusDesc,
		CreateDate)
	VALUES (
		@ContentStatusDesc,
		getdate())
END
 
GO
