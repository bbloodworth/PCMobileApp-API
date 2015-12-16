/****** Object:  StoredProcedure [dbo].[p_DeleteSurveyQuestion]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertSurveyQuestion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertSurveyQuestion]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert SurveyQuestion
      
Declarations:
      
Execute:
      exec p_InsertSurveyQuestion
		@SurveyID = 1,
		@QuestionID = 3,
		@DisplayOrder = 1
	
Objects Listing:

Tables- dbo.SurveyQuestion
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertSurveyQuestion] (
	@SurveyID int,
	@QuestionID int,
	@DisplayOrder int
)
as

BEGIN
	
	INSERT INTO dbo.SurveyQuestion (
		SurveyID
		,QuestionID
		,QuestionDisplayOrderNum
		,CreateDate)
	VALUES (
		@SurveyID
		,@QuestionID
		,@DisplayOrder
		,getdate())
		
END
 

GO


