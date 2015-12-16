namespace CchWebAPI.Areas.Animation.Models
{
    public class UserAuthenticationRequest
    {
        public string UserName;
        public string Password;
        public string FirstName;
        public string LastName;
        public string DateOfBirth;
        public string LastFourSsn;
        public string FullSsn;
        public string MedicalId;
        public string Phone;
        public string MobilePhone;
        public int SecretQuestionId;
        public string SecretQuestion;
        public string SecretAnswer;
        public string NewPassword;
    }
}