/****** Object:  StoredProcedure [dbo].[p_Promote_Animations_CaesarsHumana_alpha]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_Promote_Animations_CaesarsHumana_alpha]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_Promote_Animations_CaesarsHumana_alpha]
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
      exec p_Promote_Animations_CaesarsHumana_alpha

Objects Listing:

Tables: 


    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-02-10  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_Promote_Animations_CaesarsHumana_alpha] 
as

BEGIN

IF  @@servername = 'SAMB01VMD01' AND db_name() = 'CCH_CaesarsHumanaWeb'
BEGIN --Env
----------------------------------------------------------------------------
--Tables must be processed in order below due to FK Consraints
---------------------------------------------------------------------------
-----------------------------------------------------------------------------
--UPDATE else INSERT on Primary Key
-----------------------------------------------------------------------------
/***************************************************************************/
------------------------------------------------------------
--Campaign
------------------------------------------------------------
IF	object_id('tempdb..#CampaignDeletes') IS NOT NULL
DROP TABLE #CampaignDeletes

IF	object_id('tempdb..#CampaignInserts') IS NOT NULL
DROP TABLE #CampaignInserts

IF	object_id('tempdb..#CampaignUpdates') IS NOT NULL
DROP TABLE #CampaignUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.CampaignID
INTO
	#CampaignDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Campaign aC
	--processing.VanessaTempdb.dbo.Campaign aC
EXCEPT
SELECT
	pc.CampaignID
FROM
	dbo.Campaign pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Campaign 
		--processing.VanessaTempdb.dbo.Campaign
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Campaign aC
		--processing.VanessaTempdb.dbo.Campaign aC
		INNER JOIN #CampaignDeletes d
			on ac.CampaignID = d.CampaignID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.CampaignID
INTO
	#CampaignInserts
FROM
	dbo.Campaign pC
EXCEPT
SELECT
	ac.CampaignID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Campaign aC
	--processing.VanessaTempdb.dbo.Campaign aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.CampaignID
	,pC.CampaignDesc
	,pC.CampaignActiveInd
	,pC.TargetPopulationDesc
	,pC.CampaignPeriodDesc
	,pC.TargetProcedureName
	,pC.AuthRequiredInd
	,pC.SavingsMonthStartDate
	,pC.CreateDate
