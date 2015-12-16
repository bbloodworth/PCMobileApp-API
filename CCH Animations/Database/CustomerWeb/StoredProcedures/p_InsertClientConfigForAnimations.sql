/****** Object:  StoredProcedure [dbo].[p_InsertClientConfigForAnimations]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertClientConfigForAnimations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertClientConfigForAnimations]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-01-19
Description:
      Insert Client Config for Animations
      
Declarations:
            
Execute:
      exec p_InsertClientConfigForAnimations


Objects Listing:

Tables- dbo.ClientConfig
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertClientConfigForAnimations] 
as

BEGIN
-------------------------------------------------------------
--Declarations
-------------------------------------------------------------
	DECLARE
		@EmployerID int,
		@EmployerPhoneNum nvarchar(50),
		@EmployerURL nvarchar(100),
		@EmployerLogo nvarchar(100),
		@EmployerEmail nvarchar(50),
		@AnimationsPointsLabel nvarchar(100),
		@PushPromptWaitPeriod int
		
	SELECT	
		@EmployerID = EmployerID
	FROM
		dbo.ControlData
/*---------------------------------------------------------------
--Set Parameters for each client
-----------------------------------------------------------------
11= Caesars
9=Avaya
7=Starbucks
8=Sanmina
16=Hamilton
10000 = CustomerDemo
-------------------------------------------------------------------*/
--Starbucks	
	IF @EmployerID = 7
	BEGIN 
		SET @EmployerLogo = 'StarbucksLogo.png'
		SET @EmployerPhoneNum = '(800)390-6855'
		SET @EmployerURL = 'https://www.clearcosthealth.com/starbucks'
		SET @AnimationsPointsLabel = 'Points'
		SET @EmployerEmail = 'starbucks@clearcosthealth.com'
		SET @PushPromptWaitPeriod = 5
	END
	
--Caesars	
	IF @EmployerID in (11,12,18)
	BEGIN 
		SET @EmployerLogo = 'CaesarsLogo.png'
		SET @EmployerPhoneNum = '(800)318-4168'
		SET @EmployerURL = 'https://www.clearcosthealth.com/caesars'
		SET @AnimationsPointsLabel = 'Points'
		SET @EmployerEmail = 'caesars@clearcosthealth.com'
		SET @PushPromptWaitPeriod = 5
	END
	
--Avaya	
	IF @EmployerID = 9
	BEGIN 
		SET @EmployerLogo = 'AvayaLogo.png'
		SET @EmployerPhoneNum = '(800)371-2773'
		SET @EmployerURL = 'https://www.clearcosthealth.com/avaya'
		SET @AnimationsPointsLabel = 'Points'
		SET @EmployerEmail = 'avaya@clearcosthealth.com'	
		SET @PushPromptWaitPeriod = 5	
	END		
	
--Sanmina
	IF @EmployerID in (8,10)
	BEGIN 
		SET @EmployerLogo = 'SanminaLogo.png'
		SET @EmployerPhoneNum = '(800)419-6405'
		SET @EmployerURL = 'https://www.clearcosthealth.com/sanmina'
		SET @AnimationsPointsLabel = 'Points'
		SET @EmployerEmail = 'sanmina@clearcosthealth.com'		
		SET @PushPromptWaitPeriod = 5		
	END	
	
--Hamilton County
	IF @EmployerID = 16
	BEGIN 
		SET @EmployerLogo = 'HamiltonCountyLogo.png'
		SET @EmployerPhoneNum = '(888)217-2263'
		SET @EmployerURL = 'https://www.clearcosthealth.com/hamiltoncounty'
		SET @AnimationsPointsLabel = 'Points'
		SET @EmployerEmail = 'hamiltoncounty@clearcosthealth.com'		
		SET @PushPromptWaitPeriod = 5				
	END		
	
--Customer Demo
	IF @EmployerID = 10000
	BEGIN 
		SET @EmployerLogo = 'DemoLogo.png'
		SET @EmployerPhoneNum = '(888)888-8888'
		SET @EmployerURL = 'https://www.clearcosthealth.com/demo'
		SET @AnimationsPointsLabel = 'Points'
		SET @EmployerEmail = 'demo@clearcosthealth.com'
		SET @PushPromptWaitPeriod = 5
	END		
	
	
------------------------------------------------------
--Insert/Update ClientConfig table
-------------------------------------------------------
IF EXISTS (SELECT 1 FROM clientConfig where ConfigKey = 'EmployerLogo')
	UPDATE 
		ClientConfig
	SET 
		ConfigValuePrior = ConfigValue
		,ConfigValue = @EmployerLogo
		,ModifiedDate = GETDATE()
	WHERE
		ConfigKey = 'EmployerLogo'
