/****** Object:  StoredProcedure [dbo].[p_InsertResource]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertResource]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertResource]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Resource
      
Declarations:
      
Execute:
      exec p_InsertResource
		@ResourceName = 'Best Doctors',
		@ResourceIconFileName = 'n:\imagefilename',
		@ResourceVideoImageURL = 'n:\Resourcefilename',
		@ResourceDesc = 'Helps you find the best doctors for you.',
		@ResourcePhoneNum = '212-555-1212',
		@ResourceURL = 'www.bestdoctors.com'
	
Objects Listing:

Tables- dbo.Resource
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertResource] (
	@ResourceName nvarchar(50),
	@ResourceIconFileName nvarchar(50) = NULL,
	@ResourceVideoImageURL nvarchar(100) = NULL,
	@ResourceDesc nvarchar(100) = NULL,
	@ResourcePhoneNum nvarchar(20) = NULL,
	@ResourceURL nvarchar(100) = NULL
)
as

BEGIN
	
	INSERT INTO dbo.Resource (
		ResourceName
		,ResourceIconFileName
		,ResourceVideoImageURL
		,ResourceDesc
		,ResourcePhoneNum
		,ResourceURL
		,CreateDate)
	VALUES (
		@ResourceName
		,@ResourceIconFileName
		,@ResourceVideoImageURL
		,@ResourceDesc
		,@ResourcePhoneNum
		,@ResourceURL
		,GETDATE())
END
 
GO
