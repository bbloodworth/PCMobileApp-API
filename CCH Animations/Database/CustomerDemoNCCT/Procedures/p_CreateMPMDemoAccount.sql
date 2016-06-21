USE [CCH_CustomerDemoNCCT]
GO

/****** Object:  StoredProcedure [dbo].[p_CreateMPMDemoAccount]    Script Date: 6/9/2016 2:54:09 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_CreateMPMDemoAccount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_CreateMPMDemoAccount]
GO

/****** Object:  StoredProcedure [dbo].[p_CreateMPMDemoAccount]    Script Date: 6/9/2016 2:54:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_CreateMPMDemoAccount]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[p_CreateMPMDemoAccount] AS' 
END
GO



ALTER procedure [dbo].[p_CreateMPMDemoAccount]
(@rcpts varchar(500) = null)
as
set		XACT_ABORT on;
set		nocount on;



/**************************************************************
Author: SF
Create Date: 

Purpose:
Create MPM demo account and push to A/A2/L
--------------------------------------------------------------
2016-06-08 SF Added email of credentials for registration. Removed KermitDB push.


**************************************************************/



set @rcpts = isnull(@rcpts,'sqlserveradmin@clearcosthealth.com')

begin try
begin transaction MPMDemo


-- create named transaction
-- static data
declare	@firstname varchar(50) = 'Mary', 
		@lastname varchar(50) = 'Smith', 
		@email varchar(75) = '@mpmdemo', 
		@memids varchar(10) = 'TESTMPMd',
		@cchid int

-- dynamic - will be used to increment email, ssn and memberid's
declare @num varchar(3)
IF  (select	len(cast(max(cast(reverse(substring(reverse(email),5,2)) as int)) as varchar(2)))
	from	enrollments 
	where	email like 'mary.smith@mpmdemo%.com') = 99
Begin
		PRINT 'The MPM demo account has reached max value.'
		RETURN
End
ELSE
Begin
		set		@num = 		
		(select	case when len(cast(max(cast(reverse(substring(reverse(email),5,2)) as int) + 1) as varchar(2))) = 1
					then '0' + cast(max(cast(reverse(substring(reverse(email),5,2)) as int) + 1) as varchar(2))
					else cast(max(cast(reverse(substring(reverse(email),5,2)) as int) + 1) as varchar(2))
					end
		from	enrollments 
		where	email like 'mary.smith@mpmdemo%.com')
End;

If @num = 99
Begin
		EXEC msdb.dbo.sp_send_dbmail 
		@recipients		= @rcpts,
		@subject		= N'Max MPM demo account reached.' ,
		@importance		= N'HIGH',
		@body			= N'See Processing.CCH_CustomerDemoNCCT.p_CreateMPMDemoAccount',
		@profile_name	= 'clearcosthealth'; 
End;


Insert into cch_CustomerdemoNCCT.dbo.Enrollments (
		EmployeeID,MemberMedicalID,MemberRXID,SubscriberMedicalID,SubscriberRXID,LastName,FirstName,Middle,DateOfBirth,Gender,
		RelationshipCode,Address1,Address2,City,State,Zipcode,Email,Longitude,Latitude,UnRegisteredID,
		OptInIncentiveProgram,Insurer,RXProvider,HealthPlanType,MedicalPlanType,RXPlanType,Phone,UserID,OptInEmailAlerts,OptInTextMsgAlerts,
		MobilePhone,OptInPriceConcierge,LastUpdated,UpdateNotes,MemberSSN,SubscriberSSN,Referral_CCHID,MemberClassification,Subscriber_Email_Home,Subscriber_Email_Work,
		Member_Email_Home,Member_Email_Work,Subscriber_Phone_Home,Subscriber_Phone_Mobile,Member_Phone_Home,Member_Phone_Mobile,Member_Status,Interested_In_CCH,Contract_Prefix,Contract_Suffix,
		DateAdded,Subscriber_Coverage_Effective_Dt,Subscriber_Coverage_Termination_Dt,Notes,PropertyCode,PCP_Name,PCP_NPI,HearCCH,TandCIndicator,Source_Phone,
		Email_Phone,DateRegistered_Phone,AlternatePlanSource,MemberSSN_Full,PartialGeocode,RegistrationType)
