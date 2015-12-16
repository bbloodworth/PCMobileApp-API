/****** Object:  StoredProcedure [dbo].[p_UpdateSurveyPassCount]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_UpdateSurveyPassCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_UpdateSurveyPassCount]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Updates Pass Count on the Survey table
      
Declarations:
            
Execute: exec p_UpdateSurveyPassCount
			@SurveyID = 4
			,@PassCount = 40
      

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_UpdateSurveyPassCount] (
	@SurveyID int,
	@PassCount int
)
as

BEGIN--proc
	UPDATE dbo.Survey
	SET
		SurveyPassCount = @PassCount
	WHERE
		SurveyID = @SurveyID
END --proc
 
GO
