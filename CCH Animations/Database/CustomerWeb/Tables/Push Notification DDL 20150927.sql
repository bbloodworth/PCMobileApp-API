IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'ContentTranslation' and name = 'OSNotificationText')
	ALTER TABLE ContentTranslation
	ADD OSNotificationText nvarchar(2000) NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'UserContent' and name = 'OSNotificationStatusDesc')
	ALTER TABLE UserContent
	ADD OSNotificationStatusDesc nvarchar(100) NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'OSNotificationStatusDesc')
	ALTER TABLE CampaignContent
	ADD OSNotificationStatusDesc nvarchar(100) NULL

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'OSNotificationSentDate')
	ALTER TABLE CampaignContent
	ADD OSNotificationSentDate datetime NULL