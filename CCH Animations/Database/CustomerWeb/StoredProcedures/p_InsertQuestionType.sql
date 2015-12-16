/****** Object:  StoredProcedure [dbo].[p_InsertQuestionType]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertQuestionType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertQuestionType]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:

      
Declarations:
      
Execute:
      exec p_InsertQuestionType @QuestionTypeDesc = 'True/False'

Objects Listing:

Tables- dbo.QuestionType
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertQuestionType] (
	@QuestionTypeDesc	nvarchar(100) 
)
as

BEGIN
	INSERT INTO dbo.QuestionType (
		QuestionTypeDesc,
		CreateDate)
	VALUES (
		@QuestionTypeDesc,
		getdate())
END
 
GO
