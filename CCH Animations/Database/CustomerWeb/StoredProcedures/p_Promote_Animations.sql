
/****** Object:  StoredProcedure [dbo].[p_Promote_Animations]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_Promote_Animations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_Promote_Animations]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-02-10
Description:
      Promotes data from specific animations tables to the target environment. 
      Promotions are either 'Update else insert' on primary key or 'Insert only' on primary key.
      Deletes are processed first on all tables.
      
Declarations:
            
Execute:
      exec p_Promote_Animations

Objects Listing:

Tables: 


    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-02-10  AS       Created
2015-04-14  AS		 Updated for Localization
2015-06-01	AS		 Fix Delete vs. Insert/Update Order for FKs
2015-08-07  AS	     Remove Large tables which are processed during enrollments (MemberIDCard, UserContentPreference)
2015-08-20  AS       Updated to add new columns to CampaignContent and UserContent
2015-09-29  AS       Updated to add new columns to ContentTranslation and UserContent
2015-09-30  AS       Updated to fix servername reference to work in new ClearData environment
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_Promote_Animations] (
@servername nvarchar(50)
,@dbname nvarchar(50)
)
as

BEGIN --proc

	IF  EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'SQLEnvironment' and ConfigValue = 'PROCESSING') AND db_name() = @dbname AND EXISTS (SELECT 1 FROM sys.servers WHERE name = @servername) 
	BEGIN -- env
----------------------------------------------------------------------------
--Tables must be processed in order below due to FK Consraints
---------------------------------------------------------------------------
	
DECLARE @SQL nvarchar(MAX)
----------------------------------------------------------------------------
--STEP ONE: Process all Deletes First (in Child-Parent order)
----------------------------------------------------------------------------
/***************************************************************************/
-----------------------------------------------------------------------------
--UserContent
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#UserContentDeletes') IS NOT NULL
DROP TABLE #UserContentDeletes

CREATE TABLE #UserContentDeletes
	(CampaignID int
	 ,CCHID int
	 ,ContentID int)

SET @SQL =
'INSERT INTO
	#UserContentDeletes
SELECT
	aC.CampaignID
	,aC.CCHID
	,aC.ContentID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContent aC
	--processing.VanessaTempdb.dbo.UserContent aC
EXCEPT
SELECT
	pc.CampaignID
	,pc.CCHID
	,pC.ContentID
FROM
	dbo.UserContent pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContent 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContent aC
		INNER JOIN #UserContentDeletes d
			on ac.CCHID = d.CCHID
			and ac.CampaignID = d.CampaignID
			and ac.ContentID = d.ContentID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--SurveyQuestion
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
CREATE TABLE #SurveyQuestionDeletes
	(SurveyID int
	 ,QuestionID int)
	
SET @SQL =
'INSERT INTO
	#SurveyQuestionDeletes
SELECT
	aC.SurveyID
	,aC.QuestionID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion aC
EXCEPT
SELECT
	pc.SurveyID
	,pc.QuestionID
FROM
	dbo.SurveyQuestion pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #SurveyQuestionDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion aC
		INNER JOIN #SurveyQuestionDeletes d
			on ac.QuestionID = d.QuestionID
			and ac.SurveyID = d.SurveyID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--CampaignMember
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#CampaignMemberDeletes') IS NOT NULL
DROP TABLE #CampaignMemberDeletes

CREATE TABLE #CampaignMemberDeletes
	(CampaignID int
	 ,CCHID int)

SET @SQL =
'INSERT INTO
	#CampaignMemberDeletes
SELECT
	aC.CampaignID
	,aC.CCHID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember aC
EXCEPT
SELECT
	pc.CampaignID
	,pc.CCHID
FROM
	dbo.CampaignMember pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignMemberDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember aC
		INNER JOIN #CampaignMemberDeletes d
			on ac.CCHID = d.CCHID
			and ac.CampaignID = d.CampaignID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--CampaignContent
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#CampaignContentDeletes') IS NOT NULL
DROP TABLE #CampaignContentDeletes

CREATE TABLE #CampaignContentDeletes
	(CampaignID int
	 ,ContentID int)

SET @SQL = 
'INSERT INTO
	#CampaignContentDeletes
SELECT
	aC.CampaignID
	,aC.ContentID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent aC
EXCEPT
SELECT
	pc.CampaignID
	,pc.ContentID
FROM
	dbo.CampaignContent pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignContentDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent aC
		--processing.VanessaTempdb.dbo.CampaignContent aC
		INNER JOIN #CampaignContentDeletes d
			on ac.ContentID = d.ContentID
			and ac.CampaignID = d.CampaignID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--Video
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#VideoDeletes') IS NOT NULL
DROP TABLE #VideoDeletes

CREATE TABLE #VideoDeletes 
	(VideoID int)
	
SET @SQL = 
'INSERT INTO
	#VideoDeletes
SELECT
	aC.VideoID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video aC
EXCEPT
SELECT
	pc.VideoID
FROM
	dbo.Video pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #VideoDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video aC
		INNER JOIN #VideoDeletes d
			on ac.VideoID = d.VideoID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--Animation
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#AnimationDeletes') IS NOT NULL
DROP TABLE #AnimationDeletes

CREATE TABLE #AnimationDeletes 
	(AnimationID int)

SET @SQL = 
'INSERT INTO
	#AnimationDeletes
SELECT
	aC.AnimationID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation aC
EXCEPT
SELECT
	pc.AnimationID
FROM
	dbo.Animation pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnimationDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation aC
		INNER JOIN #AnimationDeletes d
			on ac.AnimationID = d.AnimationID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--Survey
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#SurveyDeletes') IS NOT NULL
DROP TABLE #SurveyDeletes

CREATE TABLE #SurveyDeletes
	(SurveyID int)
	
SET @SQL = 
'INSERT INTO
	#SurveyDeletes
SELECT
	aC.SurveyID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey aC
EXCEPT
SELECT
	pc.SurveyID
FROM
	dbo.Survey pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #SurveyDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey aC
		INNER JOIN #SurveyDeletes d
			on ac.SurveyID = d.SurveyID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--ContentTranslation
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ContentTranslationDeletes') IS NOT NULL
DROP TABLE #ContentTranslationDeletes

CREATE TABLE #ContentTranslationDeletes 
	(ContentID int
	,LocaleID int)
	
SET @SQL =
'INSERT INTO
	#ContentTranslationDeletes
SELECT
	aC.ContentID 
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation aC 
EXCEPT
SELECT
	pc.ContentID
	,pc.LocaleID
FROM
	dbo.ContentTranslation pC'
	
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentTranslationDeletes) >0)
BEGIN	
	SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation aC
		INNER JOIN #ContentTranslationDeletes d
			on ac.ContentID = d.ContentID
			and ac.LocaleID = d.LocaleID'

	EXEC (@SQL)	
	
END
/* 2015-08-07 AS -- Removed as this table is loaded / promoted during enrollments
-----------------------------------------------------------------------------
--MemberIDCard
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#MemberIDCardDeletes') IS NOT NULL
DROP TABLE #MemberIDCardDeletes

CREATE TABLE #MemberIDCardDeletes
	( CCHID int
	 ,CardTypeID int
	 ,LocaleID int
	 ,CardViewModeID int
	 )
SET @SQL =
'INSERT INTO
	#MemberIDCardDeletes
SELECT
      aC.CCHID
	 ,aC.CardTypeID
	 ,aC.LocaleID
	 ,aC.CardViewModeID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard aC
	--processing.VanessaTempdb.dbo.MemberIDCard aC
EXCEPT
SELECT
	 pC.CCHID
	 ,pC.CardTypeID
	 ,pC.LocaleID
	 ,pC.CardViewModeID
FROM
	dbo.MemberIDCard pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #MemberIDCardDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard aC
		INNER JOIN #MemberIDCardDeletes d
			on ac.CCHID = d.CCHID
			and ac.CardTypeID = d.CardTypeID
			and ac.LocaleID = d.LocaleID
			and ac.CardViewModeID = d.CardViewModeID'
EXEC (@SQL)
END
*/
-----------------------------------------------------------------------------
--RelatedContent
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#RelatedContentDeletes') IS NOT NULL
DROP TABLE #RelatedContentDeletes

CREATE TABLE #RelatedContentDeletes
	(ParentContentID int
	 ,RelatedContentID int)
	
SET @SQL = 
'INSERT INTO
	#RelatedContentDeletes
SELECT
	aC.ParentContentID
	,aC.RelatedContentID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent aC
EXCEPT
SELECT
	pc.ParentContentID
	,pc.RelatedContentID
FROM
	dbo.RelatedContent pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #RelatedContentDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent aC
		INNER JOIN #RelatedContentDeletes d
			on ac.RelatedContentID = d.RelatedContentID
			and ac.ParentContentID = d.ParentContentID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--Content
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ContentDeletes') IS NOT NULL
DROP TABLE #ContentDeletes

CREATE TABLE #ContentDeletes 
	(ContentID int)
	
SET @SQL =
'INSERT INTO
	#ContentDeletes
SELECT
	aC.ContentID 
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content aC 
EXCEPT
SELECT
	pc.ContentID
FROM
	dbo.Content pC'
	
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentDeletes) >0)
BEGIN	
	SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content aC
		INNER JOIN #ContentDeletes d
			on ac.ContentID = d.ContentID'

	EXEC (@SQL)	
	
END

-----------------------------------------------------------------------------
--ContentTypeState
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ContentTypeStateDeletes') IS NOT NULL
DROP TABLE #ContentTypeStateDeletes

CREATE TABLE #ContentTypeStateDeletes
	(ContentTypeID int
	 ,ContentStatusID int)

SET @SQL =
'INSERT INTO
	#ContentTypeStateDeletes
SELECT
	aC.ContentTypeID
	,aC.ContentStatusID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState aC
EXCEPT
SELECT
	pc.ContentTypeID
	,pc.ContentStatusID
FROM
	dbo.ContentTypeState pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTypeStateDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState aC
		INNER JOIN #ContentTypeStateDeletes d
			on ac.ContentStatusID = d.ContentStatusID
			and ac.ContentTypeID = d.ContentTypeID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--QuestionAnswer
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#QuestionAnswerDeletes') IS NOT NULL
DROP TABLE #QuestionAnswerDeletes

CREATE TABLE #QuestionAnswerDeletes
	(QuestionID int
	 ,AnswerID int)
	 
SET @SQL =
'INSERT INTO
	#QuestionAnswerDeletes
SELECT
	aC.QuestionID
	,aC.AnswerID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer aC
EXCEPT
SELECT
	pc.QuestionID
	,pc.AnswerID
FROM
	dbo.QuestionAnswer pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionAnswerDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer aC
		INNER JOIN #QuestionAnswerDeletes d
			on ac.AnswerID = d.AnswerID
			and ac.QuestionID = d.QuestionID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--QuestionTranslation
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#QuestionTranslationDeletes') IS NOT NULL
DROP TABLE #QuestionTranslationDeletes

CREATE TABLE #QuestionTranslationDeletes 
	(QuestionID int
	,LocaleID int)
	
SET @SQL =
'INSERT INTO
	#QuestionTranslationDeletes
SELECT
	aC.QuestionID 
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation aC 
EXCEPT
SELECT
	pc.QuestionID
	,pc.LocaleID
FROM
	dbo.QuestionTranslation pC'
	
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionTranslationDeletes) >0)
BEGIN	
	SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation aC
		INNER JOIN #QuestionTranslationDeletes d
			on ac.QuestionID = d.QuestionID
			and ac.LocaleID = d.LocaleID'

	EXEC (@SQL)	
	
END

-----------------------------------------------------------------------------
--AnswerTranslation
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#AnswerTranslationDeletes') IS NOT NULL
DROP TABLE #AnswerTranslationDeletes

CREATE TABLE #AnswerTranslationDeletes 
	(AnswerID int
	,LocaleID int)
	
SET @SQL =
'INSERT INTO
	#AnswerTranslationDeletes
SELECT
	aC.AnswerID 
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation aC 
EXCEPT
SELECT
	pc.AnswerID
	,pc.LocaleID
FROM
	dbo.AnswerTranslation pC'
	
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------
IF ((SELECT COUNT(1) FROM #AnswerTranslationDeletes) >0)
BEGIN	
	SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation aC
		INNER JOIN #AnswerTranslationDeletes d
			on ac.AnswerID = d.AnswerID
			and ac.LocaleID = d.LocaleID'

	EXEC (@SQL)	
	
END

-----------------------------------------------------------------------------
--CardTypeTranslation
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#CardTypeTranslationDeletes') IS NOT NULL
DROP TABLE #CardTypeTranslationDeletes

CREATE TABLE #CardTypeTranslationDeletes 
	(CardTypeID int
	,LocaleID int)
	
SET @SQL =
'INSERT INTO
	#CardTypeTranslationDeletes
SELECT
	aC.CardTypeID 
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation aC 
EXCEPT
SELECT
	pc.CardTypeID
	,pc.LocaleID
FROM
	dbo.CardTypeTranslation pC'
	
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CardTypeTranslationDeletes) >0)
BEGIN	
	SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation aC
		INNER JOIN #CardTypeTranslationDeletes d
			on ac.CardTypeID = d.CardTypeID
			and ac.LocaleID = d.LocaleID'

	EXEC (@SQL)	
	