ELSE
	INSERT INTO ClientConfig (
		ConfigKey
		,ConfigValue
		,ConfigValuePrior
		,CreateDate
		,ModifiedDate
		,Description
	)
	VALUES (
		'EmployerLogo'
		,@EmployerLogo
		,NULL
		,GETDATE()
		,GETDATE()
		,'Filename for graphic logo for Employer for Animations Mobile App'
	)

IF EXISTS (SELECT 1 FROM clientConfig where ConfigKey = 'EmployerPhone')
	UPDATE 
		ClientConfig
	SET 
		ConfigValuePrior = ConfigValue
		,ConfigValue = @EmployerPhoneNum
		,ModifiedDate = GETDATE()
	WHERE
		ConfigKey = 'EmployerPhone'
ELSE
	INSERT INTO ClientConfig (
		ConfigKey
		,ConfigValue
		,ConfigValuePrior
		,CreateDate
		,ModifiedDate
		,Description
	)
	VALUES (
		'EmployerPhone'
		,@EmployerPhoneNum
		,NULL
		,GETDATE()
		,GETDATE()
		,'Employer Phone Number for Animations Mobile App'
	)
	
IF EXISTS (SELECT 1 FROM clientConfig where ConfigKey = 'EmployerEmail')
	UPDATE 
		ClientConfig
	SET 
		ConfigValuePrior = ConfigValue
		,ConfigValue = @EmployerEmail
		,ModifiedDate = GETDATE()
	WHERE
		ConfigKey = 'EmployerEmail'
ELSE
	INSERT INTO ClientConfig (
		ConfigKey
		,ConfigValue
		,ConfigValuePrior
		,CreateDate
		,ModifiedDate
		,Description
	)
	VALUES (
		'EmployerEmail'
		,@EmployerEmail
		,NULL
		,GETDATE()
		,GETDATE()
		,'Employer Email Address for Animations Mobile App'
	)
	
IF EXISTS (SELECT 1 FROM clientConfig where ConfigKey = 'EmployerURL')
	UPDATE 
		ClientConfig
	SET 
		ConfigValuePrior = ConfigValue
		,ConfigValue = @EmployerURL
		,ModifiedDate = GETDATE()
	WHERE
		ConfigKey = 'EmployerURL'
ELSE
	INSERT INTO ClientConfig (
		ConfigKey
		,ConfigValue
		,ConfigValuePrior
		,CreateDate
		,ModifiedDate
		,Description
		)
	VALUES (
		'EmployerURL'
		,@EmployerURL
		,NULL
		,GETDATE()
		,GETDATE()
		,'Employer URL for Animations Mobile App'
	)
	
IF EXISTS (SELECT 1 FROM clientConfig where ConfigKey = 'AnimationsPointsLabel')
	UPDATE 
		ClientConfig
	SET 
		ConfigValuePrior = ConfigValue
		,ConfigValue = @AnimationsPointsLabel
		,ModifiedDate = GETDATE()
	WHERE
		ConfigKey = 'AnimationsPointsLabel'
ELSE
	INSERT INTO ClientConfig (
		ConfigKey
		,ConfigValue
		,ConfigValuePrior
		,CreateDate
		,ModifiedDate
		,Description
	)
	VALUES (
		'AnimationsPointsLabel'
		,@AnimationsPointsLabel
		,NULL
		,GETDATE()
		,GETDATE()
		,'Label to use for Anmations points in Employee notifications.'
	)
	
IF EXISTS (SELECT 1 FROM clientConfig where ConfigKey = 'PushNotificationPromptPeriod')
	UPDATE 
		ClientConfig
	SET 
		ConfigValuePrior = ConfigValue
		,ConfigValue = @PushPromptWaitPeriod
		,ModifiedDate = GETDATE()
	WHERE
		ConfigKey = 'PushNotificationPromptPeriod'
ELSE
	INSERT INTO ClientConfig (
		ConfigKey
		,ConfigValue
		,ConfigValuePrior
		,CreateDate
		,ModifiedDate
		,Description
	)
	VALUES (
		'PushNotificationPromptPeriod'
		,@PushPromptWaitPeriod
		,NULL
		,GETDATE()
		,GETDATE()
		,'Defines how many mobile app logins to wait before re-prompting a user to allow push notifications.'
	)

END
 
GO
 
