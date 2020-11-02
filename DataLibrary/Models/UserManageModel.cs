using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLibrary.Models
{
    public class UserManageModel
    {
        [Key]
        public string Id { get; set; }

        [Display(Name ="Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