END
/* 2015-08-07 AS -- Removed as this table is loaded / promoted during enrollments
-----------------------------------------------------------------------------
--UserContentPreference
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#UserContentPreferenceDeletes') IS NOT NULL
DROP TABLE #UserContentPreferenceDeletes

CREATE TABLE #UserContentPreferenceDeletes
	(CCHID int)

SET @SQL =
'INSERT INTO
	#UserContentPreferenceDeletes
SELECT
	aC.CCHID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference aC
EXCEPT
SELECT
	pc.CCHID
FROM
	dbo.UserContentPreference pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentPreferenceDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference aC
		INNER JOIN #UserContentPreferenceDeletes d
			on ac.CCHID = d.CCHID'
EXEC (@SQL)
END
*/
-----------------------------------------------------------------------------
--Campaign
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
		IF	object_id('tempdb..#CampaignDeletes') IS NOT NULL
		DROP TABLE #CampaignDeletes
		
		CREATE TABLE #CampaignDeletes 
			(CampaignID int)
		
		SET @SQL = 
		'INSERT INTO #CampaignDeletes SELECT aC.CampaignID  FROM ' +
			LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign aC
		EXCEPT
		SELECT pc.CampaignID FROM dbo.Campaign pC'

		EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

		IF ((SELECT COUNT(1) FROM #CampaignDeletes) >0)
		BEGIN
			SET @SQL = 	
				'DELETE ' + 
					LTRIM(RTRIM(@servername)) + '.' +  LTRIM(RTRIM(@dbname)) + '.dbo.Campaign 
				FROM ' +
					LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign aC
					INNER JOIN #CampaignDeletes d
						on ac.CampaignID = d.CampaignID '
			EXEC (@SQL)
		END
-----------------------------------------------------------------------------
--ContentDisplayRule
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ContentDisplayRuleDeletes') IS NOT NULL
DROP TABLE #ContentDisplayRuleDeletes

CREATE TABLE #ContentDisplayRuleDeletes 
	(ContentDisplayRuleID int)
	
SET @SQL = 
'INSERT INTO
	#ContentDisplayRuleDeletes
SELECT
	aC.ContentDisplayRuleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule aC
	--processing.VanessaTempdb.dbo.ContentDisplayRule aC
EXCEPT
SELECT
	pc.ContentDisplayRuleID
FROM
	dbo.ContentDisplayRule pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentDisplayRuleDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule aC
		INNER JOIN #ContentDisplayRuleDeletes d
			on ac.ContentDisplayRuleID = d.ContentDisplayRuleID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--ContentType
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ContentTypeDeletes') IS NOT NULL
DROP TABLE #ContentTypeDeletes

CREATE TABLE #ContentTypeDeletes 
	(ContentTypeID int)
	
SET @SQL = 
'INSERT INTO #ContentTypeDeletes SELECT aC.ContentTypeID FROM ' + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType aC
EXCEPT
SELECT
	pc.ContentTypeID
FROM
	dbo.ContentType pC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTypeDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType aC
		INNER JOIN #ContentTypeDeletes d
			on ac.ContentTypeID = d.ContentTypeID'

EXEC (@SQL)

END

-----------------------------------------------------------------------------
--ContentStatus
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ContentStatusDeletes') IS NOT NULL
DROP TABLE #ContentStatusDeletes

CREATE TABLE #ContentStatusDeletes 
	(ContentStatusID int)
	
SET @SQL = 
'INSERT INTO 
	#ContentStatusDeletes
SELECT
	aC.ContentStatusID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus aC
EXCEPT
SELECT
	pc.ContentStatusID
FROM
	dbo.ContentStatus pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentStatusDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus aC
		INNER JOIN #ContentStatusDeletes d
			on ac.ContentStatusID = d.ContentStatusID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--Question
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#QuestionDeletes') IS NOT NULL
DROP TABLE #QuestionDeletes

CREATE TABLE #QuestionDeletes 
	(QuestionID int)
	
SET @SQL = 
'INSERT INTO
	#QuestionDeletes
SELECT
	aC.QuestionID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question aC
EXCEPT
SELECT
	pc.QuestionID
FROM
	dbo.Question pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question aC
		INNER JOIN #QuestionDeletes d
			on ac.QuestionID = d.QuestionID'
EXEC (@SQL)
END
-----------------------------------------------------------------------------
--QuestionType
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#QuestionTypeDeletes') IS NOT NULL
DROP TABLE #QuestionTypeDeletes

CREATE TABLE #QuestionTypeDeletes 
	(QuestionTypeID int)
	
SET @SQL =
'INSERT INTO
	#QuestionTypeDeletes
SELECT
	aC.QuestionTypeID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType aC
EXCEPT
SELECT
	pc.QuestionTypeID
FROM
	dbo.QuestionType pC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionTypeDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType aC
		INNER JOIN #QuestionTypeDeletes d
			on ac.QuestionTypeID = d.QuestionTypeID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--Answer
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#AnswerDeletes') IS NOT NULL
DROP TABLE #AnswerDeletes

CREATE TABLE #AnswerDeletes 
	(AnswerID int)
	
SET @SQL = 
'INSERT INTO
	#AnswerDeletes
SELECT
	aC.AnswerID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer aC
EXCEPT
SELECT
	pc.AnswerID
FROM
	dbo.Answer pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnswerDeletes) >0)
BEGIN	
	SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer aC
		INNER JOIN #AnswerDeletes d
			on ac.AnswerID = d.AnswerID'
	EXEC (@SQL)
END

-----------------------------------------------------------------------------
--ExperienceEvent
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ExperienceEventDeletes') IS NOT NULL
DROP TABLE #ExperienceEventDeletes

CREATE TABLE #ExperienceEventDeletes 
	(ExperienceEventID int)
	
SET @SQL = 
'INSERT INTO
	#ExperienceEventDeletes
SELECT
	aC.ExperienceEventID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent aC
EXCEPT
SELECT
	pc.ExperienceEventID
FROM
	dbo.ExperienceEvent pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ExperienceEventDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent aC
		INNER JOIN #ExperienceEventDeletes d
			on ac.ExperienceEventID = d.ExperienceEventID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--Locale
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
			IF	object_id('tempdb..#LocaleDeletes') IS NOT NULL
		DROP TABLE #LocaleDeletes
	
		CREATE TABLE #LocaleDeletes 
			(LocaleID int)
		
		SET @SQL = 
		'INSERT INTO #LocaleDeletes SELECT aC.LocaleID  FROM ' +
			LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale aC
		EXCEPT
		SELECT pc.LocaleID FROM dbo.Locale pC'

		EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

		IF ((SELECT COUNT(1) FROM #LocaleDeletes) >0)
		BEGIN
			SET @SQL = 	
				'DELETE ' + 
					LTRIM(RTRIM(@servername)) + '.' +  LTRIM(RTRIM(@dbname)) + '.dbo.Locale 
				FROM ' +
					LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale aC
					INNER JOIN #LocaleDeletes d
						on ac.LocaleID = d.LocaleID '
			EXEC (@SQL)
		END

-----------------------------------------------------------------------------
--CardType
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#CardTypeDeletes') IS NOT NULL
DROP TABLE #CardTypeDeletes

CREATE TABLE #CardTypeDeletes 
	(CardTypeID int)
	
SET @SQL = 
'INSERT INTO 
	#CardTypeDeletes
SELECT
	aC.CardTypeID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType aC
EXCEPT
SELECT
	pc.CardTypeID
FROM
	dbo.CardType pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CardTypeDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType aC
		INNER JOIN #CardTypeDeletes d
			on ac.CardTypeID = d.CardTypeID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--CardViewMode
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#CardViewModeDeletes') IS NOT NULL
DROP TABLE #CardViewModeDeletes

CREATE TABLE #CardViewModeDeletes 
	(CardViewModeID int)
	
SET @SQL = 
'INSERT INTO 
	#CardViewModeDeletes
SELECT
	aC.CardViewModeID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode aC
EXCEPT
SELECT
	pc.CardViewModeID
FROM
	dbo.CardViewMode pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CardViewModeDeletes) >0)
BEGIN	
SET @SQL = 
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode 
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode aC
		INNER JOIN #CardViewModeDeletes d
			on ac.CardViewModeID = d.CardViewModeID'
EXEC (@SQL)
END

-----------------------------------------------------------------------------
--Resource
-----------------------------------------------------------------------------
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
IF	object_id('tempdb..#ResourceDeletes') IS NOT NULL
DROP TABLE #ResourceDeletes

CREATE TABLE #ResourceDeletes
	(ResourceID int)
	
SET @SQL =
'INSERT INTO
	#ResourceDeletes
SELECT
	aC.ResourceID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource aC
EXCEPT
SELECT
	pc.ResourceID
FROM
	dbo.Resource pC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ResourceDeletes) >0)
BEGIN	
SET @SQL =
	'DELETE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource 
	FROM '	
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource aC
		INNER JOIN #ResourceDeletes d
			on ac.ResourceID = d.ResourceID'
EXEC (@SQL)
END

/***************************************************************************/
----------------------------------------------------------------------------
--STEP TWO: Process Inserts/Updates (in Parent-Child order)
----------------------------------------------------------------------------
-----------------------------------------------------------------------------
--UPDATE else INSERT on Primary Key
-----------------------------------------------------------------------------
/***************************************************************************/
------------------------------------------------------------
--Campaign
------------------------------------------------------------
		IF	object_id('tempdb..#CampaignInserts') IS NOT NULL
		DROP TABLE #CampaignInserts

		IF	object_id('tempdb..#CampaignUpdates') IS NOT NULL
		DROP TABLE #CampaignUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
		CREATE TABLE #CampaignInserts
		(CampaignID int)
		
		SET @SQL = 
		'INSERT INTO #CampaignInserts SELECT pC.CampaignID FROM dbo.Campaign pC
		EXCEPT SELECT ac.CampaignID FROM ' + 
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign aC'

		EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
		CREATE TABLE #CampaignUpdates (
			CampaignID int
			,CampaignDesc nvarchar(2000)
			,CampaignActiveInd bit
			,TargetPopulationDesc nvarchar(2000)
			,CampaignPeriodDesc nvarchar(100)
			,TargetProcedureName nvarchar(50)
			,AuthRequiredInd bit
			,SavingsMonthStartDate datetime
			,CampaignURL nvarchar(100)
			,CreateDate datetime
		)

		SET @SQL = 
		'INSERT INTO	
			#CampaignUpdates (
			CampaignID
			,CampaignDesc
			,CampaignActiveInd
			,TargetPopulationDesc
			,CampaignPeriodDesc
			,TargetProcedureName
			,AuthRequiredInd
			,SavingsMonthStartDate
			,CampaignURL
			,CreateDate
		)
		SELECT
			pC.CampaignID
			,pC.CampaignDesc
			,pC.CampaignActiveInd
			,pC.TargetPopulationDesc
			,pC.CampaignPeriodDesc
			,pC.TargetProcedureName
			,pC.AuthRequiredInd
			,pC.SavingsMonthStartDate
			,pC.CampaignURL
			,pC.CreateDate
		FROM
			dbo.Campaign pC
		EXCEPT
		SELECT
			aC.CampaignID
			,aC.CampaignDesc
			,aC.CampaignActiveInd
			,aC.TargetPopulationDesc
			,aC.CampaignPeriodDesc
			,aC.TargetProcedureName
			,aC.AuthRequiredInd
			,aC.SavingsMonthStartDate
			,aC.CampaignURL
			,aC.CreateDate
		FROM ' +
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign aC'
		
		EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

		IF ((SELECT COUNT(1) FROM #CampaignInserts) >0)
		BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
			DELETE 
				#CampaignUpdates
			WHERE
				CampaignID in (SELECT CampaignID from #CampaignInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
			SET @SQL = 
				'INSERT INTO ' + 
					LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign (
					CampaignID
					,CampaignDesc
					,CampaignActiveInd
					,TargetPopulationDesc
					,CampaignPeriodDesc
					,TargetProcedureName
					,AuthRequiredInd
					,SavingsMonthStartDate
					,CampaignURL
					,CreateDate
					)
				SELECT
					pC.CampaignID
					,pC.CampaignDesc
					,pC.CampaignActiveInd
					,pC.TargetPopulationDesc
					,pC.CampaignPeriodDesc
					,pC.TargetProcedureName
					,pC.AuthRequiredInd
					,pC.SavingsMonthStartDate
					,pC.CampaignURL
					,pC.CreateDate
				FROM
					dbo.Campaign pC
					INNER JOIN #CampaignInserts i
						on pC.CampaignID = i.CampaignID'

			EXEC (@SQL)

		END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CampaignUpdates) > 0)
BEGIN

SET @SQL = 
'	UPDATE ' + 
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign
	SET
		CampaignDesc = pC.CampaignDesc
		,CampaignActiveInd = pC.CampaignActiveInd
		,TargetPopulationDesc = pC.TargetPopulationDesc
		,CampaignPeriodDesc = pC.CampaignPeriodDesc
		,TargetProcedureName = pC.TargetProcedureName
		,AuthRequiredInd = pC.AuthRequiredInd
		,SavingsMonthStartDate = pC.SavingsMonthStartDate
		,CampaignURL = pC.CampaignURL
	FROM ' + 
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Campaign aC
		INNER JOIN dbo.Campaign pC
			ON ac.CampaignID = pC.CampaignID
		INNER JOIN #CampaignUpdates u
			on pC.CampaignID = u.CampaignID'

EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ContentType
------------------------------------------------------------
IF	object_id('tempdb..#ContentTypeInserts') IS NOT NULL
DROP TABLE #ContentTypeInserts

IF	object_id('tempdb..#ContentTypeUpdates') IS NOT NULL
DROP TABLE #ContentTypeUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ContentTypeInserts
	(ContentTypeID int)
		
SET @SQL = 
'INSERT INTO
	#ContentTypeInserts
SELECT pC.ContentTypeID
FROM
	dbo.ContentType pC
EXCEPT
SELECT
	ac.ContentTypeID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType aC'
 
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ContentTypeUpdates (
			ContentTypeID int
			,ContentTypeDesc nvarchar(100)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#ContentTypeUpdates(
	ContentTypeID
	,ContentTypeDesc
	,CreateDate
	)
SELECT
	pC.ContentTypeID
	,pC.ContentTypeDesc
	,pC.CreateDate
FROM
	dbo.ContentType pC
EXCEPT
SELECT
	aC.ContentTypeID
	,aC.ContentTypeDesc
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType aC'
	
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTypeInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ContentTypeUpdates
	WHERE
		ContentTypeID in (SELECT ContentTypeID from #ContentTypeInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------
	SET @SQL =	
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType (
			ContentTypeID
			,ContentTypeDesc
			,CreateDate
		)
	SELECT
		pC.ContentTypeID
		,pC.ContentTypeDesc
		,pC.CreateDate
	FROM
		dbo.ContentType pC
		INNER JOIN #ContentTypeInserts i
			on pC.ContentTypeID = i.ContentTypeID'		
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentTypeUpdates) > 0)
BEGIN
	SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType
	SET
		ContentTypeID = pC.ContentTypeID
		,ContentTypeDesc = pC.ContentTypeDesc
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentType aC
		INNER JOIN dbo.ContentType pC
			ON ac.ContentTypeID = pC.ContentTypeID
		INNER JOIN #ContentTypeUpdates u
			on pC.ContentTypeID = u.ContentTypeID'
		
	EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ContentStatus
