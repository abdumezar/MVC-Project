using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class RegisterViewModel
	{
        public string FName { get; set; }
		public string LName { get; set; }

		[Required(ErrorMessage = "Email is required!")]
		[EmailAddress(ErrorMessage = "Invalid Email!")]
        public string Email { get; set; }

		[Required(ErrorMessage ="Password id required!")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage ="Confirm Password is required")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Confirm Password does not match Password")]
		public string ConfirmPassword { get; set; }
        public bool IsAgree { get; set; }

    }
}