Select 
		@memids + @num, --<< EmployeeID,
		@memids + @num, --<< MemberMedicalID
		@memids + @num, --<< MemberRXID
		@memids + @num, --<< SubscriberMedicalID
		@memids + @num, --<< SubscriberRXID
		@lastname,--<< LastName
		@firstname, --<< FirstName
		Middle,
		'01/01/1980', --<< DateOfBirth
		Gender,	RelationshipCode,Address1,Address2,City,State,Zipcode,
		@firstname + '.' + @lastname + @email + @num + '.com', --<< Email
		Longitude,Latitude,UnRegisteredID,
		OptInIncentiveProgram,Insurer,RXProvider,HealthPlanType,MedicalPlanType,RXPlanType,Phone,UserID,OptInEmailAlerts,OptInTextMsgAlerts,
		MobilePhone,OptInPriceConcierge,LastUpdated,UpdateNotes,
		@num + @num, --<< MemberSSN
		@num + @num, --<< SubscriberSSN
		Referral_CCHID,MemberClassification,Subscriber_Email_Home,Subscriber_Email_Work,
		Member_Email_Home,Member_Email_Work,Subscriber_Phone_Home,Subscriber_Phone_Mobile,Member_Phone_Home,Member_Phone_Mobile,Member_Status,Interested_In_CCH,Contract_Prefix,Contract_Suffix,
		DateAdded,Subscriber_Coverage_Effective_Dt,Subscriber_Coverage_Termination_Dt,Notes,PropertyCode,PCP_Name,PCP_NPI,
		HearCCH, --<< mod to change brand from Caesars
		TandCIndicator,Source_Phone,
		Email_Phone,DateRegistered_Phone,AlternatePlanSource,
		'12345' + @num + @num, --<< MemberSSN_Full
		PartialGeocode,RegistrationType
From	dbo.MPM_ACCTBaseline_Enrollments;

Set		@cchid  = SCOPE_IDENTITY();

/********************************************************************************************************************************************************/



/*********************************************/
-- Select * from MPM_ACCTBaseline_PersonApplicationAccess -- <<---------------- MPM = 1
Insert into PersonApplicationAccess 
		(CCHID, 
		CCHApplicationId, 
		CreateDate)
Select	@cchid,
		CCHApplicationId, --<< only MPM
		CreateDate
From	MPM_ACCTBaseline_PersonApplicationAccess;




/*********************************************/
-- Select * from MPM_ACCTBaseline_PersonPlanMembership
insert into PersonPlanMembership
		(CCHID,
		InsuranceTypeID,
		PlanTypeName,
		CoverageTypeDesc,
		MemberId,
		SubscriberID,
		EffectiveDate,
		TerminationDate,
		GroupNum,
		GroupSuffixNum,
		NetworkDesignationDesc,
		PrimaryCareProviderName,
		CreateDate)
select
		@cchid, --<<----- CCHID
		InsuranceTypeID,
		PlanTypeName,
		CoverageTypeDesc,
		@memids + @num, -- <<----- MemberID
		SubscriberID,
		EffectiveDate,
		TerminationDate,
		GroupNum,
		GroupSuffixNum,
		NetworkDesignationDesc,
		PrimaryCareProviderName,
		CreateDate
from	MPM_ACCTBaseline_PersonPlanMembership;



/*********************************************/
--Select * from MPM_ACCTBaseline_CampaignMember
insert into CampaignMember
		(CampaignID,
		CCHID,
		Savings,
		Score,
		CreateDate,
		YourCostSavingsAmt)
