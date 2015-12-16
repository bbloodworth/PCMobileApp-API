/****** Object:  StoredProcedure [dbo].[p_InsertContentDisplayRule]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertContentDisplayRule]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertContentDisplayRule]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:

      
Declarations:
      
Execute:
      exec p_InsertContentDisplayRule @ContentDisplayRuleDesc = 'Always Display'

Objects Listing:

Tables- dbo.ContentDisplayRule
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertContentDisplayRule] (
	@ContentDisplayRuleDesc	nvarchar(100) 
)
as

BEGIN
	INSERT INTO dbo.ContentDisplayRule (
		ContentDisplayRuleDesc,
		CreateDate)
	VALUES (
		@ContentDisplayRuleDesc,
		getdate())
END
 
GO