------------------------------------------------------------
IF	object_id('tempdb..#ContentStatusInserts') IS NOT NULL
DROP TABLE #ContentStatusInserts

IF	object_id('tempdb..#ContentStatusUpdates') IS NOT NULL
DROP TABLE #ContentStatusUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ContentStatusInserts
	(ContentStatusID int)
	
SET @SQL = 
'INSERT INTO
	#ContentStatusInserts
SELECT
	pC.ContentStatusID
FROM
	dbo.ContentStatus pC
EXCEPT
SELECT
	ac.ContentStatusID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus aC
	--processing.VanessaTempdb.dbo.ContentStatus aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ContentStatusUpdates (
			ContentStatusID int
			,ContentStatusDesc nvarchar(100)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#ContentStatusUpdates(
	ContentStatusID
	,ContentStatusDesc
	,CreateDate
	)
SELECT
	pC.ContentStatusID
	,pC.ContentStatusDesc
	,pC.CreateDate
FROM
	dbo.ContentStatus pC
EXCEPT
SELECT
	aC.ContentStatusID
	,aC.ContentStatusDesc
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus aC
	--processing.VanessaTempdb.dbo.ContentStatus aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentStatusInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ContentStatusUpdates
	WHERE
		ContentStatusID in (SELECT ContentStatusID from #ContentStatusInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------
SET @SQL =	
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus (
			ContentStatusID
			,ContentStatusDesc
			,CreateDate
		)
	SELECT
		pC.ContentStatusID
		,pC.ContentStatusDesc
		,pC.CreateDate
	FROM
		dbo.ContentStatus pC
		INNER JOIN #ContentStatusInserts i
			on pC.ContentStatusID = i.ContentStatusID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentStatusUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus
	SET
		ContentStatusID = pC.ContentStatusID
		,ContentStatusDesc = pC.ContentStatusDesc
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentStatus aC
		INNER JOIN dbo.ContentStatus pC
			ON ac.ContentStatusID = pC.ContentStatusID
		INNER JOIN #ContentStatusUpdates u
			on pC.ContentStatusID = u.ContentStatusID'
EXEC (@SQL)
END
/***************************************************************************/

/***************************************************************************/
------------------------------------------------------------
--Content
------------------------------------------------------------
IF	object_id('tempdb..#ContentInserts') IS NOT NULL
DROP TABLE #ContentInserts

IF	object_id('tempdb..#ContentUpdates') IS NOT NULL
DROP TABLE #ContentUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ContentInserts
	(ContentID int)
	
SET @SQL = 
'INSERT INTO
	#ContentInserts
SELECT
	pC.ContentID 
FROM
	dbo.Content pC
EXCEPT
SELECT
	ac.ContentID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content aC'

EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ContentUpdates (
			ContentID int
			,ContentTypeID int
			--,ContentTitle nvarchar(50)
			--,ContentDesc nvarchar(2000)
			,ContentSourceDesc nvarchar(100)
			,ContentImageFileName nvarchar(100)
			,ContentFileLocationDesc nvarchar(100)
			,ContentPointsCount int
			,ContentDurationSecondsCount int
			--,ContentCaptionText nvarchar(250)
			,ContentName nvarchar(100)
			,IntroContentInd bit
			,ContentURL nvarchar(100)
			,ContentPhoneNum nvarchar(50)
			,AccumulatorsInd bit
			,CreateDate datetime
			)
			
SET @SQL =
'INSERT INTO	
	#ContentUpdates (
	ContentID
	,ContentTypeID
	,ContentSourceDesc
	,ContentImageFileName
	,ContentFileLocationDesc
	,ContentPointsCount
	,ContentDurationSecondsCount
	,ContentName
	,IntroContentInd
	,ContentURL
	,ContentPhoneNum
	,AccumulatorsInd
	,CreateDate)
SELECT
	pC.ContentID
	,pC.ContentTypeID
	,pC.ContentSourceDesc
	,pC.ContentImageFileName
	,pC.ContentFileLocationDesc
	,pC.ContentPointsCount
	,pC.ContentDurationSecondsCount
	,pC.ContentName
	,pC.IntroContentInd
	,pC.ContentURL
	,pC.ContentPhoneNum
	,pC.AccumulatorsInd
	,pC.CreateDate
FROM
	dbo.Content pC
EXCEPT
SELECT
	aC.ContentID
	,aC.ContentTypeID
	,aC.ContentSourceDesc
	,aC.ContentImageFileName
	,aC.ContentFileLocationDesc
	,aC.ContentPointsCount
	,aC.ContentDurationSecondsCount
	,aC.ContentName
	,aC.IntroContentInd
	,aC.ContentURL
	,aC.ContentPhoneNum
	,aC.AccumulatorsInd
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content aC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ContentUpdates
	WHERE
		ContentID in (SELECT ContentID from #ContentInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content (
			ContentID
			,ContentTypeID
			,ContentSourceDesc
			,ContentImageFileName
			,ContentFileLocationDesc
			,ContentPointsCount
			,ContentDurationSecondsCount
			,ContentName
			,IntroContentInd
			,ContentURL
			,ContentPhoneNum
			,AccumulatorsInd
			,CreateDate
		)
	SELECT
		pC.ContentID
		,pC.ContentTypeID
		,pC.ContentSourceDesc
		,pC.ContentImageFileName
		,pC.ContentFileLocationDesc
		,pC.ContentPointsCount
		,pC.ContentDurationSecondsCount
		,pC.ContentName
		,pC.IntroContentInd
		,pC.ContentURL
		,pC.ContentPhoneNum
		,pC.AccumulatorsInd
		,pC.CreateDate
	FROM
		dbo.Content pC
		INNER JOIN #ContentInserts i
			on pC.ContentID = i.ContentID'
	
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content
	SET
		ContentID = pC.ContentID
		,ContentTypeID = pC.ContentTypeID
		,ContentSourceDesc = pC.ContentSourceDesc
		,ContentImageFileName = pC.ContentImageFileName
		,ContentFileLocationDesc = pC.ContentFileLocationDesc
		,ContentPointsCount = pC.ContentPointsCount
		,ContentDurationSecondsCount = pC.ContentDurationSecondsCount
		,ContentName = pC.ContentName
		,IntroContentInd = pC.IntroContentInd
		,ContentURL = pC.ContentURL
		,ContentPhoneNum = pC.ContentPhoneNum
		,AccumulatorsInd = pC.AccumulatorsInd
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Content aC
		INNER JOIN dbo.Content pC
			ON ac.ContentID = pC.ContentID
		INNER JOIN #ContentUpdates u
			on pC.ContentID = u.ContentID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--QuestionType
------------------------------------------------------------

IF	object_id('tempdb..#QuestionTypeInserts') IS NOT NULL
DROP TABLE #QuestionTypeInserts

IF	object_id('tempdb..#QuestionTypeUpdates') IS NOT NULL
DROP TABLE #QuestionTypeUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #QuestionTypeInserts
	(QuestionTypeID int)
	
SET @SQL = 
'INSERT INTO
	#QuestionTypeInserts
SELECT
	pC.QuestionTypeID
FROM
	dbo.QuestionType pC
EXCEPT
SELECT
	ac.QuestionTypeID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType aC'
 
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #QuestionTypeUpdates (
			QuestionTypeID int
			,QuestionTypeDesc nvarchar(100)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#QuestionTypeUpdates (
	QuestionTypeID
	,QuestionTypeDesc
	,CreateDate)
SELECT
	pC.QuestionTypeID
	,pC.QuestionTypeDesc
	,pC.CreateDate
FROM
	dbo.QuestionType pC
EXCEPT
SELECT
	aC.QuestionTypeID
	,aC.QuestionTypeDesc
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType aC'
 
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionTypeInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
	DELETE 
		#QuestionTypeUpdates
	WHERE
		QuestionTypeID in (SELECT QuestionTypeID from #QuestionTypeInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType (
			QuestionTypeID
			,QuestionTypeDesc
			,CreateDate
		)
	SELECT
		pC.QuestionTypeID
		,pC.QuestionTypeDesc
		,pC.CreateDate
	FROM
		dbo.QuestionType pC
		INNER JOIN #QuestionTypeInserts i
			on pC.QuestionTypeID = i.QuestionTypeID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionTypeUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType
	SET
		QuestionTypeID = pC.QuestionTypeID
		,QuestionTypeDesc = pC.QuestionTypeDesc
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionType aC
		INNER JOIN dbo.QuestionType pC
			ON ac.QuestionTypeID = pC.QuestionTypeID
		INNER JOIN #QuestionTypeUpdates u
			on pC.QuestionTypeID = u.QuestionTypeID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--CardType
------------------------------------------------------------
IF	object_id('tempdb..#CardTypeInserts') IS NOT NULL
DROP TABLE #CardTypeInserts

IF	object_id('tempdb..#CardTypeUpdates') IS NOT NULL
DROP TABLE #CardTypeUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #CardTypeInserts
	(CardTypeID int)
	
SET @SQL = 
'INSERT INTO
	#CardTypeInserts
SELECT
	pC.CardTypeID
FROM
	dbo.CardType pC
EXCEPT
SELECT
	ac.CardTypeID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType aC
	--processing.VanessaTempdb.dbo.CardType aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #CardTypeUpdates (
			CardTypeID int
			,CardTypeDesc nvarchar(250)
			,InsuranceTypeID int
			,CardTypeFileName nvarchar(100)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#CardTypeUpdates(
	CardTypeID
	,CardTypeDesc
	,InsuranceTypeID
	,CardTypeFileName
	,CreateDate
	)