select 
		CampaignID,
		@cchid, --<<----- CCHID
		Savings,
		Score,
		CreateDate,
		YourCostSavingsAmt
from	MPM_ACCTBaseline_CampaignMember;



/*********************************************/
--Select * from MPM_ACCTBaseline_UserContentPreference
insert into	UserContentPreference
		(CCHID,
		SMSInd,
		EmailInd,
		OSBasedAlertInd,
		LastUpdateDate,
		CreateDate,
		DefaultLocaleID,
		PreferredContactPhoneNum)
select
		@cchid, --<<----- CCHID
		SMSInd,
		EmailInd,
		OSBasedAlertInd,
		LastUpdateDate,
		CreateDate,
		DefaultLocaleID,
		PreferredContactPhoneNum
from	MPM_ACCTBaseline_UserContentPreference;



/*********************************************/
--Select * from MPM_ACCTBaseline_MemberIDCard -- <<------------------ SecurityTolkenGUID
insert into MemberIDCard
		(CCHID,
		CardTypeID,
		LocaleID,
		CardViewModeID,
		CardMemberDataText,
		SecurityTokenGUID,
		SecurityTokenBeginDatetime,
		SecurityTokenEndDatetime,
		CreateDate)
select 
		@cchid, --<<----- CCHID
		CardTypeID,
		LocaleID,
		CardViewModeID,
		replace(CardMemberDataText,'TEST123456d',@memids + @num), -- <<----- CardMemberDataText,
		newid(), --<< SecurityTokenGUID
		SecurityTokenBeginDatetime,
		SecurityTokenEndDatetime,
		CreateDate
from	MPM_ACCTBaseline_MemberIDCard;



/*********************************************/
-- Select * from MPM_ACCTBaseline_UserContent -- <<-----------------MemberContentDataText
insert into UserContent 
		(CCHID,
		CampaignID,
		ContentID,
		ContentStatusChangeDate,
		UserContentCommentText,
		NotificationSentDate,
		ContentSavingsAmt,
		MemberContentDataText,
		CreateDate,
		ContentStatusID,
		SMSNotificationSentDate,
		SMSNotificationStatusDesc,
		OSNotificationSentDate,
		OSNotificationStatusDesc)
select
		@cchid, --<<----- CCHID
		CampaignID,
		ContentID,
		ContentStatusChangeDate,
		UserContentCommentText,
		NotificationSentDate,
		ContentSavingsAmt,
		MemberContentDataText,
		CreateDate,
		ContentStatusID,
		SMSNotificationSentDate,
		SMSNotificationStatusDesc,
		OSNotificationSentDate,
		OSNotificationStatusDesc
from	MPM_ACCTBaseline_UserContent;

/* backup database cch_customerdemoncct to disk = 'e:\CCHDEMO_pre_mpm_acountchange.bak' with copy_only, stats, init, compression */


-- Alpha
insert into alpha.cch_customerdemoncct.dbo.enrollments
select * from dbo.enrollments where cchid = @cchid;

insert into alpha.cch_customerdemoncct.dbo.CampaignMember
select * from dbo.CampaignMember where cchid = @cchid;

insert into alpha.cch_customerdemoncct.dbo.MemberIDCard
select * from dbo.MemberIDCard where cchid = @cchid;

insert into alpha.cch_customerdemoncct.dbo.PersonApplicationAccess
select * from dbo.PersonApplicationAccess where cchid = @cchid;

insert into alpha.cch_customerdemoncct.dbo.PersonPlanMembership
select * from dbo.PersonPlanMembership where cchid = @cchid;

insert into alpha.cch_customerdemoncct.dbo.UserContent
select * from dbo.UserContent where cchid = @cchid;

insert into alpha.cch_customerdemoncct.dbo.UserContentPreference
select * from dbo.UserContentPreference where cchid = @cchid;


-- alpha2
insert into alpha2.cch_customerdemoncct.dbo.enrollments
select * from dbo.enrollments where cchid = @cchid;

