/****** Object:  StoredProcedure [dbo].[p_GetBenefitPlanExplainerMemberData]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetBenefitPlanExplainerMemberData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetBenefitPlanExplainerMemberData]
GO

/****** Object:  StoredProcedure [dbo].[p_GetBenefitPlanExplainerMemberData]    Script Date: 07/10/2015 13:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-08-19
Description:
     Updates UserContent with the memberdata JSON string needed for the Animation in the Benefit Plan 
     Explainer campaign
      
Declarations:
      
Execute:
      exec p_GetBenefitPlanExplainerMemberData
	
Objects Listing:

Tables- dbo.UserContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-08-19  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetBenefitPlanExplainerMemberData] (
	@CampaignID int
	,@ContentID int
)
as

BEGIN --proc
----------------------------------------------------------
--Declarations
-----------------------------------------------------------
	DECLARE
		@EmployerURL nvarchar(100)
		--,@EmployerPhone nvarchar(50)
		,@EmployerLogo nvarchar(50)
		,@EmployerName nvarchar(100)
		,@BPEURL nvarchar(100)
		,@EmployerID int
	
---------------------------------------------------------
--Get all single valued data
---------------------------------------------------------		
	SELECT	
		@EmployerURL = ConfigValue
	FROM
		dbo.clientConfig
	WHERE
		ConfigKey = 'EmployerURL'
		
	/*SELECT	
		@EmployerPhone = ConfigValue
	FROM
		dbo.clientConfig
	WHERE
		ConfigKey = 'EmployerPhone'*/
		
	SELECT	
		@EmployerLogo = ConfigValue
	FROM
		dbo.clientConfig
	WHERE
		ConfigKey = 'EmployerLogo'
	
	SELECT @EmployerID = EmployerID
	FROM
		dbo.ControlData
	
	SELECT	
		@EmployerName = ISNULL(p.EmployerName, m.EmployerName)
	FROM
		CCH_FrontEnd2.dbo.Employers m
			LEFT JOIN CCH_FrontEnd2.dbo.Employers p
			ON m.MainEmployerID = p.EmployerID
	WHERE m.EmployerID = @EmployerID
		
	SELECT
		@BPEURL = CampaignURL 
	FROM
		dbo.Campaign
	WHERE
		CampaignID = @CampaignID
------------------------------------------------
--Update User Content with the JSON string
------------------------------------------------
	;with q1 as
	(
		SELECT 
		e.EmployeeID,
		case WHEN Count(e.EmployeeID) = 1 
		THEN 'Individual'
		ELSE 'Family'
		END As PlanType
		FROM Enrollments e
		group by e.EmployeeID
	)
	UPDATE UserContent
	SET MemberContentDataText =
		'{' +
		'"deductible": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,CASE WHEN q1.PlanType = 'Individual' THEN bp.DeductibleIndividual ELSE bp.DeductibleFamily END))),'null') + 
		',"ytdspent": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,CASE WHEN q1.PlanType = 'Individual' THEN a.OutOfPocketMaxIndividual ELSE a.OutOfPocketMaxFamily END))),'null') + 
		',"copay": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,bp.CoPay))),'null') + 
		',"coinsurance": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,bp.CoInsurance * 100))),'null') + 
		',"coinsuranceComplement": "' + ISNULL(LTRIM(RTRIM(Cast((100 - (bp.CoInsurance * 100)) as varchar(5)) )),'null') + '"' +
		',"oopMax": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,CASE WHEN q1.PlanType = 'Individual' THEN bp.OutOfPocketIndividual ELSE bp.OutOfPocketFamily END))),'null') + 
		',"leftLinkURL": "' + @BPEURL +  + '"' +
		',"rightLinkURL": "' + @EmployerURL + '/sign_in.aspx?dest=pc"' +
		',"EmployerName": "' + @EmployerName + '"' +
		'}'
	FROM
		UserContent uc
		INNER JOIN CampaignMember cm
			on uc.CCHID = cm.CCHID
			and uc.CampaignID = cm.CampaignID
		INNER JOIN Enrollments e
			on cm.CCHID = e.CCHID
		LEFT JOIN BenefitPlans bp
			on e.MedicalPlanType = bp.PlanType 
		INNER JOIN q1 
			on e.EmployeeID = q1.EmployeeID
		LEFT JOIN Accumulators a
			on e.CCHID = a.CCHID
	WHERE
		uc.CampaignID = @CampaignID
		AND uc.ContentID = @ContentID

END
 

GO


