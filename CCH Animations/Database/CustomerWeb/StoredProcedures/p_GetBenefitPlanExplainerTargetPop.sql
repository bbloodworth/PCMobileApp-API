
/****** Object:  StoredProcedure [dbo].[p_GetBenefitPlanExplainerTargetPop]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetBenefitPlanExplainerTargetPop]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetBenefitPlanExplainerTargetPop]
GO

/****** Object:  StoredProcedure [dbo].[p_GetBenefitPlanExplainerTargetPop]    Script Date: 07/10/2015 13:24:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-08-17
Description:
     Identifies the target Pop of CCHIDs for the Benefit Plan Explainer campaign and populates the
     CampainMember table.  Upddates the sproc name on the Campaign.
      
Declarations:
      
Execute:
      exec p_GetBenefitPlanExplainerTargetPop
		@CampaignID = 7
	
Objects Listing:

Tables- dbo.CampaignContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-08-17  AS		 Create
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetBenefitPlanExplainerTargetPop] (
	@CampaignID int
)
as

BEGIN --proc

------------------------------------------------------
--Insert the Target Pop Members
------------------------------------------------
		INSERT INTO dbo.CampaignMember (
			CampaignID
			,CCHID
			,CreateDate)
		SELECT
			@CampaignID
			,e.CCHID
			,getdate()
		FROM
			dbo.Enrollments e
			INNER JOIN dbo.PersonApplicationAccess p 
				on e.CCHID = p.CCHID
		WHERE
			p.CCHApplicationID = 1 
			AND GETDATE() >= ISNULL(p.AccessStartDate,DATEADD(day,-1,GETDATE()))
			AND GETDATE() < ISNULL(p.AccessTerminationDate,DATEADD(day,+1,GETDATE()))
			AND DATEDIFF(month, e.DateOfBirth, getdate())/12 >= 18 --must be 18 or older
		
-----------------------------------------------------
--Update the Campaign table with this sproc name
------------------------------------------------------
	UPDATE
		Campaign
	SET
		TargetProcedureName = 'p_GetBenefitPlanExplainerTargetPop'
	WHERE
		CampaignID = @CampaignID

END


GO
