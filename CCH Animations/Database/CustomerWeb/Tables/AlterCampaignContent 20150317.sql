IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'ActivationDate')
	ALTER TABLE CampaignContent
	ADD ActivationDate datetime NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'ExpirationDate')
	ALTER TABLE CampaignContent
	ADD ExpirationDate datetime NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'UserContentInd')
	ALTER TABLE CampaignContent
	ADD UserContentInd bit NOT NULL default 1
	
GO

