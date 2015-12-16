IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_name(object_id) = 'UserContentPreference' and name = 'PreferredContactPhoneNum')
	ALTER TABLE UserContentPreference
	ADD PreferredContactPhoneNum nvarchar(50) NOT NULL Default ''

