
----------------------------------------------------------------------------------
--This script must be run after Insurance Type table has been created and loaded
------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CardType' and name = 'InsuranceTypeID')
	ALTER TABLE CardType
	ADD InsuranceTypeID int NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'CardType' and name = 'CardTypeFileName')
	ALTER TABLE CardType
	ADD CardTypeFileName nvarchar(100) NULL

IF NOT EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_InsuranceType_CardType' and parent_object_id = OBJECT_ID('dbo.CardType'))	
ALTER TABLE CardType
	ADD CONSTRAINT FK_InsuranceType_CardType FOREIGN KEY (InsuranceTypeID) REFERENCES InsuranceType(InsuranceTypeID)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

UPDATE CardType
SET InsuranceTypeID = 1
WHERE CardTypeDesc like '%medical%'

UPDATE CardType
SET InsuranceTypeID = 2
WHERE CardTypeDesc like '%Rx%'

UPDATE CardType
SET InsuranceTypeID = 3
WHERE CardTypeDesc like '%Vision%'

UPDATE CardType
SET InsuranceTypeID = 4
WHERE CardTypeDesc like '%Dental%'