SELECT
	pC.CardTypeID
	,pC.CardTypeDesc
	,pC.InsuranceTypeID
	,pC.CardTypeFileName
	,pC.CreateDate
FROM
	dbo.CardType pC
EXCEPT
SELECT
	aC.CardTypeID
	,aC.CardTypeDesc
	,aC.InsuranceTypeID
	,aC.CardTypeFileName	
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CardTypeInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#CardTypeUpdates
	WHERE
		CardTypeID in (SELECT CardTypeID from #CardTypeInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------
SET @SQL =	
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType (
			CardTypeID
			,CardTypeDesc
			,InsuranceTypeID
			,CardTypeFileName
			,CreateDate
		)
	SELECT
		pC.CardTypeID
		,pC.CardTypeDesc
		,pC.InsuranceTypeID
		,pC.CardTypeFileName
		,pC.CreateDate
	FROM
		dbo.CardType pC
		INNER JOIN #CardTypeInserts i
			on pC.CardTypeID = i.CardTypeID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CardTypeUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType
	SET
		CardTypeID = pC.CardTypeID
		,CardTypeDesc = pC.CardTypeDesc
		,InsuranceTypeID = pC.InsuranceTypeID
		,CardTypeFileName = pC.CardTypeFileName
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardType aC
		INNER JOIN dbo.CardType pC
			ON ac.CardTypeID = pC.CardTypeID
		INNER JOIN #CardTypeUpdates u
			on pC.CardTypeID = u.CardTypeID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--CardViewMode
------------------------------------------------------------
IF	object_id('tempdb..#CardViewModeInserts') IS NOT NULL
DROP TABLE #CardViewModeInserts

IF	object_id('tempdb..#CardViewModeUpdates') IS NOT NULL
DROP TABLE #CardViewModeUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #CardViewModeInserts
	(CardViewModeID int)
	
SET @SQL = 
'INSERT INTO
	#CardViewModeInserts
SELECT
	pC.CardViewModeID
FROM
	dbo.CardViewMode pC
EXCEPT
SELECT
	ac.CardViewModeID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #CardViewModeUpdates (
			CardViewModeID int
			,CardViewModeName nvarchar(100)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#CardViewModeUpdates(
	CardViewModeID
	,CardViewModeName
	,CreateDate
	)
SELECT
	pC.CardViewModeID
	,pC.CardViewModeName
	,pC.CreateDate
FROM
	dbo.CardViewMode pC
EXCEPT
SELECT
	aC.CardViewModeID
	,aC.CardViewModeName
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CardViewModeInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#CardViewModeUpdates
	WHERE
		CardViewModeID in (SELECT CardViewModeID from #CardViewModeInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------
SET @SQL =	
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode (
			CardViewModeID
			,CardViewModeName
			,CreateDate
		)
	SELECT
		pC.CardViewModeID
		,pC.CardViewModeName
		,pC.CreateDate
	FROM
		dbo.CardViewMode pC
		INNER JOIN #CardViewModeInserts i
			on pC.CardViewModeID = i.CardViewModeID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CardViewModeUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode
	SET
		CardViewModeID = pC.CardViewModeID
		,CardViewModeName = pC.CardViewModeName
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardViewMode aC
		INNER JOIN dbo.CardViewMode pC
			ON ac.CardViewModeID = pC.CardViewModeID
		INNER JOIN #CardViewModeUpdates u
			on pC.CardViewModeID = u.CardViewModeID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Locale
------------------------------------------------------------
		IF	object_id('tempdb..#LocaleInserts') IS NOT NULL
		DROP TABLE #LocaleInserts

		IF	object_id('tempdb..#LocaleUpdates') IS NOT NULL
		DROP TABLE #LocaleUpdates
		
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
		CREATE TABLE #LocaleInserts
		(LocaleID int)
		
		SET @SQL = 
		'INSERT INTO #LocaleInserts SELECT pC.LocaleID FROM dbo.Locale pC
		EXCEPT SELECT ac.LocaleID FROM ' + 
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale aC'

		EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
		CREATE TABLE #LocaleUpdates (
			LocaleID int
			,LocaleCode nvarchar(10)
			,ISOCountryCode nvarchar(10)
			,ISOLanguageCode nvarchar(10)
			,LocaleDesc nvarchar(100)
			,DateFormatDesc nvarchar(100)
			,CreateDate datetime
		)

		SET @SQL = 
		'INSERT INTO	
			#LocaleUpdates (
			LocaleID
			,LocaleCode
			,ISOCountryCode
			,ISOLanguageCode
			,LocaleDesc
			,DateFormatDesc
			,CreateDate
		)
		SELECT
			pC.LocaleID
			,pC.LocaleCode
			,pC.ISOCountryCode
			,pC.ISOLanguageCode
			,pC.LocaleDesc
			,pC.DateFormatDesc
			,pC.CreateDate
		FROM
			dbo.Locale pC
		EXCEPT
		SELECT
			aC.LocaleID
			,aC.LocaleCode
			,aC.ISOCountryCode
			,aC.ISOLanguageCode
			,aC.LocaleDesc
			,aC.DateFormatDesc
			,aC.CreateDate
		FROM ' +
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale aC'
		
		EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

		IF ((SELECT COUNT(1) FROM #LocaleInserts) >0)
		BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
			DELETE 
				#LocaleUpdates
			WHERE
				LocaleID in (SELECT LocaleID from #LocaleInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
			SET @SQL = 
				'INSERT INTO ' + 
					LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale (
					LocaleID
					,LocaleCode
					,ISOCountryCode
					,ISOLanguageCode
					,LocaleDesc
					,DateFormatDesc
					,CreateDate
					)
				SELECT
					pC.LocaleID
					,pC.LocaleCode
					,pC.ISOCountryCode
					,pC.ISOLanguageCode
					,pC.LocaleDesc
					,pC.DateFormatDesc
					,pC.CreateDate
				FROM
					dbo.Locale pC
					INNER JOIN #LocaleInserts i
						on pC.LocaleID = i.LocaleID'

			EXEC (@SQL)

		END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #LocaleUpdates) > 0)
BEGIN

SET @SQL = 
'	UPDATE ' + 
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale
	SET
		LocaleCode = pC.LocaleCode
		,ISOCountryCode = pC.ISOCountryCode
		,ISOLanguageCode = pC.ISOLanguageCode
		,LocaleDesc = pC.LocaleDesc
		,DateFormatDesc = pC.DateFormatDesc
		,CreateDate = pC.CreateDate
	FROM ' + 
		LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Locale aC
		INNER JOIN dbo.Locale pC
			ON ac.LocaleID = pC.LocaleID
		INNER JOIN #LocaleUpdates u
			on pC.LocaleID = u.LocaleID'

EXEC (@SQL)
END
/***************************************************************************/

/***************************************************************************/
------------------------------------------------------------
--ContentDisplayRule --7:25PM
------------------------------------------------------------
IF	object_id('tempdb..#ContentDisplayRuleInserts') IS NOT NULL
DROP TABLE #ContentDisplayRuleInserts

IF	object_id('tempdb..#ContentDisplayRuleUpdates') IS NOT NULL
DROP TABLE #ContentDisplayRuleUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ContentDisplayRuleInserts
	(ContentDisplayRuleID int)
	
SET @SQL =
'INSERT INTO
	#ContentDisplayRuleInserts
SELECT
	pC.ContentDisplayRuleID
FROM
	dbo.ContentDisplayRule pC
EXCEPT
SELECT
	ac.ContentDisplayRuleID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ContentDisplayRuleUpdates (
			ContentDisplayRuleID int
			,ContentDisplayRuleDesc nvarchar(100)
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#ContentDisplayRuleUpdates (
	ContentDisplayRuleID
	,ContentDisplayRuleDesc
	,CreateDate
	)
SELECT
	pC.ContentDisplayRuleID
	,pC.ContentDisplayRuleDesc
	,pC.CreateDate
FROM
	dbo.ContentDisplayRule pC
EXCEPT
SELECT
	aC.ContentDisplayRuleID
	,aC.ContentDisplayRuleDesc
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentDisplayRuleInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ContentDisplayRuleUpdates
	WHERE
		ContentDisplayRuleID in (SELECT ContentDisplayRuleID from #ContentDisplayRuleInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL = 
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule (
			ContentDisplayRuleID
			,ContentDisplayRuleDesc
			,CreateDate
		)
	SELECT
		pC.ContentDisplayRuleID
		,pC.ContentDisplayRuleDesc
		,pC.CreateDate
	FROM
		dbo.ContentDisplayRule pC
		INNER JOIN #ContentDisplayRuleInserts i
			on pC.ContentDisplayRuleID = i.ContentDisplayRuleID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentDisplayRuleUpdates) > 0)
BEGIN
	SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule
	SET
		ContentDisplayRuleID = pC.ContentDisplayRuleID
		,ContentDisplayRuleDesc = pC.ContentDisplayRuleDesc
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentDisplayRule aC
		INNER JOIN dbo.ContentDisplayRule pC
			ON ac.ContentDisplayRuleID = pC.ContentDisplayRuleID
		INNER JOIN #ContentDisplayRuleUpdates u
			on pC.ContentDisplayRuleID = u.ContentDisplayRuleID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Answer  7:29pm
------------------------------------------------------------
IF	object_id('tempdb..#AnswerInserts') IS NOT NULL
DROP TABLE #AnswerInserts

IF	object_id('tempdb..#AnswerUpdates') IS NOT NULL
DROP TABLE #AnswerUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #AnswerInserts
	(AnswerID int)
	
SET @SQL =
'INSERT INTO
	#AnswerInserts
SELECT
	pC.AnswerID
FROM
	dbo.Answer pC
EXCEPT
SELECT
	ac.AnswerID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #AnswerUpdates (
			AnswerID int
		--	,AnswerText nvarchar(250)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#AnswerUpdates (
	AnswerID
	,CreateDate
	)
SELECT
	pC.AnswerID
	,pC.CreateDate
FROM
	dbo.Answer pC
EXCEPT
SELECT
	aC.AnswerID
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnswerInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#AnswerUpdates
	WHERE
		AnswerID in (SELECT AnswerID from #AnswerInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	SET @SQL = 
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer (
			AnswerID
			,CreateDate
		)
	SELECT
		pC.AnswerID
		,pC.CreateDate
	FROM
		dbo.Answer pC
		INNER JOIN #AnswerInserts i
			on pC.AnswerID = i.AnswerID'
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #AnswerUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer
	SET
		AnswerID = pC.AnswerID
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Answer aC
		INNER JOIN dbo.Answer pC
			ON ac.AnswerID = pC.AnswerID
		INNER JOIN #AnswerUpdates u
			on pC.AnswerID = u.AnswerID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Question
------------------------------------------------------------

IF	object_id('tempdb..#QuestionInserts') IS NOT NULL
DROP TABLE #QuestionInserts

IF	object_id('tempdb..#QuestionUpdates') IS NOT NULL
DROP TABLE #QuestionUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #QuestionInserts
	(QuestionID int)
	
SET @SQL =
'INSERT INTO
	#QuestionInserts
SELECT
	pC.QuestionID
FROM
	dbo.Question pC
EXCEPT
SELECT
	ac.QuestionID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #QuestionUpdates (
			QuestionID int
			,QuestionTypeID int
			--,QuestionText nvarchar(250)
			,ExpectedAnswerDatatypeDesc nvarchar(100)
			,ExpectedAnswerValueRangeDesc nvarchar(100)
			,ScoredQuestionInd bit
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#QuestionUpdates (
	QuestionID
	,QuestionTypeID
	,ExpectedAnswerDatatypeDesc
	,ExpectedAnswerValueRangeDesc
	,ScoredQuestionInd
	,CreateDate
	)
SELECT
	pC.QuestionID
	,pC.QuestionTypeID
	,pC.ExpectedAnswerDatatypeDesc
	,pC.ExpectedAnswerValueRangeDesc
	,pC.ScoredQuestionInd
	,pC.CreateDate
FROM
	dbo.Question pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.QuestionTypeID
	,aC.ExpectedAnswerDatatypeDesc
	,aC.ExpectedAnswerValueRangeDesc
	,aC.ScoredQuestionInd
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#QuestionUpdates
	WHERE
		QuestionID in (SELECT QuestionID from #QuestionInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL = 
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question (
			QuestionID
			,QuestionTypeID
			,ExpectedAnswerDatatypeDesc
			,ExpectedAnswerValueRangeDesc
			,ScoredQuestionInd
			,CreateDate
		)
	SELECT
		pC.QuestionID
		,pC.QuestionTypeID
		,pC.ExpectedAnswerDatatypeDesc
		,pC.ExpectedAnswerValueRangeDesc
		,pC.ScoredQuestionInd
		,pC.CreateDate
	FROM
		dbo.Question pC
		INNER JOIN #QuestionInserts i
			on pC.QuestionID = i.QuestionID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question
	SET
		QuestionID = pC.QuestionID
		,QuestionTypeID = pC.QuestionTypeID
		,ExpectedAnswerDatatypeDesc = pC.ExpectedAnswerDatatypeDesc
		,ExpectedAnswerValueRangeDesc = pC.ExpectedAnswerValueRangeDesc
		,ScoredQuestionInd = pC.ScoredQuestionInd
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Question aC
		INNER JOIN dbo.Question pC
			ON ac.QuestionID = pC.QuestionID
		INNER JOIN #QuestionUpdates u
			on pC.QuestionID = u.QuestionID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ExperienceEvent
------------------------------------------------------------

IF	object_id('tempdb..#ExperienceEventInserts') IS NOT NULL
DROP TABLE #ExperienceEventInserts

IF	object_id('tempdb..#ExperienceEventUpdates') IS NOT NULL
DROP TABLE #ExperienceEventUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ExperienceEventInserts
	(ExperienceEventID int)
	
SET @SQL = 
'INSERT INTO
	#ExperienceEventInserts
SELECT
	pC.ExperienceEventID
FROM
	dbo.ExperienceEvent pC
EXCEPT
SELECT
	ac.ExperienceEventID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ExperienceEventUpdates (
			ExperienceEventID int
			,ExperienceEventDesc nvarchar(100)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#ExperienceEventUpdates (
	ExperienceEventID
	,ExperienceEventDesc
	,CreateDate
	)
SELECT
	pC.ExperienceEventID
	,pC.ExperienceEventDesc
	,pC.CreateDate
FROM
	dbo.ExperienceEvent pC
EXCEPT
SELECT
	aC.ExperienceEventID
	,aC.ExperienceEventDesc
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ExperienceEventInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ExperienceEventUpdates
	WHERE
		ExperienceEventID in (SELECT ExperienceEventID from #ExperienceEventInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent (
			ExperienceEventID
			,ExperienceEventDesc
			,CreateDate
		)
	SELECT
		pC.ExperienceEventID
		,pC.ExperienceEventDesc
		,pC.CreateDate
	FROM
		dbo.ExperienceEvent pC
		INNER JOIN #ExperienceEventInserts i
			on pC.ExperienceEventID = i.ExperienceEventID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ExperienceEventUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent
	SET
		ExperienceEventID = pC.ExperienceEventID
		,ExperienceEventDesc = pC.ExperienceEventDesc
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ExperienceEvent aC
		INNER JOIN dbo.ExperienceEvent pC
			ON ac.ExperienceEventID = pC.ExperienceEventID
		INNER JOIN #ExperienceEventUpdates u
			on pC.ExperienceEventID = u.ExperienceEventID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Animation
------------------------------------------------------------
IF	object_id('tempdb..#AnimationInserts') IS NOT NULL
DROP TABLE #AnimationInserts

IF	object_id('tempdb..#AnimationUpdates') IS NOT NULL
DROP TABLE #AnimationUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #AnimationInserts
	(AnimationID int)
	
SET @SQL =
'INSERT INTO
	#AnimationInserts
SELECT
	pC.AnimationID
FROM
	dbo.Animation pC
EXCEPT
SELECT
	ac.AnimationID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #AnimationUpdates (
			AnimationID int
			,AnimationScriptFileName nvarchar(50)
			,InteractiveAnimationInd bit
			,AnimationDataProcName nvarchar(50)
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#AnimationUpdates (
	AnimationID
	,AnimationScriptFileName
	,InteractiveAnimationInd
	,AnimationDataProcName
	,CreateDate
	)
SELECT
	pC.AnimationID
	,pC.AnimationScriptFileName
	,pC.InteractiveAnimationInd
	,pC.AnimationDataProcName
	,pC.CreateDate
FROM
	dbo.Animation pC
EXCEPT
SELECT
	aC.AnimationID
	,aC.AnimationScriptFileName
	,aC.InteractiveAnimationInd
	,aC.AnimationDataProcName
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnimationInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#AnimationUpdates
	WHERE
		AnimationID in (SELECT AnimationID from #AnimationInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation (
			AnimationID
			,AnimationScriptFileName
			,InteractiveAnimationInd
			,AnimationDataProcName
			,CreateDate
		)
	SELECT
		pC.AnimationID
		,pC.AnimationScriptFileName
		,pC.InteractiveAnimationInd
		,pC.AnimationDataProcName
		,pC.CreateDate
	FROM
		dbo.Animation pC
		INNER JOIN #AnimationInserts i
			on pC.AnimationID = i.AnimationID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #AnimationUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation
	SET
		AnimationID = pC.AnimationID
		,AnimationScriptFileName = pC.AnimationScriptFileName
		,InteractiveAnimationInd = pC.InteractiveAnimationInd
		,AnimationDataProcName = pC.AnimationDataProcName
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Animation aC
		INNER JOIN dbo.Animation pC
			ON ac.AnimationID = pC.AnimationID
		INNER JOIN #AnimationUpdates u
			on pC.AnimationID = u.AnimationID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Video
------------------------------------------------------------
IF	object_id('tempdb..#VideoInserts') IS NOT NULL
DROP TABLE #VideoInserts

IF	object_id('tempdb..#VideoUpdates') IS NOT NULL
DROP TABLE #VideoUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #VideoInserts
	(VideoID int)
	
SET @SQL =
'INSERT INTO
	#VideoInserts
SELECT
	pC.VideoID
FROM
	dbo.Video pC
EXCEPT
SELECT
	ac.VideoID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #VideoUpdates (
			VideoID int
			,VideoFileName nvarchar(50)
			,PersonalizedVideoInd bit
			,VideoDataProcName nvarchar(50)
			,CreateDate datetime
		)

