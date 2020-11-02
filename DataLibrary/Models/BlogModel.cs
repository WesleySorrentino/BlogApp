using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class BlogModel
    {
        public int Id { get; set; }
        public int Comment_Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string Author { get; set; }
        public DateTime Post_Date { get; set; }
    }
}
