/****** Object:  StoredProcedure [dbo].[p_InsertCampaignContent]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertCampaignContent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertCampaignContent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert CampaignContent
      
Declarations:
      
Execute:
      exec p_InsertCampaignContent
		@CampaignID = 1,
		@ContentID = 3
	
Objects Listing:

Tables- dbo.CampaignContent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-09-04  AS		 Add Notifications fields (PC-435)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertCampaignContent] (
	@CampaignID int
	,@ContentID int
	,@ActivationDate datetime=null
	,@ExpirationDate datetime = null
	,@UserContentInd bit = null
	,@EmailNotificationInd bit = 0
	,@SMSNotificationInd bit = 0
	,@OSNotificationInd bit = 0
	,@OSNotificationStatusDesc nvarchar(100) = null
	,@OSNotificationSentDate datetime = null
)
as

BEGIN
	IF EXISTS (SELECT 1 from dbo.CampaignContent where CampaignID = @CampaignID and ContentID = @ContentID)
	BEGIN
		UPDATE dbo.CampaignContent
		SET
			ActivationDate = ISNULL(@ActivationDate, ActivationDate)
			,ExpirationDate = ISNULL(@ExpirationDate, ExpirationDate)
			,UserContentInd = ISNULL(@UserContentInd,UserContentInd)
			,EmailNotificationInd = ISNULL(@EmailNotificationInd, EmailNotificationInd)
			,SMSNotificationInd = ISNULL(@SMSNotificationInd, SMSNotificationInd)
			,OSNotificationInd = ISNULL(@OSNotificationInd, OSNotificationInd)
			,OSNotificationStatusDesc = ISNULL(@OSNotificationStatusDesc, OSNotificationStatusDesc)
			,OSNotificationSentDate = ISNULL(@OSNotificationSentDate, OSNotificationSentDate)
		WHERE
			CampaignID = @CampaignID
			AND ContentID = @ContentID
	END
	ELSE
	BEGIN
	
		INSERT INTO dbo.CampaignContent (
			CampaignID
			,ContentID
			,ActivationDate
			,ExpirationDate
			,UserContentInd
			,EmailNotificationInd
			,SMSNotificationInd
			,OSNotificationInd
			,OSNotificationStatusDesc
			,OSNotificationSentDate
			,CreateDate)
		VALUES (
			@CampaignID
			,@ContentID
			,@ActivationDate
			,@ExpirationDate
			,ISNULL(@UserContentInd,1)
			,ISNULL(@EmailNotificationInd,0)
			,ISNULL(@SMSNotificationInd,0)
			,ISNULL(@OSNotificationInd,0)
			,@OSNotificationStatusDesc
			,@OSNotificationSentDate
			,getdate())
	END		
END
 
GO
