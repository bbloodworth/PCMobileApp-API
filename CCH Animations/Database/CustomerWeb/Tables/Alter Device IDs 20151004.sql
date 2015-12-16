IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'Device')
BEGIN
	ALTER TABLE Device
	DROP CONSTRAINT idx_pk_Device

	IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Device' and name = 'DeviceID')
		ALTER TABLE Device
		ALTER COLUMN DeviceID nvarchar(100) NOT NULL

	ALTER TABLE Device
		ADD CONSTRAINT idx_pk_Device PRIMARY KEY  CLUSTERED (DeviceID ASC)
END
go

IF EXISTS (SELECT 1 FROM SYSOBJECTS WHERE TYPE = 'U' AND NAME = 'ExperienceDevice')
BEGIN
	ALTER TABLE ExperienceDevice
	DROP CONSTRAINT idx_pk_ExperienceDevice
	
	IF EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'ExperienceDevice' and name = 'DeviceID')
		ALTER TABLE ExperienceDevice
		ALTER COLUMN DeviceID nvarchar(100) NOT NULL
		
	ALTER TABLE ExperienceDevice
	ADD CONSTRAINT idx_pk_ExperienceDevice PRIMARY KEY  CLUSTERED (ExperienceUserID ASC,DeviceID ASC)
END
		
