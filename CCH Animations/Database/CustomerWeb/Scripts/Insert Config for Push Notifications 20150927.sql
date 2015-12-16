------------------------------------------------
--PC-435
--2015-09-03
--Insert Instance Config for Push  notifications
--------------------------------------------
IF EXISTS (SELECT 1 FROM CCH_FrontEnd2.dbo.InstanceConfig where ConfigKey = 'PushAccessToken')
	UPDATE CCH_FrontEnd2.dbo.InstanceConfig
	SET ConfigValue = 'wvn0vQ9SP3Bc6yhNlkjOYF8c3BaiA2ABb2YRWLbZ1zI7AtP9Uj7Cwe1d4zTExgmShrb3dTXrLAhQOhFHJUl7', ModifiedDate = GETDATE()
	WHERE ConfigKey = 'PushAccessToken'
ELSE
	INSERT INTO CCH_FrontEnd2.dbo.InstanceConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('PushAccessToken', 'wvn0vQ9SP3Bc6yhNlkjOYF8c3BaiA2ABb2YRWLbZ1zI7AtP9Uj7Cwe1d4zTExgmShrb3dTXrLAhQOhFHJUl7',GETDATE(), GETDATE());

IF EXISTS (SELECT 1 FROM dbo.ClientConfig where ConfigKey = 'PushApplicationCode')
	UPDATE dbo.ClientConfig
	SET ConfigValue = '57BE8-1FB8D', ModifiedDate = GETDATE()
	WHERE ConfigKey = 'PushApplicationCode'
ELSE
	INSERT INTO dbo.ClientConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('PushApplicationCode', '57BE8-1FB8D',GETDATE(), GETDATE());

IF EXISTS (SELECT 1 FROM dbo.ClientConfig where ConfigKey = 'PushApplicationGroup')
	UPDATE dbo.ClientConfig
	SET ConfigValue = '', ModifiedDate = GETDATE()
	WHERE ConfigKey = 'PushApplicationGroup'
ELSE
	INSERT INTO dbo.ClientConfig (ConfigKey, ConfigValue, CreateDate, ModifiedDate)
	VALUES ('PushApplicationGroup', '',GETDATE(), GETDATE());


--select * from CCH_FrontEnd2.dbo.InstanceConfig
--select * from dbo.ClientConfig