SET @SQL = 
'SELECT
	pC.VideoID
	,pC.VideoFileName
	,pC.PersonalizedVideoInd
	,pC.VideoDataProcName
	,pC.CreateDate
INTO	
	#VideoUpdates
FROM
	dbo.Video pC
EXCEPT
SELECT
	aC.VideoID
	,aC.VideoFileName
	,aC.PersonalizedVideoInd
	,aC.VideoDataProcName
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #VideoInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#VideoUpdates
	WHERE
		VideoID in (SELECT VideoID from #VideoInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video (
			VideoID
			,VideoFileName
			,PersonalizedVideoInd
			,VideoDataProcName
			,CreateDate
		)
	SELECT
		pC.VideoID
		,pC.VideoFileName
		,pC.PersonalizedVideoInd
		,pC.VideoDataProcName
		,pC.CreateDate
	FROM
		dbo.Video pC
		INNER JOIN #VideoInserts i
			on pC.VideoID = i.VideoID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #VideoUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video
	SET
		VideoID = pC.VideoID
		,VideoFileName = pC.VideoFileName
		,PersonalizedVideoInd = pC.PersonalizedVideoInd
		,VideoDataProcName = pC.VideoDataProcName
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Video aC
		INNER JOIN dbo.Video pC
			ON ac.VideoID = pC.VideoID
		INNER JOIN #VideoUpdates u
			on pC.VideoID = u.VideoID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Survey
------------------------------------------------------------
IF	object_id('tempdb..#SurveyInserts') IS NOT NULL
DROP TABLE #SurveyInserts

IF	object_id('tempdb..#SurveyUpdates') IS NOT NULL
DROP TABLE #SurveyUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #SurveyInserts
	(SurveyID int)

SET @SQL =
'INSERT INTO
	#SurveyInserts
SELECT
	pC.SurveyID
FROM
	dbo.Survey pC
EXCEPT
SELECT
	ac.SurveyID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #SurveyUpdates (
			SurveyID int
			,SurveyPassCount int
			,EmbeddedSurveyInd bit
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#SurveyUpdates (
	SurveyID
	,SurveyPassCount
	,EmbeddedSurveyInd
	,CreateDate
)
SELECT
	pC.SurveyID
	,pC.SurveyPassCount
	,pC.EmbeddedSurveyInd
	,pC.CreateDate
FROM
	dbo.Survey pC
EXCEPT
SELECT
	aC.SurveyID
	,aC.SurveyPassCount
	,aC.EmbeddedSurveyInd
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #SurveyInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#SurveyUpdates
	WHERE
		SurveyID in (SELECT SurveyID from #SurveyInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey (
			SurveyID
			,SurveyPassCount
			,EmbeddedSurveyInd
			,CreateDate
		)
	SELECT
		pC.SurveyID
		,pC.SurveyPassCount
		,pC.EmbeddedSurveyInd
		,pC.CreateDate
	FROM
		dbo.Survey pC
		INNER JOIN #SurveyInserts i
			on pC.SurveyID = i.SurveyID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #SurveyUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey
	SET
		SurveyID = pC.SurveyID
		,SurveyPassCount = pC.SurveyPassCount
		,EmbeddedSurveyInd = pC.EmbeddedSurveyInd
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Survey aC
		INNER JOIN dbo.Survey pC
			ON ac.SurveyID = pC.SurveyID
		INNER JOIN #SurveyUpdates u
			on pC.SurveyID = u.SurveyID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Resource
------------------------------------------------------------
IF	object_id('tempdb..#ResourceInserts') IS NOT NULL
DROP TABLE #ResourceInserts

IF	object_id('tempdb..#ResourceUpdates') IS NOT NULL
DROP TABLE #ResourceUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ResourceInserts
	(ResourceID int)

SET @SQL =
'INSERT INTO
	#ResourceInserts
SELECT
	pC.ResourceID
FROM
	dbo.Resource pC
EXCEPT
SELECT
	ac.ResourceID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ResourceUpdates (
			ResourceID int
			,ResourceName nvarchar(50)
			,ResourceIconFileName nvarchar(500)
			,ResourceVideoImageURL nvarchar(100)
			,ResourceDesc nvarchar(100)
			,ResourcePhoneNum nvarchar(20)
			,ResourceURL nvarchar(100)
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#ResourceUpdates (
	ResourceID
	,ResourceName
	,ResourceIconFileName
	,ResourceVideoImageURL
	,ResourceDesc
	,ResourcePhoneNum
	,ResourceURL
	,CreateDate
	)
SELECT
	pC.ResourceID
	,pC.ResourceName
	,pC.ResourceIconFileName
	,pC.ResourceVideoImageURL
	,pC.ResourceDesc
	,pC.ResourcePhoneNum
	,pC.ResourceURL
	,pC.CreateDate
FROM
	dbo.Resource pC
EXCEPT
SELECT
	aC.ResourceID
	,aC.ResourceName
	,aC.ResourceIconFileName
	,aC.ResourceVideoImageURL
	,aC.ResourceDesc
	,aC.ResourcePhoneNum
	,aC.ResourceURL
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ResourceInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ResourceUpdates
	WHERE
		ResourceID in (SELECT ResourceID from #ResourceInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource (
			ResourceID
			,ResourceName
			,ResourceIconFileName
			,ResourceVideoImageURL
			,ResourceDesc
			,ResourcePhoneNum
			,ResourceURL
			,CreateDate
		)
	SELECT
		pC.ResourceID
		,pC.ResourceName
		,pC.ResourceIconFileName
		,pC.ResourceVideoImageURL
		,pC.ResourceDesc
		,pC.ResourcePhoneNum
		,pC.ResourceURL
		,pC.CreateDate
	FROM
		dbo.Resource pC
		INNER JOIN #ResourceInserts i
			on pC.ResourceID = i.ResourceID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ResourceUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource
	SET
		ResourceID = pC.ResourceID
		,ResourceName = pC.ResourceName
		,ResourceIconFileName = pC.ResourceIconFileName
		,ResourceVideoImageURL = pC.ResourceVideoImageURL
		,ResourceDesc = pC.ResourceDesc
		,ResourcePhoneNum = pC.ResourcePhoneNum
		,ResourceURL = pC.ResourceURL
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.Resource aC
		INNER JOIN dbo.Resource pC
			ON ac.ResourceID = pC.ResourceID
		INNER JOIN #ResourceUpdates u
			on pC.ResourceID = u.ResourceID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--RelatedContent
------------------------------------------------------------

IF	object_id('tempdb..#RelatedContentInserts') IS NOT NULL
DROP TABLE #RelatedContentInserts

IF	object_id('tempdb..#RelatedContentUpdates') IS NOT NULL
DROP TABLE #RelatedContentUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #RelatedContentInserts
	(ParentContentID int
	 ,RelatedContentID int)

SET @SQL =
'INSERT INTO
	#RelatedContentInserts
SELECT
	pC.ParentContentID
	,pC.RelatedContentID
FROM
	dbo.RelatedContent pC
EXCEPT
SELECT
	aC.ParentContentID
	,aC.RelatedContentID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #RelatedContentUpdates (
			ParentContentID int
			,RelatedContentID int
			,RelatedContentDisplayRuleID int
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#RelatedContentUpdates (
	ParentContentID
	,RelatedContentID
	,RelatedContentDisplayRuleID
	,CreateDate
	)
