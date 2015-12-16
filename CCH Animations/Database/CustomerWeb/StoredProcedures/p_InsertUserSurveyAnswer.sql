/****** Object:  StoredProcedure [dbo].[p_InsertUserSurveyAnswer]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertUserSurveyAnswer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertUserSurveyAnswer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Records user responses to survey questions.
      
Declarations:
            
Execute:
      exec p_InsertUserSurveyAnswer
		@CampaignID = 1
		@CCHID = 57020
		@SurveyID = 4
		@QuestionID = 1
		@AnswerID = 1
		--@FreeFormAnswerText = NULL

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertUserSurveyAnswer] (
	@CCHID int,
	@CampaignID int,
	@SurveyID int,
	@QuestionID int,
	@AnswerID int = NULL,
	@FreeFormAnswerText nvarchar(500) = NULL
)
as

BEGIN

	INSERT UserSurveyAnswer(
		CCHID
		,CampaignID
		,SurveyID
		,SurveyIterationNum
		,QuestionID
		,AnswerID
		,FreeFormAnswerText
		,CreateDate)
	VALUES(
		@CCHID
		,@CampaignID
		,@SurveyID
		,1 --Hard code to 1 for now - need to add in this logic
		,@QuestionID
		,@AnswerID
		,@FreeFormAnswerText
		,GETDATE())

END

GO
 
