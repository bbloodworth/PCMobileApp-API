/****** Object:  StoredProcedure [dbo].[p_InsertQuestionAnswer]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertQuestionAnswer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertQuestionAnswer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert QuestionAnswer
      
Declarations:
      
Execute:
      exec p_InsertQuestionAnswer
		@QuestionID = 3,
		@AnswerID = 1,
		@DisplayOrder = 1,
		@CorrectAnswerInd = 1
	
Objects Listing:

Tables- dbo.QuestionAnswer
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertQuestionAnswer] (
	@QuestionID int,
	@AnswerID int,
	@DisplayOrder int,
	@CorrectAnswerInd bit
)
as

BEGIN
	
	INSERT INTO dbo.QuestionAnswer (
		QuestionID
		,AnswerID
		,AnswerDisplayOrderNum
		,CorrectAnswerInd
		,CreateDate)
	VALUES (
		@QuestionID
		,@AnswerID
		,@DisplayOrder
		,@CorrectAnswerInd
		,getdate())
		
END

GO
 
