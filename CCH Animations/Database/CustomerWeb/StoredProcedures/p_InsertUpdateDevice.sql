/****** Object:  StoredProcedure [dbo].[p_InsertUpdateDevice]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertUpdateDevice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertUpdateDevice]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-09-17
Description:
      Insert/Update Device
      
Declarations:
            
Execute:
exec p_InserUpdateDevice 
	@DeviceID = 'TEST12345', 
	@ClientAllowPushInd = 1, 
	@NativeAllowPushInd = 0,
	@LastPushPromptDate = GETDATE()


Objects Listing:

Tables- dbo.Device
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-09-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertUpdateDevice] (
	@DeviceID             nvarchar(100),
	@ClientAllowPushInd   bit = NULL ,
	@NativeAllowPushInd	 bit = NULL,
	@LastPushPromptDate   datetime = NULL
	)
as

BEGIN--proc
	IF EXISTS (SELECT 1 FROM dbo.Device where DeviceID = @DeviceID)
	BEGIN -- IF
		UPDATE dbo.Device
		SET 
			ClientAllowPushInd = ISNULL(@ClientAllowPushInd, ClientAllowPushInd)
			,NativeAllowPushInd = ISNULL(@NativeAllowPushInd, NativeAllowPushInd)
			,LastPushPromptDate = ISNULL(@LastPushPromptDate, LastPushPromptDate)
		WHERE 
			DeviceID = @DeviceID
	END --IF
	ELSE
	BEGIN --ELSE
		INSERT INTO dbo.Device (
			DeviceID
			,ClientAllowPushInd
			,NativeAllowPushInd
			,LastPushPromptDate
			,CreateDate)
		VALUES (
			@DeviceID
			,ISNULL(@ClientAllowPushInd, 0)
			,ISNULL(@NativeAllowPushInd,0)
			,@LastPushPromptDate
			,GETDATE())
	END--ELSE
END--proc

GO


 
