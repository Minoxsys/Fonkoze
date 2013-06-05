using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web.LocalizationResources;
using Web.Validation.Account;

namespace Web.Models.Account
{
    /// <summary>
    /// This is the model used by the LogOn view of the <c ref="Web.Controllers.AccountController">AccountController</c>
    /// </summary>
    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessageResourceType = typeof (Strings),
        ErrorMessageResourceName = "ChangePasswordModel_ChangePasswordModel_The_new_password_and_confirmation_password_do_not_match_")]
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName(@"Current password")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName(@"New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName(@"Confirm new password")]
        public string ConfirmPassword { get; set; }
    }
}
