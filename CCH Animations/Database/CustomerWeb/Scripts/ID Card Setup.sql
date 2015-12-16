-----------------------------------------------------
--Insert the Card Types
----------------------------------------------------
if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Medical ID Card (PPO)')
	exec p_InsertCardType @CardTypeName = 'Medical ID Card (PPO)', @CardTypeDesc = 'A medical card is an insurance identification card that displays key attributes and information about a person''s medical insurance policy', @LocaleCode = 'en-us'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Rx ID Card')	
	exec p_InsertCardType @CardTypeName = 'Rx ID Card', @CardTypeDesc = 'An Rx ID card is an insurance identification card that displays key attributes and information about a person''s pharmacy/prescription insurance policy', @LocaleCode = 'en-us'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Vision ID Card')		
	exec p_InsertCardType @CardTypeName = 'Vision ID Card', @CardTypeDesc = 'A Vision ID card is an insurance identification card that displays key attributes and information about a person''s vision insurance policy', @LocaleCode = 'en-us'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Dental ID Card')		
	exec p_InsertCardType @CardTypeName = 'Dental ID Card', @CardTypeDesc = 'A Dental ID card is an insurance identification card that displays key attributes and information about a person''s dental insurance policy', @LocaleCode = 'en-us'
	
if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Medical ID Card (POS)')
	exec p_InsertCardType @CardTypeName = 'Medical ID Card (POS)', @CardTypeDesc = 'A medical card is an insurance identification card that displays key attributes and information about a person''s medical insurance policy', @LocaleCode = 'en-us'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Medical ID Card (HMO)')
	exec p_InsertCardType @CardTypeName = 'Medical ID Card (HMO)', @CardTypeDesc = 'A medical card is an insurance identification card that displays key attributes and information about a person''s medical insurance policy', @LocaleCode = 'en-us'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Credencial Médica (PPO)')		
	exec p_InsertCardTypeTranslation @CardTypeID = 1, @LocaleID = 2, @CardTypeName = 'Credencial Médica (PPO)'
	
if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Credencial de Recetas')			
	exec p_InsertCardTypeTranslation @CardTypeID = 2, @LocaleID = 2, @CardTypeName = 'Credencial de Recetas'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Credencial Óptica')			
	exec p_InsertCardTypeTranslation @CardTypeID = 3, @LocaleID = 2, @CardTypeName = 'Credencial Óptica'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Credencial Dental')			
	exec p_InsertCardTypeTranslation @CardTypeID = 4, @LocaleID = 2, @CardTypeName = 'Credencial Dental'

if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Credencial Médica (POS)')		
	exec p_InsertCardTypeTranslation @CardTypeID = 5, @LocaleID = 2, @CardTypeName = 'Credencial Médica (POS)'
	
if not exists (select 1 from CardType ct inner join CardTypeTranslation ctt on ct.CardTypeID = ctt.CardTypeID where ctt.CardTypeName = 'Credencial Médica (HMO)')		
	exec p_InsertCardTypeTranslation @CardTypeID = 6, @LocaleID = 2, @CardTypeName = 'Credencial Médica (HMO)'


--select * from cardtype ct inner join cardtypetranslation ctt on ct.cardtypeid = ctt.cardtypeid

-----------------------------------------------------
--Insert the View Modes
----------------------------------------------------
if not exists (select 1 from CardViewMode where CardViewModeName = 'Front')
	exec p_InsertCardViewMode @CardViewModeName = 'Front'

if not exists (select 1 from CardViewMode where CardViewModeName = 'Back')	
	exec p_InsertCardViewMode @CardViewModeName = 'Back'
	
if not exists (select 1 from CardViewMode where CardViewModeName = 'Full Screen')
	exec p_InsertCardViewMode @CardViewModeName = 'Full Screen'
	
if not exists (select 1 from CardViewMode where CardViewModeName = 'Portrait')
	exec p_InsertCardViewMode @CardViewModeName = 'Portrait'

--select * from cardviewmode

----------------------------------------------------------------------
--Insert 'starter' ID Cards
----------------------------------------------------------------------

INSERT INTO MemberIDCard(
	CCHID
	,CardTypeID
	,LocaleID
	,CardViewModeID
	,CardMemberDataText
	,SecurityTokenGUID
	,SecurityTokenBeginDatetime
	,SecurityTokenEndDatetime
	,CreateDate
	)
SELECT
	e.CCHID
	,ctt.CardTypeID
	,ctt.LocaleID
	,cvm.CardViewModeID
	,'Placeholder for JSON'
	,NEWID()
	,GETDATE()
	,GETDATE()
	,GETDATE()
FROM	
	dbo.Enrollments e
	CROSS JOIN dbo.CardTypeTranslation ctt
	CROSS JOIN dbo.CardViewMode cvm
	
--select * from dbo.MemberIDCard

