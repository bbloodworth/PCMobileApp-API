USE [CCH_FrontEnd2]
GO

/****** Object:  StoredProcedure [dbo].[GetEnrollmentsForAllEmployers]    Script Date: 4/5/2016 4:15:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE [dbo].[GetEnrollmentsForAllEmployers] 
GO

-- =============================================
-- Author:		Justin Morris
-- Create date: Unknown
-- Description:	Used by mobile application to
--   determine information about user trying to
--   log in.  One of the most important of these
--   is determining which EmployerID (database)
--   the user belongs to.
-- Modified:    10/8/2013 (elm)
--   Added code to handle databases that have
--   a main employer database and alternate
--   plan database(s), such as Caesars and
--   CaesarsHorizon.
-- Modified:    10/17/2013 (elm)
--   Added dynamic charindex() calculation to
--   find the start of the database name from
--   the connection string. Looks for "Catalog=".
--   In prior version it was a static position (44).
-- Modified:    12/9/13 (JRM)
--   Updated to not fault on employers with employer ids greater than 2 characters
--   Also updated to ignore the CCH_CustomerWeb database as it has no enrollment table
-- Modified:	01/31/14 (RL)
-- MOB-202: Make mobile registration check all 9 digits of SSN
-- 2014/02/13 SF Added case statement to dynamic SQL to check length of @lastfour to direct query
-- 2014/04/23  RD  Disregard employer records that are only there for client lookup 
--                 (i.e. no corresponding employer database)
-- 2014/07/25  RD  Account for double last names (e.g. Van Damme) 
--                 that would come in from the Mobile app with a '+' character separator
-- 2014/10/17  RD  Add the PropertyCode column
-- 2015/07/21  RD  The @stmt variable exceeded the varchar(max) limit of 8000 characters because we added the MSC databases
-- 2016/04/05  RD  Replace the parsing logic on the connection string to get the Client DB Name with column EmployerDBName
-- =============================================

CREATE PROCEDURE [dbo].[GetEnrollmentsForAllEmployers] 
	-- Add the parameters for the stored procedure here
	@LastName varchar(40) = '',
	@LastFour varchar(9) = '',
	@DateOfBirth DateTime = '1900-01-01 00:00:00.000',
	@Email varchar(200) = ''
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if	object_id('tempdb..#tempEnrollmentResults') is not null
	drop table #tempEnrollmentResults

    -- Insert statements for procedure here
    create table #tempEnrollmentResults (
		EmployerID int,
		MainEmployerID int,
		Firstname varchar(100),
		Lastname varchar(100),
		CCHID int,
		DateOfBirth datetime,
		MemberSSN_Full int,
		Address1 varchar(100),
		Address2 varchar(100),
		City varchar(50),
		[State] char(2),
		ZipCode varchar(15),
		Latitude float,
		Longitude float,
		Insurer varchar(50),
		RXProvider varchar(50),
		HealthPlanType varchar(50),
		MedicalPlanType varchar(100),
		RXPlanType varchar(50),
		PropertyCode varchar(10)
	)
  
	declare @stmt varchar(max) = 'insert into #tempEnrollmentResults select * from (';
	declare @colsHolder varchar(max) = 'FirstName, LastName, CCHID, DateOfBirth, MemberSSN_Full, Address1, Address2, City, State, ZipCode, Latitude, Longitude, Insurer, RXProvider, HealthPlanType, MedicalPlanType, RXPlanType, PropertyCode from ';
	
	select @stmt = @stmt +
		'select ' + cast(EmployerID as varchar(6)) + ' as EmployerID, ' + 
		cast(isnull(MainEmployerID,EmployerID) as varchar(6)) + ' as MainEmployerID, ' + @colsHolder + 
		EmployerDBName +
		'.dbo.enrollments where ' +
		(case when @LastName <> '' then 'LastName = ''' + REPLACE(@LastName, '+', ' ')  + '''' else '1 = 1' end) + 
		(case when len(@LastFour) = 9  then ' and MemberSSN_Full = ''' + @LastFour + '''' 
				when len(@LastFour) = 4  then ' and MemberSSN = ''' + @LastFour + '''' 
				when len(@LastFour) not in(4,9)  then ' and MemberSSN = ''' + @LastFour + '''' end) +
		(case when @DateOfBirth <> '1900-01-01 00:00:00.000' then ' and DateOfBirth = ''' + cast(@DateOfBirth as varchar(23)) + '''' else '' end) +
		(case when @Email <> '' then ' and Email = ''' + @Email + '''' else '' end)
	from employers
	where employerid = 7 and employername not like '%customer%' and len(connectionstring) > 0 and EmployerDBName not like '%CCH_MSC%' 

	select @stmt = @stmt + 
		' union all select ' + cast(EmployerID as varchar(6)) + ' as EmployerID, ' + 
		cast(isnull(MainEmployerID,EmployerID) as varchar(6)) + ' as MainEmployerID, ' + @colsHolder + 
		EmployerDBName +
		'.dbo.enrollments where ' +
		(case when @LastName <> '' then 'lastname = ''' + REPLACE(@LastName, '+', ' ')  + '''' else '1 = 1' end) + 
		(case when len(@LastFour) = 9  then ' and MemberSSN_Full = ''' + @LastFour + '''' 
				when len(@LastFour) = 4  then ' and MemberSSN = ''' + @LastFour + '''' 
				when len(@LastFour) not in(4,9)  then ' and MemberSSN = ''' + @LastFour + '''' end) +
		(case when @DateOfBirth <> '1900-01-01 00:00:00.000' then ' and dateofbirth = ''' + cast(@DateOfBirth as varchar(23)) + '''' else '' end) +
		(case when @Email <> '' then ' and email = ''' + @Email + '''' else '' end)
	from employers
	where employerid > 7 and employername not like '%customer%' and len(connectionstring) > 0 and EmployerDBName not like '%CCH_MSC%' 

	set @stmt = @stmt + ') as tmpEnr'

	-- print @stmt
	 exec (@stmt)

	-- The following statement deletes rows in the temp table that belong to the "main database" where there are rows for that
	-- CCHID in one of the "alternate plan" databases.
	;with tERs as
	(
		select
			EmployerID
		,	MainEmployerID
		,	FirstName
		,	LastName
		,	CCHID
		,	DateofBirth
		,	MemberSSN_Full
		,	Address1
		,	Address2
		,	City
		,	[State]
		,	ZipCode
		,	Latitude
		,	Longitude
		,	Insurer
		,	RXProvider
		,	HealthPlanType
		,	MedicalplanType
		,	RXPlantype
		,   PropertyCode
		from #tempEnrollmentResults tER
	)
	delete temp
	from tERs t1
	join tERs t2 on
		t1.EmployerID = t2.MainEmployerID
	and t1.CCHID = t2.CCHID
	join #tempEnrollmentResults temp on
		t1.EmployerID = temp.EmployerID
	and t1.CCHID = temp.CCHID
	where t2.EmployerID <> t2.MainEmployerID

	-- Only returns results if there is exactly one match
	-- This is a safety to make sure that if more than one
	-- user matches the criteria provided, we do not return
	-- information on another user.
	if (select COUNT(*) from #tempEnrollmentResults) = 1
	begin
		select tER.*, e.connectionstring
		from #tempEnrollmentResults tER
		join employers e
			on tER.employerid = e.employerid
	end

	drop table #tempEnrollmentResults
END


GO

