-----------------------------------------------------------------------------
--Drop FK to Enrollments created by mistake
-----------------------------------------------------------------------------

IF EXISTS (SELECT 1 FROM SYS.FOREIGN_KEYS WHERE name = 'FK_Enrollments_MemberIDCard' and parent_object_id = OBJECT_ID('dbo.MemberIDCard'))
ALTER TABLE MemberIDCard
	DROP CONSTRAINT FK_Enrollments_MemberIDCard 
go
