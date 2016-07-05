/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-07-05
-- Description:	Replace all references to Mary Smith with Jim Jones
-- =============================================
*/

UPDATE CCH_CustomerDemoNCCT.dbo.MemberIDCard 
SET CardMemberDataText = REPLACE(CardMemberDataText, 'Mary Smith', 'Jim Jones') 
WHERE CCHID = 63880 AND CardMemberDataText LIKE '%Mary Smith%'

