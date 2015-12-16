using System.ComponentModel.DataAnnotations;

namespace PreRendered.Models
{
    public class SignInModel
    {
        public string Id { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", 
            ErrorMessage = "Please use the MM/DD/YYYY format")]
        [Display(Name = "Date of Birth:")]
        public string BirthDate { get; set; }

        //[Range(1, 12)]
        //[Display(Name = "Birth Month")]
        //public int BirthMonth { get; set; }

        //[Range(1,31)]
        //[Display(Name = "Birth Day")]
        //public int BirthDay { get; set; }

        //[Range(1, 99)]
        //[Display(Name = "Birth Year")]
        //public int BirthYear { get; set; }

        //[Range(1, 9999)]
        //[Display(Name = "Last 4 digits of your Social Security Number")]
        //public int Ssn { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}$",
            ErrorMessage = "Please enter last 4 digits of your SSN")]
        [Display(Name = "Last 4 digits of your Social Security Number:")]
        public string Ssn { get; set; }
        public string EmployerId { get; set; }
    }
}