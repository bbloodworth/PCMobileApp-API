/****** Object:  StoredProcedure [dbo].[p_InsertRelatedContent]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertRelatedContent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertRelatedContent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert RelatedContent
      
Declarations:
      
Execute:
      exec p_InsertRelatedContent
		@ParentContentID = 3,
		@RelatedContentID = 1,
		@RelatedContentDisplayRuleID = 1
	
Objects Listing:

Tables- dbo.RelatedContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertRelatedContent] (
	@ParentContentID int,
	@RelatedContentID int,
	@RelatedContentDisplayRuleID int
)
as

BEGIN
	
	INSERT INTO dbo.RelatedContent (
		ParentContentID
		,RelatedContentID
		,RelatedContentDisplayRuleID
		,CreateDate)
	VALUES (
		@ParentContentID
		,@RelatedContentID
		,@RelatedContentDisplayRuleID
		,GETDATE())	
END

GO
 
