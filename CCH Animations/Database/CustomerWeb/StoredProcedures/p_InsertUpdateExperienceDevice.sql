/****** Object:  StoredProcedure [dbo].[p_InsertUpdateExperienceDevice]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertUpdateExperienceDevice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertUpdateExperienceDevice]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-05-26
Description:
      Insert/Update ExperienceDevice
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.ExperienceDevice
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-05-26  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertUpdateExperienceDevice] (
	@ExperienceUserID nvarchar(36)
	,@DeviceID nvarchar(100)
	,@CCHID int = NULL)
as

BEGIN--proc
	IF EXISTS (SELECT 1 FROM dbo.ExperienceDevice where ExperienceUserID = @ExperienceUserID and DeviceID = @DeviceID)
	BEGIN -- IF
		UPDATE dbo.ExperienceDevice
		SET 
			ExperienceUserID = @ExperienceUserID
			,DeviceID = @DeviceID
			,CCHID = ISNULL(@CCHID, CCHID)
		WHERE 
			ExperienceUserID = @ExperienceUserID
			AND DeviceID = @DeviceID
	END --IF
	ELSE
	BEGIN --ELSE
		INSERT INTO dbo.ExperienceDevice (
			ExperienceUserID
			,DeviceID
			,CCHID
			,CreateDate)
		VALUES (
			@ExperienceUserID
			,@DeviceID
			,@CCHID
			,GETDATE())
	END--ELSE
END--proc

GO


 
