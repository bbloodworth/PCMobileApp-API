
-----------------------------------------------------------------------------
--Drop Constraints
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Locale_MemberIDCard' and parent_object_id = OBJECT_ID('dbo.MemberIDCard'))
ALTER TABLE MemberIDCard
	DROP CONSTRAINT FK_Locale_MemberIDCard 
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


-----------------------------------------------------------------------------
--Drop Indexes
-----------------------------------------------------------------------------

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


-----------------------------------------------------------------------------
--Table: CardType
-----------------------------------------------------------------------------

CREATE TABLE CardType
( 
	CardTypeID           integer  NOT NULL  ,
	CardTypeDesc         nvarchar(250)  NULL ,
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
	CardViewModeID       integer  NOT NULL  ,
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
--Table: ExperienceDevice	
-----------------------------------------------------------------------------
CREATE TABLE ExperienceDevice
( 
	ExperienceUserID     nvarchar(36)  NOT NULL ,
	DeviceID             nvarchar(36)  NOT NULL ,
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
	ADD CONSTRAINT FK_CardTypeTranslation_MemberIDCard FOREIGN KEY (CardTypeID, LocaleID) REFERENCES CardTypeTranslation(CardTypeID, LocaleID)
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

