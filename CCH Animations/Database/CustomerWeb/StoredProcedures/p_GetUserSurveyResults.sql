/****** Object:  StoredProcedure [dbo].[p_GetUserSurveyResults]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserSurveyResults]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserSurveyResults]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets the user's results for a survey
      
Declarations:
            
Execute:
      exec p_GetUserSurveyResults
		@CCHID= 57020
		@SurveyID = 4

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetUserSurveyResults] (
	@CCHID int,
	@SurveyID int,
	@SurveyIterationNum int = 0
)
as

BEGIN --sproc
------------------------------------------------
--First get the iteration number if one wasn't supplied
------------------------------------------------
	IF EXISTS (SELECT 1 FROM UserSurveyAnswer WHERE CCHID = @CCHID and SurveyID = @SurveyID)
	BEGIN --IF1
		IF @SurveyIterationNum = 0
		BEGIN --IF2
			SELECT 
				@SurveyIterationNum = MAX(SurveyIterationNum)
			FROM
				UserSurveyAnswer
			WHERE
				CCHID =@CCHID
				AND SurveyID = @SurveyID
		END --IF2

------------------------------------------------
--Get Score and Pass/Fail
------------------------------------------------
		SELECT
			usa.CCHID
			,usa.SurveyID
			,usa.SurveyIterationNum
			,s.SurveyPassCount
			,SUM(CASE WHEN qa.CorrectAnswerInd = 1 THEN 1 ELSE 0 END) as UserScore
			,CASE WHEN (SUM(CASE WHEN qa.CorrectAnswerInd = 1 THEN 1 ELSE 0 END)) >= s.SurveyPassCount
				THEN 'Pass' ELSE 'Fail' END as UserResult	
		FROM
			dbo.UserSurveyAnswer usa
			INNER JOIN QuestionAnswer qa
				on usa.QuestionID = qa.QuestionID
				and usa.AnswerID = qa.AnswerID
			INNER JOIN Survey s
				on usa.SurveyID = s.SurveyID
		WHERE
			usa.CCHID = @CCHID
			AND usa.SurveyID = @SurveyID
			AND usa.SurveyIterationNum = @SurveyIterationNum
		GROUP BY
			usa.CCHID
			,usa.SurveyID
			,usa.SurveyIterationNum
			,s.SurveyPassCount
	END--IF1
	ELSE
	BEGIN --ELSE
		print 'User has no results for selected Survey'
	END --ELSE
END -- proc

GO
 
