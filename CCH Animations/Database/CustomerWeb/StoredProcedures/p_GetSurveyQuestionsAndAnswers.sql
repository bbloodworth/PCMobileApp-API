
/****** Object:  StoredProcedure [dbo].[p_GetSurveyQuestionsAndAnswers]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetSurveyQuestionsAndAnswers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetSurveyQuestionsAndAnswers]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets all Questions and Answers associated with  a Survey
      
Declarations:
            
Execute:
      exec p_GetSurveyQuestionsAndAnswers
		@SurveyID= 4

Objects Listing:

Tables: 
dbo.Survey
dbo.Content

    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetSurveyQuestionsAndAnswers] (
	@SurveyID int
	,@LocaleCode nvarchar(10) = NULL
)
as

BEGIN
-----------------------------------------------------------
--Set the User's Preferred Locale (default to en-us)
-----------------------------------------------------------
IF @LocaleCode is NULL
	SET @LocaleCode = 'en-us'
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'en'
	SET @LocaleCode = 'en-us'
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'es'
	SET @LocaleCode = 'es-us'
ELSE 
	SET @LocaleCode = 'en-us'

-----------------------------------------------------------
--Get LocaleID
-----------------------------------------------------------
DECLARE
	@LocaleID int

SELECT @LocaleID = LocaleID	
FROM dbo.Locale
WHERE LocaleCode = @LocaleCode

-----------------------------------------------------------
--Get Survey Questions & Answers
------------------------------------------------------------
	SELECT
		s.SurveyID
		,q.QuestionID
		,q.QuestionTypeID
		,qt.QuestionTypeDesc
		,qtn.QuestionText
		,q.ScoredQuestionInd
		,q.ExpectedAnswerDatatypeDesc
		,q.ExpectedAnswerValueRangeDesc
		,sq.QuestionDisplayOrderNum
		,a.AnswerID
		,atn.AnswerText
		,qa.AnswerDisplayOrderNum
		,qa.CorrectAnswerInd
	FROM
		dbo.Survey s
		INNER JOIN dbo.SurveyQuestion sq
			ON s.SurveyID = sq.SurveyID
		INNER JOIN dbo.Question q
			ON sq.QuestionID = q.QuestionID
		INNER JOIN dbo.QuestionTranslation qtn
			ON q.QuestionID = qtn.QuestionID
		INNER JOIN dbo.QuestionAnswer qa
			ON q.QuestionID = qa.QuestionID
		INNER JOIN dbo.Answer a
			ON qa.AnswerID = a.AnswerID
		INNER JOIN dbo.AnswerTranslation atn
			ON a.AnswerID = atn.AnswerID
		INNER JOIN dbo.QuestionType qt
			on q.QuestionTypeID = qt.QuestionTypeID
	WHERE
		s.SurveyID = @SurveyID
		AND qtn.LocaleID = @LocaleID
		AND atn.LocaleID = @LocaleID
	ORDER BY
		sq.QuestionDisplayOrderNum,
		qa.AnswerDisplayOrderNum
END
 
 GO
 
 
