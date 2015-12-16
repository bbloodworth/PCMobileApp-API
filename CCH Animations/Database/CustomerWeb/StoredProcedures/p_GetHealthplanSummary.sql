/****** Object:  StoredProcedure [dbo].[p_GetHealthplanSummary]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetHealthplanSummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetHealthplanSummary]
GO

/****** Object:  StoredProcedure [dbo].[p_GetHealthplanSummary]    Script Date: 07/10/2015 13:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-10-08
Description:
     Pulls key information / metrics about a person's healthplan for a given CCHID
      
Declarations:
      
Execute:
      exec p_GetHealthplanSummary @CCHID = 57020
	
Objects Listing:

Tables- dbo.UserContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-10-08  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetHealthplanSummary] (
	@CCHID int
	)
as

BEGIN --proc
	DECLARE @EmployerID int
	
	SELECT @EmployerID = EmployerID from ControlData

	;with q1 as
	(
		SELECT 
		e.EmployeeID,
		case WHEN Count(e.EmployeeID) = 1 
		THEN 'Individual'
		ELSE 'Family'
		END As PlanType
		FROM Enrollments e
		group by e.EmployeeID
	)
	SELECT
		 CASE WHEN @EmployerID IN (11,12,18,10000) and e.MedicalPlanType like '%HSA%' THEN 'HSA Plan' 
			WHEN @EmployerID IN (11,12,18,10000) and e.MedicalPlanType like '%HRA%' THEN 'HRA Plan' 
			WHEN @EmployerID IN (11,12,18,10000) and e.MedicalPlanType like '%PPO%' THEN 'PPO Plan' 
			ELSE ISNULL(e.MedicalPlanType, '')
		 END AS PlanName
		,CASE WHEN q1.PlanType = 'Individual' THEN bp.DeductibleIndividual ELSE bp.DeductibleFamily END as Deductible
		,CASE WHEN q1.PlanType = 'Individual' THEN a.OutOfPocketMaxIndividual ELSE a.OutOfPocketMaxFamily END as YTDSpent
		,bp.CoPay
		,bp.CoInsurance * 100 as Coinsurance 
		,100 - (bp.CoInsurance * 100) as CoinsuranceComplement
		,CASE WHEN q1.PlanType = 'Individual' THEN bp.OutOfPocketIndividual ELSE bp.OutOfPocketFamily END as OOPMax
		,a.AsOfDate 
	FROM
		Enrollments e
		INNER JOIN q1 
			on e.EmployeeID = q1.EmployeeID
		LEFT JOIN BenefitPlans bp
			on e.MedicalPlanType = bp.PlanType 
		LEFT JOIN Accumulators a
			on e.CCHID = a.CCHID
	WHERE
		e.CCHID= @CCHID

END
 

GO


