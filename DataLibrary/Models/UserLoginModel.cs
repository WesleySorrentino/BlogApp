using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLibrary.Models
{
    public class UserLoginModel
    {
        [Key]
        public string Id { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please Input a valid Email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password does not match email")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]       
        public string Password { get; set; }
    }
}
