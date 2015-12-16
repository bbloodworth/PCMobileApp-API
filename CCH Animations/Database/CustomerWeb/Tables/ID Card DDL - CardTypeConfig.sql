-----------------------------------------------------------------------------
--Drop Constraints
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_CardType_CardTypeConfig' and parent_object_id = OBJECT_ID('dbo.CardTypeConfig'))
ALTER TABLE CardTypeConfig
	DROP CONSTRAINT FK_CardType_CardTypeConfig 
go


-----------------------------------------------------------------------------
--Drop Tables
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM sysobjects WHERE TYPE = 'U' and name = 'CardTypeConfig')
DROP TABLE CardTypeConfig
go



-----------------------------------------------------------------------------
--Table: CardTypeConfig
-----------------------------------------------------------------------------

CREATE TABLE CardTypeConfig
( 
	CardTypeID           integer  NOT NULL ,
	CardTypeConfigKey    nvarchar(250)  NOT NULL ,
	CardTypeConfigValue  nvarchar(250)  NULL ,
	CardTypeConfigValuePrior nvarchar(250)  NULL ,
	CreateDate           datetime  NULL ,
	ModifiedDate         datetime  NULL 
)
go


--------------------------------------------------------------------------
--FKs
--------------------------------------------------------------------------

ALTER TABLE CardTypeConfig
	ADD CONSTRAINT idx_pk_CardTypeConfig PRIMARY KEY  CLUSTERED (CardTypeID ASC,CardTypeConfigKey ASC)
go

ALTER TABLE CardTypeConfig
	ADD CONSTRAINT FK_CardType_CardTypeConfig FOREIGN KEY (CardTypeID) REFERENCES CardType(CardTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

