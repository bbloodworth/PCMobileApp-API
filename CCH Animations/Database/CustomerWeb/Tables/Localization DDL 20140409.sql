
-----------------------------------------------------------------------------
--Table: Locale
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'Locale')
	DROP TABLE Locale
go

CREATE TABLE [Locale]
( 
	[LocaleID]           integer  NOT NULL  IDENTITY ( 1,1 ) ,
	[LocaleCode]         nvarchar(10) ,
	[ISOCountryCode]     nvarchar(10) ,
	[ISOLanguageCode]    nvarchar(10) ,
	[LocaleDesc]         nvarchar(100)  NULL ,
	[DateFormatDesc]     nvarchar(100)  NULL,
	[CreateDate]         datetime  NULL
)
go

ALTER TABLE [Locale]
	ADD CONSTRAINT [XPKLocale] PRIMARY KEY  CLUSTERED ([LocaleID] ASC)
go

-----------------------------------------------------------------------------
--Table: ContentTranslation
-----------------------------------------------------------------------------


IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'ContentTranslation')
	DROP TABLE ContentTranslation
go

CREATE TABLE [ContentTranslation]
( 
	[ContentID]          integer  NOT NULL ,
	[LocaleID]           integer  NOT NULL ,
	[ContentTitle]       nvarchar(50)  NULL ,
	[ContentCaptionText] nvarchar(250)  NULL ,
	[ContentDesc]        nvarchar(2000)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [ContentTranslation]
	ADD CONSTRAINT [XPKContentTranslation] PRIMARY KEY  CLUSTERED ([ContentID] ASC,[LocaleID] ASC)
go

CREATE NONCLUSTERED INDEX [XIF1ContentTranslation] ON [ContentTranslation]
( 
	[ContentID]           ASC
)
go

CREATE NONCLUSTERED INDEX [XIF2ContentTranslation] ON [ContentTranslation]
( 
	[LocaleID]            ASC
)
go

-----------------------------------------------------------------------------
--Table: AnswerTranslation
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'AnswerTranslation')
	DROP TABLE AnswerTranslation
go

CREATE TABLE [AnswerTranslation]
( 
	[AnswerID]           integer  NOT NULL ,
	[LocaleID]           integer  NOT NULL ,
	[AnswerText]         nvarchar(250)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [AnswerTranslation]
	ADD CONSTRAINT [XPKAnswerTranslation] PRIMARY KEY  CLUSTERED ([AnswerID] ASC,[LocaleID] ASC)
go

CREATE NONCLUSTERED INDEX [XIF1AnswerTranslation] ON [AnswerTranslation]
( 
	[AnswerID]            ASC
)
go

CREATE NONCLUSTERED INDEX [XIF2AnswerTranslation] ON [AnswerTranslation]
( 
	[LocaleID]            ASC
)
go

-----------------------------------------------------------------------------
--Table: QuestionTranslation
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'QuestionTranslation')
	DROP TABLE QuestionTranslation
go

CREATE TABLE [QuestionTranslation]
( 
	[QuestionID]         integer  NOT NULL ,
	[LocaleID]           integer  NOT NULL ,
	[QuestionText]       nvarchar(250)  NULL ,
	[CreateDate]         datetime  NULL 
)
go

ALTER TABLE [QuestionTranslation]
	ADD CONSTRAINT [XPKQuestionTranslation] PRIMARY KEY  CLUSTERED ([QuestionID] ASC,[LocaleID] ASC)
go

CREATE NONCLUSTERED INDEX [XIF1QuestionTranslation] ON [QuestionTranslation]
( 
	[QuestionID]          ASC
)
go

CREATE NONCLUSTERED INDEX [XIF2QuestionTranslation] ON [QuestionTranslation]
( 
	[LocaleID]            ASC
)
go

---------------------------------------------------------------------------------
--The FKs
----------------------------------------------------------------------------------
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

----------------------------------------------------------------------------------
--Add FK to UserContentPreference
-----------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'UserContentPreference' and name = 'DefaultLocaleID')
	ALTER TABLE UserContentPreference
	ADD DefaultLocaleID int NULL

ALTER TABLE [UserContentPreference]
	ADD CONSTRAINT [FK_Locale_UserContentPreference] FOREIGN KEY ([DefaultLocaleID]) REFERENCES [Locale]([LocaleID])
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


