------------------------------------------------------------
--HR App Release 7.0
--Ticket: PC-429
--Date: 2015-08-20
--------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'EmailNotificationInd')
	ALTER TABLE CampaignContent
	ADD EmailNotificationInd bit NOT NULL default 1
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'SMSNotificationInd')
	ALTER TABLE CampaignContent
	ADD SMSNotificationInd bit NOT NULL default 0
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CampaignContent' and name = 'OSNotificationInd')
	ALTER TABLE CampaignContent
	ADD OSNotificationInd bit NOT NULL default 0
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'UserContent' and name = 'SMSNotificationSentDate')
	ALTER TABLE UserContent
	ADD SMSNotificationSentDate datetime NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'UserContent' and name = 'SMSNotificationStatusDesc')
	ALTER TABLE UserContent
	ADD SMSNotificationStatusDesc nvarchar(100) NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'UserContent' and name = 'OSNotificationSentDate')
	ALTER TABLE UserContent
	ADD OSNotificationSentDate datetime NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'ContentTranslation' and name = 'SMSNotificationText')
	ALTER TABLE ContentTranslation
	ADD SMSNotificationText nvarchar(2000) NULL
	
	


--select * from CampaignContent
--select * from UserContent
--select * from content