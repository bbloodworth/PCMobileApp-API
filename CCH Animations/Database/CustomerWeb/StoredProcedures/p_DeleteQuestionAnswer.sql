/****** Object:  StoredProcedure [dbo].[p_DeleteQuestionAnswer]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_DeleteQuestionAnswer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_DeleteQuestionAnswer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Delete QuestionAnswer
      
Declarations:
      
Execute:
      exec p_DeleteQuestionAnswer
		@QuestionID = 3,
		@AnswerID = 1
		
Objects Listing:

Tables- dbo.QuestionAnswer
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-29  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_DeleteQuestionAnswer] (
	@QuestionID int,
	@AnswerID int
)
as

BEGIN
	
	DELETE 
		dbo.QuestionAnswer 
	WHERE 
		QuestionID = @QuestionID
		AND AnswerID = @AnswerID
		
END
 
GO
