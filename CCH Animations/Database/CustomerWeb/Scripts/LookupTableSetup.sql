-------------------------------------------------------------------
--Lookup Table Set up
---------------------------------------------------------------------
---------------------------------------------------------------------
--Content Types
---------------------------------------------------------------------

exec p_InsertContentType @ContentTypeDesc = 'Animation'
exec p_InsertContentType @ContentTypeDesc = 'Video'
exec p_InsertContentType @ContentTypeDesc = 'Survey'
exec p_InsertContentType @ContentTypeDesc = 'Action'
exec p_InsertContentType @ContentTypeDesc = 'Message'

---------------------------------------------------------------------
--Content Status
---------------------------------------------------------------------
exec p_InsertContentStatus @ContentStatusDesc = 'Viewed'
exec p_InsertContentStatus @ContentStatusDesc = 'NotViewed'
exec p_InsertContentStatus @ContentStatusDesc = 'NotTaken'
exec p_InsertContentStatus @ContentStatusDesc = 'Passed'
exec p_InsertContentStatus @ContentStatusDesc = 'Failed'
exec p_InsertContentStatus @ContentStatusDesc = 'Unread'
exec p_InsertContentStatus @ContentStatusDesc = 'Read'
exec p_InsertContentStatus @ContentStatusDesc = 'Taken'

---------------------------------------------------------------------
--Content Display Rule
---------------------------------------------------------------------
exec p_InsertContentDisplayRule @ContentDisplayRuleDesc = 'Always Display'
exec p_InsertContentDisplayRule @ContentDisplayRuleDesc = 'Display on Parent End State'

---------------------------------------------------------------------
--QuestionType
---------------------------------------------------------------------
exec p_InsertQuestionType @QuestionTypeDesc = 'True/False'
exec p_InsertQuestionType @QuestionTypeDesc = 'MultipleChoice'
exec p_InsertQuestionType @QuestionTypeDesc = 'FreeText'

---------------------------------------------------------------------
--ExperienceEvent
---------------------------------------------------------------------
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartAndroidApp'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartMobileWeb'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartiOSApp'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartAnimation'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'EndAnimation'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartQuiz'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'EndQuiz'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'VisitResourcePage'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'ExitAndroidApp'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'ExitiOSApp'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartIntro'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'EndIntro'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'HelpfulYes'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'HelpfulNo'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'AuthenticationSuccess'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'AuthenticationFail'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'ReplayAnimation'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'GoToCch'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'GoToPlan'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'NoQueryParameters'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'InvalidQueryParameters'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'Error'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'StartWebsite'
exec p_InsertExperienceEvent @ExperienceEventDesc = 'LoadTimer'

----------------------------------------------------------------------
--Content Type State
----------------------------------------------------------------------
exec p_InsertContentTypeState @ContentTypeDesc = 'Animation', @ContentStatusDesc = 'Viewed', @InitialStateInd = 0, @EndStateInd = 1
exec p_InsertContentTypeState @ContentTypeDesc = 'Animation', @ContentStatusDesc = 'NotViewed', @InitialStateInd = 1, @EndStateInd = 0
exec p_InsertContentTypeState @ContentTypeDesc = 'Video', @ContentStatusDesc = 'Viewed', @InitialStateInd = 0, @EndStateInd = 1
exec p_InsertContentTypeState @ContentTypeDesc = 'Video', @ContentStatusDesc = 'NotViewed', @InitialStateInd = 1, @EndStateInd = 0
exec p_InsertContentTypeState @ContentTypeDesc = 'Survey', @ContentStatusDesc = 'NotTaken', @InitialStateInd = 1, @EndStateInd = 0
exec p_InsertContentTypeState @ContentTypeDesc = 'Survey', @ContentStatusDesc = 'Passed', @InitialStateInd = 0, @EndStateInd = 1
exec p_InsertContentTypeState @ContentTypeDesc = 'Survey', @ContentStatusDesc = 'Failed', @InitialStateInd = 0, @EndStateInd = 1
exec p_InsertContentTypeState @ContentTypeDesc = 'Action', @ContentStatusDesc = 'NotTaken', @InitialStateInd = 1, @EndStateInd = 0
exec p_InsertContentTypeState @ContentTypeDesc = 'Action', @ContentStatusDesc = 'Taken', @InitialStateInd = 0, @EndStateInd = 1
exec p_InsertContentTypeState @ContentTypeDesc = 'Message', @ContentStatusDesc = 'Unread', @InitialStateInd = 1, @EndStateInd = 0
exec p_InsertContentTypeState @ContentTypeDesc = 'Message', @ContentStatusDesc = 'Read', @InitialStateInd = 0, @EndStateInd = 1



