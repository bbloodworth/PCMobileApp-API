IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Content' and name = 'ContentURL')
	ALTER TABLE Content
	ADD ContentURL nvarchar(100) NULL
	
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'Content' and name = 'ContentPhoneNum')
	ALTER TABLE Content
	ADD ContentPhoneNum nvarchar(50) NULL
	
GO


