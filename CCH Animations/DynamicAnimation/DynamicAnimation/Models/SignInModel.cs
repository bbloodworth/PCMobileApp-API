using System.ComponentModel.DataAnnotations;

namespace DynamicAnimation.Models
{
    public class SignInModel
    {
        public string Id { get; set; }

        //[RegularExpression(@"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$", 

        [Required]
        [RegularExpression(@"^(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])|((0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d)$", 
            ErrorMessage = "Please use the YYYY-MM-DD format")]
        [Display(Name = "Date of Birth (yyyy-mm-dd):")]
        public string BirthDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}$",
            ErrorMessage = "Please enter last 4 digits of your SSN")]
        [Display(Name = "Last 4 digits of your Social Security Number:")]
        public string Ssn { get; set; }
        public string EmployerId { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}