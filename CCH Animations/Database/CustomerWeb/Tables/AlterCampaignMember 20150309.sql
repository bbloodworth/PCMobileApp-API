IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignMember' and name = 'YourCostSavingsAmt')
	ALTER TABLE CampaignMember
	ADD YourCostSavingsAmt money NULL
	
--If needed to fix other columns use this example	
--EXEC sp_rename 'CCH_StarbucksWeb.dbo.CampaignMember.YourCostSavingsAmount', 'YourCostSavingsAmt', 'COLUMN';

GO