SELECT
	pC.ParentContentID
	,pC.RelatedContentID
	,pC.RelatedContentDisplayRuleID
	,pC.CreateDate
FROM
	dbo.RelatedContent pC
EXCEPT
SELECT
	aC.ParentContentID
	,aC.RelatedContentID
	,aC.RelatedContentDisplayRuleID
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #RelatedContentInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#RelatedContentUpdates
	FROM
		#RelatedContentUpdates u
		INNER JOIN #RelatedContentInserts i
			ON u.ParentContentID = i.ParentContentID
			AND u.RelatedContentID = i.RelatedContentID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent (
			ParentContentID
			,RelatedContentID
			,RelatedContentDisplayRuleID
			,CreateDate
		)
	SELECT
		pC.ParentContentID
		,pC.RelatedContentID
		,pC.RelatedContentDisplayRuleID
		,pC.CreateDate
	FROM
		dbo.RelatedContent pC
		INNER JOIN #RelatedContentInserts i
			on pC.RelatedContentID = i.RelatedContentID
			AND pC.ParentContentID = i.ParentContentID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #RelatedContentUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent
	SET
		ParentContentID = pC.ParentContentID
		,RelatedContentID = pC.RelatedContentID
		,RelatedContentDisplayRuleID = pC.RelatedContentDisplayRuleID
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.RelatedContent aC
		INNER JOIN dbo.RelatedContent pC
			ON ac.RelatedContentID = pC.RelatedContentID
			AND ac.ParentContentID = pC.ParentContentID
		INNER JOIN #RelatedContentUpdates u
			on pC.RelatedContentID = u.RelatedContentID
			AND pC.ParentContentID = u.ParentContentID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--SurveyQuestion
------------------------------------------------------------
IF	object_id('tempdb..#SurveyQuestionInserts') IS NOT NULL
DROP TABLE #SurveyQuestionInserts

IF	object_id('tempdb..#SurveyQuestionUpdates') IS NOT NULL
DROP TABLE #SurveyQuestionUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #SurveyQuestionInserts
	(SurveyID int
	 ,QuestionID int)

SET @SQL =
'INSERT INTO
	#SurveyQuestionInserts
SELECT
	pC.SurveyID
	,pC.QuestionID
FROM
	dbo.SurveyQuestion pC
EXCEPT
SELECT
	aC.SurveyID
	,aC.QuestionID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #SurveyQuestionUpdates (
			SurveyID int
			,QuestionID int
			,QuestionDisplayOrderNum int
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#SurveyQuestionUpdates (
	SurveyID
	,QuestionID
	,QuestionDisplayOrderNum
	,CreateDate
	)
SELECT
	pC.SurveyID
	,pC.QuestionID
	,pC.QuestionDisplayOrderNum
	,pC.CreateDate
FROM
	dbo.SurveyQuestion pC
EXCEPT
SELECT
	aC.SurveyID
	,aC.QuestionID
	,aC.QuestionDisplayOrderNum
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #SurveyQuestionInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#SurveyQuestionUpdates
	FROM
		#SurveyQuestionUpdates u
		INNER JOIN #SurveyQuestionInserts i
			ON u.SurveyID = i.SurveyID
			AND u.QuestionID = i.QuestionID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion (
			SurveyID
			,QuestionID
			,QuestionDisplayOrderNum
			,CreateDate
		)
	SELECT
		pC.SurveyID
		,pC.QuestionID
		,pC.QuestionDisplayOrderNum
		,pC.CreateDate
	FROM
		dbo.SurveyQuestion pC
		INNER JOIN #SurveyQuestionInserts i
			on pC.QuestionID = i.QuestionID
			AND pC.SurveyID = i.SurveyID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #SurveyQuestionUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion
	SET
		SurveyID = pC.SurveyID
		,QuestionID = pC.QuestionID
		,QuestionDisplayOrderNum = pC.QuestionDisplayOrderNum
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.SurveyQuestion aC
		INNER JOIN dbo.SurveyQuestion pC
			ON ac.QuestionID = pC.QuestionID
			AND ac.SurveyID = pC.SurveyID
		INNER JOIN #SurveyQuestionUpdates u
			on pC.QuestionID = u.QuestionID
			AND pC.SurveyID = u.SurveyID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--QuestionAnswer
------------------------------------------------------------
IF	object_id('tempdb..#QuestionAnswerInserts') IS NOT NULL
DROP TABLE #QuestionAnswerInserts

IF	object_id('tempdb..#QuestionAnswerUpdates') IS NOT NULL
DROP TABLE #QuestionAnswerUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #QuestionAnswerInserts
	(QuestionID int
	 ,AnswerID int)

SET @SQL =
'INSERT INTO
	#QuestionAnswerInserts
SELECT
	pC.QuestionID
	,pC.AnswerID
FROM
	dbo.QuestionAnswer pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.AnswerID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #QuestionAnswerUpdates (
			QuestionID int
			,AnswerID int
			,AnswerDisplayOrderNum int
			,CorrectAnswerInd bit
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#QuestionAnswerUpdates (
	QuestionID
	,AnswerID
	,AnswerDisplayOrderNum
	,CorrectAnswerInd
	,CreateDate
	)
SELECT
	pC.QuestionID
	,pC.AnswerID
	,pC.AnswerDisplayOrderNum
	,pC.CorrectAnswerInd
	,pC.CreateDate
FROM
	dbo.QuestionAnswer pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.AnswerID
	,aC.AnswerDisplayOrderNum
	,aC.CorrectAnswerInd
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer aC'
 EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionAnswerInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#QuestionAnswerUpdates
	FROM
		#QuestionAnswerUpdates u
		INNER JOIN #QuestionAnswerInserts i
			ON u.QuestionID = i.QuestionID
			AND u.AnswerID = i.AnswerID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------
SET @SQL =	
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer (
			QuestionID
			,AnswerID
			,AnswerDisplayOrderNum
			,CorrectAnswerInd
			,CreateDate
		)
	SELECT
		pC.QuestionID
		,pC.AnswerID
		,pC.AnswerDisplayOrderNum
		,pC.CorrectAnswerInd
		,pC.CreateDate
	FROM
		dbo.QuestionAnswer pC
		INNER JOIN #QuestionAnswerInserts i
			on pC.AnswerID = i.AnswerID
			AND pC.QuestionID = i.QuestionID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionAnswerUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer
	SET
		QuestionID = pC.QuestionID
		,AnswerID = pC.AnswerID
		,AnswerDisplayOrderNum = pC.AnswerDisplayOrderNum
		,CorrectAnswerInd = pC.CorrectAnswerInd
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionAnswer aC
		INNER JOIN dbo.QuestionAnswer pC
			ON ac.AnswerID = pC.AnswerID
			AND ac.QuestionID = pC.QuestionID
		INNER JOIN #QuestionAnswerUpdates u
			on pC.AnswerID = u.AnswerID
			AND pC.QuestionID = u.QuestionID'
EXEC (@SQL)
END
/***************************************************************************/

/***************************************************************************/
------------------------------------------------------------
--ContentTypeState
------------------------------------------------------------
IF	object_id('tempdb..#ContentTypeStateInserts') IS NOT NULL
DROP TABLE #ContentTypeStateInserts

IF	object_id('tempdb..#ContentTypeStateUpdates') IS NOT NULL
DROP TABLE #ContentTypeStateUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ContentTypeStateInserts
	(ContentTypeID int
	 ,ContentStatusID int)

SET @SQL =
'INSERT INTO
	#ContentTypeStateInserts
SELECT
	pC.ContentTypeID
	,pC.ContentStatusID
FROM
	dbo.ContentTypeState pC
EXCEPT
SELECT
	aC.ContentTypeID
	,aC.ContentStatusID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState aC
	--processing.VanessaTempdb.dbo.ContentTypeState aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ContentTypeStateUpdates (
			ContentTypeID int
			,ContentStatusID int
			,InitialStateInd bit
			,EndStateInd bit
			,ContentStatusCaptionText nvarchar(250)
			,CreateDate datetime
		)

SET @SQL = 
'INSERT INTO	
	#ContentTypeStateUpdates (
	ContentTypeID
	,ContentStatusID
	,InitialStateInd
	,EndStateInd
	,ContentStatusCaptionText
	,CreateDate
	)
SELECT
	pC.ContentTypeID
	,pC.ContentStatusID
	,pC.InitialStateInd
	,pC.EndStateInd
	,pC.ContentStatusCaptionText
	,pC.CreateDate
FROM
	dbo.ContentTypeState pC
EXCEPT
SELECT
	aC.ContentTypeID
	,aC.ContentStatusID
	,aC.InitialStateInd
	,aC.EndStateInd
	,aC.ContentStatusCaptionText
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTypeStateInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ContentTypeStateUpdates
	FROM
		#ContentTypeStateUpdates u
		INNER JOIN #ContentTypeStateInserts i
			ON u.ContentTypeID = i.ContentTypeID
			AND u.ContentStatusID = i.ContentStatusID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL = 
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState (
			ContentTypeID
			,ContentStatusID
			,InitialStateInd
			,EndStateInd
			,ContentStatusCaptionText
			,CreateDate
		)
	SELECT
		pC.ContentTypeID
		,pC.ContentStatusID
		,pC.InitialStateInd
		,pC.EndStateInd
		,pC.ContentStatusCaptionText
		,pC.CreateDate
	FROM
		dbo.ContentTypeState pC
		INNER JOIN #ContentTypeStateInserts i
			on pC.ContentStatusID = i.ContentStatusID
			AND pC.ContentTypeID = i.ContentTypeID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentTypeStateUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState
	SET
		ContentTypeID = pC.ContentTypeID
		,ContentStatusID = pC.ContentStatusID
		,InitialStateInd = pC.InitialStateInd
		,EndStateInd = pC.EndStateInd
		,ContentStatusCaptionText = pC.ContentStatusCaptionText
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTypeState aC
		INNER JOIN dbo.ContentTypeState pC
			ON ac.ContentStatusID = pC.ContentStatusID
			AND ac.ContentTypeID = pC.ContentTypeID
		INNER JOIN #ContentTypeStateUpdates u
			on pC.ContentStatusID = u.ContentStatusID
			AND pC.ContentTypeID = u.ContentTypeID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
/* 2015-08-07 AS -- Removed as this table is loaded / promoted during enrollments
------------------------------------------------------------
--UserContentPreference
------------------------------------------------------------
IF	object_id('tempdb..#UserContentPreferenceInserts') IS NOT NULL
DROP TABLE #UserContentPreferenceInserts

IF	object_id('tempdb..#UserContentPreferenceUpdates') IS NOT NULL
DROP TABLE #UserContentPreferenceUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #UserContentPreferenceInserts
	(CCHID int)

SET @SQL = 
'INSERT INTO
	#UserContentPreferenceInserts
SELECT
	pC.CCHID
FROM
	dbo.UserContentPreference pC
EXCEPT
SELECT
	ac.CCHID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #UserContentPreferenceUpdates (
			CCHID int
			,SMSInd bit
			,EmailInd bit
			,OSBasedAlertInd bit
			,LastUpdateDate datetime
			,DefaultLocaleID int
			,PreferredContactPhoneNum nvarchar(50)
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#UserContentPreferenceUpdates (
	CCHID
	,SMSInd
	,EmailInd
	,OSBasedAlertInd
	,LastUpdateDate
	,DefaultLocaleID
	,PreferredContactPhoneNum
	,CreateDate
	)
SELECT
	pC.CCHID
	,pC.SMSInd
	,pC.EmailInd
	,pC.OSBasedAlertInd
	,pC.LastUpdateDate
	,pC.DefaultLocaleID
	,pC.PreferredContactPhoneNum
	,pC.CreateDate
FROM
	dbo.UserContentPreference pC
EXCEPT
SELECT
	aC.CCHID
	,aC.SMSInd
	,aC.EmailInd
	,aC.OSBasedAlertInd
	,aC.LastUpdateDate
	,aC.DefaultLocaleID
	,aC.PreferredContactPhoneNum
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentPreferenceInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#UserContentPreferenceUpdates
	WHERE
		CCHID in (SELECT CCHID from #UserContentPreferenceInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference (
			CCHID
			,SMSInd
			,EmailInd
			,OSBasedAlertInd
			,LastUpdateDate
			,DefaultLocaleID
			,PreferredContactPhoneNum
			,CreateDate
		)
	SELECT
		pC.CCHID
		,pC.SMSInd
		,pC.EmailInd
		,pC.OSBasedAlertInd
		,pC.LastUpdateDate
		,pC.DefaultLocaleID
		,pC.PreferredContactPhoneNum
		,pC.CreateDate
	FROM
		dbo.UserContentPreference pC
		INNER JOIN #UserContentPreferenceInserts i
			on pC.CCHID = i.CCHID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #UserContentPreferenceUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference
	SET
		CCHID = pC.CCHID
		,SMSInd = pC.SMSInd
		,EmailInd = pC.EmailInd
		,OSBasedAlertInd = pC.OSBasedAlertInd
		,LastUpdateDate = pC.LastUpdateDate
		,DefaultLocaleID = pC.DefaultLocaleID
		,PreferredContactPhoneNum = ISNULL(NULLIF(aC.PreferredContactPhoneNum,''''),pC.PreferredContactPhoneNum)
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContentPreference aC
		INNER JOIN dbo.UserContentPreference pC
			ON ac.CCHID = pC.CCHID
		INNER JOIN #UserContentPreferenceUpdates u
			on pC.CCHID = u.CCHID'
EXEC (@SQL)
END
*/
/***************************************************************************/
------------------------------------------------------------
--CampaignContent
------------------------------------------------------------