insert into alpha2.cch_customerdemoncct.dbo.CampaignMember
select * from dbo.CampaignMember where cchid = @cchid;

insert into alpha2.cch_customerdemoncct.dbo.MemberIDCard
select * from dbo.MemberIDCard where cchid = @cchid;

insert into alpha2.cch_customerdemoncct.dbo.PersonApplicationAccess
select * from dbo.PersonApplicationAccess where cchid = @cchid;

insert into alpha2.cch_customerdemoncct.dbo.PersonPlanMembership
select * from dbo.PersonPlanMembership where cchid = @cchid;

insert into alpha2.cch_customerdemoncct.dbo.UserContent
select * from dbo.UserContent where cchid = @cchid;

insert into alpha2.cch_customerdemoncct.dbo.UserContentPreference
select * from dbo.UserContentPreference where cchid = @cchid;


---- kermitdb
--insert into kermitdb.cch_customerdemoncct.dbo.enrollments
--select * from dbo.enrollments where cchid = @cchid;

--insert into kermitdb.cch_customerdemoncct.dbo.CampaignMember
--select * from dbo.CampaignMember where cchid = @cchid;

--insert into kermitdb.cch_customerdemoncct.dbo.MemberIDCard
--select * from dbo.MemberIDCard where cchid = @cchid;

--insert into kermitdb.cch_customerdemoncct.dbo.PersonApplicationAccess
--select * from dbo.PersonApplicationAccess where cchid = @cchid;

--insert into kermitdb.cch_customerdemoncct.dbo.PersonPlanMembership
--select * from dbo.PersonPlanMembership where cchid = @cchid;

--insert into kermitdb.cch_customerdemoncct.dbo.UserContent
--select * from dbo.UserContent where cchid = @cchid;

--insert into kermitdb.cch_customerdemoncct.dbo.UserContentPreference
--select * from dbo.UserContentPreference where cchid = @cchid;



-- Live
insert into live.cch_customerdemoncct.dbo.enrollments
select * from dbo.enrollments where cchid = @cchid;

insert into live.cch_customerdemoncct.dbo.CampaignMember
select * from dbo.CampaignMember where cchid = @cchid;

insert into live.cch_customerdemoncct.dbo.MemberIDCard
select * from dbo.MemberIDCard where cchid = @cchid;

insert into live.cch_customerdemoncct.dbo.PersonApplicationAccess
select * from dbo.PersonApplicationAccess where cchid = @cchid;

insert into live.cch_customerdemoncct.dbo.PersonPlanMembership
select * from dbo.PersonPlanMembership where cchid = @cchid;

insert into live.cch_customerdemoncct.dbo.UserContent
select * from dbo.UserContent where cchid = @cchid;

insert into live.cch_customerdemoncct.dbo.UserContentPreference
select * from dbo.UserContentPreference where cchid = @cchid;

commit transaction MPMDemo


declare @accountinfo varchar(150);

select @accountinfo =  email 
		+ ' : ' 
		+ lastname 
		+ ' : 1980-01-01 : ' 
		+ MemberSSN 
from
	Enrollments 
where
	cchid = @cchid;

EXEC msdb.dbo.sp_send_dbmail 
	@recipients		= N'avija@clearcosthealth.com; achen@clearcosthealth.com; sfogli@clearcosthealth.com',
	@subject		= N'MPM demo account created.' ,
	@importance		= N'HIGH',
	@body			= @accountinfo,
	@profile_name	= 'clearcosthealth'; 
end try


begin catch
	ROLLBACK Transaction MPMDemo;

		    EXEC msdb.dbo.sp_send_dbmail 
			@recipients		= @rcpts, 
			@subject		= N'MPM Demo error',
			@body			= 'There was an error incrementing/creating the MPM demo account. Activity has been rolled back.',
			@profile_name	= 'clearcosthealth';   

	Throw
end catch;



GO


