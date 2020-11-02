using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Blog_User_Id { get; set; }
        public int Blog_Id { get; set; }
        public string Name { get; set; }       
        [Required(ErrorMessage = "Please enter some text!")]
        public string Content { get; set; }
        public DateTime Post_date { get; set; }
    }
}
