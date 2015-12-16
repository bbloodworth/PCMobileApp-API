
-----------------------------------------------------------------------------
--Drop Constraints
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_ContentType_Content' and parent_object_id = OBJECT_ID('dbo.Content'))
ALTER TABLE Content
	DROP CONSTRAINT FK_ContentType_Content 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_Video' and parent_object_id = OBJECT_ID('dbo.Video'))
ALTER TABLE Video
	DROP CONSTRAINT FK_Content_Video 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_Animation' and parent_object_id = OBJECT_ID('dbo.Animation'))
ALTER TABLE Animation
	DROP CONSTRAINT FK_Content_Animation 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Campaign_CampaignContent' and parent_object_id = OBJECT_ID('dbo.CampaignContent'))
ALTER TABLE CampaignContent
	DROP CONSTRAINT FK_Campaign_CampaignContent 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_CampaignContent' and parent_object_id = OBJECT_ID('dbo.CampaignContent'))
ALTER TABLE CampaignContent
	DROP CONSTRAINT FK_Content_CampaignContent 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Campaign_CampaignMember' and parent_object_id = OBJECT_ID('dbo.CampaignMember'))
ALTER TABLE CampaignMember
	DROP CONSTRAINT FK_Campaign_CampaignMember
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_CampaignMember' and parent_object_id = OBJECT_ID('dbo.CampaignMember'))
ALTER TABLE CampaignMember
	DROP CONSTRAINT FK_Enrollments_CampaignMember
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_ContType_ContTypeState' and parent_object_id = OBJECT_ID('dbo.ContentTypeState'))
ALTER TABLE ContentTypeState
	DROP CONSTRAINT FK_ContType_ContTypeState 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_ContStatus_ContTypeState' and parent_object_id = OBJECT_ID('dbo.ContentTypeState'))
ALTER TABLE ContentTypeState
	DROP CONSTRAINT FK_ContStatus_ContTypeState 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_UserContentPref' and parent_object_id = OBJECT_ID('dbo.UserContentPreference'))
ALTER TABLE UserContentPreference
	DROP CONSTRAINT FK_Enrollments_UserContentPref 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_ExperienceLog' and parent_object_id = OBJECT_ID('dbo.ExperienceLog'))
ALTER TABLE ExperienceLog
	DROP CONSTRAINT FK_Enrollments_ExperienceLog 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_ExpEvent_ExpLog' and parent_object_id = OBJECT_ID('dbo.ExperienceLog'))
ALTER TABLE ExperienceLog
	DROP CONSTRAINT FK_ExpEvent_ExpLog 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_QuestionType_Question' and parent_object_id = OBJECT_ID('dbo.Question'))
ALTER TABLE Question
	DROP CONSTRAINT FK_QuestionType_Question 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Answer_QuestionAnswer' and parent_object_id = OBJECT_ID('dbo.QuestionAnswer'))
ALTER TABLE QuestionAnswer
	DROP CONSTRAINT FK_Answer_QuestionAnswer 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Question_QuestionAnswer' and parent_object_id = OBJECT_ID('dbo.QuestionAnswer'))
ALTER TABLE QuestionAnswer
	DROP CONSTRAINT FK_Question_QuestionAnswer 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_RelatedContent_1' and parent_object_id = OBJECT_ID('dbo.RelatedContent'))
ALTER TABLE RelatedContent
	DROP CONSTRAINT FK_Content_RelatedContent_1 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_RelatedContent_2' and parent_object_id = OBJECT_ID('dbo.RelatedContent'))
ALTER TABLE RelatedContent
	DROP CONSTRAINT FK_Content_RelatedContent_2 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_ContDisplayRule_RelatedCont' and parent_object_id = OBJECT_ID('dbo.RelatedContent'))
ALTER TABLE RelatedContent
	DROP CONSTRAINT FK_ContDisplayRule_RelatedCont 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_Survey' and parent_object_id = OBJECT_ID('dbo.Survey'))
ALTER TABLE Survey
	DROP CONSTRAINT FK_Content_Survey 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Survey_SurveyQuestion' and parent_object_id = OBJECT_ID('dbo.SurveyQuestion'))
ALTER TABLE SurveyQuestion
	DROP CONSTRAINT FK_Survey_SurveyQuestion 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Question_SurveyQuestion' and parent_object_id = OBJECT_ID('dbo.SurveyQuestion'))
ALTER TABLE SurveyQuestion
	DROP CONSTRAINT FK_Question_SurveyQuestion 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_UserContent' and parent_object_id = OBJECT_ID('dbo.UserContent'))
ALTER TABLE UserContent
	DROP CONSTRAINT FK_Enrollments_UserContent 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_UserContent' and parent_object_id = OBJECT_ID('dbo.UserContent'))
ALTER TABLE UserContent
	DROP CONSTRAINT FK_Content_UserContent 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_CampaignContent_UserContent' and parent_object_id = OBJECT_ID('dbo.UserContent'))
ALTER TABLE UserContent
	DROP CONSTRAINT FK_CampaignContent_UserContent 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_ContentStatus_UserContent' and parent_object_id = OBJECT_ID('dbo.UserContent'))
ALTER TABLE UserContent
	DROP CONSTRAINT FK_ContentStatus_UserContent 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_UserSurveyAns' and parent_object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
ALTER TABLE UserSurveyAnswer
	DROP CONSTRAINT FK_Enrollments_UserSurveyAns 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_QuestionAns_UserSurveyAns' and parent_object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
ALTER TABLE UserSurveyAnswer
	DROP CONSTRAINT FK_QuestionAns_UserSurveyAns 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_UserContent_UserSurveyAnswer' and parent_object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
ALTER TABLE UserSurveyAnswer
	DROP CONSTRAINT FK_UserContent_UserSurveyAnswer 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_SurveyQuestion_UserSurveyAnswer' and parent_object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
ALTER TABLE UserSurveyAnswer
	DROP CONSTRAINT FK_SurveyQuestion_UserSurveyAnswer 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_ExcludedMember' and parent_object_id = OBJECT_ID('dbo.ExcludedMember'))
ALTER TABLE ExcludedMember
	DROP CONSTRAINT FK_Enrollments_ExcludedMember
go


IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Answer_AnswerTranslation' and parent_object_id = OBJECT_ID('dbo.AnswerTranslation'))
ALTER TABLE [AnswerTranslation]
	DROP CONSTRAINT [FK_Answer_AnswerTranslation] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_AnswerTranslation' and parent_object_id = OBJECT_ID('dbo.AnswerTranslation'))
ALTER TABLE [AnswerTranslation]
	DROP CONSTRAINT [FK_Locale_AnswerTranslation] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Content_ContentTranslation' and parent_object_id = OBJECT_ID('dbo.ContentTranslation'))
ALTER TABLE [ContentTranslation]
	DROP CONSTRAINT [FK_Content_ContentTranslation] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_ContentTranslation' and parent_object_id = OBJECT_ID('dbo.ContentTranslation'))
ALTER TABLE [ContentTranslation]
	DROP CONSTRAINT [FK_Locale_ContentTranslation] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Question_QuestionTranslation' and parent_object_id = OBJECT_ID('dbo.QuestionTranslation'))
ALTER TABLE [QuestionTranslation]
	DROP CONSTRAINT [FK_Question_QuestionTranslation] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_QuestionTranslation' and parent_object_id = OBJECT_ID('dbo.QuestionTranslation'))
ALTER TABLE [QuestionTranslation]
	DROP CONSTRAINT [FK_Locale_QuestionTranslation] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_UserContentPreference' and parent_object_id = OBJECT_ID('dbo.UserContentPreference'))
ALTER TABLE [UserContentPreference]
	DROP CONSTRAINT [FK_Locale_UserContentPreference] 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_MemberIDCard' and parent_object_id = OBJECT_ID('dbo.MemberIDCard'))
ALTER TABLE MemberIDCard
	DROP CONSTRAINT FK_Locale_MemberIDCard 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_MemberIDCard' and parent_object_id = OBJECT_ID('dbo.MemberIDCard'))
ALTER TABLE MemberIDCard
	DROP CONSTRAINT FK_Enrollments_MemberIDCard 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_CardViewMode_MemberIDCard' and parent_object_id = OBJECT_ID('dbo.MemberIDCard'))
ALTER TABLE MemberIDCard
	DROP CONSTRAINT FK_CardViewMode_MemberIDCard 
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_CardTypeTranslation_MemberIDCard' and parent_object_id = OBJECT_ID('dbo.MemberIDCard'))
ALTER TABLE MemberIDCard
	DROP CONSTRAINT FK_CardTypeTranslation_MemberIDCard
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_CardTypeTranslation' and parent_object_id = OBJECT_ID('dbo.CardTypeTranslation'))
ALTER TABLE CardTypeTranslation
	DROP CONSTRAINT FK_Locale_CardTypeTranslation
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_CardType_CardTypeTranslation' and parent_object_id = OBJECT_ID('dbo.CardTypeTranslation'))
ALTER TABLE CardTypeTranslation
	DROP CONSTRAINT FK_CardType_CardTypeTranslation
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_InsuranceType_CardType' and parent_object_id = OBJECT_ID('dbo.CardType'))
ALTER TABLE CardType
	DROP CONSTRAINT FK_InsuranceType_CardType
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_CardType_CardTypeConfig' and parent_object_id = OBJECT_ID('dbo.CardTypeConfig'))
ALTER TABLE CardTypeConfig
	DROP CONSTRAINT FK_CardType_CardTypeConfig
go

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_InsuranceType_CardType' and parent_object_id = OBJECT_ID('dbo.CardType'))	
ALTER TABLE CardType
	DROP CONSTRAINT FK_InsuranceType_CardType
