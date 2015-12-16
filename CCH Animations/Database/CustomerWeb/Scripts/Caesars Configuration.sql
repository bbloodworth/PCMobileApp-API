---------------------------------------------------------------------
--Add EmployerDisplayName to ControlData
---------------------------------------------------------------------
IF DB_NAME() like '%Caesars%'
BEGIN
	UPDATE
		ControlData
	SET
		EmployerDisplayName = 'Caesars'


---------------------------------------------------------------------
--Excluded Members
---------------------------------------------------------------------
--Caesars Only
	exec p_InsertExcludedMember @CCHID = 23136 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 37904 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 41038 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 45206 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 118546 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 146435 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 147152 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 124397 , @ExcludeReasonDesc = 'Key Client'
	exec p_InsertExcludedMember @CCHID = 15509 , @ExcludeReasonDesc = 'Key Client'
END
-----------------------------------------------------------------------
--Client config
-----------------------------------------------------------------------
exec p_InsertClientConfigForAnimations

GO

