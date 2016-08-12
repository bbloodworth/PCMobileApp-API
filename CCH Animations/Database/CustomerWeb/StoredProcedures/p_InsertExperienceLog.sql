

If object_id('p_InsertExperienceLog','P') is not null
Begin
	DROP PROCEDURE [dbo].[p_InsertExperienceLog];
End

GO

/****** Object:  StoredProcedure [dbo].[p_InsertExperienceLog]    Script Date: 8/3/2016 8:02:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert ExperienceLog
      
Declarations:
      
Execute:
      exec p_InsertExperienceLog
		@ExperienceEventID = 1,
		@ExperienceUserID = 'ABCDEFG12345',
		@CCHID = 12345,
		@Comment = 'Comment',
		@DeviceID = 'TEST45678'  

Objects Listing:

Tables- dbo.ExperienceLog
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-09-17  AS       Updated to include optional DeviceID
2016-08-03  SF		 Added ClientVersion per 
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertExperienceLog] (
	@ExperienceEventID int = 0,
	@ExperienceEventDesc nvarchar(100) = '',
	@ExperienceUserID nvarchar(36),
	@CCHID int = NULL,
	@ContentID int = NULL,
	@Comment nvarchar(250) = NULL,
	@DeviceID nvarchar(100) = NULL,
	@ClientVersion nvarchar(50) = NULL
)
as

BEGIN --proc
--------------------------------------------------------------------
--If a DeviceID was passed, first make sure it's in the appropriate parent tables
------------------------------------------------------------------
	IF ISNULL(@DeviceID,'') != ''
	BEGIN
		exec p_InsertUpdateDevice 
			@DeviceID = @DeviceID
		
		exec p_InsertUpdateExperienceDevice 
			@ExperienceUserID = @ExperienceUserID
			,@DeviceID = @DeviceID
			,@CCHID = @CCHID
	END
--------------------------------------------------------------------
--Now Log the Event
------------------------------------------------------------------
	IF @ExperienceEventID > 0
	BEGIN --IF
		INSERT INTO dbo.ExperienceLog (
			ExperienceEventID,
			ExperienceUserID,
			CCHID,
			ContentID,
			ExperienceLogCommentText,
			CreateDate,
			ClientVersion)
		VALUES (
			@ExperienceEventID,
			@ExperienceUserID,
			@CCHID,
			@ContentID,
			@Comment,
			getdate(),
			@ClientVersion)
	END--IF
	ELSE 
	BEGIN --ELSE
		INSERT INTO dbo.ExperienceLog (
			ExperienceEventID,
			ExperienceUserID,
			CCHID,
			ContentID,
			ExperienceLogCommentText,
			CreateDate,
			ClientVersion)
		SELECT
			ee.ExperienceEventID,
			@ExperienceUserID,
			@CCHID,
			@ContentID,
			@Comment,
			getdate(),
			@ClientVersion
		FROM	
			dbo.ExperienceEvent ee
		WHERE
			ee.ExperienceEventDesc = @ExperienceEventDesc
	END--ELSE
END--proc
 

GO


