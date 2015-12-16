---------------------------------------------------------------------
--Add EmployerDisplayName to ControlData
---------------------------------------------------------------------
IF DB_NAME() = 'CCH_AvayaWeb'
BEGIN
	UPDATE
		ControlData
	SET
		EmployerDisplayName = 'Avaya'


---------------------------------------------------------------------
--Excluded Members
---------------------------------------------------------------------
--Avaya Only
	exec p_InsertExcludedMember @CCHID = 14141 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 151, @ExcludeReasonDesc = 'Key Client'
END
-----------------------------------------------------------------------
--Client config
-----------------------------------------------------------------------
exec p_InsertClientConfigForAnimations

GO

