/****** Object:  StoredProcedure [dbo].[p_InsertContentType]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertContentType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertContentType]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Experience Event
      
Declarations:
      
Execute:
      exec p_InsertContentType @ContentTypeDesc = 'Animation'

Objects Listing:

Tables- dbo.ContentType
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertContentType] (
	@ContentTypeDesc	nvarchar(100) 
)
as

BEGIN
	INSERT INTO dbo.ContentType (
		ContentTypeDesc,
		CreateDate)
	VALUES (
		@ContentTypeDesc,
		getdate())
END
 
GO
