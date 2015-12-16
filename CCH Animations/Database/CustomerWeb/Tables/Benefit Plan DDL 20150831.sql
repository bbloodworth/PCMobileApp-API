------------------------------------------------------------
--HR App Release 7.0
--Ticket: PC-413
--Date: 2015-08-31
--------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Campaign' and name = 'CampaignURL')
	ALTER TABLE Campaign
	ADD CampaignURL nvarchar(100) NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Content' and name = 'AccumulatorsInd')
	ALTER TABLE Content
	ADD AccumulatorsInd bit NOT NULL default 0
	

--select * from Campaign
--select * from Content