------------------------------------------------
--PC-435
--2015-09-03
--Insert Instance Config for Text Message notifications
--------------------------------------------
IF EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'SMSAccountID')
	UPDATE CCH_FrontEnd2.dbo.InstanceConfig
	SET ConfigValue = 'AC530cc20afcfe940de4e58da39f7ebd00', ModifiedDate = GETDATE()
	WHERE ConfigKey = 'SMSAccountID'
ELSE
	INSERT INTO CCH_FrontEnd2.dbo.InstanceConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('SMSAccountID', 'AC530cc20afcfe940de4e58da39f7ebd00',GETDATE(), GETDATE());

IF EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'SMSAuthToken')
	UPDATE CCH_FrontEnd2.dbo.InstanceConfig
	SET ConfigValue = '095d4b35f614152e384a3f6a63aa5587', ModifiedDate = GETDATE()
	WHERE ConfigKey = 'SMSAuthToken'
ELSE
	INSERT INTO CCH_FrontEnd2.dbo.InstanceConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('SMSAuthToken', '095d4b35f614152e384a3f6a63aa5587',GETDATE(), GETDATE());

IF EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'SMSFromPhone')
	UPDATE CCH_FrontEnd2.dbo.InstanceConfig
	SET ConfigValue = '19252369758', ModifiedDate = GETDATE()
	WHERE ConfigKey = 'SMSFromPhone'
ELSE
	INSERT INTO CCH_FrontEnd2.dbo.InstanceConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('SMSFromPhone', '19252369758',GETDATE(), GETDATE());

IF  @@servername = 'SAMB01VM002' 
BEGIN
	IF EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'IsLive')
		UPDATE CCH_FrontEnd2.dbo.InstanceConfig
		SET ConfigValue = 'True', ModifiedDate = GETDATE()
		WHERE ConfigKey = 'IsLive'
	ELSE
		INSERT INTO CCH_FrontEnd2.dbo.InstanceConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
		VALUES ('IsLIve', 'True',GETDATE(), GETDATE())
END
ELSE 
BEGIN
	IF EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'IsLive')
		UPDATE CCH_FrontEnd2.dbo.InstanceConfig
		SET ConfigValue = 'False', ModifiedDate = GETDATE()
		WHERE ConfigKey = 'IsLive'
		
	INSERT INTO CCH_FrontEnd2.dbo.InstanceConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('IsLIve', 'False',GETDATE(), GETDATE())
END

--select * from CCH_FrontEnd2.dbo.InstanceConfig