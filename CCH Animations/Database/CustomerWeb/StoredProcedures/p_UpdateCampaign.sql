/****** Object:  StoredProcedure [dbo].[p_UpdateCampaign]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_UpdateCampaign]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_UpdateCampaign]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
     UPdate Campaign
      
Declarations:
            
Execute:
      exec p_UpdateCampaign
		@CampaignID = 1
		@CampaignDesc = 'Healthplan Explainer',
		@CampaignActiveInd = 1,
		@TargetPopulationDesc = 'All Caesars Employees',
		@CampaignPeriodDesc = 'Monthly',
		@TargetProcedureName = 'p_GetHealthplanExplainerPop',
		@CampaignURL = 'www.myplan.com'


Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_UpdateCampaign] (
	@CampaignID int,
	@CampaignDesc nvarchar(100) = null,
	@CampaignActiveInd bit = null,
	@TargetPopulationDesc nvarchar(100) = null,
	@CampaignPeriodDesc nvarchar(100) = null,
	@TargetProcedureName nvarchar(50) = null,
	@AuthRequiredInd bit = null,
	@SavingsMonthStartDate datetime = null,
	@CampaignURL nvarchar(100) = null
)
as

BEGIN
	UPDATE 
		dbo.Campaign 
	SET
		CampaignDesc = ISNULL(@CampaignDesc, CampaignDesc),
		CampaignActiveInd = ISNULL(@CampaignActiveInd, CampaignActiveInd),
		TargetPopulationDesc = ISNULL(@TargetPopulationDesc, TargetPopulationDesc),
		CampaignPeriodDesc = ISNULL(@CampaignPeriodDesc, CampaignPeriodDesc),
		TargetProcedureName = ISNULL(@TargetProcedureName, TargetProcedureName),
		AuthRequiredInd = ISNULL(@AuthRequiredInd, AuthRequiredInd),
		SavingsMonthStartDate = ISNULL(@SavingsMonthStartDate, SavingsMonthStartDate),
		CampaignURL = ISNULL(@CampaignURL, CampaignURL)
	WHERE
		CampaignID = @CampaignID
	
END

GO
 