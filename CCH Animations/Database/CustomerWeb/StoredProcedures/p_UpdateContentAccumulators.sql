/****** Object:  StoredProcedure [dbo].[p_UpdateContentAccumulators]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_UpdateContentAccumulators]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_UpdateContentAccumulators]
GO

/****** Object:  StoredProcedure [dbo].[p_UpdateContentAccumulators]    Script Date: 07/10/2015 13:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-08-31
Description:
     Checks UserContent for any content associated with a CCHID with an AccumulatorsInd of 1, and then updates the JSON string with the memberdata JSON string needed for the Animation in the Benefit Plan 
     the JSON string for that table with the most recent accumulators data
      
Declarations:
      
Execute:
      exec p_UpdateContentAccumulators
	
Objects Listing:

Tables- dbo.UserContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-08-19  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_UpdateContentAccumulators] (
	@CCHID int
)
as

BEGIN --proc
----------------------------------------------------------
--Declarations
-----------------------------------------------------------
	DECLARE
		@CampaignID int
		,@ContentID int
		,@ContentTypeID int
		,@DataSproc nvarchar(100)
	
IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' and name = 'UserContent')
BEGIN --exists
----------------------------------------------------------
--Get the list of campaigns / contents / data sprocs
-----------------------------------------------------------
	SELECT DISTINCT 
		uc.CampaignID
		,uc.ContentID
		,c.ContentTypeID
		,a.AnimationDataProcName
	INTO #Content
	FROM
		UserContent uc
		INNER JOIN Content c 
			on uc.ContentID = c.ContentID
		LEFT JOIN Animation a
			on c.ContentID = a.AnimationID
	WHERE
		uc.CCHID = @CCHID
		AND c.AccumulatorsInd = 1

------------------------------------------------
--Update User Content with the appropriate JSON string
------------------------------------------------
	WHILE (SELECT COUNT(*) FROM #Content) > 0
	BEGIN --WHILE
		SELECT TOP 1
			@CampaignID = CampaignID
			,@ContentID = ContentID
			,@ContentTypeID = ContentTypeID
			,@DataSproc = AnimationDataProcName
		FROM
			#Content	
	
		IF @DataSproc = 'p_GetBenefitPlanExplainerMemberData'
		BEGIN --IF
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
				
			SELECT
				@EmployerID = EmployerID 
			FROM
				ControlData
	
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
	-------------------------------------------------------------
	--UPDATE the JSON String
	--------------------------------------------------------------		
			;with q1 as
			(
			SELECT 
				e.EmployeeID,
				case WHEN Count(e.EmployeeID) = 1 
				THEN 'Individual'
				ELSE 'Family'
				END As PlanType
			FROM Enrollments e
			GROUP BY e.EmployeeID
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
				INNER JOIN Enrollments e
					on uc.CCHID = e.CCHID
				LEFT JOIN BenefitPlans bp
					on e.MedicalPlanType = bp.PlanType
				INNER JOIN q1 
					on e.EmployeeID = q1.EmployeeID
				LEFT JOIN Accumulators a
					on e.CCHID = a.CCHID
			WHERE
				e.CCHID = @CCHID
				AND uc.CampaignID = @CampaignID
				AND uc.ContentID = @ContentID
		END -- IF
			
		DELETE 
			#Content
		WHERE 1=1
			AND CampaignID = @CampaignID
			AND ContentID = @ContentID
			AND ContentTypeID = @ContentTypeID
			AND AnimationDataProcName = @DataSproc
	END --WHILE	
END --exists
END --proc
 

GO


