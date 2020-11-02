using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLibrary.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please Enter a Name.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        [Required(ErrorMessage ="Please enter a valid email address.")]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Message")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage ="Please provide some type of text.")]
        [MinLength(5)]
        [MaxLength(500)]
        public string Message { get; set; }
    }
}
