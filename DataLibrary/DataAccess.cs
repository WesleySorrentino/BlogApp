using System;
using System.Collections.Generic;
using System.Text;
using DataLibrary.Models;
using Dapper;
using Npgsql;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataLibrary
{
    public class DataAccess
    {
        #region BlogDbAccess

        //Shows All Blog Posts in Database
        public IEnumerable<BlogModel> GetBlogsFromDB()
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<BlogModel>("select * from show_all_blogs() order by id DESC");

            connection.Close();

            return output;
        }

        //Adds Blog to Database
        public IEnumerable<BlogModel> AddBlogToDb(string title, string content, string author)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            
            var output = connection.Query<BlogModel>($"call add_blog_to_db('{title}','{content}','{author}','{DateTime.Now}')");

            connection.Close();

            return output;
        }

        //Selects a Blog from the database matching a Id
        public IEnumerable<BlogModel> GetBlogIdFromDb(long id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<BlogModel>($"select * from get_blog_by_id({id})");

            connection.Close();

            return output;
        }

        //Updates a blog in the database
        public IEnumerable<BlogModel> UpdateBlog(long id, string title, string content, string author)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            
            var output = connection.Query<BlogModel>($"call update_blog({id},'{title}','{content}','{author}');");

            connection.Close();

            return output;
        }

        //Removes Blog from Database
        public IEnumerable<BlogModel> RemovePost(long id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            var output = connection.Query<BlogModel>($"call delete_blog({id})");

            connection.Close();

            return output;
        }
        #endregion

        #region CommentDbAccess

        //Adds a comment to the database
        public IEnumerable<CommentModel> AddCommentToDb(long blogId, string userName, string userId, string content)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            
            var output = connection.Query<CommentModel>($"call add_comment_to_db ('{userId}', '{blogId}', '{userName}','{content}','{DateTime.Now}');");

            connection.Close();

            return output;
        }

        //Displays all comments from the database belonging to the selected blog
        public IEnumerable<CommentModel> GetComments(long id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<CommentModel>($"select * from get_comments_by_blog_id({id}) order by id");

            connection.Close();

            return output;
        }

        //Gets comment from database        
        public IEnumerable<CommentModel> GetCommentIdFromDb(long id, string userId)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<CommentModel>($"select * from get_comment_by_id({id},'{userId}')");

            connection.Close();

            return output;
        }
        
        //Updates comment in database
        public IEnumerable<CommentModel> UpdateComment(long id, string content, string userId)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<CommentModel>($"call update_comment({id},'{content}','{userId}')");

            connection.Close();

            return output;
        }

        //Removes comment from database
        public IEnumerable<CommentModel> RemoveComment(long id,string userId)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
          
            var output = connection.Query<CommentModel>($"call delete_comment({id},'{userId}')");

            connection.Close();

            return output;
        }

        //Removes all comments belonging to provided blog
        public IEnumerable<CommentModel> RemoveCommentsFromBlog(long id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            //Delete from comment where blog_id = {id}
            var output = connection.Query<CommentModel>($"call delete_all_comments_from_blog({id})");

            connection.Close();

            return output;
        }

        #endregion

        #region CategoryDbAccess

        public IEnumerable<CategoryModel> GetCategoryFromDb(long id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var category = new List<CategoryModel>();

            var getCategories = GetCategoryFromDb();

            //Gets and category associated to current id
            var getCategory = connection.Query<CategoriesModel>($"select * from get_category_by_blog_id({id})");            

            foreach (var item in getCategory)
            {
                foreach (var i in getCategories)
                {
                    if (i.Id == item.Categories_Id)
                    {
                        category.Add(new CategoryModel(i.Id, i.Name));
                    }

                    continue;
                }
            }

            connection.Close();

            return category;
        }

        public IEnumerable<CategoryModel> GetCategoryFromDb()
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            //SELECT * FROM categories;
            var output = connection.Query<CategoryModel>("select * from get_categories()");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> AddCategoryToBlog(int categoriesId, int blog_id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            
            var output = connection.Query<CategoriesModel>($"call add_category_to_blog({categoriesId},{blog_id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> GetCategoriesFromDb()
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<CategoriesModel>("SELECT * FROM get_all_category();");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> GetCategoriesIdFromDb(int id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
            //SELECT * FROM category where id = {id};
            var output = connection.Query<CategoriesModel>($"SELECT * FROM get_category_by_id({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> RemoveCategoryFromBlog(int id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));
       
            var output = connection.Query<CategoriesModel>($"call remove_category_from_blog({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> RemoveAllCategoriesFromBlog(int id)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<CategoriesModel>($"call remove_all_categories_from_blog({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoryModel> AddCategoryToDb(string name)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<CategoryModel>($"call add_category_to_db('{name}')");

            connection.Close();

            return output;
        }

        #endregion

        #region ContactDbAccess
        public IEnumerable<Contact> AddContactInfoToDb(string name, string email, string message)
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<Contact>($"call add_contact_info_to_db('{name}','{email}','{message}')");

            connection.Close();

            return output;
        }

        public IEnumerable<Contact> GetContactInfoToDb()
        {
            using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONSTRING"));

            var output = connection.Query<Contact>($"select * from get_all_contact_info()");

            connection.Close();

            return output;
        }
        #endregion
    }
}