go
-----------------------------------------------------------------------------
--Drop Indexes
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1UserContentPreference' and object_id = OBJECT_ID('dbo.UserContentPreference'))
DROP INDEX idx_fk_1UserContentPreference ON UserContentPreference
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1Video' and object_id = OBJECT_ID('dbo.Video'))
DROP INDEX idx_fk_1Video ON Video
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1UserSurveyAnswer' and object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
DROP INDEX idx_fk_1UserSurveyAnswer ON UserSurveyAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_3UserSurveyAnswer' and object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
DROP INDEX idx_fk_3UserSurveyAnswer ON UserSurveyAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_6UserSurveyAnswer' and object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
DROP INDEX idx_fk_6UserSurveyAnswer ON UserSurveyAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_5UserSurveyAnswer' and object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
DROP INDEX idx_fk_5UserSurveyAnswer ON UserSurveyAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1UserContent' and object_id = OBJECT_ID('dbo.UserContent'))
DROP INDEX idx_fk_1UserContent ON UserContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_4UserContent' and object_id = OBJECT_ID('dbo.UserContent'))
DROP INDEX idx_fk_4UserContent ON UserContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_5UserContent' and object_id = OBJECT_ID('dbo.UserContent'))
DROP INDEX idx_fk_5UserContent ON UserContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_6UserContent' and object_id = OBJECT_ID('dbo.UserContent'))
DROP INDEX idx_fk_6UserContent ON UserContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1CampaignContent' and object_id = OBJECT_ID('dbo.CampaignContent'))
DROP INDEX idx_fk_1CampaignContent ON CampaignContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2CampaignContent' and object_id = OBJECT_ID('dbo.CampaignContent'))
DROP INDEX idx_fk_2CampaignContent ON CampaignContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1CampaignMember' and object_id = OBJECT_ID('dbo.CampaignMember'))
DROP INDEX idx_fk_1CampaignMember ON CampaignMember
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2CampaignMember' and object_id = OBJECT_ID('dbo.CampaignMember'))
DROP INDEX idx_fk_2CampaignMember ON CampaignMember
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1SurveyQuestion' and object_id = OBJECT_ID('dbo.SurveyQuestion'))
DROP INDEX idx_fk_1SurveyQuestion ON SurveyQuestion
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2SurveyQuestion' and object_id = OBJECT_ID('dbo.SurveyQuestion'))
DROP INDEX idx_fk_2SurveyQuestion ON SurveyQuestion
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1Survey' and object_id = OBJECT_ID('dbo.Survey'))
DROP INDEX idx_fk_1Survey ON Survey
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2QuestionAnswer' and object_id = OBJECT_ID('dbo.QuestionAnswer'))
DROP INDEX idx_fk_2QuestionAnswer ON QuestionAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_3QuestionAnswer' and object_id = OBJECT_ID('dbo.QuestionAnswer'))
DROP INDEX idx_fk_3QuestionAnswer ON QuestionAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1Question' and object_id = OBJECT_ID('dbo.Question'))
DROP INDEX idx_fk_1Question ON Question
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1ExperienceLog' and object_id = OBJECT_ID('dbo.ExperienceLog'))
DROP INDEX idx_fk_1ExperienceLog ON ExperienceLog
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2ExperienceLog' and object_id = OBJECT_ID('dbo.ExperienceLog'))
DROP INDEX idx_fk_2ExperienceLog ON ExperienceLog
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1Animation' and object_id = OBJECT_ID('dbo.Animation'))
DROP INDEX idx_fk_1Animation ON Animation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1ContentTypeState' and object_id = OBJECT_ID('dbo.ContentTypeState'))
DROP INDEX idx_fk_1ContentTypeState ON ContentTypeState
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2ContentTypeState' and object_id = OBJECT_ID('dbo.ContentTypeState'))
DROP INDEX idx_fk_2ContentTypeState ON ContentTypeState
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_1RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2RelatedConten' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_2RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_3RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_3RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2Content' and object_id = OBJECT_ID('dbo.Content'))
DROP INDEX idx_fk_2Content ON Content
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_1ContentTranslation ON ContentTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_2ContentTranslation ON ContentTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_1AnswerTranslation ON AnswerTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_2AnswerTranslation ON AnswerTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_1QuestionTranslation ON QuestionTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1RelatedContent' and object_id = OBJECT_ID('dbo.RelatedContent'))
DROP INDEX idx_fk_2QuestionTranslation ON QuestionTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1CardTypeTranslation' and object_id = OBJECT_ID('dbo.CardTypeTranslation'))
DROP INDEX idx_fk_1CardTypeTranslation ON CardTypeTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2CardTypeTranslation' and object_id = OBJECT_ID('dbo.CardTypeTranslation'))
DROP INDEX idx_fk_2CardTypeTranslation ON CardTypeTranslation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2MemberIDCard' and object_id = OBJECT_ID('dbo.MemberIDCard'))
DROP INDEX idx_fk_2MemberIDCard ON MemberIDCard
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_3MemberIDCard' and object_id = OBJECT_ID('dbo.MemberIDCard'))
DROP INDEX idx_fk_3MemberIDCard ON MemberIDCard
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_5MemberIDCard' and object_id = OBJECT_ID('dbo.MemberIDCard'))
DROP INDEX idx_fk_5MemberIDCard ON MemberIDCard
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_6MemberIDCard' and object_id = OBJECT_ID('dbo.MemberIDCard'))
DROP INDEX idx_fk_6MemberIDCard ON MemberIDCard
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_6MemberIDCard' and object_id = OBJECT_ID('dbo.MemberIDCard'))
DROP INDEX idx_fx_1ExperienceDevice ON ExperienceDevice
go
-----------------------------------------------------------------------------
--Drop Tables
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'UserContentPreference')
DROP TABLE UserContentPreference
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Video')
DROP TABLE Video
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'UserSurveyAnswer')
DROP TABLE UserSurveyAnswer
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'UserContent')
DROP TABLE UserContent
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'CampaignContent')
DROP TABLE CampaignContent
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Campaign')
DROP TABLE Campaign
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'SurveyQuestion')
DROP TABLE SurveyQuestion
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Survey')
DROP TABLE Survey
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'QuestionAnswer')
DROP TABLE QuestionAnswer
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Question')
DROP TABLE Question
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'QuestionType')
DROP TABLE QuestionType
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Answer')
DROP TABLE Answer
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ExperienceLog')
DROP TABLE ExperienceLog
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ExperienceEvent')
DROP TABLE ExperienceEvent
go


IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Resource')
DROP TABLE Resource
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Animation')
DROP TABLE Animation
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ContentTypeState')
DROP TABLE ContentTypeState
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'CampaignMember')
DROP TABLE CampaignMember
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ContentStatus')
DROP TABLE ContentStatus
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'RelatedContent')
DROP TABLE RelatedContent
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ContentDisplayRule')
DROP TABLE ContentDisplayRule
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'Content')
DROP TABLE Content
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ContentType')
DROP TABLE ContentType
go

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'ExcludedMember')
DROP TABLE ExcludedMember
go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'Locale')
	DROP TABLE Locale
go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'ContentTranslation')
	DROP TABLE ContentTranslation
go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'AnswerTranslation')
	DROP TABLE AnswerTranslation
go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'QuestionTranslation')
	DROP TABLE QuestionTranslation
go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'CardTypeTranslation')
	DROP TABLE CardTypeTranslation
	go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'MemberIDCard')
	DROP TABLE MemberIDCard
	go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'CardViewMode')
	DROP TABLE CardViewMode
	go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'CardType')
	DROP TABLE CardType
	go	

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'ExperienceDevice')
	DROP TABLE ExperienceDevice
	go	
	
IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'Device')
	DROP TABLE Device
	go	

-----------------------------------------------------------------------------
--Table: Answer
-----------------------------------------------------------------------------

CREATE TABLE [Answer]
( 
	[AnswerID]           integer  NOT NULL  ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Answer]
	ADD CONSTRAINT [idx_pk_Answer] PRIMARY KEY  CLUSTERED ([AnswerID] ASC)
go

-----------------------------------------------------------------------------
--Table: Campaign
-----------------------------------------------------------------------------

CREATE TABLE [Campaign]
( 
	[CampaignID]         integer  NOT NULL  ,
	[CampaignDesc]       nvarchar(2000)  NULL ,
	[CampaignActiveInd]  bit  NULL ,
	[TargetPopulationDesc] nvarchar(2000)  NULL ,
	[CampaignPeriodDesc] nvarchar(100)  NULL ,
	[TargetProcedureName] nvarchar(50)  NULL, 
	[AuthRequiredInd] bit NOT NULL DEFAULT 1,
	[SavingsMonthStartDate] datetime NULL,
	[CampaignURL] nvarchar(100) NULL,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Campaign]
	ADD CONSTRAINT [idx_pk_Campaign] PRIMARY KEY  CLUSTERED ([CampaignID] ASC)
go

-----------------------------------------------------------------------------
--Table: Content
-----------------------------------------------------------------------------

