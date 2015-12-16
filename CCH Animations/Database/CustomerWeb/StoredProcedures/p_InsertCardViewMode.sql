

/****** Object:  StoredProcedure [dbo].[p_InsertCardViewMode]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertCardViewMode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertCardViewMode]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-05-11
Description:
      Insert Card View Mode
      
Declarations:
      
Execute:
      exec p_InsertCardViewMode @CardViewModeName = 'Front'

Objects Listing:

Tables- dbo.CardViewMode
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-11  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertCardViewMode] (
	@CardViewModeName	nvarchar(50) 
)
as

BEGIN
	INSERT INTO dbo.CardViewMode (
		CardViewModeName,
		CreateDate)
	VALUES (
		@CardViewModeName,
		getdate())
END
 
 GO
 
 
