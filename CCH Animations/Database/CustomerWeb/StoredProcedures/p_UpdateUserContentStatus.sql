/****** Object:  StoredProcedure [dbo].[p_UpdateUserContentStatus]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_UpdateUserContentStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_UpdateUserContentStatus]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Updates status on User Content table
      
Declarations:
            
Execute:
      

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_UpdateUserContentStatus] (
	@CampaignID int,
	@CCHID int,
	@ContentID int,
	@StatusID int = 0,
	@StatusDesc nvarchar(100) = ''
)
as

BEGIN--proc
	IF @StatusID > 0
	BEGIN --IF
		UPDATE UserContent
		SET
			ContentStatusID = @StatusID
			,ContentStatusChangeDate = GETDATE()
		WHERE
			CampaignID = @CampaignID
			AND ContentID = @ContentID
			AND CCHID = @CCHID
	END --IF
	ELSE 
	BEGIN --ELSE
		SELECT 
			@StatusID = ContentStatusID
		FROM
			dbo.ContentStatus
		WHERE
			ContentStatusDesc = @StatusDesc
			
		UPDATE UserContent
		SET
			ContentStatusID = @StatusID
			,ContentStatusChangeDate = GETDATE()
		WHERE
			CampaignID = @CampaignID
			AND ContentID = @ContentID
			AND CCHID = @CCHID
	END --ELSE
END --proc

GO
 