IF	object_id('tempdb..#CampaignContentInserts') IS NOT NULL
DROP TABLE #CampaignContentInserts

IF	object_id('tempdb..#CampaignContentUpdates') IS NOT NULL
DROP TABLE #CampaignContentUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #CampaignContentInserts
	(CampaignID int
	 ,ContentID int)

SET @SQL =
'INSERT INTO
	#CampaignContentInserts
SELECT
	pC.CampaignID
	,pC.ContentID
FROM
	dbo.CampaignContent pC
EXCEPT
SELECT
	aC.CampaignID
	,aC.ContentID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #CampaignContentUpdates (
			CampaignID int
			,ContentID int
			,ActivationDate datetime
			,ExpirationDate datetime
			,UserContentInd bit
			,EmailNotificationInd bit
			,SMSNotificationInd bit
			,OSNotificationInd bit
			,OSNotificationStatusDesc nvarchar(100)
			,OSNotificationSentDate datetime
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#CampaignContentUpdates (
		CampaignID
		,ContentID
		,ActivationDate
		,ExpirationDate
		,UserContentInd
		,EmailNotificationInd
		,SMSNotificationInd
		,OSNotificationInd
		,OSNotificationStatusDesc
		,OSNotificationSentDate
		,CreateDate
	)
SELECT
		pC.CampaignID
		,pC.ContentID
		,pC.ActivationDate
		,pC.ExpirationDate
		,pC.UserContentInd
		,pC.EmailNotificationInd
		,pC.SMSNotificationInd
		,pC.OSNotificationInd
		,pC.OSNotificationStatusDesc
		,pC.OSNotificationSentDate
		,pC.CreateDate
FROM
	dbo.CampaignContent pC
EXCEPT
SELECT
		aC.CampaignID
		,aC.ContentID
		,aC.ActivationDate
		,aC.ExpirationDate
		,aC.UserContentInd
		,aC.EmailNotificationInd
		,aC.SMSNotificationInd
		,aC.OSNotificationInd
		,aC.OSNotificationStatusDesc
		,aC.OSNotificationSentDate
		,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent aC'
EXEC (@SQL)

-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignContentInserts) >0)
BEGIN	

-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#CampaignContentUpdates
	FROM
		#CampaignContentUpdates u
		JOIN #CampaignContentInserts i on
			u.CampaignID = i.CampaignID
			AND u.ContentID = i.ContentID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent (
			CampaignID
			,ContentID
			,ActivationDate
			,ExpirationDate
			,UserContentInd
			,EmailNotificationInd
			,SMSNotificationInd
			,OSNotificationInd
			,OSNotificationStatusDesc
			,OSNotificationSentDate
			,CreateDate
		)
	SELECT
		pC.CampaignID
		,pC.ContentID
		,pC.ActivationDate
		,pC.ExpirationDate
		,pC.UserContentInd
		,pC.EmailNotificationInd
		,pC.SMSNotificationInd
		,pC.OSNotificationInd
		,pC.OSNotificationStatusDesc
		,pC.OSNotificationSentDate
		,pC.CreateDate
	FROM
		dbo.CampaignContent pC
		INNER JOIN #CampaignContentInserts i
			on pC.ContentID = i.ContentID
			AND pC.CampaignID = i.CampaignID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CampaignContentUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent
	SET
		CampaignID = pC.CampaignID
		,ContentID = pC.ContentID
		,ActivationDate = pC.ActivationDate
		,ExpirationDate = pC.ExpirationDate
		,UserContentInd = pC.UserContentInd
		,EmailNotificationInd = pC.EmailNotificationInd
		,SMSNotificationInd = pC.SMSNotificationInd
		,OSNotificationInd = pC.OSNotificationInd
		,OSNotificationStatusDesc = pC.OSNotificationStatusDesc
		,OSNotificationSentDate = pC.OSNotificationSentDate
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignContent aC
		INNER JOIN dbo.CampaignContent pC
			ON ac.CampaignID = pC.CampaignID
				AND aC.ContentID = pC.ContentID
		INNER JOIN #CampaignContentUpdates u
			on pC.CampaignID = u.CampaignID
				AND pC.ContentID = u.ContentID'
EXEC (@SQL)
END
/***************************************************************************/
------------------------------------------------------------
--CampaignMember
------------------------------------------------------------

IF	object_id('tempdb..#CampaignMemberInserts') IS NOT NULL
DROP TABLE #CampaignMemberInserts

IF	object_id('tempdb..#CampaignMemberUpdates') IS NOT NULL
DROP TABLE #CampaignMemberUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #CampaignMemberInserts
	(CampaignID int
	 ,CCHID int)

SET @SQL =
'INSERT INTO
	#CampaignMemberInserts
SELECT
	pC.CampaignID
	,pC.CCHID
FROM
	dbo.CampaignMember pC
EXCEPT
SELECT
	aC.CampaignID
	,aC.CCHID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #CampaignMemberUpdates (
			CampaignID int
			,CCHID int
			,Savings money
			,Score float
			,YourCostSavingsAmt money
			,CreateDate datetime
		)

SET @SQL =
'INSERT INTO	
	#CampaignMemberUpdates (
		CampaignID
		,CCHID
		,Savings
		,Score
		,YourCostSavingsAmt
		,CreateDate
	)
SELECT
		pC.CampaignID
		,pC.CCHID
		,pC.Savings
		,pC.Score
		,pC.YourCostSavingsAmt
		,pC.CreateDate
FROM
	dbo.CampaignMember pC
EXCEPT
SELECT
		aC.CampaignID
		,aC.CCHID
		,aC.Savings
		,aC.Score
		,aC.YourCostSavingsAmt
		,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember aC'
EXEC (@SQL)


-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignMemberInserts) >0)
BEGIN	

-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#CampaignMemberUpdates
	FROM
		#CampaignMemberUpdates u
		JOIN #CampaignMemberInserts i on
			u.CampaignID = i.CampaignID
			AND u.CCHID = i.CCHID

------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember (
			CampaignID
			,CCHID
			,Savings
			,Score
			,YourCostSavingsAmt
			,CreateDate
		)
	SELECT
		pC.CampaignID
		,pC.CCHID
		,pC.Savings
		,pC.Score
		,pC.YourCostSavingsAmt
		,pC.CreateDate
	FROM
		dbo.CampaignMember pC
		INNER JOIN #CampaignMemberInserts i
			on pC.CCHID = i.CCHID
			AND pC.CampaignID = i.CampaignID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CampaignMemberUpdates) > 0)
BEGIN
SET @SQL =
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember
	SET
		CampaignID = pC.CampaignID
		,CCHID = pC.CCHID
		,Savings = pC.Savings
		,Score = pC.Score
		,YourCostSavingsAmt = pC.YourCostSavingsAmt
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CampaignMember aC
		INNER JOIN dbo.CampaignMember pC
			ON ac.CampaignID = pC.CampaignID
				AND aC.CCHID = pC.CCHID
		INNER JOIN #CampaignMemberUpdates u
			on pC.CampaignID = u.CampaignID
				AND pC.CCHID = u.CCHID'
EXEC (@SQL)
END

/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--UserContent
------------------------------------------------------------

IF	object_id('tempdb..#UserContentInserts') IS NOT NULL
DROP TABLE #UserContentInserts

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #UserContentInserts
	(CampaignID int
	 ,CCHID int
	 ,ContentID int)

SET @SQL =
'INSERT INTO
	#UserContentInserts
SELECT
	pC.CampaignID
	,pC.CCHID
	,pC.ContentID
FROM
	dbo.UserContent pC
EXCEPT
SELECT
	aC.CampaignID
	,aC.CCHID
	,aC.ContentID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContent aC'
EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentInserts) >0)
BEGIN	
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.UserContent (
			CampaignID
			,CCHID
			,ContentID
			,ContentStatusChangeDate
			,UserContentCommentText
			,NotificationSentDate
			,SMSNotificationSentDate
			,SMSNotificationStatusDesc
			,OSNotificationSentDate
			,OSNotificationStatusDesc
			,ContentSavingsAmt
			,MemberContentDataText
			,ContentStatusID
			,CreateDate
		)
	SELECT
		pC.CampaignID
		,pC.CCHID
		,pC.ContentID
		,pC.ContentStatusChangeDate
		,pC.UserContentCommentText
		,pC.NotificationSentDate
		,pC.SMSNotificationSentDate
		,pC.SMSNotificationStatusDesc
		,pC.OSNotificationSentDate
		,pC.OSNotificationStatusDesc
		,pC.ContentSavingsAmt
		,pC.MemberContentDataText
		,pC.ContentStatusID
		,pC.CreateDate
	FROM
		dbo.UserContent pC
		INNER JOIN #UserContentInserts i
			on pC.CCHID = i.CCHID
			AND pC.CampaignID = i.CampaignID
			AND pC.ContentID = i.ContentID'
EXEC (@SQL)
END

/***********************************************************************/

/***************************************************************************/
------------------------------------------------------------
--ContentTranslation
------------------------------------------------------------
IF	object_id('tempdb..#ContentTranslationInserts') IS NOT NULL
DROP TABLE #ContentTranslationInserts

IF	object_id('tempdb..#ContentTranslationUpdates') IS NOT NULL
DROP TABLE #ContentTranslationUpdates

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #ContentTranslationInserts
	(ContentID int
	,LocaleID int)
	
SET @SQL = 
'INSERT INTO
	#ContentTranslationInserts
SELECT
	pC.ContentID 
	,pC.LocaleID
FROM
	dbo.ContentTranslation pC
EXCEPT
SELECT
	ac.ContentID
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #ContentTranslationUpdates (
			ContentID int
			,LocaleID int
			,ContentTitle nvarchar(50)
			,ContentCaptionText nvarchar(250)
			,SMSNotificationText nvarchar(2000)
			,OSNotificationText nvarchar(2000)
			,ContentDesc nvarchar(2000)
			,CreateDate datetime
			)
			
SET @SQL =
'INSERT INTO	
	#ContentTranslationUpdates (
	ContentID
	,LocaleID
	,ContentTitle
	,ContentCaptionText
	,SMSNotificationText
	,OSNotificationText
	,ContentDesc
	,CreateDate)
SELECT
	pC.ContentID
	,pC.LocaleID
	,pC.ContentTitle
	,pC.ContentCaptionText
	,pC.SMSNotificationText
	,pC.OSNotificationText
	,pC.ContentDesc
	,pC.CreateDate
FROM
	dbo.ContentTranslation pC
EXCEPT
SELECT
	aC.ContentID
	,aC.LocaleID
	,aC.ContentTitle
	,aC.ContentCaptionText
	,aC.SMSNotificationText
	,aC.OSNotificationText
	,aC.ContentDesc
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTranslationInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#ContentTranslationUpdates
	FROM
		#ContentTranslationUpdates u
		INNER JOIN #ContentTranslationInserts i
			ON u.ContentID = i.ContentID
			AND u.LocaleID = i.LocaleID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation (
			ContentID
			,LocaleID
			,ContentTitle
			,ContentCaptionText
			,SMSNotificationText
			,OSNotificationText
			,ContentDesc
			,CreateDate
		)
	SELECT
		pC.ContentID
		,pC.LocaleID
		,pC.ContentTitle
		,pC.ContentCaptionText
		,pC.SMSNotificationText
		,pC.OSNotificationText
		,pC.ContentDesc
		,pC.CreateDate
	FROM
		dbo.ContentTranslation pC
		INNER JOIN #ContentTranslationInserts i
			on pC.ContentID = i.ContentID
			and pC.LocaleID = i.LocaleID'
	
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentTranslationUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation
	SET
		ContentID = pC.ContentID
		,LocaleID = pC.LocaleID
		,ContentTitle = pC.ContentTitle
		,ContentCaptionText = pC.ContentCaptionText
		,SMSNotificationText = pC.SMSNotificationText
		,OSNotificationText = pC.OSNotificationText
		,ContentDesc = pC.ContentDesc
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.ContentTranslation aC
		INNER JOIN dbo.ContentTranslation pC
			ON ac.ContentID = pC.ContentID
			AND ac.LocaleID = pC.LocaleID
		INNER JOIN #ContentTranslationUpdates u
			on pC.ContentID = u.ContentID
			AND pC.LocaleID = u.LocaleID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--QuestionTranslation
