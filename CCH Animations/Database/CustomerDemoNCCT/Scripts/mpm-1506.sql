
use cch_customerdemoncct
go


select * into bk_Enrollments_MPM1506 from Enrollments;
select * into bk_MemberIDCard_MPM1506 from MemberIDCard;



-- remove mary.s dependents
update cch_customerdemoncct.dbo.enrollments
set		membermedicalid = 'TESTZYXWVU', 
		EmployeeID = 'TESTZYXWVU'
where cchid = 63842;

update cch_customerdemoncct.dbo.enrollments
set		membermedicalid = 'TESTUVWXYZ', 
		EmployeeID = 'TESTUVWXYZ'
where cchid = 63843;

update cch_customerdemoncct.dbo.enrollments
set		membermedicalid = 'TESTLMNOPQ', 
		EmployeeID = 'TESTLMNOPQ'
where cchid = 63846;



-- add jim jones as a dependent to mary.s
update cch_customerdemoncct.dbo.enrollments
set		EmployeeId = 'TEST123456d'
where cchid = 63880;


-- recreate id cards
exec p_GetIDCardMemberData @CCHID = 63841;
exec p_GetIDCardMemberData @CCHID = 63842;
exec p_GetIDCardMemberData @CCHID = 63843;
exec p_GetIDCardMemberData @CCHID = 63846;
exec p_GetIDCardMemberData @CCHID = 63880;


-- show dependents
exec GetKeyEmployeeInfo @Email=N'mary.smith@demo.com';
go


