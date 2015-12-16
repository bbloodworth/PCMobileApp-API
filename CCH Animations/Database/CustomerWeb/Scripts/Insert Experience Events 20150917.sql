IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'Info')
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'Info'

IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'Debug')
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'Debug'
	
IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'Warning')
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'Warning'

IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'ClientPushYes')
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'ClientPushYes'
	
IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'ClientPushNo')	
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'ClientPushNo'
	
IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'NativePushYes')
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'NativePushYes'
	
IF NOT EXISTS (SELECT 1 FROM ExperienceEvent where ExperienceEventDesc = 'NativePushNo')
	exec p_InsertExperienceEvent @ExperienceEventDesc = 'NativePushNo'
	
	--select * from ExperienceEvent