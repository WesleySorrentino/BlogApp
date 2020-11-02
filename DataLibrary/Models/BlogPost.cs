using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class BlogPost
    {
        public IEnumerable<BlogModel> Blog { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }
        public IEnumerable<CategoryModel> Category { get; set; }
        public IEnumerable<CategoriesModel> Categories { get; set; }
    }
}
