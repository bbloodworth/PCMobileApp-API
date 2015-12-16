/****** Object:  StoredProcedure [dbo].[p_GetCampaignIntro]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetCampaignIntro]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetCampaignIntro]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets information about the intro video or animation for a Campaign
      
Declarations:
            
Execute:
      exec p_GetCampaignIntro
		@CampaignID= 5

Objects Listing:

Tables: 
dbo.Content

    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-01-20  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetCampaignIntro] (
	@CampaignID int
)
as

BEGIN
	SELECT
		c.ContentID,
		c.ContentName,
		ct.ContentTypeDesc
	FROM
		dbo.CampaignContent cc
		INNER JOIN dbo.Content c
			on cc.ContentID = c.ContentID
		INNER JOIN dbo.ContentType ct
			ON c.ContentTypeID = ct.ContentTypeID
	WHERE
		cc.CampaignID = @CampaignID
		AND c.IntroContentInd = 1
END
 
GO
