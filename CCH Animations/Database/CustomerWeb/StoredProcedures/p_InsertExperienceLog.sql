/****** Object:  StoredProcedure [dbo].[p_InsertExperienceLog]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertExperienceLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertExperienceLog]
GO

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
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertExperienceLog] (
	@ExperienceEventID int = 0,
	@ExperienceEventDesc nvarchar(100) = '',
	@ExperienceUserID nvarchar(36),
	@CCHID int = NULL,
	@ContentID int = NULL,
	@Comment nvarchar(250) = NULL,
	@DeviceID nvarchar(100) = NULL	
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
			CreateDate)
		VALUES (
			@ExperienceEventID,
			@ExperienceUserID,
			@CCHID,
			@ContentID,
			@Comment,
			getdate())
	END--IF
	ELSE 
	BEGIN --ELSE
		INSERT INTO dbo.ExperienceLog (
			ExperienceEventID,
			ExperienceUserID,
			CCHID,
			ContentID,
			ExperienceLogCommentText,
			CreateDate)
		SELECT
			ee.ExperienceEventID,
			@ExperienceUserID,
			@CCHID,
			@ContentID,
			@Comment,
			getdate()
		FROM	
			dbo.ExperienceEvent ee
		WHERE
			ee.ExperienceEventDesc = @ExperienceEventDesc
	END--ELSE
END--proc
 
GO
