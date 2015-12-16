-----------------------------------------------------------------------------
--Drop Tables
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'Device')
	DROP TABLE Device
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

