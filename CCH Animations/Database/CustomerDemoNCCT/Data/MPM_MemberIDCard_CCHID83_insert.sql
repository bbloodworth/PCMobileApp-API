/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-04-18
-- Description:	For the Customer DEMO DB
--              Adds new ID Cards for mary smith accenture, and removes old ID Cards
-- =============================================
*/

USE CCH_CustomerDemoNCCT
GO
    
  IF (NOT EXISTS(SELECT * FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 7) )
	BEGIN 
		INSERT INTO [dbo].[MemberIDCard] (CCHID, CardTypeID, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate)
		SELECT CCHID, 7, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate
		FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 1
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 8) )
	BEGIN 
		INSERT INTO [dbo].[MemberIDCard] (CCHID, CardTypeID, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate)
		SELECT CCHID, 8, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate
		FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 2
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 9) )
	BEGIN 
		INSERT INTO [dbo].[MemberIDCard] (CCHID, CardTypeID, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate)
		SELECT CCHID, 9, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate
		FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 3
	END
  GO

  IF (NOT EXISTS(SELECT * FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 10) )
	BEGIN 
		INSERT INTO [dbo].[MemberIDCard] (CCHID, CardTypeID, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate)
		SELECT CCHID, 10, LocaleID, CardViewModeID, CardMemberDataText, 
			SecurityTokenGUID, SecurityTokenBeginDatetime, SecurityTokenEndDatetime, CreateDate
		FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 4
	END
  GO

  DELETE FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 1
  GO

  DELETE FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 2
  GO

  DELETE FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 3
  GO

  DELETE FROM [dbo].[MemberIDCard] WHERE CCHID = 83 AND CardTypeID = 4
  GO

--  SELECT * FROM [dbo].[MemberIDCard] ORDER BY CardTypeID DESC, CCHID ASC


    
