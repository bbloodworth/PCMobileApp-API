/****** Object:  StoredProcedure [dbo].[p_GetSavingsAlertMemberData]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetSavingsAlertMemberData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetSavingsAlertMemberData]
GO

/****** Object:  StoredProcedure [dbo].[p_GetSavingsAlertMemberData]    Script Date: 07/10/2015 13:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-01-20
Description:
     Updates UserContent with the memberdata JSON string needed for the Animation in the Saving's 
     Alert Campaign
      
Declarations:
      
Execute:
      exec p_GetSavingsAlertMemberData
	
Objects Listing:

Tables- dbo.UserContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-01-20  AS       Created
2015-02-05  AS       Updated to pass SavingsMonthStartDate to fn_GetSavingsForCCHID
2015-06-15  AS		 Updated to include Employee Phone number & Employer phone number
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetSavingsAlertMemberData] (
	@CampaignID int
	,@ContentID int
)
as

BEGIN --proc
----------------------------------------------------------
--Declarations
-----------------------------------------------------------
	DECLARE
		@EmployerURL nvarchar(100)
		,@EmployerPhone nvarchar(50)
		,@EmployerLogo nvarchar(50)
		,@EmployerName nvarchar(100)
		,@FPThreshold nvarchar(255)
		,@FPYellowLevel nvarchar(255)
		,@EmployerFPScore float
	
---------------------------------------------------------
--Get all single valued data
---------------------------------------------------------		
	SELECT	
		@EmployerURL = ConfigValue
	FROM
		dbo.clientConfig
	WHERE
		ConfigKey = 'EmployerURL'
		
	SELECT	
		@EmployerPhone = ConfigValue
	FROM
		dbo.clientConfig
	WHERE
		ConfigKey = 'EmployerPhone'
		
	SELECT	
		@EmployerLogo = ConfigValue
	FROM
		dbo.clientConfig
	WHERE
		ConfigKey = 'EmployerLogo'
	
	SELECT	
		@EmployerName = EmployerDisplayName
	FROM
		dbo.ControlData
		
	SELECT 
		@FPThreshold = ConfigValue
	FROM
		dbo.ClientConfig
	WHERE ConfigKey = 'ComplianceThreshold'
	
	SELECT
		@FPYellowLevel = ConfigValue
	FROM
		dbo.ClientConfig
	WHERE
		ConfigKey = 'ComplianceYellowLevel'
		
	IF (SELECT COUNT(*) FROM SC_Scores)=0
	BEGIN
		SELECT @EmployerFPScore = ''
	END
	ELSE
	BEGIN
		SELECT @EmployerFPScore = AVG(Score)
		FROM SC_Scores
		WHERE ISNULL(RelationshipCode,'20')='20'
	END
		
------------------------------------------------
--Update User Content with the JSON string
------------------------------------------------

	UPDATE UserContent
	SET MemberContentDataText =
		'{' +
		'"EmployeeSavings": ' + LTRIM(RTRIM(CONVERT(CHAR,cm.Savings))) + 
		',"EmployeeSCScore": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,cm.Score))),'null') + 
		',"EmployeePhone": "' + ISNULL(LTRIM(RTRIM(ucp.PreferredContactPhoneNum)),'null') + '"' +
		',"FPThreshold": ' + LTRIM(RTRIM(CONVERT(CHAR,@FPThreshold))) + 
		',"FPYellowLevel": ' + LTRIM(RTRIM(CONVERT(CHAR,@FPYellowLevel))) + 
		',"EmployerSCScore": ' + LTRIM(RTRIM(CONVERT(CHAR,@EmployerFPScore))) + 
		',"EmployerLogo": "' + @EmployerLogo + '"' +
		',"EmployerName": "' + @EmployerName + '"' +
		',"EmployerPhone": "' + @EmployerPhone + '"' +
		',"EmployerURL": "' + @EmployerURL + '/sign_in.aspx?dest=pc"' +
		'}'
	FROM
		UserContent uc
		INNER JOIN CampaignMember cm
			on uc.CCHID = cm.CCHID
			and uc.CampaignID = cm.CampaignID
		INNER JOIN UserContentPreference ucp
			on uc.CCHID = ucp.CCHID
	WHERE
		uc.CampaignID = @CampaignID
		AND uc.ContentID = @ContentID

END
 

GO