CREATE TABLE [Content]
( 
	[ContentID]          integer  NOT NULL   ,
	[ContentTypeID]      integer  NOT NULL,
	[ContentName]		 nvarchar(100) NULL,
	[ContentSourceDesc]  nvarchar(100)  NULL ,
	[ContentImageFileName] nvarchar(100)  NULL ,
	[ContentFileLocationDesc] nvarchar(100)  NULL ,
	[ContentPointsCount] integer  NULL ,
	[ContentDurationSecondsCount] integer  NULL ,
	[IntroContentInd]	 bit NOT NULL DEFAULT 0,
	[ContentURL]		 nvarchar(100) NULL,
	[ContentPhoneNum]	 nvarchar(50) NULL,
	[AccumulatorsInd]    bit NOT NULL DEFAULT 0,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Content]
	ADD CONSTRAINT [idx_pk_Content] PRIMARY KEY  CLUSTERED ([ContentID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_2Content] ON [Content]
( 
	[ContentTypeID]       ASC
)
go

-----------------------------------------------------------------------------
--Table: Video
-----------------------------------------------------------------------------

CREATE TABLE [Video]
( 
	[VideoID]            integer  NOT NULL ,
	[VideoFileName]      nvarchar(50)  NULL ,
	[PersonalizedVideoInd] bit  NULL ,
	[VideoDataProcName]  nvarchar(50)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Video]
	ADD CONSTRAINT [idx_pk_Video] PRIMARY KEY  CLUSTERED ([VideoID] ASC)
go
/*
CREATE UNIQUE NONCLUSTERED INDEX [idx_fk_1Video] ON [Video]
( 
	[VideoID]             ASC
)
go
*/

-----------------------------------------------------------------------------
--Table: Animation
-----------------------------------------------------------------------------

CREATE TABLE [Animation]
( 
	[AnimationID]        integer  NOT NULL ,
	[AnimationScriptFileName] nvarchar(50)  NULL ,
	[InteractiveAnimationInd] bit  NULL ,
	[AnimationDataProcName] nvarchar(50)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Animation]
	ADD CONSTRAINT [idx_pk_Animation] PRIMARY KEY  CLUSTERED ([AnimationID] ASC)
go

/*CREATE UNIQUE NONCLUSTERED INDEX [idx_fk_1Animation] ON [Animation]
( 
	[AnimationID]         ASC
)
go
*/

-----------------------------------------------------------------------------
--Table: CampaignContent
-----------------------------------------------------------------------------

CREATE TABLE [CampaignContent]
( 
	[CampaignID]         integer  NOT NULL ,
	[ContentID]          integer  NOT NULL ,
	[ActivationDate]	 datetime NULL,
	[ExpirationDate]	 datetime NULL,
	[UserContentInd]	 bit NOT NULL default 1,
	[EmailNotificationInd]	 bit NOT NULL default 1,
	[SMSNotificationInd]	 bit NOT NULL default 1,
	[OSNotificationInd]	 bit NOT NULL default 1,
	[OSNotificationStatusDesc] nvarchar(100) NULL,
	[OSNotificationSentDate] datetime NULL,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [CampaignContent]
	ADD CONSTRAINT [idx_pk_CampaignContent] PRIMARY KEY  CLUSTERED ([CampaignID] ASC,[ContentID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1CampaignContent] ON [CampaignContent]
( 
	[CampaignID]          ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2CampaignContent] ON [CampaignContent]
( 
	[ContentID]           ASC
)
go

-----------------------------------------------------------------------------
--Table: CampaignMember
-----------------------------------------------------------------------------

CREATE TABLE [CampaignMember]
( 
	[CampaignID]         integer  NOT NULL ,
	[CCHID]              integer  NOT NULL ,
	[Savings]			 money NULL,
	[Score]				 float NULL,
	[YourCostSavingsAmt]	 money NULL,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [CampaignMember]
	ADD CONSTRAINT [idx_pk_CampaignMember] PRIMARY KEY  CLUSTERED ([CampaignID] ASC,[CCHID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1CampaignMember] ON [CampaignMember]
( 
	[CampaignID]          ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2CampaignMember] ON [CampaignMember]
( 
	[CCHID]               ASC
)
go

-----------------------------------------------------------------------------
--Table: ContentDisplayRule
-----------------------------------------------------------------------------

CREATE TABLE [ContentDisplayRule]
( 
	[ContentDisplayRuleID] integer  NOT NULL   ,
	[ContentDisplayRuleDesc] nvarchar(100)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ContentDisplayRule]
	ADD CONSTRAINT [idx_pk_ContentDisplayRule] PRIMARY KEY  CLUSTERED ([ContentDisplayRuleID] ASC)
go

-----------------------------------------------------------------------------
--Table: ContentStatus
-----------------------------------------------------------------------------

CREATE TABLE [ContentStatus]
( 
	[ContentStatusID]    integer  NOT NULL   ,
	[ContentStatusDesc]  nvarchar(100)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ContentStatus]
	ADD CONSTRAINT [idx_pk_ContentStatus] PRIMARY KEY  CLUSTERED ([ContentStatusID] ASC)
go

-----------------------------------------------------------------------------
--Table: ContentType
-----------------------------------------------------------------------------

CREATE TABLE [ContentType]
( 
	[ContentTypeID]      integer  NOT NULL   ,
	[ContentTypeDesc]    nvarchar(100)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ContentType]
	ADD CONSTRAINT [idx_pk_ContentType] PRIMARY KEY  CLUSTERED ([ContentTypeID] ASC)
go

-----------------------------------------------------------------------------
--Table: ContentTypeState
-----------------------------------------------------------------------------

CREATE TABLE [ContentTypeState]
( 
	[ContentTypeID]      integer  NOT NULL ,
	[ContentStatusID]    integer  NOT NULL ,
	[InitialStateInd]    bit  NULL ,
	[EndStateInd]        bit  NULL ,
	[ContentStatusCaptionText] nvarchar(250)  NULL,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ContentTypeState]
	ADD CONSTRAINT [idx_pk_ContentTypeState] PRIMARY KEY  CLUSTERED ([ContentTypeID] ASC,[ContentStatusID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1ContentTypeState] ON [ContentTypeState]
( 
	[ContentTypeID]       ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2ContentTypeState] ON [ContentTypeState]
( 
	[ContentStatusID]     ASC
)
go

-----------------------------------------------------------------------------
--Table: UserContentPreference
-----------------------------------------------------------------------------

CREATE TABLE [UserContentPreference]
( 
	[CCHID]              integer  NOT NULL ,
	[SMSInd]             bit  NULL ,
	[EmailInd]           bit  NULL ,
	[OSBasedAlertInd]    bit  NULL ,
	[DefaultLocaleID]	 int NULL,
	[PreferredContactPhoneNum]	 nvarchar(50) NULL,
	[LastUpdateDate]     datetime  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [UserContentPreference]
	ADD CONSTRAINT [idx_pk_UserContentPreference] PRIMARY KEY  CLUSTERED ([CCHID] ASC)
go


/*CREATE UNIQUE NONCLUSTERED INDEX [idx_fk_1UserContentPreference] ON [UserContentPreference]
( 
	[CCHID]               ASC
)
go
*/

-----------------------------------------------------------------------------
--Table: ExperienceEvent
-----------------------------------------------------------------------------

CREATE TABLE [ExperienceEvent]
( 
	[ExperienceEventID]  integer  NOT NULL   ,
	[ExperienceEventDesc] nvarchar(100)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ExperienceEvent]
	ADD CONSTRAINT [idx_pk_ExperienceEvent] PRIMARY KEY  CLUSTERED ([ExperienceEventID] ASC)
go

-----------------------------------------------------------------------------
--Table: ExperienceLog
-----------------------------------------------------------------------------

CREATE TABLE [ExperienceLog]
( 
	[ExperienceLogID]    integer  NOT NULL   ,
	[ExperienceEventID]  integer  NOT NULL ,
	[CCHID]              integer  NULL ,
	[ContentID]			 integer NULL,
	[ExperienceUserID]   nvarchar(36)  NOT NULL,
	[ExperienceLogCommentText] nvarchar(250)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ExperienceLog]
	ADD CONSTRAINT [idx_pk_ExperienceLog] PRIMARY KEY  CLUSTERED ([ExperienceLogID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1ExperienceLog] ON [ExperienceLog]
( 
	[CCHID]               ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2ExperienceLog] ON [ExperienceLog]
( 
	[ExperienceEventID]   ASC
)
go

-----------------------------------------------------------------------------
--Table: Question
-----------------------------------------------------------------------------

CREATE TABLE [Question]
( 
	[QuestionID]         integer  NOT NULL   ,
	[QuestionTypeID]     integer  NOT NULL,
	[ExpectedAnswerDatatypeDesc] nvarchar(100)  NULL ,
	[ExpectedAnswerValueRangeDesc] nvarchar(100)  NULL ,
	[ScoredQuestionInd]  bit  NULL ,
	[CreateDate]         datetime  NULL ,
)
go

ALTER TABLE [Question]
	ADD CONSTRAINT [idx_pk_Question] PRIMARY KEY  CLUSTERED ([QuestionID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1Question] ON [Question]
( 
	[QuestionTypeID]      ASC
)
go

-----------------------------------------------------------------------------
--Table: QuestionAnswer
-----------------------------------------------------------------------------

CREATE TABLE [QuestionAnswer]
( 
	[QuestionID]         integer  NOT NULL,
	[AnswerID]           integer  NOT NULL ,
	[AnswerDisplayOrderNum] integer  NULL ,
	[CorrectAnswerInd]   bit  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [QuestionAnswer]
	ADD CONSTRAINT [idx_pk_QuestionAnswer] PRIMARY KEY  CLUSTERED ([AnswerID] ASC,[QuestionID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_2QuestionAnswer] ON [QuestionAnswer]
( 
	[AnswerID]            ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_3QuestionAnswer] ON [QuestionAnswer]
( 
	[QuestionID]          ASC
)
go

-----------------------------------------------------------------------------
--Table: QuestionType
-----------------------------------------------------------------------------

CREATE TABLE [QuestionType]
( 
	[QuestionTypeID]     integer  NOT NULL  ,
	[QuestionTypeDesc]   nvarchar(100)  NULL ,
	[CreateDate]	datetime,
)
go

ALTER TABLE [QuestionType]
	ADD CONSTRAINT [idx_pk_QuestionType] PRIMARY KEY  CLUSTERED ([QuestionTypeID] ASC)
go

-----------------------------------------------------------------------------
--Table: RelatedContent
-----------------------------------------------------------------------------

CREATE TABLE [RelatedContent]
( 
	[ParentContentID]    integer  NOT NULL ,
	[RelatedContentID]   integer  NOT NULL ,
	[RelatedContentDisplayRuleID] integer  NULL,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [RelatedContent]
	ADD CONSTRAINT [idx_pk_RelatedContent] PRIMARY KEY  CLUSTERED ([ParentContentID] ASC,[RelatedContentID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1RelatedContent] ON [RelatedContent]
( 
	[ParentContentID]     ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2RelatedContent] ON [RelatedContent]
( 
	[RelatedContentID]    ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_3RelatedContent] ON [RelatedContent]
( 
	[RelatedContentDisplayRuleID]  ASC
)
go

-----------------------------------------------------------------------------
--Table: Resource
-----------------------------------------------------------------------------

CREATE TABLE [Resource]
( 
	[ResourceID]         integer  NOT NULL  ,
	[ResourceName]       nvarchar(50)  NULL ,
	[ResourceIconFileName] nvarchar(50)  NULL ,
	[ResourceVideoImageURL] nvarchar(100)  NULL ,
	[ResourceDesc]       nvarchar(100)  NULL ,
	[ResourcePhoneNum]   nvarchar(20) NULL ,
	[ResourceURL]        nvarchar(100)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Resource]
	ADD CONSTRAINT [idx_pk_Resource] PRIMARY KEY  CLUSTERED ([ResourceID] ASC)
go

-----------------------------------------------------------------------------
--Table: Survey
-----------------------------------------------------------------------------

CREATE TABLE [Survey]
( 
	[SurveyID]           integer  NOT NULL ,
	[SurveyPassCount]    integer  NULL ,
	[EmbeddedSurveyInd]  bit  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [Survey]
	ADD CONSTRAINT [idx_pk_Survey] PRIMARY KEY  CLUSTERED ([SurveyID] ASC)
go

CREATE UNIQUE NONCLUSTERED INDEX [idx_fk_1Survey] ON [Survey]
( 
	[SurveyID]            ASC
)
go

-----------------------------------------------------------------------------
--Table: SurveyQuestion
-----------------------------------------------------------------------------

CREATE TABLE [SurveyQuestion]
( 
	[SurveyID]           integer  NOT NULL ,
	[QuestionID]         integer  NOT NULL ,
	[QuestionDisplayOrderNum] integer  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [SurveyQuestion]
	ADD CONSTRAINT [idx_pk_SurveyQuestion] PRIMARY KEY  CLUSTERED ([SurveyID] ASC,[QuestionID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1SurveyQuestion] ON [SurveyQuestion]
( 
	[SurveyID]            ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2SurveyQuestion] ON [SurveyQuestion]
( 
	[QuestionID]          ASC
)
go

-----------------------------------------------------------------------------
--Table: UserContent
-----------------------------------------------------------------------------

CREATE TABLE [UserContent]
( 
	[CCHID]              integer  NOT NULL ,
	[CampaignID]         integer  NOT NULL ,
	[ContentID]          integer  NOT NULL ,
	[ContentStatusChangeDate] datetime  NULL ,
	[UserContentCommentText] nvarchar(2000)  NULL ,
	[NotificationSentDate] datetime  NULL ,
	[SMSNotificationSentDate] datetime  NULL ,
	[SMSNotificationStatusDesc] nvarchar(100) NULL,
	[OSNotificationSentDate] datetime  NULL ,
	[OSNotificationStatusDesc] nvarchar(100) NULL,
	[ContentSavingsAmt]  decimal(8,2)  NULL ,
	[MemberContentDataText] nvarchar(2000)  NULL ,
	[CreateDate]         datetime  NULL ,
	[ContentStatusID]    integer  NOT NULL 
)
go

ALTER TABLE [UserContent]
	ADD CONSTRAINT [idx_pk_UserContent] PRIMARY KEY  CLUSTERED ([CCHID] ASC,[CampaignID] ASC,[ContentID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1UserContent] ON [UserContent]
( 
	[CCHID]               ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_4UserContent] ON [UserContent]
( 
	[ContentID]           ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_5UserContent] ON [UserContent]
( 
	[CampaignID]          ASC,
	[ContentID]           ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_6UserContent] ON [UserContent]
( 
	[ContentStatusID]     ASC
)
go


-----------------------------------------------------------------------------
--Table: ExcludedMember
-----------------------------------------------------------------------------

CREATE TABLE [ExcludedMember]
( 
	[CCHID]              integer  NOT NULL ,
	[ExcludeReasonDesc]  nvarchar(100)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ExcludedMember]
	ADD CONSTRAINT [XPKExcludedMember] PRIMARY KEY  CLUSTERED ([CCHID] ASC)
go

-----------------------------------------------------------------------------
--Table: UserSurveyAnswer
-----------------------------------------------------------------------------

CREATE TABLE [UserSurveyAnswer]
( 
	[CCHID]              integer  NOT NULL ,
	[CampaignID]         integer  NOT NULL ,
	[SurveyID]           integer  NOT NULL ,
	[SurveyIterationNum] integer  NOT NULL ,
	[QuestionID]         integer  NOT NULL ,
	[AnswerID]           integer  NULL ,
	[FreeFormAnswerText] nvarchar(500)  NULL ,
	[CreateDate]         datetime  NULL  
)
go

ALTER TABLE [UserSurveyAnswer]
	ADD CONSTRAINT [idx_pk_UserSurveyAnswer] PRIMARY KEY  CLUSTERED ([CCHID] ASC,[SurveyID] ASC,[QuestionID] ASC,[SurveyIterationNum] ASC,[CampaignID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1UserSurveyAnswer] ON [UserSurveyAnswer]
( 
	[CCHID]               ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_3UserSurveyAnswer] ON [UserSurveyAnswer]
( 
	[AnswerID]            ASC,
	[QuestionID]          ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_6UserSurveyAnswer] ON [UserSurveyAnswer]
( 
	[CCHID]               ASC,
	[CampaignID]          ASC,
	[SurveyID]            ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_5UserSurveyAnswer] ON [UserSurveyAnswer]
( 
	[SurveyID]            ASC,
	[QuestionID]          ASC
)
go

-----------------------------------------------------------------------------
--Table: Locale
-----------------------------------------------------------------------------

CREATE TABLE [Locale]
( 
	[LocaleID]           integer  NOT NULL   ,
	[LocaleCode]         nvarchar(10) ,
	[ISOCountryCode]     nvarchar(10) ,
	[ISOLanguageCode]    nvarchar(10) ,
	[LocaleDesc]         nvarchar(100)  NULL ,
	[DateFormatDesc]     nvarchar(100)  NULL,
	[CreateDate]         datetime  NULL
)
go

ALTER TABLE [Locale]
	ADD CONSTRAINT [idx_pk_Locale] PRIMARY KEY  CLUSTERED ([LocaleID] ASC)
go

-----------------------------------------------------------------------------
--Table: ContentTranslation
-----------------------------------------------------------------------------

CREATE TABLE [ContentTranslation]
( 
	[ContentID]          integer  NOT NULL ,
	[LocaleID]           integer  NOT NULL ,
	[ContentTitle]       nvarchar(50)  NULL ,
	[ContentCaptionText] nvarchar(250)  NULL ,
	[SMSNotificationText]	 nvarchar(2000) NULL,
	[OSNotificationText] nvarchar(2000) NULL,
	[ContentDesc]        nvarchar(2000)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ContentTranslation]
	ADD CONSTRAINT [idx_pk_ContentTranslation] PRIMARY KEY  CLUSTERED ([ContentID] ASC,[LocaleID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1ContentTranslation] ON [ContentTranslation]
( 
	[ContentID]           ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2ContentTranslation] ON [ContentTranslation]
( 
	[LocaleID]            ASC
)
go

-----------------------------------------------------------------------------
--Table: AnswerTranslation
-----------------------------------------------------------------------------

CREATE TABLE [AnswerTranslation]
( 
	[AnswerID]           integer  NOT NULL ,
	[LocaleID]           integer  NOT NULL ,
	[AnswerText]         nvarchar(250)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [AnswerTranslation]
	ADD CONSTRAINT [idx_pk_AnswerTranslation] PRIMARY KEY  CLUSTERED ([AnswerID] ASC,[LocaleID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1AnswerTranslation] ON [AnswerTranslation]
( 
	[AnswerID]            ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2AnswerTranslation] ON [AnswerTranslation]
( 
	[LocaleID]            ASC
)
go

-----------------------------------------------------------------------------
--Table: QuestionTranslation
-----------------------------------------------------------------------------

CREATE TABLE [QuestionTranslation]
( 
	[QuestionID]         integer  NOT NULL ,
	[LocaleID]           integer  NOT NULL ,
	[QuestionText]       nvarchar(250)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [QuestionTranslation]
	ADD CONSTRAINT [idx_pk_QuestionTranslation] PRIMARY KEY  CLUSTERED ([QuestionID] ASC,[LocaleID] ASC)
go

CREATE NONCLUSTERED INDEX [idx_fk_1QuestionTranslation] ON [QuestionTranslation]
( 
	[QuestionID]          ASC
)
go

CREATE NONCLUSTERED INDEX [idx_fk_2QuestionTranslation] ON [QuestionTranslation]
( 
	[LocaleID]            ASC
)
go

-----------------------------------------------------------------------------
--Table: CardType
-----------------------------------------------------------------------------

CREATE TABLE CardType
( 
	CardTypeID           integer  NOT NULL   ,
	CardTypeDesc         nvarchar(250)  NULL ,
	InsuranceTypeID		 int NULL,
	CardTypeFileName     nvarchar(100) NULL,
	CreateDate           datetime  NULL 
)
go

ALTER TABLE CardType
	ADD CONSTRAINT idx_pk_CardType PRIMARY KEY  CLUSTERED (CardTypeID ASC)
go

-----------------------------------------------------------------------------
--Table: CardTypeTranslation
-----------------------------------------------------------------------------

CREATE TABLE CardTypeTranslation
( 
	CardTypeID           integer  NOT NULL ,
	LocaleID             integer  NOT NULL ,
	CardTypeName         nvarchar(50)  NULL ,
	CreateDate           datetime  NULL 
)
go

ALTER TABLE CardTypeTranslation
	ADD CONSTRAINT idx_pk_CardTypeTranslation PRIMARY KEY  CLUSTERED (CardTypeID ASC,LocaleID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1CardTypeTranslation ON CardTypeTranslation
( 
	CardTypeID            ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2CardTypeTranslation ON CardTypeTranslation
( 
	LocaleID              ASC
)
go

-----------------------------------------------------------------------------
--Table: CardViewMode
-----------------------------------------------------------------------------

CREATE TABLE CardViewMode
( 
	CardViewModeID       integer  NOT NULL   ,
	CardViewModeName     nvarchar(50)  NULL ,
	CreateDate           datetime  NULL 
)
go

ALTER TABLE CardViewMode
	ADD CONSTRAINT idx_pk_CardViewMode PRIMARY KEY  CLUSTERED (CardViewModeID ASC)
go

-----------------------------------------------------------------------------
--Table: MemberIDCard
-----------------------------------------------------------------------------

CREATE TABLE MemberIDCard
( 
	CCHID                integer  NOT NULL ,
	CardTypeID           integer  NOT NULL ,
	LocaleID             integer  NOT NULL ,
	CardViewModeID       integer  NOT NULL ,
	CardMemberDataText nvarchar(2000)  NULL ,
	SecurityTokenGUID    uniqueidentifier  NULL ,
	SecurityTokenBeginDatetime datetime  NULL ,
	SecurityTokenEndDatetime datetime  NULL ,
	CreateDate           datetime  NULL 
)
go

ALTER TABLE MemberIDCard
	ADD CONSTRAINT idx_pk_MemberIDCard PRIMARY KEY  CLUSTERED (CCHID ASC,CardTypeID ASC,CardViewModeID ASC,LocaleID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_2MemberIDCard ON MemberIDCard
( 
	CardTypeID            ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_3MemberIDCard ON MemberIDCard
( 
	CardViewModeID        ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_5MemberIDCard ON MemberIDCard
( 
	CCHID                 ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_6MemberIDCard ON MemberIDCard
( 
	LocaleID              ASC
)
go
-----------------------------------------------------------------------------
--Table: Device	
-----------------------------------------------------------------------------
CREATE TABLE Device
( 
	DeviceID             nvarchar(100)  NOT NULL  ,
	ClientAllowPushInd   bit  NULL ,
	NativeAllowPushInd	 bit NULL,
	LastPushPromptDate   datetime NULL,
	CreateDate           datetime  NULL 
)
go

ALTER TABLE Device
	ADD CONSTRAINT idx_pk_Device PRIMARY KEY  CLUSTERED (DeviceID ASC)
go
-----------------------------------------------------------------------------
--Table: ExperienceDevice	
-----------------------------------------------------------------------------
CREATE TABLE ExperienceDevice
( 
	ExperienceUserID     nvarchar(36)  NOT NULL ,
	DeviceID             nvarchar(100)  NOT NULL ,
	CCHID                integer  NULL ,
	CreateDate           datetime  NULL 
)
go

ALTER TABLE ExperienceDevice
	ADD CONSTRAINT idx_pk_ExperienceDevice PRIMARY KEY  CLUSTERED (ExperienceUserID ASC,DeviceID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk1_ExperienceDevice ON ExperienceDevice
( 
	CCHID                 ASC
)
go


-------------------------------------------------------------------------
--FK Constraints
-------------------------------------------------------------------------

ALTER TABLE [Content]
	ADD CONSTRAINT [FK_ContentType_Content] FOREIGN KEY ([ContentTypeID]) REFERENCES [ContentType]([ContentTypeID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Video]
	ADD CONSTRAINT [FK_Content_Video] FOREIGN KEY ([VideoID]) REFERENCES [Content]([ContentID])
		ON DELETE CASCADE
		ON UPDATE CASCADE
go


ALTER TABLE [Animation]
	ADD CONSTRAINT [FK_Content_Animation] FOREIGN KEY ([AnimationID]) REFERENCES [Content]([ContentID])
		ON DELETE CASCADE
		ON UPDATE CASCADE
go


ALTER TABLE [CampaignContent]
	ADD CONSTRAINT [FK_Campaign_CampaignContent] FOREIGN KEY ([CampaignID]) REFERENCES [Campaign]([CampaignID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [CampaignContent]
	ADD CONSTRAINT [FK_Content_CampaignContent] FOREIGN KEY ([ContentID]) REFERENCES [Content]([ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [CampaignMember]
	ADD CONSTRAINT [FK_Campaign_CampaignMember] FOREIGN KEY ([CampaignID]) REFERENCES [Campaign]([CampaignID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go



ALTER TABLE [ContentTypeState]
	ADD CONSTRAINT [FK_ContType_ContTypeState] FOREIGN KEY ([ContentTypeID]) REFERENCES [ContentType]([ContentTypeID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [ContentTypeState]
	ADD CONSTRAINT [FK_ContStatus_ContTypeState] FOREIGN KEY ([ContentStatusID]) REFERENCES [ContentStatus]([ContentStatusID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go



ALTER TABLE [ExperienceLog]
	ADD CONSTRAINT [FK_ExpEvent_ExpLog] FOREIGN KEY ([ExperienceEventID]) REFERENCES [ExperienceEvent]([ExperienceEventID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Question]
	ADD CONSTRAINT [FK_QuestionType_Question] FOREIGN KEY ([QuestionTypeID]) REFERENCES [QuestionType]([QuestionTypeID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [QuestionAnswer]
	ADD CONSTRAINT [FK_Answer_QuestionAnswer] FOREIGN KEY ([AnswerID]) REFERENCES [Answer]([AnswerID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [QuestionAnswer]
	ADD CONSTRAINT [FK_Question_QuestionAnswer] FOREIGN KEY ([QuestionID]) REFERENCES [Question]([QuestionID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [RelatedContent]
	ADD CONSTRAINT [FK_Content_RelatedContent_1] FOREIGN KEY ([ParentContentID]) REFERENCES [Content]([ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [RelatedContent]
	ADD CONSTRAINT [FK_Content_RelatedContent_2] FOREIGN KEY ([RelatedContentID]) REFERENCES [Content]([ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [RelatedContent]
	ADD CONSTRAINT [FK_ContDisplayRule_RelatedCont] FOREIGN KEY ([RelatedContentDisplayRuleID]) REFERENCES [ContentDisplayRule]([ContentDisplayRuleID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [Survey]
	ADD CONSTRAINT [FK_Content_Survey] FOREIGN KEY ([SurveyID]) REFERENCES [Content]([ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [SurveyQuestion]
	ADD CONSTRAINT [FK_Survey_SurveyQuestion] FOREIGN KEY ([SurveyID]) REFERENCES [Survey]([SurveyID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [SurveyQuestion]
	ADD CONSTRAINT [FK_Question_SurveyQuestion] FOREIGN KEY ([QuestionID]) REFERENCES [Question]([QuestionID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [UserContent]
	ADD CONSTRAINT [FK_Content_UserContent] FOREIGN KEY ([ContentID]) REFERENCES [Content]([ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [UserContent]
	ADD CONSTRAINT [FK_CampaignContent_UserContent] FOREIGN KEY ([CampaignID],[ContentID]) REFERENCES [CampaignContent]([CampaignID],[ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [UserContent]
	ADD CONSTRAINT [FK_ContentStatus_UserContent] FOREIGN KEY ([ContentStatusID]) REFERENCES [ContentStatus]([ContentStatusID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE [UserSurveyAnswer]
	ADD CONSTRAINT [FK_QuestionAns_UserSurveyAns] FOREIGN KEY ([AnswerID],[QuestionID]) REFERENCES [QuestionAnswer]([AnswerID],[QuestionID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [UserSurveyAnswer]
	ADD CONSTRAINT [FK_UserContent_UserSurveyAnswer] FOREIGN KEY ([CCHID],[CampaignID],[SurveyID]) REFERENCES [UserContent]([CCHID],[CampaignID],[ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [UserSurveyAnswer]
	ADD CONSTRAINT [FK_SurveyQuestion_UserSurveyAnswer] FOREIGN KEY ([SurveyID],[QuestionID]) REFERENCES [SurveyQuestion]([SurveyID],[QuestionID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [AnswerTranslation]
	ADD CONSTRAINT [FK_Answer_AnswerTranslation] FOREIGN KEY ([AnswerID]) REFERENCES [Answer]([AnswerID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [AnswerTranslation]
	ADD CONSTRAINT [FK_Locale_AnswerTranslation] FOREIGN KEY ([LocaleID]) REFERENCES [Locale]([LocaleID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [ContentTranslation]
	ADD CONSTRAINT [FK_Content_ContentTranslation] FOREIGN KEY ([ContentID]) REFERENCES [Content]([ContentID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [ContentTranslation]
	ADD CONSTRAINT [FK_Locale_ContentTranslation] FOREIGN KEY ([LocaleID]) REFERENCES [Locale]([LocaleID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [QuestionTranslation]
	ADD CONSTRAINT [FK_Question_QuestionTranslation] FOREIGN KEY ([QuestionID]) REFERENCES [Question]([QuestionID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [QuestionTranslation]
	ADD CONSTRAINT [FK_Locale_QuestionTranslation] FOREIGN KEY ([LocaleID]) REFERENCES [Locale]([LocaleID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE [UserContentPreference]
	ADD CONSTRAINT [FK_Locale_UserContentPreference] FOREIGN KEY ([DefaultLocaleID]) REFERENCES [Locale]([LocaleID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION

ALTER TABLE CardTypeTranslation
	ADD CONSTRAINT FK_CardType_CardTypeTranslation FOREIGN KEY (CardTypeID) REFERENCES CardType(CardTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE CardTypeTranslation
	ADD CONSTRAINT FK_Locale_CardTypeTranslation FOREIGN KEY (LocaleID) REFERENCES Locale(LocaleID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE MemberIDCard
	ADD CONSTRAINT FK_CardTypeTranslation_MemberIDCard FOREIGN KEY (CardTypeID, LocaleID) REFERENCES CardTypeTranslation(CardTypeID,LocaleID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE MemberIDCard
	ADD CONSTRAINT FK_CardViewMode_MemberIDCard FOREIGN KEY (CardViewModeID) REFERENCES CardViewMode(CardViewModeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE MemberIDCard
	ADD CONSTRAINT FK_Locale_MemberIDCard FOREIGN KEY (LocaleID) REFERENCES Locale(LocaleID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE CardType
	ADD CONSTRAINT FK_InsuranceType_CardType FOREIGN KEY (InsuranceTypeID) REFERENCES InsuranceType(InsuranceTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go






