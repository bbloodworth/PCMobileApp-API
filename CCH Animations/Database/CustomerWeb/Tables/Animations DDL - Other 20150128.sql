
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

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_UserContentPreference' and object_id = OBJECT_ID('dbo.UserContentPreference'))
DROP INDEX idx_pk_UserContentPreference ON UserContentPreference
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1UserContentPreference' and object_id = OBJECT_ID('dbo.UserContentPreference'))
DROP INDEX idx_fk_1UserContentPreference ON UserContentPreference
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_Video' and object_id = OBJECT_ID('dbo.Video'))
DROP INDEX idx_pk_Video ON Video
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1Video' and object_id = OBJECT_ID('dbo.Video'))
DROP INDEX idx_fk_1Video ON Video
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_UserSurveyAnswer' and object_id = OBJECT_ID('dbo.UserSurveyAnswer'))
DROP INDEX idx_pk_UserSurveyAnswer ON UserSurveyAnswer
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

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_UserContent' and object_id = OBJECT_ID('dbo.UserContent'))
DROP INDEX idx_pk_UserContent ON UserContent
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

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_CampaignContent' and object_id = OBJECT_ID('dbo.CampaignContent'))
DROP INDEX idx_pk_CampaignContent ON CampaignContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1CampaignContent' and object_id = OBJECT_ID('dbo.CampaignContent'))
DROP INDEX idx_fk_1CampaignContent ON CampaignContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2CampaignContent' and object_id = OBJECT_ID('dbo.CampaignContent'))
DROP INDEX idx_fk_2CampaignContent ON CampaignContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_CampaignMember' and object_id = OBJECT_ID('dbo.CampaignMember'))
DROP INDEX idx_pk_CampaignMember ON CampaignMember
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1CampaignMember' and object_id = OBJECT_ID('dbo.CampaignMember'))
DROP INDEX idx_fk_1CampaignMember ON CampaignMember
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2CampaignMember' and object_id = OBJECT_ID('dbo.CampaignMember'))
DROP INDEX idx_fk_2CampaignMember ON CampaignMember
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_Campaign' and object_id = OBJECT_ID('dbo.Campaign'))
DROP INDEX idx_pk_Campaign ON Campaign
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_SurveyQuestion' and object_id = OBJECT_ID('dbo.SurveyQuestion'))
DROP INDEX idx_pk_SurveyQuestion ON SurveyQuestion
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1SurveyQuestion' and object_id = OBJECT_ID('dbo.SurveyQuestion'))
DROP INDEX idx_fk_1SurveyQuestion ON SurveyQuestion
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2SurveyQuestion' and object_id = OBJECT_ID('dbo.SurveyQuestion'))
DROP INDEX idx_fk_2SurveyQuestion ON SurveyQuestion
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_Survey' and object_id = OBJECT_ID('dbo.Survey'))
DROP INDEX idx_pk_Survey ON Survey
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_1Survey' and object_id = OBJECT_ID('dbo.Survey'))
DROP INDEX idx_fk_1Survey ON Survey
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_QuestionAnswer' and object_id = OBJECT_ID('dbo.QuestionAnswer'))
DROP INDEX idx_pk_QuestionAnswer ON QuestionAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_2QuestionAnswer' and object_id = OBJECT_ID('dbo.QuestionAnswer'))
DROP INDEX idx_fk_2QuestionAnswer ON QuestionAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_fk_3QuestionAnswer' and object_id = OBJECT_ID('dbo.QuestionAnswer'))
DROP INDEX idx_fk_3QuestionAnswer ON QuestionAnswer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_pk_Question' and object_id = OBJECT_ID('dbo.Question'))
DROP INDEX idx_pk_Question ON Question
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_1Question ON Question
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_QuestionType ON QuestionType
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_Answer ON Answer
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ExperienceLog ON ExperienceLog
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_1ExperienceLog ON ExperienceLog
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_2ExperienceLog ON ExperienceLog
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ExperienceEvent ON ExperienceEvent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_Resource ON Resource
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_Animation ON Animation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_1Animation ON Animation
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ContentTypeState ON ContentTypeState
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_1ContentTypeState ON ContentTypeState
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_2ContentTypeState ON ContentTypeState
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ContentStatus ON ContentStatus
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_1RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_2RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_3RelatedContent ON RelatedContent
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ContentDisplayRule ON ContentDisplayRule
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_Content ON Content
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_fk_2Content ON Content
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ContentType ON ContentType
go

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '' and object_id = OBJECT_ID('dbo.'))
DROP INDEX idx_pk_ExcludedMember ON ExcludedMember
go

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

