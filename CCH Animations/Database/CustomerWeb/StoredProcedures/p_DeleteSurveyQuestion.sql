/****** Object:  StoredProcedure [dbo].[p_DeleteSurveyQuestion]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_DeleteSurveyQuestion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_DeleteSurveyQuestion]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Delete SurveyQuestion
      
Declarations:
      
Execute:
      exec p_DeleteSurveyQuestion
		@SurveyID = 1,
		@QuestionID = 3
	
Objects Listing:

Tables- dbo.SurveyQuestion
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_DeleteSurveyQuestion] (
	@SurveyID int,
	@QuestionID int
)
as

BEGIN
	
	DELETE 
		dbo.SurveyQuestion
	WHERE
		SurveyID = @SurveyID
		AND QuestionID = @QuestionID
		
END
 
GO