INTO	
	#CampaignUpdates
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
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Campaign aC
	--processing.VanessaTempdb.dbo.Campaign aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Campaign (
		alpha.CCH_CaesarsHumanaWeb.dbo.Campaign (
		CampaignID
		,CampaignDesc
		,CampaignActiveInd
		,TargetPopulationDesc
		,CampaignPeriodDesc
		,TargetProcedureName
		,AuthRequiredInd
		,SavingsMonthStartDate
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
		,pC.CreateDate
	FROM
		dbo.Campaign pC
		INNER JOIN #CampaignInserts i
			on pC.CampaignID = i.CampaignID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #CampaignUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Campaign
		alpha.CCH_CaesarsHumanaWeb.dbo.Campaign
	SET
		CampaignDesc = pC.CampaignDesc
		,CampaignActiveInd = pC.CampaignActiveInd
		,TargetPopulationDesc = pC.TargetPopulationDesc
		,CampaignPeriodDesc = pC.CampaignPeriodDesc
		,TargetProcedureName = pC.TargetProcedureName
		,AuthRequiredInd = pC.AuthRequiredIndF
		,SavingsMonthStartDate = pC.SavingsMonthStartDate
	FROM
		--processing.VanessaTempdb.dbo.Campaign aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Campaign aC
		INNER JOIN dbo.Campaign pC
			ON ac.CampaignID = pC.CampaignID
		INNER JOIN #CampaignUpdates u
			on pC.CampaignID = u.CampaignID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ContentType
------------------------------------------------------------
IF	object_id('tempdb..#ContentTypeDeletes') IS NOT NULL
DROP TABLE #ContentTypeDeletes

IF	object_id('tempdb..#ContentTypeInserts') IS NOT NULL
DROP TABLE #ContentTypeInserts

IF	object_id('tempdb..#ContentTypeUpdates') IS NOT NULL
DROP TABLE #ContentTypeUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ContentTypeID
INTO
	#ContentTypeDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.ContentType aC
	--processing.VanessaTempdb.dbo.ContentType aC
EXCEPT
SELECT
	pc.ContentTypeID
FROM
	dbo.ContentType pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTypeDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentType 
		--processing.VanessaTempdb.dbo.ContentType
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentType aC
		--processing.VanessaTempdb.dbo.ContentType aC
		INNER JOIN #ContentTypeDeletes d
			on ac.ContentTypeID = d.ContentTypeID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ContentTypeID
INTO
	#ContentTypeInserts
FROM
	dbo.ContentType pC
EXCEPT
SELECT
	ac.ContentTypeID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentType aC
	--processing.VanessaTempdb.dbo.ContentType aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ContentTypeID
	,pC.ContentTypeDesc
	,pC.CreateDate
INTO	
	#ContentTypeUpdates
FROM
	dbo.ContentType pC
EXCEPT
SELECT
	aC.ContentTypeID
	,aC.ContentTypeDesc
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentType aC
	--processing.VanessaTempdb.dbo.ContentType aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.ContentType (
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentType (
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
			on pC.ContentTypeID = i.ContentTypeID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentTypeUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.ContentType
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentType
	SET
		ContentTypeID = pC.ContentTypeID
		,ContentTypeDesc = pC.ContentTypeDesc
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.ContentType aC
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentType aC
		INNER JOIN dbo.ContentType pC
			ON ac.ContentTypeID = pC.ContentTypeID
		INNER JOIN #ContentTypeUpdates u
			on pC.ContentTypeID = u.ContentTypeID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Content
------------------------------------------------------------
IF	object_id('tempdb..#ContentDeletes') IS NOT NULL
DROP TABLE #ContentDeletes

IF	object_id('tempdb..#ContentInserts') IS NOT NULL
DROP TABLE #ContentInserts

IF	object_id('tempdb..#ContentUpdates') IS NOT NULL
DROP TABLE #ContentUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ContentID
INTO
	#ContentDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Content aC
	--processing.VanessaTempdb.dbo.Content aC
EXCEPT
SELECT
	pc.ContentID
FROM
	dbo.Content pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Content 
		--processing.VanessaTempdb.dbo.Content
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Content aC
		--processing.VanessaTempdb.dbo.Content aC
		INNER JOIN #ContentDeletes d
			on ac.ContentID = d.ContentID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ContentID
INTO
	#ContentInserts
FROM
	dbo.Content pC
EXCEPT
SELECT
	ac.ContentID
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Content aC
	--processing.VanessaTempdb.dbo.Content aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ContentID
	,pC.ContentTypeID
	,pC.ContentTitle
	,pC.ContentDesc
	,pC.ContentSourceDesc
	,pC.ContentImageFileName
	,pC.ContentFileLocationDesc
	,pC.ContentPointsCount
	,pC.ContentDurationSecondsCount
	,pC.ContentCaptionText
	,pC.ContentName
	,pC.IntroContentInd
	,pC.CreateDate
INTO	
	#ContentUpdates
FROM
	dbo.Content pC
EXCEPT
SELECT
	aC.ContentID
	,aC.ContentTypeID
	,aC.ContentTitle
	,aC.ContentDesc
	,aC.ContentSourceDesc
	,aC.ContentImageFileName
	,aC.ContentFileLocationDesc
	,aC.ContentPointsCount
	,aC.ContentDurationSecondsCount
	,aC.ContentCaptionText
	,aC.ContentName
	,aC.IntroContentInd
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Content aC
	--processing.VanessaTempdb.dbo.Content aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Content (
		alpha.CCH_CaesarsHumanaWeb.dbo.Content (
			ContentID
			,ContentTypeID
			,ContentTitle
			,ContentDesc
			,ContentSourceDesc
			,ContentImageFileName
			,ContentFileLocationDesc
			,ContentPointsCount
			,ContentDurationSecondsCount
			,ContentCaptionText
			,ContentName
			,IntroContentInd
			,CreateDate
		)
	SELECT
		pC.ContentID
		,pC.ContentTypeID
		,pC.ContentTitle
		,pC.ContentDesc
		,pC.ContentSourceDesc
		,pC.ContentImageFileName
		,pC.ContentFileLocationDesc
		,pC.ContentPointsCount
		,pC.ContentDurationSecondsCount
		,pC.ContentCaptionText
		,pC.ContentName
		,pC.IntroContentInd
		,pC.CreateDate
	FROM
		dbo.Content pC
		INNER JOIN #ContentInserts i
			on pC.ContentID = i.ContentID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Content
		alpha.CCH_CaesarsHumanaWeb.dbo.Content
	SET
		ContentID = pC.ContentID
		,ContentTypeID = pC.ContentTypeID
		,ContentTitle = pC.ContentTitle
		,ContentDesc = pC.ContentDesc
		,ContentSourceDesc = pC.ContentSourceDesc
		,ContentImageFileName = pC.ContentImageFileName
		,ContentFileLocationDesc = pC.ContentFileLocationDesc
		,ContentPointsCount = pC.ContentPointsCount
		,ContentDurationSecondsCount = pC.ContentDurationSecondsCount
		,ContentCaptionText = pC.ContentCaptionText
		,ContentName = pC.ContentName
		,IntroContentInd = pC.IntroContentInd
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Content aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Content aC
		INNER JOIN dbo.Content pC
			ON ac.ContentID = pC.ContentID
		INNER JOIN #ContentUpdates u
			on pC.ContentID = u.ContentID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--QuestionType
------------------------------------------------------------
IF	object_id('tempdb..#QuestionTypeDeletes') IS NOT NULL
DROP TABLE #QuestionTypeDeletes

IF	object_id('tempdb..#QuestionTypeInserts') IS NOT NULL
DROP TABLE #QuestionTypeInserts

IF	object_id('tempdb..#QuestionTypeUpdates') IS NOT NULL
DROP TABLE #QuestionTypeUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.QuestionTypeID
INTO
	#QuestionTypeDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType aC
	--processing.VanessaTempdb.dbo.QuestionType aC
EXCEPT
SELECT
	pc.QuestionTypeID
FROM
	dbo.QuestionType pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionTypeDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType 
		--processing.VanessaTempdb.dbo.QuestionType
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType aC
		--processing.VanessaTempdb.dbo.QuestionType aC
		INNER JOIN #QuestionTypeDeletes d
			on ac.QuestionTypeID = d.QuestionTypeID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.QuestionTypeID
INTO
	#QuestionTypeInserts
FROM
	dbo.QuestionType pC
EXCEPT
SELECT
	ac.QuestionTypeID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType aC
	--processing.VanessaTempdb.dbo.QuestionType aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.QuestionTypeID
	,pC.QuestionTypeDesc
	,pC.CreateDate
INTO	
	#QuestionTypeUpdates
FROM
	dbo.QuestionType pC
EXCEPT
SELECT
	aC.QuestionTypeID
	,aC.QuestionTypeDesc
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType aC
	--processing.VanessaTempdb.dbo.QuestionType aC
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionTypeInserts) >0)
BEGIN	
-----------------------------------------------------------
--Remove the Inserts from the Updates table so they won't be processed twice
-----------------------------------------------------------
	DELETE 
		#QuestionTypeUpdates
	WHERE
		QuestionTypeID in (SELECT QuestionTypeID from #QuestionTypeInserts)
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	INSERT INTO
		--processing.VanessaTempdb.dbo.QuestionType (
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType (
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
			on pC.QuestionTypeID = i.QuestionTypeID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionTypeUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.QuestionType
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType
	SET
		QuestionTypeID = pC.QuestionTypeID
		,QuestionTypeDesc = pC.QuestionTypeDesc
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.QuestionType aC
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionType aC
		INNER JOIN dbo.QuestionType pC
			ON ac.QuestionTypeID = pC.QuestionTypeID
		INNER JOIN #QuestionTypeUpdates u
			on pC.QuestionTypeID = u.QuestionTypeID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ContentStatus
------------------------------------------------------------
IF	object_id('tempdb..#ContentStatusDeletes') IS NOT NULL
DROP TABLE #ContentStatusDeletes

IF	object_id('tempdb..#ContentStatusInserts') IS NOT NULL
DROP TABLE #ContentStatusInserts

IF	object_id('tempdb..#ContentStatusUpdates') IS NOT NULL
DROP TABLE #ContentStatusUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ContentStatusID
INTO
	#ContentStatusDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus aC
	--processing.VanessaTempdb.dbo.ContentStatus aC
EXCEPT
SELECT
	pc.ContentStatusID
FROM
	dbo.ContentStatus pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentStatusDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus 
		--processing.VanessaTempdb.dbo.ContentStatus
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus aC
		--processing.VanessaTempdb.dbo.ContentStatus aC
		INNER JOIN #ContentStatusDeletes d
			on ac.ContentStatusID = d.ContentStatusID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ContentStatusID
INTO
	#ContentStatusInserts
FROM
	dbo.ContentStatus pC
EXCEPT
SELECT
	ac.ContentStatusID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus aC
	--processing.VanessaTempdb.dbo.ContentStatus aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ContentStatusID
	,pC.ContentStatusDesc
	,pC.CreateDate
INTO	
	#ContentStatusUpdates
FROM
	dbo.ContentStatus pC
EXCEPT
SELECT
	aC.ContentStatusID
	,aC.ContentStatusDesc
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus aC
	--processing.VanessaTempdb.dbo.ContentStatus aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.ContentStatus (
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus (
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
			on pC.ContentStatusID = i.ContentStatusID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentStatusUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.ContentStatus
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus
	SET
		ContentStatusID = pC.ContentStatusID
		,ContentStatusDesc = pC.ContentStatusDesc
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.ContentStatus aC
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentStatus aC
		INNER JOIN dbo.ContentStatus pC
			ON ac.ContentStatusID = pC.ContentStatusID
		INNER JOIN #ContentStatusUpdates u
			on pC.ContentStatusID = u.ContentStatusID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ContentDisplayRule
------------------------------------------------------------
IF	object_id('tempdb..#ContentDisplayRuleDeletes') IS NOT NULL
DROP TABLE #ContentDisplayRuleDeletes

IF	object_id('tempdb..#ContentDisplayRuleInserts') IS NOT NULL
DROP TABLE #ContentDisplayRuleInserts

IF	object_id('tempdb..#ContentDisplayRuleUpdates') IS NOT NULL
DROP TABLE #ContentDisplayRuleUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ContentDisplayRuleID
INTO
	#ContentDisplayRuleDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule aC
	--processing.VanessaTempdb.dbo.ContentDisplayRule aC
EXCEPT
SELECT
	pc.ContentDisplayRuleID
FROM
	dbo.ContentDisplayRule pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentDisplayRuleDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule 
		--processing.VanessaTempdb.dbo.ContentDisplayRule
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule aC
		--processing.VanessaTempdb.dbo.ContentDisplayRule aC
		INNER JOIN #ContentDisplayRuleDeletes d
			on ac.ContentDisplayRuleID = d.ContentDisplayRuleID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ContentDisplayRuleID
INTO
	#ContentDisplayRuleInserts
FROM
	dbo.ContentDisplayRule pC
EXCEPT
SELECT
	ac.ContentDisplayRuleID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule aC
	--processing.VanessaTempdb.dbo.ContentDisplayRule aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ContentDisplayRuleID
	,pC.ContentDisplayRuleDesc
	,pC.CreateDate
INTO	
	#ContentDisplayRuleUpdates
FROM
	dbo.ContentDisplayRule pC
EXCEPT
SELECT
	aC.ContentDisplayRuleID
	,aC.ContentDisplayRuleDesc
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule aC
	--processing.VanessaTempdb.dbo.ContentDisplayRule aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.ContentDisplayRule (
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule (
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
			on pC.ContentDisplayRuleID = i.ContentDisplayRuleID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentDisplayRuleUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.ContentDisplayRule
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule
	SET
		ContentDisplayRuleID = pC.ContentDisplayRuleID
		,ContentDisplayRuleDesc = pC.ContentDisplayRuleDesc
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.ContentDisplayRule aC
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentDisplayRule aC
		INNER JOIN dbo.ContentDisplayRule pC
			ON ac.ContentDisplayRuleID = pC.ContentDisplayRuleID
		INNER JOIN #ContentDisplayRuleUpdates u
			on pC.ContentDisplayRuleID = u.ContentDisplayRuleID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Answer
------------------------------------------------------------
IF	object_id('tempdb..#AnswerDeletes') IS NOT NULL
DROP TABLE #AnswerDeletes

IF	object_id('tempdb..#AnswerInserts') IS NOT NULL
DROP TABLE #AnswerInserts

IF	object_id('tempdb..#AnswerUpdates') IS NOT NULL
DROP TABLE #AnswerUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.AnswerID
INTO
	#AnswerDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Answer aC
	--processing.VanessaTempdb.dbo.Answer aC
EXCEPT
SELECT
	pc.AnswerID
FROM
	dbo.Answer pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnswerDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Answer 
		--processing.VanessaTempdb.dbo.Answer
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Answer aC
		--processing.VanessaTempdb.dbo.Answer aC
		INNER JOIN #AnswerDeletes d
			on ac.AnswerID = d.AnswerID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.AnswerID
INTO
	#AnswerInserts
FROM
	dbo.Answer pC
EXCEPT
SELECT
	ac.AnswerID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Answer aC
	--processing.VanessaTempdb.dbo.Answer aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.AnswerID
	,pC.AnswerText
	,pC.CreateDate
INTO	
	#AnswerUpdates
FROM
	dbo.Answer pC
EXCEPT
SELECT
	aC.AnswerID
	,aC.AnswerText
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Answer aC
	--processing.VanessaTempdb.dbo.Answer aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Answer (
		alpha.CCH_CaesarsHumanaWeb.dbo.Answer (
			AnswerID
			,AnswerText
			,CreateDate
		)
	SELECT
		pC.AnswerID
		,pC.AnswerText
		,pC.CreateDate
	FROM
		dbo.Answer pC
		INNER JOIN #AnswerInserts i
			on pC.AnswerID = i.AnswerID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #AnswerUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Answer
		alpha.CCH_CaesarsHumanaWeb.dbo.Answer
	SET
		AnswerID = pC.AnswerID
		,AnswerText = pC.AnswerText
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Answer aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Answer aC
		INNER JOIN dbo.Answer pC
			ON ac.AnswerID = pC.AnswerID
		INNER JOIN #AnswerUpdates u
			on pC.AnswerID = u.AnswerID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Question
------------------------------------------------------------
IF	object_id('tempdb..#QuestionDeletes') IS NOT NULL
DROP TABLE #QuestionDeletes

IF	object_id('tempdb..#QuestionInserts') IS NOT NULL
DROP TABLE #QuestionInserts

IF	object_id('tempdb..#QuestionUpdates') IS NOT NULL
DROP TABLE #QuestionUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.QuestionID
INTO
	#QuestionDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Question aC
	--processing.VanessaTempdb.dbo.Question aC
EXCEPT
SELECT
	pc.QuestionID
FROM
	dbo.Question pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Question 
		--processing.VanessaTempdb.dbo.Question
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Question aC
		--processing.VanessaTempdb.dbo.Question aC
		INNER JOIN #QuestionDeletes d
			on ac.QuestionID = d.QuestionID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.QuestionID
INTO
	#QuestionInserts
FROM
	dbo.Question pC
EXCEPT
SELECT
	ac.QuestionID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Question aC
	--processing.VanessaTempdb.dbo.Question aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.QuestionID
	,pC.QuestionTypeID
	,pC.QuestionText
	,pC.ExpectedAnswerDatatypeDesc
	,pC.ExpectedAnswerValueRangeDesc
	,pC.ScoredQuestionInd
	,pC.CreateDate
INTO	
	#QuestionUpdates
FROM
	dbo.Question pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.QuestionTypeID
	,aC.QuestionText
	,aC.ExpectedAnswerDatatypeDesc
	,aC.ExpectedAnswerValueRangeDesc
	,aC.ScoredQuestionInd
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Question aC
	--processing.VanessaTempdb.dbo.Question aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Question (
		alpha.CCH_CaesarsHumanaWeb.dbo.Question (
			QuestionID
			,QuestionTypeID
			,QuestionText
			,ExpectedAnswerDatatypeDesc
			,ExpectedAnswerValueRangeDesc
			,ScoredQuestionInd
			,CreateDate
		)
	SELECT
		pC.QuestionID
		,pC.QuestionTypeID
		,pC.QuestionText
		,pC.ExpectedAnswerDatatypeDesc
		,pC.ExpectedAnswerValueRangeDesc
		,pC.ScoredQuestionInd
		,pC.CreateDate
	FROM
		dbo.Question pC
		INNER JOIN #QuestionInserts i
			on pC.QuestionID = i.QuestionID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Question
		alpha.CCH_CaesarsHumanaWeb.dbo.Question
	SET
		QuestionID = pC.QuestionID
		,QuestionTypeID = pC.QuestionTypeID
		,QuestionText = pC.QuestionText
		,ExpectedAnswerDatatypeDesc = pC.ExpectedAnswerDatatypeDesc
		,ExpectedAnswerValueRangeDesc = pC.ExpectedAnswerValueRangeDesc
		,ScoredQuestionInd = pC.ScoredQuestionInd
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Question aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Question aC
		INNER JOIN dbo.Question pC
			ON ac.QuestionID = pC.QuestionID
		INNER JOIN #QuestionUpdates u
			on pC.QuestionID = u.QuestionID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--ExperienceEvent
------------------------------------------------------------
IF	object_id('tempdb..#ExperienceEventDeletes') IS NOT NULL
DROP TABLE #ExperienceEventDeletes

IF	object_id('tempdb..#ExperienceEventInserts') IS NOT NULL
DROP TABLE #ExperienceEventInserts

IF	object_id('tempdb..#ExperienceEventUpdates') IS NOT NULL
DROP TABLE #ExperienceEventUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ExperienceEventID
INTO
	#ExperienceEventDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent aC
	--processing.VanessaTempdb.dbo.ExperienceEvent aC
EXCEPT
SELECT
	pc.ExperienceEventID
FROM
	dbo.ExperienceEvent pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ExperienceEventDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent 
		--processing.VanessaTempdb.dbo.ExperienceEvent
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent aC
		--processing.VanessaTempdb.dbo.ExperienceEvent aC
		INNER JOIN #ExperienceEventDeletes d
			on ac.ExperienceEventID = d.ExperienceEventID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ExperienceEventID
INTO
	#ExperienceEventInserts
FROM
	dbo.ExperienceEvent pC
EXCEPT
SELECT
	ac.ExperienceEventID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent aC
	--processing.VanessaTempdb.dbo.ExperienceEvent aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ExperienceEventID
	,pC.ExperienceEventDesc
	,pC.CreateDate
INTO	
	#ExperienceEventUpdates
FROM
	dbo.ExperienceEvent pC
EXCEPT
SELECT
	aC.ExperienceEventID
	,aC.ExperienceEventDesc
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent aC
	--processing.VanessaTempdb.dbo.ExperienceEvent aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.ExperienceEvent (
		alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent (
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
			on pC.ExperienceEventID = i.ExperienceEventID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ExperienceEventUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.ExperienceEvent
		alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent
	SET
		ExperienceEventID = pC.ExperienceEventID
		,ExperienceEventDesc = pC.ExperienceEventDesc
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.ExperienceEvent aC
		alpha.CCH_CaesarsHumanaWeb.dbo.ExperienceEvent aC
		INNER JOIN dbo.ExperienceEvent pC
			ON ac.ExperienceEventID = pC.ExperienceEventID
		INNER JOIN #ExperienceEventUpdates u
			on pC.ExperienceEventID = u.ExperienceEventID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Animation
------------------------------------------------------------
IF	object_id('tempdb..#AnimationDeletes') IS NOT NULL
DROP TABLE #AnimationDeletes

IF	object_id('tempdb..#AnimationInserts') IS NOT NULL
DROP TABLE #AnimationInserts

IF	object_id('tempdb..#AnimationUpdates') IS NOT NULL
DROP TABLE #AnimationUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.AnimationID
INTO
	#AnimationDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Animation aC
	--processing.VanessaTempdb.dbo.Animation aC
EXCEPT
SELECT
	pc.AnimationID
FROM
	dbo.Animation pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #AnimationDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Animation 
		--processing.VanessaTempdb.dbo.Animation
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Animation aC
		--processing.VanessaTempdb.dbo.Animation aC
		INNER JOIN #AnimationDeletes d
			on ac.AnimationID = d.AnimationID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.AnimationID
INTO
	#AnimationInserts
FROM
	dbo.Animation pC
EXCEPT
SELECT
	ac.AnimationID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Animation aC
	--processing.VanessaTempdb.dbo.Animation aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.AnimationID
	,pC.AnimationScriptFileName
	,pC.InteractiveAnimationInd
	,pC.AnimationDataProcName
	,pC.CreateDate
INTO	
	#AnimationUpdates
FROM
	dbo.Animation pC
EXCEPT
SELECT
	aC.AnimationID
	,aC.AnimationScriptFileName
	,aC.InteractiveAnimationInd
	,aC.AnimationDataProcName
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Animation aC
	--processing.VanessaTempdb.dbo.Animation aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Animation (
		alpha.CCH_CaesarsHumanaWeb.dbo.Animation (
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
			on pC.AnimationID = i.AnimationID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #AnimationUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Animation
		alpha.CCH_CaesarsHumanaWeb.dbo.Animation
	SET
		AnimationID = pC.AnimationID
		,AnimationScriptFileName = pC.AnimationScriptFileName
		,InteractiveAnimationInd = pC.InteractiveAnimationInd
		,AnimationDataProcName = pC.AnimationDataProcName
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Animation aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Animation aC
		INNER JOIN dbo.Animation pC
			ON ac.AnimationID = pC.AnimationID
		INNER JOIN #AnimationUpdates u
			on pC.AnimationID = u.AnimationID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Video
------------------------------------------------------------
IF	object_id('tempdb..#VideoDeletes') IS NOT NULL
DROP TABLE #VideoDeletes

IF	object_id('tempdb..#VideoInserts') IS NOT NULL
DROP TABLE #VideoInserts

IF	object_id('tempdb..#VideoUpdates') IS NOT NULL
DROP TABLE #VideoUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.VideoID
INTO
	#VideoDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Video aC
	--processing.VanessaTempdb.dbo.Video aC
EXCEPT
SELECT
	pc.VideoID
FROM
	dbo.Video pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #VideoDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Video 
		--processing.VanessaTempdb.dbo.Video
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Video aC
		--processing.VanessaTempdb.dbo.Video aC
		INNER JOIN #VideoDeletes d
			on ac.VideoID = d.VideoID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.VideoID
INTO
	#VideoInserts
FROM
	dbo.Video pC
EXCEPT
SELECT
	ac.VideoID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Video aC
	--processing.VanessaTempdb.dbo.Video aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
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
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Video aC
	--processing.VanessaTempdb.dbo.Video aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Video (
		alpha.CCH_CaesarsHumanaWeb.dbo.Video (
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
			on pC.VideoID = i.VideoID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #VideoUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Video
		alpha.CCH_CaesarsHumanaWeb.dbo.Video
	SET
		VideoID = pC.VideoID
		,VideoFileName = pC.VideoFileName
		,PersonalizedVideoInd = pC.PersonalizedVideoInd
		,VideoDataProcName = pC.VideoDataProcName
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Video aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Video aC
		INNER JOIN dbo.Video pC
			ON ac.VideoID = pC.VideoID
		INNER JOIN #VideoUpdates u
			on pC.VideoID = u.VideoID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Survey
------------------------------------------------------------
IF	object_id('tempdb..#SurveyDeletes') IS NOT NULL
DROP TABLE #SurveyDeletes

IF	object_id('tempdb..#SurveyInserts') IS NOT NULL
DROP TABLE #SurveyInserts

IF	object_id('tempdb..#SurveyUpdates') IS NOT NULL
DROP TABLE #SurveyUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.SurveyID
INTO
	#SurveyDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Survey aC
	--processing.VanessaTempdb.dbo.Survey aC
EXCEPT
SELECT
	pc.SurveyID
FROM
	dbo.Survey pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #SurveyDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Survey 
		--processing.VanessaTempdb.dbo.Survey
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Survey aC
		--processing.VanessaTempdb.dbo.Survey aC
		INNER JOIN #SurveyDeletes d
			on ac.SurveyID = d.SurveyID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.SurveyID
INTO
	#SurveyInserts
FROM
	dbo.Survey pC
EXCEPT
SELECT
	ac.SurveyID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Survey aC
	--processing.VanessaTempdb.dbo.Survey aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.SurveyID
	,pC.SurveyPassCount
	,pC.EmbeddedSurveyInd
	,pC.CreateDate
INTO	
	#SurveyUpdates
FROM
	dbo.Survey pC
EXCEPT
SELECT
	aC.SurveyID
	,aC.SurveyPassCount
	,aC.EmbeddedSurveyInd
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Survey aC
	--processing.VanessaTempdb.dbo.Survey aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Survey (
		alpha.CCH_CaesarsHumanaWeb.dbo.Survey (
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
			on pC.SurveyID = i.SurveyID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #SurveyUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Survey
		alpha.CCH_CaesarsHumanaWeb.dbo.Survey
	SET
		SurveyID = pC.SurveyID
		,SurveyPassCount = pC.SurveyPassCount
		,EmbeddedSurveyInd = pC.EmbeddedSurveyInd
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Survey aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Survey aC
		INNER JOIN dbo.Survey pC
			ON ac.SurveyID = pC.SurveyID
		INNER JOIN #SurveyUpdates u
			on pC.SurveyID = u.SurveyID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--Resource
------------------------------------------------------------
IF	object_id('tempdb..#ResourceDeletes') IS NOT NULL
DROP TABLE #ResourceDeletes

IF	object_id('tempdb..#ResourceInserts') IS NOT NULL
DROP TABLE #ResourceInserts

IF	object_id('tempdb..#ResourceUpdates') IS NOT NULL
DROP TABLE #ResourceUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ResourceID
INTO
	#ResourceDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.Resource aC
	--processing.VanessaTempdb.dbo.Resource aC
EXCEPT
SELECT
	pc.ResourceID
FROM
	dbo.Resource pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ResourceDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.Resource 
		--processing.VanessaTempdb.dbo.Resource
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.Resource aC
		--processing.VanessaTempdb.dbo.Resource aC
		INNER JOIN #ResourceDeletes d
			on ac.ResourceID = d.ResourceID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ResourceID
INTO
	#ResourceInserts
FROM
	dbo.Resource pC
EXCEPT
SELECT
	ac.ResourceID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Resource aC
	--processing.VanessaTempdb.dbo.Resource aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ResourceID
	,pC.ResourceName
	,pC.ResourceIconFileName
	,pC.ResourceVideoImageURL
	,pC.ResourceDesc
	,pC.ResourcePhoneNum
	,pC.ResourceURL
	,pC.CreateDate
INTO	
	#ResourceUpdates
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
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.Resource aC
	--processing.VanessaTempdb.dbo.Resource aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.Resource (
		alpha.CCH_CaesarsHumanaWeb.dbo.Resource (
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
			on pC.ResourceID = i.ResourceID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ResourceUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.Resource
		alpha.CCH_CaesarsHumanaWeb.dbo.Resource
	SET
		ResourceID = pC.ResourceID
		,ResourceName = pC.ResourceName
		,ResourceIconFileName = pC.ResourceIconFileName
		,ResourceVideoImageURL = pC.ResourceVideoImageURL
		,ResourceDesc = pC.ResourceDesc
		,ResourcePhoneNum = pC.ResourcePhoneNum
		,ResourceURL = pC.ResourceURL
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.Resource aC
		alpha.CCH_CaesarsHumanaWeb.dbo.Resource aC
		INNER JOIN dbo.Resource pC
			ON ac.ResourceID = pC.ResourceID
		INNER JOIN #ResourceUpdates u
			on pC.ResourceID = u.ResourceID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--RelatedContent
------------------------------------------------------------
IF	object_id('tempdb..#RelatedContentDeletes') IS NOT NULL
DROP TABLE #RelatedContentDeletes

IF	object_id('tempdb..#RelatedContentInserts') IS NOT NULL
DROP TABLE #RelatedContentInserts

IF	object_id('tempdb..#RelatedContentUpdates') IS NOT NULL
DROP TABLE #RelatedContentUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ParentContentID
	,aC.RelatedContentID
INTO
	#RelatedContentDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent aC
	--processing.VanessaTempdb.dbo.RelatedContent aC
EXCEPT
SELECT
	pc.ParentContentID
	,pc.RelatedContentID
FROM
	dbo.RelatedContent pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #RelatedContentDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent 
		--processing.VanessaTempdb.dbo.RelatedContent
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent aC
		--processing.VanessaTempdb.dbo.RelatedContent aC
		INNER JOIN #RelatedContentDeletes d
			on ac.RelatedContentID = d.RelatedContentID
			and ac.ParentContentID = d.ParentContentID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ParentContentID
	,pC.RelatedContentID
INTO
	#RelatedContentInserts
FROM
	dbo.RelatedContent pC
EXCEPT
SELECT
	aC.ParentContentID
	,aC.RelatedContentID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent aC
	--processing.VanessaTempdb.dbo.RelatedContent aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ParentContentID
	,pC.RelatedContentID
	,pC.RelatedContentDisplayRuleID
	,pC.CreateDate
INTO	
	#RelatedContentUpdates
FROM
	dbo.RelatedContent pC
EXCEPT
SELECT
	aC.ParentContentID
	,aC.RelatedContentID
	,aC.RelatedContentDisplayRuleID
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent aC
	--processing.VanessaTempdb.dbo.RelatedContent aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.RelatedContent (
		alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent (
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
			AND pC.ParentContentID = i.ParentContentID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #RelatedContentUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.RelatedContent
		alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent
	SET
		ParentContentID = pC.ParentContentID
		,RelatedContentID = pC.RelatedContentID
		,RelatedContentDisplayRuleID = pC.RelatedContentDisplayRuleID
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.RelatedContent aC
		alpha.CCH_CaesarsHumanaWeb.dbo.RelatedContent aC
		INNER JOIN dbo.RelatedContent pC
			ON ac.RelatedContentID = pC.RelatedContentID
			AND ac.ParentContentID = pC.ParentContentID
		INNER JOIN #RelatedContentUpdates u
			on pC.RelatedContentID = u.RelatedContentID
			AND pC.ParentContentID = u.ParentContentID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--SurveyQuestion
------------------------------------------------------------
IF	object_id('tempdb..#SurveyQuestionDeletes') IS NOT NULL
DROP TABLE #SurveyQuestionDeletes

IF	object_id('tempdb..#SurveyQuestionInserts') IS NOT NULL
DROP TABLE #SurveyQuestionInserts

IF	object_id('tempdb..#SurveyQuestionUpdates') IS NOT NULL
DROP TABLE #SurveyQuestionUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.SurveyID
	,aC.QuestionID
INTO
	#SurveyQuestionDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion aC
	--processing.VanessaTempdb.dbo.SurveyQuestion aC
EXCEPT
SELECT
	pc.SurveyID
	,pc.QuestionID
FROM
	dbo.SurveyQuestion pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #SurveyQuestionDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion 
		--processing.VanessaTempdb.dbo.SurveyQuestion
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion aC
		--processing.VanessaTempdb.dbo.SurveyQuestion aC
		INNER JOIN #SurveyQuestionDeletes d
			on ac.QuestionID = d.QuestionID
			and ac.SurveyID = d.SurveyID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.SurveyID
	,pC.QuestionID
INTO
	#SurveyQuestionInserts
FROM
	dbo.SurveyQuestion pC
EXCEPT
SELECT
	aC.SurveyID
	,aC.QuestionID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion aC
	--processing.VanessaTempdb.dbo.SurveyQuestion aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.SurveyID
	,pC.QuestionID
	,pC.QuestionDisplayOrderNum
	,pC.CreateDate
INTO	
	#SurveyQuestionUpdates
FROM
	dbo.SurveyQuestion pC
EXCEPT
SELECT
	aC.SurveyID
	,aC.QuestionID
	,aC.QuestionDisplayOrderNum
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion aC
	--processing.VanessaTempdb.dbo.SurveyQuestion aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.SurveyQuestion (
		alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion (
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
			AND pC.SurveyID = i.SurveyID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #SurveyQuestionUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.SurveyQuestion
		alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion
	SET
		SurveyID = pC.SurveyID
		,QuestionID = pC.QuestionID
		,QuestionDisplayOrderNum = pC.QuestionDisplayOrderNum
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.SurveyQuestion aC
		alpha.CCH_CaesarsHumanaWeb.dbo.SurveyQuestion aC
		INNER JOIN dbo.SurveyQuestion pC
			ON ac.QuestionID = pC.QuestionID
			AND ac.SurveyID = pC.SurveyID
		INNER JOIN #SurveyQuestionUpdates u
			on pC.QuestionID = u.QuestionID
			AND pC.SurveyID = u.SurveyID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--QuestionAnswer
------------------------------------------------------------
IF	object_id('tempdb..#QuestionAnswerDeletes') IS NOT NULL
DROP TABLE #QuestionAnswerDeletes

IF	object_id('tempdb..#QuestionAnswerInserts') IS NOT NULL
DROP TABLE #QuestionAnswerInserts

IF	object_id('tempdb..#QuestionAnswerUpdates') IS NOT NULL
DROP TABLE #QuestionAnswerUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.QuestionID
	,aC.AnswerID
INTO
	#QuestionAnswerDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer aC
	--processing.VanessaTempdb.dbo.QuestionAnswer aC
EXCEPT
SELECT
	pc.QuestionID
	,pc.AnswerID
FROM
	dbo.QuestionAnswer pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #QuestionAnswerDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer 
		--processing.VanessaTempdb.dbo.QuestionAnswer
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer aC
		--processing.VanessaTempdb.dbo.QuestionAnswer aC
		INNER JOIN #QuestionAnswerDeletes d
			on ac.AnswerID = d.AnswerID
			and ac.QuestionID = d.QuestionID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.QuestionID
	,pC.AnswerID
INTO
	#QuestionAnswerInserts
FROM
	dbo.QuestionAnswer pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.AnswerID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer aC
	--processing.VanessaTempdb.dbo.QuestionAnswer aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.QuestionID
	,pC.AnswerID
	,pC.AnswerDisplayOrderNum
	,pC.CorrectAnswerInd
	,pC.CreateDate
INTO	
	#QuestionAnswerUpdates
FROM
	dbo.QuestionAnswer pC
EXCEPT
SELECT
	aC.QuestionID
	,aC.AnswerID
	,aC.AnswerDisplayOrderNum
	,aC.CorrectAnswerInd
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer aC
	--processing.VanessaTempdb.dbo.QuestionAnswer aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.QuestionAnswer (
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer (
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
			AND pC.QuestionID = i.QuestionID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #QuestionAnswerUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.QuestionAnswer
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer
	SET
		QuestionID = pC.QuestionID
		,AnswerID = pC.AnswerID
		,AnswerDisplayOrderNum = pC.AnswerDisplayOrderNum
		,CorrectAnswerInd = pC.CorrectAnswerInd
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.QuestionAnswer aC
		alpha.CCH_CaesarsHumanaWeb.dbo.QuestionAnswer aC
		INNER JOIN dbo.QuestionAnswer pC
			ON ac.AnswerID = pC.AnswerID
			AND ac.QuestionID = pC.QuestionID
		INNER JOIN #QuestionAnswerUpdates u
			on pC.AnswerID = u.AnswerID
			AND pC.QuestionID = u.QuestionID
END
/***************************************************************************/

/***************************************************************************/
------------------------------------------------------------
--ContentTypeState
------------------------------------------------------------
IF	object_id('tempdb..#ContentTypeStateDeletes') IS NOT NULL
DROP TABLE #ContentTypeStateDeletes

IF	object_id('tempdb..#ContentTypeStateInserts') IS NOT NULL
DROP TABLE #ContentTypeStateInserts

IF	object_id('tempdb..#ContentTypeStateUpdates') IS NOT NULL
DROP TABLE #ContentTypeStateUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.ContentTypeID
	,aC.ContentStatusID
INTO
	#ContentTypeStateDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState aC
	--processing.VanessaTempdb.dbo.ContentTypeState aC
EXCEPT
SELECT
	pc.ContentTypeID
	,pc.ContentStatusID
FROM
	dbo.ContentTypeState pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #ContentTypeStateDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState 
		--processing.VanessaTempdb.dbo.ContentTypeState
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState aC
		--processing.VanessaTempdb.dbo.ContentTypeState aC
		INNER JOIN #ContentTypeStateDeletes d
			on ac.ContentStatusID = d.ContentStatusID
			and ac.ContentTypeID = d.ContentTypeID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.ContentTypeID
	,pC.ContentStatusID
INTO
	#ContentTypeStateInserts
FROM
	dbo.ContentTypeState pC
EXCEPT
SELECT
	aC.ContentTypeID
	,aC.ContentStatusID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState aC
	--processing.VanessaTempdb.dbo.ContentTypeState aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.ContentTypeID
	,pC.ContentStatusID
	,pC.InitialStateInd
	,pC.EndStateInd
	,pC.ContentStatusCaptionText
	,pC.CreateDate
INTO	
	#ContentTypeStateUpdates
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
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState aC
	--processing.VanessaTempdb.dbo.ContentTypeState aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.ContentTypeState (
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState (
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
			AND pC.ContentTypeID = i.ContentTypeID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #ContentTypeStateUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.ContentTypeState
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState
	SET
		ContentTypeID = pC.ContentTypeID
		,ContentStatusID = pC.ContentStatusID
		,InitialStateInd = pC.InitialStateInd
		,EndStateInd = pC.EndStateInd
		,ContentStatusCaptionText = pC.ContentStatusCaptionText
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.ContentTypeState aC
		alpha.CCH_CaesarsHumanaWeb.dbo.ContentTypeState aC
		INNER JOIN dbo.ContentTypeState pC
			ON ac.ContentStatusID = pC.ContentStatusID
			AND ac.ContentTypeID = pC.ContentTypeID
		INNER JOIN #ContentTypeStateUpdates u
			on pC.ContentStatusID = u.ContentStatusID
			AND pC.ContentTypeID = u.ContentTypeID
END
/***************************************************************************/
/***************************************************************************/
------------------------------------------------------------
--UserContentPreference
------------------------------------------------------------
IF	object_id('tempdb..#UserContentPreferenceDeletes') IS NOT NULL
DROP TABLE #UserContentPreferenceDeletes

IF	object_id('tempdb..#UserContentPreferenceInserts') IS NOT NULL
DROP TABLE #UserContentPreferenceInserts

IF	object_id('tempdb..#UserContentPreferenceUpdates') IS NOT NULL
DROP TABLE #UserContentPreferenceUpdates
-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.CCHID
INTO
	#UserContentPreferenceDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference aC
	--processing.VanessaTempdb.dbo.UserContentPreference aC
EXCEPT
SELECT
	pc.CCHID
FROM
	dbo.UserContentPreference pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentPreferenceDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference 
		--processing.VanessaTempdb.dbo.UserContentPreference
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference aC
		--processing.VanessaTempdb.dbo.UserContentPreference aC
		INNER JOIN #UserContentPreferenceDeletes d
			on ac.CCHID = d.CCHID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.CCHID
INTO
	#UserContentPreferenceInserts
FROM
	dbo.UserContentPreference pC
EXCEPT
SELECT
	ac.CCHID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference aC
	--processing.VanessaTempdb.dbo.UserContentPreference aC
-----------------------------------------------------------
--Find Records with different values for same PK (Updates)
-----------------------------------------------------------
SELECT
	pC.CCHID
	,pC.SMSInd
	,pC.EmailInd
	,pC.OSBasedAlertInd
	,pC.LastUpdateDate
	,pC.CreateDate
INTO	
	#UserContentPreferenceUpdates
FROM
	dbo.UserContentPreference pC
EXCEPT
SELECT
	aC.CCHID
	,aC.SMSInd
	,aC.EmailInd
	,aC.OSBasedAlertInd
	,aC.LastUpdateDate
	,aC.CreateDate
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference aC
	--processing.VanessaTempdb.dbo.UserContentPreference aC
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
	INSERT INTO
		--processing.VanessaTempdb.dbo.UserContentPreference (
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference (
			CCHID
			,SMSInd
			,EmailInd
			,OSBasedAlertInd
			,LastUpdateDate
			,CreateDate
		)
	SELECT
		pC.CCHID
		,pC.SMSInd
		,pC.EmailInd
		,pC.OSBasedAlertInd
		,pC.LastUpdateDate
		,pC.CreateDate
	FROM
		dbo.UserContentPreference pC
		INNER JOIN #UserContentPreferenceInserts i
			on pC.CCHID = i.CCHID
END
----------------------------------------------------------------
--Now process the Updates if there are any
-----------------------------------------------------------------
IF ((SELECT COUNT(1) FROM #UserContentPreferenceUpdates) > 0)
BEGIN
	UPDATE
		--processing.VanessaTempdb.dbo.UserContentPreference
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference
	SET
		CCHID = pC.CCHID
		,SMSInd = pC.SMSInd
		,EmailInd = pC.EmailInd
		,OSBasedAlertInd = pC.OSBasedAlertInd
		,LastUpdateDate = pC.LastUpdateDate
		,CreateDate = pC.CreateDate
	FROM
		--processing.VanessaTempdb.dbo.UserContentPreference aC
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContentPreference aC
		INNER JOIN dbo.UserContentPreference pC
			ON ac.CCHID = pC.CCHID
		INNER JOIN #UserContentPreferenceUpdates u
			on pC.CCHID = u.CCHID
END
/***************************************************************************/
------------------------------------------------------------------------------
--Delete / Insert only on Primary Key
------------------------------------------------------------------------------
/***************************************************************************/
------------------------------------------------------------
--CampaignContent
------------------------------------------------------------
IF	object_id('tempdb..#CampaignContentDeletes') IS NOT NULL
DROP TABLE #CampaignContentDeletes

IF	object_id('tempdb..#CampaignContentInserts') IS NOT NULL
DROP TABLE #CampaignContentInserts

-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.CampaignID
	,aC.ContentID
INTO
	#CampaignContentDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.CampaignContent aC
	--processing.VanessaTempdb.dbo.CampaignContent aC
EXCEPT
SELECT
	pc.CampaignID
	,pc.ContentID
FROM
	dbo.CampaignContent pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignContentDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.CampaignContent 
		--processing.VanessaTempdb.dbo.CampaignContent
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.CampaignContent aC
		--processing.VanessaTempdb.dbo.CampaignContent aC
		INNER JOIN #CampaignContentDeletes d
			on ac.ContentID = d.ContentID
			and ac.CampaignID = d.CampaignID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.CampaignID
	,pC.ContentID
INTO
	#CampaignContentInserts
FROM
	dbo.CampaignContent pC
EXCEPT
SELECT
	aC.CampaignID
	,aC.ContentID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.CampaignContent aC
	--processing.VanessaTempdb.dbo.CampaignContent aC
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignContentInserts) >0)
BEGIN	
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	INSERT INTO
		--processing.VanessaTempdb.dbo.CampaignContent (
		alpha.CCH_CaesarsHumanaWeb.dbo.CampaignContent (
			CampaignID
			,ContentID
			,CreateDate
		)
	SELECT
		pC.CampaignID
		,pC.ContentID
		,pC.CreateDate
	FROM
		dbo.CampaignContent pC
		INNER JOIN #CampaignContentInserts i
			on pC.ContentID = i.ContentID
			AND pC.CampaignID = i.CampaignID
END

/***************************************************************************/
------------------------------------------------------------
--CampaignMember
------------------------------------------------------------
IF	object_id('tempdb..#CampaignMemberDeletes') IS NOT NULL
DROP TABLE #CampaignMemberDeletes

IF	object_id('tempdb..#CampaignMemberInserts') IS NOT NULL
DROP TABLE #CampaignMemberInserts

-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.CampaignID
	,aC.CCHID
INTO
	#CampaignMemberDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.CampaignMember aC
	--processing.VanessaTempdb.dbo.CampaignMember aC
EXCEPT
SELECT
	pc.CampaignID
	,pc.CCHID
FROM
	dbo.CampaignMember pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignMemberDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.CampaignMember 
		--processing.VanessaTempdb.dbo.CampaignMember
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.CampaignMember aC
		--processing.VanessaTempdb.dbo.CampaignMember aC
		INNER JOIN #CampaignMemberDeletes d
			on ac.CCHID = d.CCHID
			and ac.CampaignID = d.CampaignID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.CampaignID
	,pC.CCHID
INTO
	#CampaignMemberInserts
FROM
	dbo.CampaignMember pC
EXCEPT
SELECT
	aC.CampaignID
	,aC.CCHID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.CampaignMember aC
	--processing.VanessaTempdb.dbo.CampaignMember aC
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #CampaignMemberInserts) >0)
BEGIN	
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	INSERT INTO
		--processing.VanessaTempdb.dbo.CampaignMember (
		alpha.CCH_CaesarsHumanaWeb.dbo.CampaignMember (
			CampaignID
			,CCHID
			,Savings
			,Score
			,CreateDate
		)
	SELECT
		pC.CampaignID
		,pC.CCHID
		,pC.Savings
		,pC.Score
		,pC.CreateDate
	FROM
		dbo.CampaignMember pC
		INNER JOIN #CampaignMemberInserts i
			on pC.CCHID = i.CCHID
			AND pC.CampaignID = i.CampaignID
END
/***************************************************************************/
------------------------------------------------------------
--UserContent
------------------------------------------------------------
IF	object_id('tempdb..#UserContentDeletes') IS NOT NULL
DROP TABLE #UserContentDeletes

IF	object_id('tempdb..#UserContentInserts') IS NOT NULL
DROP TABLE #UserContentInserts

-----------------------------------------------------------
--Find PKs in the target table that don't exist anymore (Deletes)
-----------------------------------------------------------
SELECT
	aC.CampaignID
	,aC.CCHID
	,aC.ContentID
INTO
	#UserContentDeletes
FROM
	alpha.CCH_CaesarsHumanaWeb.dbo.UserContent aC
	--processing.VanessaTempdb.dbo.UserContent aC
EXCEPT
SELECT
	pc.CampaignID
	,pc.CCHID
	,pC.ContentID
FROM
	dbo.UserContent pC
-----------------------------------------------------------
--Process the Deletes if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentDeletes) >0)
BEGIN	
	DELETE
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContent 
		--processing.VanessaTempdb.dbo.UserContent
	FROM	
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContent aC
		--processing.VanessaTempdb.dbo.UserContent aC
		INNER JOIN #UserContentDeletes d
			on ac.CCHID = d.CCHID
			and ac.CampaignID = d.CampaignID
			and ac.ContentID = d.ContentID
END
-----------------------------------------------------------
--Find new PKs that aren't in target table yet (Inserts)
-----------------------------------------------------------
SELECT
	pC.CampaignID
	,pC.CCHID
	,pC.ContentID
INTO
	#UserContentInserts
FROM
	dbo.UserContent pC
EXCEPT
SELECT
	aC.CampaignID
	,aC.CCHID
	,aC.ContentID
FROM
alpha.CCH_CaesarsHumanaWeb.dbo.UserContent aC
	--processing.VanessaTempdb.dbo.UserContent aC
-----------------------------------------------------------
--Process the Inserts if they exist
-----------------------------------------------------------

IF ((SELECT COUNT(1) FROM #UserContentInserts) >0)
BEGIN	
------------------------------------------------------------
--Insert the new rows
-------------------------------------------------------------	
	INSERT INTO
		--processing.VanessaTempdb.dbo.UserContent (
		alpha.CCH_CaesarsHumanaWeb.dbo.UserContent (
			CampaignID
			,CCHID
			,ContentID
			,ContentStatusChangeDate
			,UserContentCommentText
			,NotificationSentDate
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
		,pC.ContentSavingsAmt
		,pC.MemberContentDataText
		,pC.ContentStatusID
		,pC.CreateDate
	FROM
		dbo.UserContent pC
		INNER JOIN #UserContentInserts i
			on pC.CCHID = i.CCHID
			AND pC.CampaignID = i.CampaignID
			AND pC.ContentID = i.ContentID
END

END --env
END -- proc

GO

