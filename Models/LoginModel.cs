using System.ComponentModel.DataAnnotations;

namespace CarsWeb.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        
        public string LoginResult { get; set; }

        public bool Empty()
        {
            return string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Password) ? true : false;
        }

        public LoginModel()
        {
            LoginResult = string.Empty;
        }
    }
}
