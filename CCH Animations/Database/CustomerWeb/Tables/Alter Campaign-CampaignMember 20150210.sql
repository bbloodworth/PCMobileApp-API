ALTER TABLE Campaign
ADD SavingsMonthStartDate datetime

ALTER TABLE CampaignMember
ADD 
	Savings money,
	Score float
