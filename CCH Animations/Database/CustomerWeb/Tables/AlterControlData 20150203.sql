IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'ControlData' and name = 'EmployerDisplayName')
	ALTER TABLE ControlData
	ADD EmployerDisplayName nvarchar(100) NULL