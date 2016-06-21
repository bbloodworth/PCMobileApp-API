/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-05-03
-- Description:	For the Customer DEMO DB and Caesars DBs
--              Adds new Content Type
-- =============================================
*/

  IF (NOT EXISTS(SELECT * FROM [dbo].[ContentType] WHERE ContentTypeDesc = 'Vimeo') )
	BEGIN 
	  INSERT INTO [dbo].[ContentType] (ContentTypeID, ContentTypeDesc, CreateDate)
	  VALUES (6, 'Vimeo', GETDATE())
	END
  GO

