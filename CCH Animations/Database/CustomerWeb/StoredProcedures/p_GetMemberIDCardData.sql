
/****** Object:  StoredProcedure [dbo].[p_GetMemberIDCardData]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetMemberIDCardData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetMemberIDCardData]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-05-12
Description:
      Returns member data associated with a member id card as identified by a unique security token.
      
Declarations:
            
Execute:
      exec p_GetMemberIDCardData
		@SecurityToken = '1BA2D639-1082-4FA7-A8FB-EE02B401469B'

Objects Listing:

Tables- dbo.MemberIDCard
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-12  AS       Created
2015-07-18  AS		 Altered to add in CardTypeFileName 
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetMemberIDCardData] (
	@SecurityToken nvarchar(36)
)
as

BEGIN
-----------------------------------------------------------
--Check that the token has not expired
-----------------------------------------------------------
	DECLARE 
		@CurrentDatetime datetime = GETDATE()
		,@BeginDatetime datetime
		,@EndDatetime datetime

	SELECT
		@BeginDatetime = SecurityTokenBeginDateTime
		,@EndDatetime = SecurityTokenEndDatetime
	FROM
		dbo.MemberIDCard
	WHERE
		SecurityTokenGUID = @SecurityToken
	
	IF ISNULL(@CurrentDatetime,'1900-01-01') < @BeginDatetime OR ISNULL(@CurrentDatetime, '1900-01-01') >= @EndDatetime
	BEGIN
		PRINT 'Security Token is not valid'
		RETURN
	END
	ELSE
		SELECT
			m.CardTypeID
			,m.CardViewModeID
			,m.CardMemberDataText
			,ct.CardTypeFileName
		FROM
			dbo.MemberIDCard m
			INNER JOIN dbo.CardType ct
				ON m.CardTypeID = ct.CardTypeID
		WHERE
			SecurityTokenGUID = @SecurityToken
			AND @CurrentDatetime between SecurityTokenBeginDatetime and SecurityTokenEndDatetime		
END

GO

