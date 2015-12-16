
/****** Object:  StoredProcedure [dbo].[p_InsertQuestion]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertQuestion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertQuestion]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Question
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.Question
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertQuestion] (
	@QuestionTypeID int,
	@QuestionText nvarchar(250),
	@ExpectedAnswerDatatype nvarchar(100) = NULL,
	@ExpectedAnswerValueRange nvarchar(100) = NULL,
	@ScoredQuestionInd bit = 1,
	@LocaleCode nvarchar(10) = NULL
	
)
as

BEGIN
	DECLARE 
		@QuestionID int
		,@LocaleID int
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode

--------------------------------------------------------------
--Insert the Question Record
---------------------------------------------------------------

	INSERT INTO dbo.Question (
		QuestionTypeID
		--,QuestionText
		,ExpectedAnswerDatatypeDesc
		,ExpectedAnswerValueRangeDesc
		,ScoredQuestionInd
		,CreateDate)
	VALUES (
		@QuestionTypeID
		--,@QuestionText
		,@ExpectedAnswerDatatype
		,@ExpectedAnswerValueRange
		,@ScoredQuestionInd
		,getdate())
		
	SELECT @QuestionID = SCOPE_IDENTITY()
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @QuestionText IS NOT NULL 
	BEGIN
	
		exec p_InsertQuestionTranslation
			@QuestionID = @QuestionID
			,@LocaleID = @LocaleID
			,@QuestionText = @QuestionText
	END --if		

END
 
GO

