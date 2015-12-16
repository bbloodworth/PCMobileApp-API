/****** Object:  StoredProcedure [dbo].[p_InsertCampaign]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertCampaign]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertCampaign]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Campaign
      
Declarations:
            
Execute:
      exec p_InsertCampaign
		@CampaignIDDesc = 'Healthplan Explainer',
		@CampaignActiveInd = 1
		@TargetPopulationDesc = 'All Caesars Employees'
		@CampaignPeriodDesc = 'Monthly'
		@TargetProcedureName = 'p_GetHealthplanExplainerPop'


Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertCampaign] (
	@CampaignDesc nvarchar(2000),
	@CampaignActiveInd bit = 1,
	@TargetPopulationDesc nvarchar(2000) = null,
	@CampaignPeriodDesc nvarchar(100) = null,
	@TargetProcedureName nvarchar(50) = null,
	@AuthRequiredInd bit = 1,
	@SavingsMonthStartDate datetime = null,
	@CampaignURL nvarchar(100) = null
)
as

BEGIN
	INSERT INTO dbo.Campaign (
		CampaignDesc,
		CampaignActiveInd,
		TargetPopulationDesc,
		CampaignPeriodDesc,
		TargetProcedureName,
		AuthRequiredInd,
		SavingsMonthStartDate,
		CampaignURL,
		CreateDate)
	VALUES (
		@CampaignDesc,
		@CampaignActiveInd,
		@TargetPopulationDesc,
		@CampaignPeriodDesc,
		@TargetProcedureName,
		@AuthRequiredInd,
		@SavingsMonthStartDate,
		@CampaignURL,
		getdate())
		
END
 
GO

