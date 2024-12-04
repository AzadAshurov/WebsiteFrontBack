using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class RegisterVM
    {
        [MinLength(3, ErrorMessage = "Too short")]
        [MaxLength(40, ErrorMessage = "Too long")]
        public string Name { get; set; }
        [MinLength(3, ErrorMessage = "Too short")]
        [MaxLength(40, ErrorMessage = "Too long")]
        public string Surname { get; set; }
        [MinLength(3, ErrorMessage = "Too short")]
        [MaxLength(40, ErrorMessage = "Too long")]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }

}
