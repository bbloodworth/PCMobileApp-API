/****** Object:  StoredProcedure [dbo].[p_InsertExperienceEvent]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertExperienceEvent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertExperienceEvent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Experience Event
      
Declarations:
      
Execute:
      exec p_InsertExperienceEvent @ExperienceEventDesc = 'TestDescription'

Objects Listing:

Tables- dbo.ExperienceEvent
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertExperienceEvent] (
	@ExperienceEventDesc	nvarchar(100) 
)
as

BEGIN
	INSERT INTO dbo.ExperienceEvent (
		ExperienceEventDesc,
		CreateDate)
	VALUES (
		@ExperienceEventDesc,
		getdate())
END
 
GO
