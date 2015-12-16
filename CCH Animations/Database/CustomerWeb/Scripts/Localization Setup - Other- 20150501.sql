

--------------------------------------------------------------------------
--Insert Supported Locales
--------------------------------------------------------------------------
INSERT INTO Locale (
	LocaleID
	,LocaleCode
	,ISOCountryCode
	,ISOLanguageCode
	,LocaleDesc
	,DateFormatDesc
	,CreateDate )
VALUES (
	1
	,'en-us'
	,'us'
	,'en'
	,'English (United States)'
	,'MM-DD-YYYY'
	,GETDATE()
	)
	
GO

INSERT INTO Locale (
	LocaleID
	,LocaleCode
	,ISOCountryCode
	,ISOLanguageCode
	,LocaleDesc
	,DateFormatDesc
	,CreateDate )
VALUES (
	2
	,'es-us'
	,'us'
	,'es'
	,'Spanish (United States)'
	,'MM-DD-YYYY'
	,GETDATE()
	)
GO
--select * from Locale

--------------------------------------------------------------------------
--Update everyone's default Locale to English
--------------------------------------------------------------------------
UPDATE
	UserContentPreference
SET
	DefaultLocaleID = 1


--------------------------------------------------------------------------
--Insert the English Translations
--------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Content c INNER JOIN ContentTranslation ct on c.ContentID = ct.ContentID)
INSERT INTO ContentTranslation (
	ContentID
	,LocaleID
	,ContentTitle
	,ContentCaptionText
	,ContentDesc
	,CreateDate
)
SELECT
	ContentID
	,1
	,ContentTitle
	,ContentCaptionText
	,ContentDesc
	,GETDATE()
FROM
	dbo.Content
	
--select * from Content
--select * from ContentTranslation

IF NOT EXISTS (SELECT 1 FROM Answer a INNER JOIN AnswerTranslation at on a.AnswerText = at.AnswerID)
INSERT INTO AnswerTranslation (
	AnswerID
	,LocaleID
	,AnswerText
	,CreateDate
)
SELECT
	AnswerID
	,1
	,AnswerText
	,GETDATE()
FROM
	dbo.Answer
	
--select * from Answer
--select * from AnswerTranslation

IF NOT EXISTS (SELECT 1 FROM Question a INNER JOIN QuestionTranslation at on a.QuestionText = at.QuestionID)
INSERT INTO QuestionTranslation (
	QuestionID
	,LocaleID
	,QuestionText
	,CreateDate
)
SELECT
	QuestionID
	,1
	,QuestionText
	,GETDATE()
FROM
	dbo.Question
	
--select * from Question
--select * from QuestionTranslation


--------------------------------------------------------------------------
--Drop the old columns
---------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Content' and name = 'ContentTitle')
	ALTER TABLE Content
	DROP COLUMN ContentTitle

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Content' and name = 'ContentCaptionText')
	ALTER TABLE Content
	DROP COLUMN ContentCaptionText
	
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Content' and name = 'ContentDesc')
	ALTER TABLE Content
	DROP COLUMN ContentDesc
	
--select * from Content
--select * from ContentTranslation

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Answer' and name = 'AnswerText')
	ALTER TABLE Answer
	DROP COLUMN AnswerText
	
--select * from Answer
--select * from AnswerTranslation

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Question' and name = 'QuestionText')
	ALTER TABLE Question
	DROP COLUMN QuestionText
	
--select * from Question
--select * from QuestionTranslation

GO



