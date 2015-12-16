/****** Object:  StoredProcedure [dbo].[p_InsertContentTypeState]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertContentTypeState]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertContentTypeState]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert ContentTypeState
      
Declarations:
      
Execute:
      exec p_InsertContentTypeState
		@ContentTypeID = 1,
		@ContentStatusID = 3,
		@InitialStateInd = 1,
		@EndStateInd = 0
	
Objects Listing:

Tables- dbo.ContentTypeState
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertContentTypeState] (
	@ContentTypeID int = NULL,
	@ContentTypeDesc nvarchar(250) = NULL,
	@ContentStatusID int = NULL,
	@ContentStatusDesc nvarchar(250) = NULL,
	@InitialStateInd bit = 0,
	@EndStateInd bit = 0
)
as

BEGIN
	IF @ContentTypeID IS NULL
		SELECT
			@ContentTypeID = ContentTypeID 
		FROM
			dbo.ContentType
		WHERE
			ContentTypeDesc = @ContentTypeDesc
			
	IF @ContentStatusID IS NULL
		SELECT 
			@ContentStatusID = ContentStatusID
		FROM
			dbo.ContentStatus
		WHERE
			ContentStatusDesc = @ContentStatusDesc
	
	INSERT INTO dbo.ContentTypeState (
		ContentTypeID
		,ContentStatusID
		,InitialStateInd
		,EndStateInd
		,CreateDate)
	VALUES (
		@ContentTypeID
		,@ContentStatusID
		,@InitialStateInd
		,@EndStateInd
		,getdate())		
END
 
GO
