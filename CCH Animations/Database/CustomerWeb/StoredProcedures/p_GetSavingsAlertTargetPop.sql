
/****** Object:  StoredProcedure [dbo].[p_GetSavingsAlertTargetPop]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetSavingsAlertTargetPop]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetSavingsAlertTargetPop]
GO

/****** Object:  StoredProcedure [dbo].[p_GetSavingsAlertTargetPop]    Script Date: 07/10/2015 13:24:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
     Identifies the target Pop of CCHIDs for the Saving's Alert campaign and populates the
     CampainMember table.  Upddates the sproc name on the Campaign.
      
Declarations:
      
Execute:
      exec p_GetSavingsAlertTargetPop
		@CampaignID = 1
	
Objects Listing:

Tables- dbo.CampaignContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-30  AS		 Updated to not exclude Key clients (they will be flagged on the constant contact file)
2015-01-30  AS		 Updated to use last complete month of data from each Pastcare table, not most recent 30 days
2015-02-05  AS		 Updated to pass month to fn_GetSavingsForCCHID
2015-05-06  AS		 Updated to include phone registered as well as web registered members
2015-06-15  AS		 Updated to fix your cost savings calculation
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetSavingsAlertTargetPop] (
	@CampaignID int
)
as

BEGIN --proc
----------------------------------------------------------
--Declarations
-----------------------------------------------------------
	DECLARE
		@MaxDateMedical datetime
		,@MaxDateRx datetime
		,@MinDateMedical datetime
		,@MinDateRx datetime
		,@MedicalLoadedThrough datetime
		,@RxLoadedThrough datetime
		,@EmployerID int
		,@SavingsThreshold money = 0
		,@YourCostThreshold money = 0
		,@IncludeYourCost bit = 0
		,@SavingsMonthStartDate datetime
--------------------------------------------------------
--Get the control data & Set Variables
---------------------------------------------------------		
	SELECT	
		@EmployerID = EmployerID
		,@MedicalLoadedThrough = DataLoadedThrough
		,@RxLoadedThrough= RX_DataLoadedThrough
	FROM
		dbo.ControlData
		
	SELECT
		@SavingsMonthStartDate = SavingsMonthStartDate
	FROM
		dbo.Campaign
	WHERE
		CampaignID = @CampaignID
--------------------------------------------------------------------
--Set Medical / Rx start and end dates
--------------------------------------------------------------------
	SET @MinDateMedical = DATEADD(mm, DATEDIFF(mm, 0, @SavingsMonthStartDate), 0)
	SET @MaxDateMedical = DATEADD(dd, -DAY(DATEADD(m,1,@SavingsMonthStartDate)), DATEADD(m,1,@SavingsMonthStartDate))
	SET @MinDateRx = DATEADD(mm, DATEDIFF(mm, 0, @SavingsMonthStartDate), 0)
	SET @MaxDateRx = DATEADD(dd, -DAY(DATEADD(m,1,@SavingsMonthStartDate)), DATEADD(m,1,@SavingsMonthStartDate))

------------------------------------------------------------------
--Set thresholds / variables
-----------------------------------------------------------------
	
	IF @EmployerID = 7 --Starbucks
	BEGIN --IF
		SET @IncludeYourCost = 1
		SET @SavingsThreshold = 75
		SET @YourCostThreshold = 25
	END --IF
	ELSE IF @EmployerID IN ( 11,12,18) --Caesars
		SET @SavingsThreshold = 25
	ELSE
		SET @SavingsThreshold = 75
------------------------------------------------------------------------------
--Pull Healthshopper Outreach folks into a table (from Rheal)
------------------------------------------------------------------------------
	SELECT 
		DENSE_RANK() OVER (ORDER BY C.[EmployerID]) AS Row,
		C.[EmployerID], 
		C.[MemberID], 
		C.[DateAdded], 
		C.[StatusChangeDate], 
		C.[CurrentAgentID], 
		C.[CallTypeID], 
		C.[ContactTypeID], 
		ct.[ContactDetail], 
		CASE WHEN C.[CaseStatusID]='RS' then 'Resolved'
			WHEN C.[CaseStatusID]='OP' then 'Open'
		END as CaseStatus,
		Emp.[EmployerName], 
		'                ' as CCHID  
	INTO #hsoutreach
	FROM  
		live.cch_frontend2.dbo.CC_Cases C
		INNER JOIN live.cch_frontend2.dbo.CC_CaseDetails CD 
			ON C.CaseID = CD.CaseID 
		LEFT OUTER JOIN live.cch_frontend2.dbo.CC_ContactDetails CT 
			ON C.CallTypeID = CT.ContactDetailID 
			AND C.ContactTypeID=CT.ContactTypeID
		RIGHT OUTER JOIN live.cch_frontend2.dbo.CC_ContactType CTT 
			ON C.ContactTypeID=CTT.ContactTypeID
		INNER JOIN live.cch_frontend2.dbo.CC_CaseStatuses CS 
			ON C.CaseStatusID = CS.CaseStatusID
		INNER JOIN live.[CCH_FrontEnd2].[dbo].[Employers] Emp 
			ON c.EmployerID = Emp.EmployerID
	WHERE 
		c.ContactTypeID='HS' 
		AND c.isactive=1
		AND DATEDIFF(month, CAST (StatusChangeDate AS DATE )  ,  getdate())<=2
	ORDER BY EmployerID

	SELECT DISTINCT 
		employerid 

	INTO 
		#employers 
	FROM 
		#hsoutreach 

	SELECT 
		row_number() over (order by employerid) as row_no,
		* 
	INTO 
		#employerids 
	FROM 
		#employers

	DECLARE 
		@Counter int=1 ;
	
	WHILE @Counter <= (select count(distinct row_no) from #employerids)
	BEGIN --While    
		DECLARE @memberId varchar(20);
		DECLARE 
			@EmployerDB as varchar(120)
			,@Sql as nvarchar(max)=''
			,@EmpID as int

		SELECT 
			@EmpID=EmployerID 
		FROM 
			#employerids 
		WHERE 
			row_no =@Counter

		SELECT 
			@EmployerDB=ConnectionString 
		FROM 
			live.cch_frontend2.dbo.Employers 
		WHERE 
			EmployerID = @EmployerID	
		SET 
			@EmployerDB = Substring(@EmployerDB,Charindex('Catalog=',@EmployerDB)+8,Charindex(';User',@EmployerDB) - Charindex('Catalog=',@EmployerDB)-8)

		SET @sql ='update temp set temp.cchid = e.CCHID from '
			   +@EmployerDB+'..enrollments as e join #hsoutreach as temp on e.MemberMedicalID=temp.memberId where temp.employerid='+cast(@EmpID as varchar)+''
	
		EXECUTE sp_executesql @sql

		SET @sql ='update temp set temp.cchid = e.CCHID from '+@EmployerDB+'..enrollmentsterminated as e join #hsoutreach as temp on e.MemberMedicalID=temp.memberId where temp.cchid='''' 
			   and  temp.employerid='+cast(@EmpID as varchar)+''
		EXECUTE sp_executesql @sql
	
		SET @Counter = @Counter + 1;
	END -- while
------------------------------------------------------------------------------
--Pull Targeted People and their Savings
-------------------------------------------------------------------------------
	SELECT
		e.CCHID,
		ROUND(dbo.fn_GetSavingsForCCHID(e.CCHID, @SavingsMonthStartDate),0) as TotalSavings
	INTO
		#Savings
	FROM
		Enrollments e				
		INNER JOIN dbo.fn_GetRegisteredMembers(GETDATE()) rm
			on e.CCHID = rm.CCHID
	WHERE
		e.OptInEmailAlerts = 1
		--AND rm.WebRegistered = 1
		AND rm.Terminated = 0
		--and e.CCHID not in (select CCHID from dbo.ExcludedMember where ExcludeReasonDesc = 'Key Client')
		--and e.CCHID not in (select CCHID from dbo.ExcludedMember where ExcludeReasonDesc = 'CEO')
		and e.CCHID not in (select CCHID from #hsoutreach where EmployerID = @EmployerID)

------------------------------------------------------------------
--Pull YourCost Savings for everyone (per syam 3/9/2015)
------------------------------------------------------------------
---------------------------------------------------------------
--Medical Piece of YourCost
---------------------------------------------------------------
	SELECT
		pc.CCHID
		,pc.Category
		,pc.ProcedureCode
		,pc.OrganizationID
		,pc.SpecialtyID
		,(SELECT TOP 1 PriceMin FROM dbo.YourCost(pc.CCHID, NULL, ProcedureCode, YouCouldHavePaid, AllowedAmount, OrganizationID, SpecialtyID)) as MinYourCost
		,(SELECT TOP 1 PriceMax FROM dbo.YourCost(pc.CCHID, NULL, ProcedureCode, YouCouldHavePaid , AllowedAmount, OrganizationID, SpecialtyID)) as MaxYourCost
	INTO 
		#PastCareDataYC
	FROM
		dbo.PastCare pc
		INNER JOIN dbo.Enrollments e
			on pc.CCHID = e.CCHID
		INNER JOIN dbo.fn_GetRegisteredMembers(GETDATE()) rm
			on e.CCHID = rm.CCHID
	WHERE
		e.OptInEmailAlerts = 1
		and rm.Terminated = 0
		--and rm.WebRegistered = 1
		and pc.ServiceDate between @MinDateMedical and @MaxDateMedical
		and pc.Category in ('Office', 'Lab', 'Imaging')
		and pc.FairPrice != 1
		--and e.CCHID not in (select CCHID from dbo.ExcludedMember where ExcludeReasonDesc = 'Key Client')
---------------------------------------------------------------
--Rx Piece of YourCost
---------------------------------------------------------------		
	SELECT
		pc.CCHID
		,pc.GPI
		,pc.Quantity
		,pc.PharmacyID
		,(SELECT TOP 1 Price FROM dbo.YourCostRx(pc.CCHID,pc.GPI,pc.Quantity,isnull(pc.AllowedAmount, 0), CASE WHEN (SELECT Mail_Retail_ind FROM CCH_ReferenceData.dbo.Pharmacies WHERE PharmacyID = pc.PharmacyID) = 'Mail' THEN 1 ELSE 0 END,0 )) as YourCost
		,(SELECT TOP 1 Price FROM dbo.YourCostRx(pc.CCHID,pc.GPI,pc.Quantity,isnull(pc.AllowedAmount,0)-isnull(pc.YouCouldHaveSaved,0), CASE WHEN (SELECT Mail_Retail_ind FROM CCH_ReferenceData.dbo.Pharmacies WHERE PharmacyID = pc.PharmacyID) = 'Mail' THEN 1 ELSE 0 END,0 )) as YourCost_YouCouldHaveSaved	
	INTO 
		#PastCareRxDataYC
	FROM
		dbo.PastCareRx pc
		INNER JOIN dbo.Enrollments e
			on pc.CCHID = e.CCHID
		INNER JOIN dbo.fn_GetRegisteredMembers(GETDATE()) rm
			on e.CCHID = rm.CCHID
	WHERE
		e.OptInEmailAlerts = 1
		and rm.DateRemoved is NULL
		and pc.ServiceDate between @MinDateMedical and @MaxDateMedical
		--and e.CCHID not in (select CCHID from dbo.ExcludedMember where ExcludeReasonDesc = 'Key Client')
---------------------------------------------------------------------
--Calculate Total Your Cost Savings
----------------------------------------------------------------------
	CREATE TABLE #SavingsYC(
		CCHID int
		,TotalSavingsYC money
	)
	
	SELECT
		pc.CCHID 
		,SUM(ISNULL((pc.MaxYourCost - pc.MinYourCost),0)) as MedicalYC
	INTO
		#MedicalYCSavings
	FROM
		#PastCareDataYC pc
	GROUP BY
		pc.CCHID
		
	SELECT
		pr.CCHID
		,SUM(ISNULL((pr.YourCost - pr.YourCost_YouCouldHaveSaved),0)) as RxYC
	INTO
		#RxYCSavings
	FROM
		#PastCareRxDataYC pr
	GROUP BY
		pr.CCHID
		
	INSERT INTO #SavingsYC
		(CCHID, TotalSavingsYC)
	SELECT
		ISNULL(pc.CCHID, pr.CCHID) as CCHID
		,ISNULL(pc.MedicalYC,0) + ISNULL(pr.RxYC,0) as TotalSavingsYC
	FROM
		#MedicalYCSavings pc
		FULL OUTER JOIN #RxYCSavings pr
			on pc.CCHID = pr.CCHID

---------------------------------------------------------------
--Now pull the target popultation for Employers where YourCost is included
---------------------------------------------------------------	
	IF @IncludeYourCost = 1		
	BEGIN --YourCostIncl
		SELECT
			ISNULL(s.CCHID, syc.CCHID) as CCHID
			,ISNULL(s.TotalSavings,0) as TotalSavings
			,ISNULL(syc.TotalSavingsYC,0) as TotalSavingsYC
		INTO	
			#TargetPopYC
		FROM
			#Savings s
			FULL OUTER JOIN #SavingsYC syc
				on s.CCHID = syc.CCHID
		WHERE
			ISNULL(s.TotalSavings,0) > @SavingsThreshold
			AND ISNULL(syc.TotalSavingsYC,0) > @YourCostThreshold
		
------------------------------------------------------
--Insert the YC Target Pop Members
------------------------------------------------	
		INSERT INTO dbo.CampaignMember (
			CampaignID
			,CCHID
			,Savings
			,YourCostSavingsAmt
			,Score
			,CreateDate)
		SELECT
			@CampaignID
			,CCHID
			,TotalSavings
			,TotalSavingsYC
			,(SELECT PctScore FROM GetThermometerValue(CCHID) WHERE Category = 'Overall')
			,getdate()
		FROM
			#TargetPopYC
	END--YourCost incl	
---------------------------------------------------------------------
--Find people with Savings > Thresholds
----------------------------------------------------------------------
	ELSE
	BEGIN --no your cost threshold
		SELECT
			s.CCHID
			,s.TotalSavings
			,syc.TotalSavingsYC
		INTO	
			#TargetPop
		FROM
			#Savings s
			LEFT JOIN #SavingsYC syc
				on s.CCHID = syc.CCHID
		WHERE
			TotalSavings > @SavingsThreshold
------------------------------------------------------
--Insert the Target Pop Members
------------------------------------------------
		INSERT INTO dbo.CampaignMember (
			CampaignID
			,CCHID
			,Savings
			,YourCostSavingsAmt
			,Score
			,CreateDate)
		SELECT
			@CampaignID
			,CCHID
			,TotalSavings
			,TotalSavingsYC
			,(SELECT PctScore FROM GetThermometerValue(CCHID) WHERE Category = 'Overall')
			,getdate()
		FROM
			#TargetPop
	END --No Your Cost threshold
		
-----------------------------------------------------
--Update the Campaign table with this sproc name
------------------------------------------------------
	UPDATE
		Campaign
	SET
		TargetProcedureName = 'p_GetSavingsAlertTargetPop'
	WHERE
		CampaignID = @CampaignID

END


GO