------------------------------------------------------------
IF	object_id('tempdb..#QuestionTranslationInserts') IS NOT NULL
DROP TABLE #QuestionTranslationInserts

IF	object_id('tempdb..#QuestionTranslationUpdates') IS NOT NULL
DROP TABLE #QuestionTranslationUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #QuestionTranslationInserts
	(QuestionID int
	,LocaleID int)
	
SET @SQL = 
'INSERT INTO
	#QuestionTranslationInserts
SELECT
	pC.QuestionID 
	,pC.LocaleID
FROM
	dbo.QuestionTranslation pC
EXCEPT
SELECT
	ac.QuestionID
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #QuestionTranslationUpdates (
			QuestionID int
			,LocaleID int
			,QuestionText nvarchar(250)
			,CreateDate datetime
			)
			
SET @SQL =
'INSERT INTO	
	#QuestionTranslationUpdates (
	QuestionID
	,LocaleID
	,QuestionText
	,CreateDate)
SELECT
	pC.QuestionID
	,pC.LocaleID
	,pC.QuestionText
	,pC.CreateDate
FROM
	dbo.QuestionTranslation pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.LocaleID
	,aC.QuestionText
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionTranslationInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#QuestionTranslationUpdates
	FROM
		#QuestionTranslationUpdates u
		INNER JOIN #QuestionTranslationInserts i
			ON u.QuestionID = i.QuestionID
			AND u.LocaleID = i.LocaleID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation (
			QuestionID
			,LocaleID
			,QuestionText
			,CreateDate
		)
	SELECT
		pC.QuestionID
		,pC.LocaleID
		,pC.QuestionText
		,pC.CreateDate
	FROM
		dbo.QuestionTranslation pC
		INNER JOIN #QuestionTranslationInserts i
			on pC.QuestionID = i.QuestionID
			and pC.LocaleID = i.LocaleID'
	
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionTranslationUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation
	SET
		QuestionID = pC.QuestionID
		,LocaleID = pC.LocaleID
		,QuestionText = pC.QuestionText
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.QuestionTranslation aC
		INNER JOIN dbo.QuestionTranslation pC
			ON ac.QuestionID = pC.QuestionID
			AND ac.LocaleID = pC.LocaleID
		INNER JOIN #QuestionTranslationUpdates u
			on pC.QuestionID = u.QuestionID
			AND pC.LocaleID = u.LocaleID'
EXEC (@SQL)
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--AnswerTranslation
------------------------------------------------------------
IF	object_id('tempdb..#AnswerTranslationInserts') IS NOT NULL
DROP TABLE #AnswerTranslationInserts

IF	object_id('tempdb..#AnswerTranslationUpdates') IS NOT NULL
DROP TABLE #AnswerTranslationUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #AnswerTranslationInserts
	(AnswerID int
	,LocaleID int)
	
SET @SQL = 
'INSERT INTO
	#AnswerTranslationInserts
SELECT
	pC.AnswerID 
	,pC.LocaleID
FROM
	dbo.AnswerTranslation pC
EXCEPT
SELECT
	ac.AnswerID
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #AnswerTranslationUpdates (
			AnswerID int
			,LocaleID int
			,AnswerText nvarchar(250)
			,CreateDate datetime
			)
			
SET @SQL =
'INSERT INTO	
	#AnswerTranslationUpdates (
	AnswerID
	,LocaleID
	,AnswerText
	,CreateDate)
SELECT
	pC.AnswerID
	,pC.LocaleID
	,pC.AnswerText
	,pC.CreateDate
FROM
	dbo.AnswerTranslation pC
EXCEPT
SELECT
	aC.AnswerID
	,aC.LocaleID
	,aC.AnswerText
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnswerTranslationInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#AnswerTranslationUpdates
	FROM
		#AnswerTranslationUpdates u
		INNER JOIN #AnswerTranslationInserts i
			ON u.AnswerID = i.AnswerID
			AND u.LocaleID = i.LocaleID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation (
			AnswerID
			,LocaleID
			,AnswerText
			,CreateDate
		)
	SELECT
		pC.AnswerID
		,pC.LocaleID
		,pC.AnswerText
		,pC.CreateDate
	FROM
		dbo.AnswerTranslation pC
		INNER JOIN #AnswerTranslationInserts i
			on pC.AnswerID = i.AnswerID
			and pC.LocaleID = i.LocaleID'
	
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #AnswerTranslationUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation
	SET
		AnswerID = pC.AnswerID
		,LocaleID = pC.LocaleID
		,AnswerText = pC.AnswerText
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.AnswerTranslation aC
		INNER JOIN dbo.AnswerTranslation pC
			ON ac.AnswerID = pC.AnswerID
			AND ac.LocaleID = pC.LocaleID
		INNER JOIN #AnswerTranslationUpdates u
			on pC.AnswerID = u.AnswerID
			AND pC.LocaleID = u.LocaleID'
EXEC (@SQL)
END
/***************************************************************************/
------------------------------------------------------------
--CardTypeTranslation
------------------------------------------------------------
IF	object_id('tempdb..#CardTypeTranslationInserts') IS NOT NULL
DROP TABLE #CardTypeTranslationInserts

IF	object_id('tempdb..#CardTypeTranslationUpdates') IS NOT NULL
DROP TABLE #CardTypeTranslationUpdates
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #CardTypeTranslationInserts
	(CardTypeID int
	,LocaleID int)
	
SET @SQL = 
'INSERT INTO
	#CardTypeTranslationInserts
SELECT
	pC.CardTypeID 
	,pC.LocaleID
FROM
	dbo.CardTypeTranslation pC
EXCEPT
SELECT
	ac.CardTypeID
	,aC.LocaleID
FROM '
	 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #CardTypeTranslationUpdates (
			CardTypeID int
			,LocaleID int
			,CardTypeName nvarchar(250)
			,CreateDate datetime
			)
			
SET @SQL =
'INSERT INTO	
	#CardTypeTranslationUpdates (
	CardTypeID
	,LocaleID
	,CardTypeName
	,CreateDate)
SELECT
	pC.CardTypeID
	,pC.LocaleID
	,pC.CardTypeName
	,pC.CreateDate
FROM
	dbo.CardTypeTranslation pC
EXCEPT
SELECT
	aC.CardTypeID
	,aC.LocaleID
	,aC.CardTypeName
	,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation aC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CardTypeTranslationInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#CardTypeTranslationUpdates
	FROM
		#CardTypeTranslationUpdates u
		INNER JOIN #CardTypeTranslationInserts i
			ON u.CardTypeID = i.CardTypeID
			AND u.LocaleID = i.LocaleID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation (
			CardTypeID
			,LocaleID
			,CardTypeName
			,CreateDate
		)
	SELECT
		pC.CardTypeID
		,pC.LocaleID
		,pC.CardTypeName
		,pC.CreateDate
	FROM
		dbo.CardTypeTranslation pC
		INNER JOIN #CardTypeTranslationInserts i
			on pC.CardTypeID = i.CardTypeID
			and pC.LocaleID = i.LocaleID'
	
	EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CardTypeTranslationUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation
	SET
		CardTypeID = pC.CardTypeID
		,LocaleID = pC.LocaleID
		,CardTypeName = pC.CardTypeName
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.CardTypeTranslation aC
		INNER JOIN dbo.CardTypeTranslation pC
			ON ac.CardTypeID = pC.CardTypeID
			AND ac.LocaleID = pC.LocaleID
		INNER JOIN #CardTypeTranslationUpdates u
			on pC.CardTypeID = u.CardTypeID
			AND pC.LocaleID = u.LocaleID'
EXEC (@SQL)
END
/***************************************************************************/

/***************************************************************************/
/* 2015-08-07 AS -- Removed as this table is loaded / promoted during enrollments
------------------------------------------------------------
--MemberIDCard
------------------------------------------------------------
IF	object_id('tempdb..#MemberIDCardInserts') IS NOT NULL
DROP TABLE #MemberIDCardInserts

-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
CREATE TABLE #MemberIDCardInserts
	( CCHID int
	 ,CardTypeID int
	 ,LocaleID int
	 ,CardViewModeID int
	 )

SET @SQL =
'INSERT INTO
	#MemberIDCardInserts
SELECT
	 pC.CCHID
	 ,pC.CardTypeID
	 ,pC.LocaleID
	 ,pC.CardViewModeID
FROM
	dbo.MemberIDCard pC
EXCEPT
SELECT
	  aC.CCHID
	 ,aC.CardTypeID
	 ,aC.LocaleID
	 ,aC.CardViewModeID
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard aC'
EXEC (@SQL)
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
CREATE TABLE #MemberIDCardUpdates (
			CCHID int
			,CardTypeID int
			,LocaleID int
			,CardViewModeID int
			,CardMemberDataText nvarchar(2000)
			,CreateDate datetime
			)
			
SET @SQL =
'INSERT INTO	
	#MemberIDCardUpdates (
			CCHID
			,CardTypeID
			,LocaleID
			,CardViewModeID
			,CardMemberDataText
			,CreateDate)
SELECT
		pC.CCHID
		,pC.CardTypeID
		,pC.LocaleID
		,pC.CardViewModeID
		,pC.CardMemberDataText
		,pC.CreateDate
FROM
	dbo.MemberIDCard pC
EXCEPT
SELECT
		aC.CCHID
		,aC.CardTypeID
		,aC.LocaleID
		,aC.CardViewModeID
		,aC.CardMemberDataText
		,aC.CreateDate
FROM '
 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard aC'

EXEC (@SQL)
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #MemberIDCardInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#MemberIDCardUpdates
	FROM
		#MemberIDCardUpdates u
		INNER JOIN #MemberIDCardInserts i
			ON u.CCHID  = i.CCHID
			AND u.CardTypeID = i.CardTypeID
			AND u.LocaleID = i.LocaleID
			AND u.CardViewModeID = i.CardViewModeID
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
SET @SQL =
	'INSERT INTO '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard (
			 CCHID
			,CardTypeID
			,LocaleID
			,CardViewModeID
			,CardMemberDataText
			,SecurityTokenGUID
			,SecurityTokenBeginDatetime
			,SecurityTokenEndDatetime
			,CreateDate
		)
	SELECT
		pC.CCHID
		,pC.CardTypeID
		,pC.LocaleID
		,pC.CardViewModeID
		,pC.CardMemberDataText
		,pC.SecurityTokenGUID
		,pC.SecurityTokenBeginDatetime
		,pC.SecurityTokenEndDatetime
		,pC.CreateDate
	FROM
		dbo.MemberIDCard pC
		INNER JOIN #MemberIDCardInserts i
			on pc.CCHID = i.CCHID
			and pc.CardTypeID = i.CardTypeID
			and pc.LocaleID = i.LocaleID
			and pc.CardViewModeID = i.CardViewModeID'
EXEC (@SQL)
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #MemberIDCardUpdates) > 0)
BEGIN
SET @SQL = 
	'UPDATE '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard
	SET
		CCHID = pC.CCHID
		,CardTypeID = pC.CardTypeID
		,LocaleID = pC.LocaleID
		,CardViewModeID = pC.CardViewModeID
		,CardMemberDataText = pC.CardMemberDataText
		,CreateDate = pC.CreateDate
	FROM '
		 + LTRIM(RTRIM(@servername)) + '.' + LTRIM(RTRIM(@dbname)) + '.dbo.MemberIDCard aC
		INNER JOIN dbo.MemberIDCard pC
			ON ac.CCHID = pC.CCHID
			AND ac.CardTypeID = pC.CardTypeID
			AND ac.LocaleID = pC.LocaleID
			AND ac.CardViewModeID = pC.CardViewModeID
		INNER JOIN #MemberIDCardUpdates u
			on pC.CCHID = u.CCHID
			AND pc.CardTypeID = u.CardTypeID
			AND pC.LocaleID = u.LocaleID
			and pC.CardViewModeID = u.CardViewModeID'
EXEC (@SQL)
END
------------------------------------------------------------------------------
*/
END --env
ELSE
BEGIN
	IF NOT EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'SQLEnvironment' and ConfigValue = 'PROCESSING')
		PRINT 'Animations Promotions must be done from Processing.'
	IF  db_name() != @dbname 
		PRINT 'Animations must be promoted to the same client database.'
	IF NOT EXISTS (SELECT 1 FROM sys.servers WHERE name = @servername)
		PRINT 'Linked server ' + @servername + ' does not exist.'
END
/***********************************************************************/
END --proc

GO




