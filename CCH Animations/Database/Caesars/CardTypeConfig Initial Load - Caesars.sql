------------------------------------------
--Load CardTypeConfig
------------------------------------------


USE CCH_CaesarsWeb
GO

-------------------------------------------------
--Medical Card Labels & Values
-------------------------------------------------
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'InNetworkCoinsuranceLabel', @CardTypeConfigValue = 'In'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'InNetworkCoinsuranceValue', @CardTypeConfigValue = '80%/20%'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'OutNetworkCoinsuranceLabel', @CardTypeConfigValue = 'Out'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'OutNetworkCoinsuranceValue', @CardTypeConfigValue = '50%/50%'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'EffectiveDateLabel', @CardTypeConfigValue = 'Coverage Effective Date'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'MemberNameLabel', @CardTypeConfigValue = 'Name'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'MemberMedicalIDLabel', @CardTypeConfigValue = 'ID'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 1, @CardTypeConfigKey = 'GroupDesignationLabel', @CardTypeConfigValue = 'Group'

-------------------------------------------------
--Rx Card Labels & Values
-------------------------------------------------
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxBinLabel', @CardTypeConfigValue = 'RxBin'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxBinValue', @CardTypeConfigValue = '003858'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxPCNLabel', @CardTypeConfigValue = 'RxPCN'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxPCNValue', @CardTypeConfigValue = 'A4'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxGrpLabel', @CardTypeConfigValue = 'RxGrp'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxGrpLabel', @CardTypeConfigValue = 'RxGrp'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'RxIDLabel', @CardTypeConfigValue = 'ID'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 2, @CardTypeConfigKey = 'MemberNameLabel', @CardTypeConfigValue = 'Name'

-------------------------------------------------
--Dental Card Labels & Values
-------------------------------------------------
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'PlanNameLabel', @CardTypeConfigValue = 'Plan Name'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'PlanNameValue', @CardTypeConfigValue = 'PDP Network'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'EmployeeNameLabel', @CardTypeConfigValue = 'Employee Name'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'EmployeeIDLabel', @CardTypeConfigValue = 'Employee ID'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'EmployeeIDValue', @CardTypeConfigValue = 'Your SSN'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'GroupNameLabel', @CardTypeConfigValue = 'Group Name'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'GroupNameValue', @CardTypeConfigValue = 'Caesars Entertainment'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 4, @CardTypeConfigKey = 'GroupNumberLabel', @CardTypeConfigValue = 'MetLife Group Number'

-------------------------------------------------
--Eyemed Card Labels & Values
-------------------------------------------------

exec p_InsertUpdateCardTypeConfig @CardTypeID = 3, @CardTypeConfigKey = 'PlanNameLabel', @CardTypeConfigValue = 'Plan Name'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 3, @CardTypeConfigKey = 'PlanNameValue', @CardTypeConfigValue = 'Select Plan'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 3, @CardTypeConfigKey = 'MemberNameLabel', @CardTypeConfigValue = 'Member Name'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 3, @CardTypeConfigKey = 'MemberIDLabel', @CardTypeConfigValue = 'Member ID'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 3, @CardTypeConfigKey = 'GroupNumberLabel', @CardTypeConfigValue = 'Group Number'
exec p_InsertUpdateCardTypeConfig @CardTypeID = 3, @CardTypeConfigKey = 'EffectiveDateLabel', @CardTypeConfigValue = 'Effective Date'

