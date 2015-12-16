/****** Object:  StoredProcedure [dbo].[p_InsertCampaign0]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertCampaign0]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertCampaign0]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-03-16
Description:
      Insert Campaign 0
      
Declarations:
            
Execute:
      exec p_InsertCampaign0
		@CampaignIDDesc = 'Employer Notifications',
		@CampaignActiveInd = 0
		@TargetPopulationDesc = 'All Caesars Employees'
		@CampaignPeriodDesc = 'Monthly'
		@TargetProcedureName = 'p_GetHealthplanExplainerPop'


Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-03-16  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertCampaign0] 
AS

BEGIN
	IF NOT EXISTS (SELECT 1 FROM dbo.Campaign where CampaignID = 0)
	BEGIN
	
		IF (SELECT OBJECTPROPERTY(object_id('dbo.Campaign'), 'TableHasIdentity')) = 1
		BEGIN
			SET IDENTITY_INSERT dbo.Campaign ON
		END
		
		INSERT INTO dbo.Campaign (
			CampaignID
			,CampaignDesc
			,CampaignActiveInd
			,TargetPopulationDesc
			,CampaignPeriodDesc
			,AuthRequiredInd
			,CreateDate)
		VALUES (
			0
			,'Employer Notifications'
			,1
			,'All Employees'
			,'Ongoing'
			,0
			,getdate())
			
		IF (SELECT OBJECTPROPERTY(object_id('dbo.Campaign'), 'TableHasIdentity')) = 1
			BEGIN		
				SET IDENTITY_INSERT dbo.Campaign OFF
			END
	END --if		
END
 
GO

