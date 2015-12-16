
USE CCH_AvayaWeb
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_GetRegisteredMembers]')AND type in (N'TF') )
DROP FUNCTION [dbo].[fn_GetRegisteredMembers]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




/****** Object:  UserDefinedFunction dbo.NCCTGetProfessionalCost    Script Date: 10/21/2014 14:37:01 ******/
CREATE FUNCTION [dbo].[fn_GetRegisteredMembers](@RegistrationEndDate datetime = NULL)
/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-01-19
Description:
     Pulls from live, web registered, phone registered and terminated members as of a specified 
     'End Registration Date'
      
Declarations:
      
Execute:
     fn_GetRegisteredMembers('2015-01-29')
	
Objects Listing:

    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-01-19  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

RETURNS @RegisteredMembers TABLE (
CCHID int NULL,
Email nvarchar(100) NULL,
MobilePhone nvarchar(50) NULL,
CreateDate datetime NULL,
WebRegistered bit NULL,
PhoneRegistered bit NULL,
Terminated bit NULL,
DateRemoved datetime NULL
)
AS 
BEGIN

	IF @RegistrationEndDate IS NULL 
	SET @RegistrationEndDate = SYSDATETIME()


	------------------------------------------------------
	-- Web registrants
	------------------------------------------------------

	INSERT INTO	@RegisteredMembers (cchid, email, MobilePhone, CreateDate, WebRegistered, PhoneRegistered, Terminated)
	SELECT		e.cchid 
				, u.email 
				,e.MobilePhone
				, m.createdate
				,1
				,0
				,0
	FROM		live.CCH_AvayaWeb.dbo.Enrollments e	
				JOIN		live.cch_frontend2.dbo.userprofile u
							ON e.email = u.email
				JOIN		live.cch_frontend2.dbo.aspnet_membership m 
							ON u.userid = m.userid
	WHERE		m.createdate <= @RegistrationEndDate 
	
	------------------------------------------------------
	-- Phone registrants
	------------------------------------------------------
	INSERT INTO	@RegisteredMembers (cchid, email, MobilePhone, CreateDate, WebRegistered, PhoneRegistered, Terminated)
	SELECT		e.cchid
				, e.email
				,e.MobilePhone
				, e.dateregistered_phone as CreateDate
				, 0
				, 1
				, 0
	FROM		live.CCH_AvayaWeb.dbo.enrollments e							
				LEFT JOIN	@RegisteredMembers r
							ON e.cchid = r.cchid
	WHERE		r.cchid IS NULL
				AND tandcindicator = 'Y' 
				AND LEN(phone) > 0
				AND e.DateRegistered_phone <=@RegistrationEndDate
	 
	-----------------------------------------------------------------------------
	-- Add terminated employees.
	----------------------------------------------------------------------------- 
	INSERT INTO @RegisteredMembers (CCHID, Email, MobilePhone, CreateDate, WebRegistered, PhoneRegistered, Terminated, DateRemoved)
	SELECT		e.CCHID
				, e.Email
				,e.MobilePhone
				, m.CreateDate
				,0
				,0
				,1
				, e.DateRemoved
	FROM		live.CCH_AvayaWeb.dbo.EnrollmentsTerminated e
				LEFT JOIN	@RegisteredMembers r
							ON e.cchid = r.cchid
				JOIN		live.cch_frontend2.dbo.userprofile u
							ON e.email = u.email
				JOIN		live.cch_frontend2.dbo.aspnet_membership m 
							ON u.userid = m.userid
	WHERE		r.cchid IS NULL
				AND m.createdate <=@RegistrationEndDate 
	
	RETURN
END

GO


