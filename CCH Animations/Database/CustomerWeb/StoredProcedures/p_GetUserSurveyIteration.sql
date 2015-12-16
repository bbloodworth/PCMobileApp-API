/****** Object:  StoredProcedure [dbo].[p_GetUserSurveyIteration]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserSurveyIteration]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserSurveyIteration]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets the current and next iteration for a survey
      
Declarations:
            
Execute:
      exec p_GetUserSurveyIteration
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

CREATE PROCEDURE [dbo].[p_GetUserSurveyIteration] (
	@CCHID int,
	@SurveyID int
)
as

BEGIN --sproc

	IF EXISTS (SELECT 1 FROM UserSurveyAnswer WHERE CCHID = @CCHID and SurveyID = @SurveyID)
	BEGIN --IF
		SELECT 
			CurrentSurveyIterationNum= MAX(SurveyIterationNum),
			NextSurveyIterationNum = MAX(SurveyIterationNum) + 1
		FROM
			UserSurveyAnswer
		WHERE
			CCHID =@CCHID
			AND SurveyID = @SurveyID
	END --IF
	ELSE
	BEGIN--ELSE
		SELECT
			CurrentSurveyIterationNum = 0,
			NextSurveyIterationNum = 1
	END--ELSE
END --proc
 
GO