CREATE TABLE Answer
( 
	AnswerID           integer  NOT NULL,
	AnswerText         nvarchar(250)  NULL ,
	CreateDate         datetime  NULL
)
go

ALTER TABLE Answer
	ADD CONSTRAINT idx_pk_Answer PRIMARY KEY  CLUSTERED (AnswerID ASC)
go

CREATE TABLE Campaign
( 
	CampaignID         integer  NOT NULL ,
	CampaignDesc       nvarchar(2000)  NULL ,
	CampaignActiveInd  bit  NOT NULL ,
	TargetPopulationDesc nvarchar(2000)  NULL ,
	CampaignPeriodDesc nvarchar(100)  NULL ,
	TargetProcedureName nvarchar(50)  NULL, 
	AuthRequiredInd bit NOT NULL DEFAULT 1,
	SavingsMonthStartDate datetime NULL,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE Campaign
	ADD CONSTRAINT idx_pk_Campaign PRIMARY KEY  CLUSTERED (CampaignID ASC)
go

CREATE TABLE Content
( 
	ContentID          integer  NOT NULL,
	ContentTypeID      integer  NOT NULL,
	ContentName		 nvarchar(100) NULL,
	ContentTitle       nvarchar(50)  NULL ,
	ContentDesc        nvarchar(2000)  NULL ,
	ContentSourceDesc  nvarchar(100)  NULL ,
	ContentImageFileName nvarchar(100)  NULL ,
	ContentFileLocationDesc nvarchar(100)  NULL ,
	ContentPointsCount integer  NULL ,
	ContentDurationSecondsCount integer  NULL ,
	ContentCaptionText nvarchar(250)  NULL ,
	IntroContentInd	 bit NOT NULL DEFAULT 0,
	ContentURL		 nvarchar(100) NULL,
	ContentPhoneNum	 nvarchar(50) NULL,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE Content
	ADD CONSTRAINT idx_pk_Content PRIMARY KEY  CLUSTERED (ContentID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_2Content ON Content
( 
	ContentTypeID       ASC
)
go

CREATE TABLE Video
( 
	VideoID            integer  NOT NULL ,
	VideoFileName      nvarchar(50)  NULL ,
	PersonalizedVideoInd bit  NULL ,
	VideoDataProcName  nvarchar(50)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE Video
	ADD CONSTRAINT idx_pk_Video PRIMARY KEY  CLUSTERED (VideoID ASC)
go
/*
CREATE UNIQUE NONCLUSTERED INDEX idx_fk_1Video ON Video
( 
	VideoID             ASC
)
go
*/

CREATE TABLE Animation
( 
	AnimationID        integer  NOT NULL ,
	AnimationScriptFileName nvarchar(50)  NULL ,
	InteractiveAnimationInd bit  NULL ,
	AnimationDataProcName nvarchar(50)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE Animation
	ADD CONSTRAINT idx_pk_Animation PRIMARY KEY  CLUSTERED (AnimationID ASC)
go

/*CREATE UNIQUE NONCLUSTERED INDEX idx_fk_1Animation ON Animation
( 
	AnimationID         ASC
)
go
*/

CREATE TABLE CampaignContent
( 
	CampaignID         integer  NOT NULL ,
	ContentID          integer  NOT NULL ,
	ActivationDate	 datetime NULL,
	ExpirationDate	 datetime NULL,
	UserContentInd	 bit NOT NULL default 1,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE CampaignContent
	ADD CONSTRAINT idx_pk_CampaignContent PRIMARY KEY  CLUSTERED (CampaignID ASC,ContentID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1CampaignContent ON CampaignContent
( 
	CampaignID          ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2CampaignContent ON CampaignContent
( 
	ContentID           ASC
)
go

CREATE TABLE CampaignMember
( 
	CampaignID         integer  NOT NULL ,
	CCHID              integer  NOT NULL ,
	Savings			 money NULL,
	Score				 float NULL,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE CampaignMember
	ADD CONSTRAINT idx_pk_CampaignMember PRIMARY KEY  CLUSTERED (CampaignID ASC,CCHID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1CampaignMember ON CampaignMember
( 
	CampaignID          ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2CampaignMember ON CampaignMember
( 
	CCHID               ASC
)
go


CREATE TABLE ContentDisplayRule
( 
	ContentDisplayRuleID integer  NOT NULL,
	ContentDisplayRuleDesc nvarchar(100)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ContentDisplayRule
	ADD CONSTRAINT idx_pk_ContentDisplayRule PRIMARY KEY  CLUSTERED (ContentDisplayRuleID ASC)
go

CREATE TABLE ContentStatus
( 
	ContentStatusID    integer  NOT NULL  ,
	ContentStatusDesc  nvarchar(100)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ContentStatus
	ADD CONSTRAINT idx_pk_ContentStatus PRIMARY KEY  CLUSTERED (ContentStatusID ASC)
go

CREATE TABLE ContentType
( 
	ContentTypeID      integer  NOT NULL ,
	ContentTypeDesc    nvarchar(100)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ContentType
	ADD CONSTRAINT idx_pk_ContentType PRIMARY KEY  CLUSTERED (ContentTypeID ASC)
go

CREATE TABLE ContentTypeState
( 
	ContentTypeID      integer  NOT NULL ,
	ContentStatusID    integer  NOT NULL ,
	InitialStateInd    bit  NULL ,
	EndStateInd        bit  NULL ,
	ContentStatusCaptionText nvarchar(250)  NULL,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ContentTypeState
	ADD CONSTRAINT idx_pk_ContentTypeState PRIMARY KEY  CLUSTERED (ContentTypeID ASC,ContentStatusID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1ContentTypeState ON ContentTypeState
( 
	ContentTypeID       ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2ContentTypeState ON ContentTypeState
( 
	ContentStatusID     ASC
)
go

CREATE TABLE UserContentPreference
( 
	CCHID              integer  NOT NULL ,
	SMSInd             bit  NULL ,
	EmailInd           bit  NULL ,
	OSBasedAlertInd    bit  NULL ,
	LastUpdateDate     datetime  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE UserContentPreference
	ADD CONSTRAINT idx_pk_UserContentPreference PRIMARY KEY  CLUSTERED (CCHID ASC)
go

/*CREATE UNIQUE NONCLUSTERED INDEX idx_fk_1UserContentPreference ON UserContentPreference
( 
	CCHID               ASC
)
go
*/

CREATE TABLE ExperienceEvent
( 
	ExperienceEventID  integer  NOT NULL ,
	ExperienceEventDesc nvarchar(100)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ExperienceEvent
	ADD CONSTRAINT idx_pk_ExperienceEvent PRIMARY KEY  CLUSTERED (ExperienceEventID ASC)
go

CREATE TABLE ExperienceLog
( 
	ExperienceLogID    integer  NOT NULL  IDENTITY ( 1,1 ) ,
	ExperienceEventID  integer  NOT NULL ,
	CCHID              integer  NULL ,
	ContentID			 integer NULL,
	ExperienceUserID   nvarchar(36)  NOT NULL,
	ExperienceLogCommentText nvarchar(250)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ExperienceLog
	ADD CONSTRAINT idx_pk_ExperienceLog PRIMARY KEY  CLUSTERED (ExperienceLogID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1ExperienceLog ON ExperienceLog
( 
	CCHID               ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2ExperienceLog ON ExperienceLog
( 
	ExperienceEventID   ASC
)
go

CREATE TABLE Question
( 
	QuestionID         integer  NOT NULL ,
	QuestionTypeID     integer  NOT NULL,
	QuestionText       nvarchar(250)  NULL ,
	ExpectedAnswerDatatypeDesc nvarchar(100)  NULL ,
	ExpectedAnswerValueRangeDesc nvarchar(100)  NULL ,
	ScoredQuestionInd  bit  NULL ,
	CreateDate         datetime  NULL ,
)
go

ALTER TABLE Question
	ADD CONSTRAINT idx_pk_Question PRIMARY KEY  CLUSTERED (QuestionID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1Question ON Question
( 
	QuestionTypeID      ASC
)
go

CREATE TABLE QuestionAnswer
( 
	QuestionID         integer  NOT NULL,
	AnswerID           integer  NOT NULL ,
	AnswerDisplayOrderNum integer  NULL ,
	CorrectAnswerInd   bit  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE QuestionAnswer
	ADD CONSTRAINT idx_pk_QuestionAnswer PRIMARY KEY  CLUSTERED (AnswerID ASC,QuestionID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_2QuestionAnswer ON QuestionAnswer
( 
	AnswerID            ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_3QuestionAnswer ON QuestionAnswer
( 
	QuestionID          ASC
)
go

CREATE TABLE QuestionType
( 
	QuestionTypeID     integer  NOT NULL ,
	QuestionTypeDesc   nvarchar(100)  NULL ,
	CreateDate	datetime,
)
go

ALTER TABLE QuestionType
	ADD CONSTRAINT idx_pk_QuestionType PRIMARY KEY  CLUSTERED (QuestionTypeID ASC)
go

CREATE TABLE RelatedContent
( 
	ParentContentID    integer  NOT NULL ,
	RelatedContentID   integer  NOT NULL ,
	RelatedContentDisplayRuleID integer  NULL,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE RelatedContent
	ADD CONSTRAINT idx_pk_RelatedContent PRIMARY KEY  CLUSTERED (ParentContentID ASC,RelatedContentID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1RelatedContent ON RelatedContent
( 
	ParentContentID     ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2RelatedContent ON RelatedContent
( 
	RelatedContentID    ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_3RelatedContent ON RelatedContent
( 
	RelatedContentDisplayRuleID  ASC
)
go

CREATE TABLE Resource
( 
	ResourceID         integer  NOT NULL ,
	ResourceName       nvarchar(50)  NULL ,
	ResourceIconFileName nvarchar(50)  NULL ,
	ResourceVideoImageURL nvarchar(100)  NULL ,
	ResourceDesc       nvarchar(100)  NULL ,
	ResourcePhoneNum   nvarchar(20) NULL ,
	ResourceURL        nvarchar(100)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE Resource
	ADD CONSTRAINT idx_pk_Resource PRIMARY KEY  CLUSTERED (ResourceID ASC)
go

CREATE TABLE Survey
( 
	SurveyID           integer  NOT NULL ,
	SurveyPassCount    integer  NULL ,
	EmbeddedSurveyInd  bit  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE Survey
	ADD CONSTRAINT idx_pk_Survey PRIMARY KEY  CLUSTERED (SurveyID ASC)
go

CREATE UNIQUE NONCLUSTERED INDEX idx_fk_1Survey ON Survey
( 
	SurveyID            ASC
)
go

CREATE TABLE SurveyQuestion
( 
	SurveyID           integer  NOT NULL ,
	QuestionID         integer  NOT NULL ,
	QuestionDisplayOrderNum integer  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE SurveyQuestion
	ADD CONSTRAINT idx_pk_SurveyQuestion PRIMARY KEY  CLUSTERED (SurveyID ASC,QuestionID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1SurveyQuestion ON SurveyQuestion
( 
	SurveyID            ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_2SurveyQuestion ON SurveyQuestion
( 
	QuestionID          ASC
)
go

CREATE TABLE UserContent
( 
	CCHID              integer  NOT NULL ,
	CampaignID         integer  NOT NULL ,
	ContentID          integer  NOT NULL ,
	ContentStatusChangeDate datetime  NULL ,
	UserContentCommentText nvarchar(2000)  NULL ,
	NotificationSentDate datetime  NULL ,
	ContentSavingsAmt  decimal(8,2)  NULL ,
	MemberContentDataText nvarchar(2000)  NULL ,
	CreateDate         datetime  NULL ,
	ContentStatusID    integer  NOT NULL 
)
go

ALTER TABLE UserContent
	ADD CONSTRAINT idx_pk_UserContent PRIMARY KEY  CLUSTERED (CCHID ASC,CampaignID ASC,ContentID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1UserContent ON UserContent
( 
	CCHID               ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_4UserContent ON UserContent
( 
	ContentID           ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_5UserContent ON UserContent
( 
	CampaignID          ASC,
	ContentID           ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_6UserContent ON UserContent
( 
	ContentStatusID     ASC
)
go


CREATE TABLE ExcludedMember
( 
	CCHID              integer  NOT NULL ,
	ExcludeReasonDesc  nvarchar(100)  NULL ,
	CreateDate         datetime  NULL 
)
go

ALTER TABLE ExcludedMember
	ADD CONSTRAINT XPKExcludedMember PRIMARY KEY  CLUSTERED (CCHID ASC)
go

CREATE TABLE UserSurveyAnswer
( 
	CCHID              integer  NOT NULL ,
	CampaignID         integer  NOT NULL ,
	SurveyID           integer  NOT NULL ,
	SurveyIterationNum integer  NOT NULL ,
	QuestionID         integer  NOT NULL ,
	AnswerID           integer  NULL ,
	FreeFormAnswerText nvarchar(500)  NULL ,
	CreateDate         datetime  NULL  
)
go

ALTER TABLE UserSurveyAnswer
	ADD CONSTRAINT idx_pk_UserSurveyAnswer PRIMARY KEY  CLUSTERED (CCHID ASC,SurveyID ASC,QuestionID ASC,SurveyIterationNum ASC,CampaignID ASC)
go

CREATE NONCLUSTERED INDEX idx_fk_1UserSurveyAnswer ON UserSurveyAnswer
( 
	CCHID               ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_3UserSurveyAnswer ON UserSurveyAnswer
( 
	AnswerID            ASC,
	QuestionID          ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_6UserSurveyAnswer ON UserSurveyAnswer
( 
	CCHID               ASC,
	CampaignID          ASC,
	SurveyID            ASC
)
go

CREATE NONCLUSTERED INDEX idx_fk_5UserSurveyAnswer ON UserSurveyAnswer
( 
	SurveyID            ASC,
	QuestionID          ASC
)
go


ALTER TABLE Content
	ADD CONSTRAINT FK_ContentType_Content FOREIGN KEY (ContentTypeID) REFERENCES ContentType(ContentTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE Video
	ADD CONSTRAINT FK_Content_Video FOREIGN KEY (VideoID) REFERENCES Content(ContentID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
go


ALTER TABLE Animation
	ADD CONSTRAINT FK_Content_Animation FOREIGN KEY (AnimationID) REFERENCES Content(ContentID)
		ON DELETE CASCADE
		ON UPDATE CASCADE
go


ALTER TABLE CampaignContent
	ADD CONSTRAINT FK_Campaign_CampaignContent FOREIGN KEY (CampaignID) REFERENCES Campaign(CampaignID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE CampaignContent
	ADD CONSTRAINT FK_Content_CampaignContent FOREIGN KEY (ContentID) REFERENCES Content(ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE CampaignMember
	ADD CONSTRAINT FK_Campaign_CampaignMember FOREIGN KEY (CampaignID) REFERENCES Campaign(CampaignID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

/* -- Remove FK's to Enrollments
ALTER TABLE CampaignMember
	ADD CONSTRAINT FK_Enrollments_CampaignMember FOREIGN KEY (CCHID) REFERENCES Enrollments(CCHID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go
*/

ALTER TABLE ContentTypeState
	ADD CONSTRAINT FK_ContType_ContTypeState FOREIGN KEY (ContentTypeID) REFERENCES ContentType(ContentTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE ContentTypeState
	ADD CONSTRAINT FK_ContStatus_ContTypeState FOREIGN KEY (ContentStatusID) REFERENCES ContentStatus(ContentStatusID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

/*-- Remove FKs to Enrollments
ALTER TABLE UserContentPreference
	ADD CONSTRAINT FK_Enrollments_UserContentPref FOREIGN KEY (CCHID) REFERENCES Enrollments(CCHID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go
*/

/*-- Remove FKs to Enrollments
ALTER TABLE ExperienceLog
	ADD CONSTRAINT FK_Enrollments_ExperienceLog FOREIGN KEY (CCHID) REFERENCES Enrollments(CCHID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go
*/

ALTER TABLE ExperienceLog
	ADD CONSTRAINT FK_ExpEvent_ExpLog FOREIGN KEY (ExperienceEventID) REFERENCES ExperienceEvent(ExperienceEventID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE Question
	ADD CONSTRAINT FK_QuestionType_Question FOREIGN KEY (QuestionTypeID) REFERENCES QuestionType(QuestionTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE QuestionAnswer
	ADD CONSTRAINT FK_Answer_QuestionAnswer FOREIGN KEY (AnswerID) REFERENCES Answer(AnswerID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE QuestionAnswer
	ADD CONSTRAINT FK_Question_QuestionAnswer FOREIGN KEY (QuestionID) REFERENCES Question(QuestionID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE RelatedContent
	ADD CONSTRAINT FK_Content_RelatedContent_1 FOREIGN KEY (ParentContentID) REFERENCES Content(ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE RelatedContent
	ADD CONSTRAINT FK_Content_RelatedContent_2 FOREIGN KEY (RelatedContentID) REFERENCES Content(ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE RelatedContent
	ADD CONSTRAINT FK_ContDisplayRule_RelatedCont FOREIGN KEY (RelatedContentDisplayRuleID) REFERENCES ContentDisplayRule(ContentDisplayRuleID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE Survey
	ADD CONSTRAINT FK_Content_Survey FOREIGN KEY (SurveyID) REFERENCES Content(ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE SurveyQuestion
	ADD CONSTRAINT FK_Survey_SurveyQuestion FOREIGN KEY (SurveyID) REFERENCES Survey(SurveyID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE SurveyQuestion
	ADD CONSTRAINT FK_Question_SurveyQuestion FOREIGN KEY (QuestionID) REFERENCES Question(QuestionID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

/*-- Remove FKs to Enrollments
ALTER TABLE UserContent
	ADD CONSTRAINT FK_Enrollments_UserContent FOREIGN KEY (CCHID) REFERENCES Enrollments(CCHID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go
*/

ALTER TABLE UserContent
	ADD CONSTRAINT FK_Content_UserContent FOREIGN KEY (ContentID) REFERENCES Content(ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE UserContent
	ADD CONSTRAINT FK_CampaignContent_UserContent FOREIGN KEY (CampaignID,ContentID) REFERENCES CampaignContent(CampaignID,ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE UserContent
	ADD CONSTRAINT FK_ContentStatus_UserContent FOREIGN KEY (ContentStatusID) REFERENCES ContentStatus(ContentStatusID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

/*-- Remove FKs to Enrollments
ALTER TABLE UserSurveyAnswer
	ADD CONSTRAINT FK_Enrollments_UserSurveyAns FOREIGN KEY (CCHID) REFERENCES Enrollments(CCHID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go
*/

ALTER TABLE UserSurveyAnswer
	ADD CONSTRAINT FK_QuestionAns_UserSurveyAns FOREIGN KEY (AnswerID,QuestionID) REFERENCES QuestionAnswer(AnswerID,QuestionID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE UserSurveyAnswer
	ADD CONSTRAINT FK_UserContent_UserSurveyAnswer FOREIGN KEY (CCHID,CampaignID,SurveyID) REFERENCES UserContent(CCHID,CampaignID,ContentID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE UserSurveyAnswer
	ADD CONSTRAINT FK_SurveyQuestion_UserSurveyAnswer FOREIGN KEY (SurveyID,QuestionID) REFERENCES SurveyQuestion(SurveyID,QuestionID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

/*-- Remove FKs to Enrollments
ALTER TABLE ExcludedMember
	ADD CONSTRAINT FK_Enrollments_ExcludedMember FOREIGN KEY (CCHID) REFERENCES Enrollments(CCHID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go
*/



