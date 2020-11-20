using System;
using System.Collections.Generic;
using System.Text;
using DataLibrary.Models;
using Dapper;
using Npgsql;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;

namespace DataLibrary
{
    public class DataAccess
    {
        public NpgsqlConnection connection = new NpgsqlConnection(HerokuConnection());

        #region HerokoAccess
        public static string HerokuConnection()
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }
        #endregion

        #region BlogDbAccess

        //Shows All Blog Posts in Database
        public IEnumerable<BlogModel> GetBlogsFromDB()
        {
            var output = connection.Query<BlogModel>("select * from show_all_blogs() order by id DESC");

            connection.Close();

            return output;
        }

        //Adds Blog to Database
        public IEnumerable<BlogModel> AddBlogToDb(string title, string content, string author, bool showPost)
        {
            //Sanitizes input
            var t = new { Title = title.Replace("'","''"), DbType.String, ParameterDirection.Input };
            var c = new { Content = content.Replace("'", "''"), DbType.String, ParameterDirection.Input };

            var output =  connection.Query<BlogModel>($"call add_blog('{t.Title}','{c.Content}','{author}', {showPost},'{DateTime.Now}')");
            
            connection.Close();

            return output;
        }

        //Selects a Blog from the database matching a Id
        public IEnumerable<BlogModel> GetBlogIdFromDb(long id)
        {
            var output = connection.Query<BlogModel>($"select * from get_blog_by_id({id})");

            connection.Close();

            return output;
        }

        //Updates a blog in the database
        public IEnumerable<BlogModel> UpdateBlog(long id, string title, string content, string author, bool showPost)
        {
            //Sanitizes input            
            var t = new { Title = title.Replace("'", "''"), DbType.String, ParameterDirection.Input };
            var c = new { Content = content.Replace("'", "''"), DbType.String, ParameterDirection.Input };

            var output = connection.Query<BlogModel>($"call update_blog_post({id},{showPost},'{t.Title}','{c.Content}','{author}');");

            connection.Close();

            return output;
        }

        //Removes Blog from Database
        public IEnumerable<BlogModel> RemovePost(long id)
        {
            var output = connection.Query<BlogModel>($"call delete_blog({id})");

            connection.Close();

            return output;
        }
        #endregion

        #region CommentDbAccess

        //Adds a comment to the database
        public IEnumerable<CommentModel> AddCommentToDb(long blogId, string userName, string userId, string content)
        {            
            //Sanitizes Comment
            var c = new { Content = content.Replace("'", "''"), DbType.String, ParameterDirection.Input };

            var output = connection.Query<CommentModel>($"call add_comment_to_db ('{userId}', '{blogId}', '{userName}','{c.Content}','{DateTime.Now}');");

            connection.Close();

            return output;
        }

        //Displays all comments from the database belonging to the selected blog
        public IEnumerable<CommentModel> GetComments(long id)
        {
            var output = connection.Query<CommentModel>($"select * from get_comments_by_blog_id({id}) order by id");

            connection.Close();

            return output;
        }

        //Gets comment from database        
        public IEnumerable<CommentModel> GetCommentIdFromDb(long id, string userId)
        {
            var output = connection.Query<CommentModel>($"select * from get_comment_by_id({id},'{userId}')");

            connection.Close();

            return output;
        }
        
        //Updates comment in database
        public IEnumerable<CommentModel> UpdateComment(long id, string content, string userId)
        {
            //Sanitizes Comment
            var c = new { Content = content.Replace("'", "''"), DbType.String, ParameterDirection.Input };

            var output = connection.Query<CommentModel>($"call update_comment({id},'{c.Content}','{userId}')");

            connection.Close();

            return output;
        }

        //Removes comment from database
        public IEnumerable<CommentModel> RemoveComment(long id,string userId)
        {
            var output = connection.Query<CommentModel>($"call delete_comment({id},'{userId}')");

            connection.Close();

            return output;
        }

        //Removes all comments belonging to provided blog
        public IEnumerable<CommentModel> RemoveCommentsFromBlog(long id)
        {
            var output = connection.Query<CommentModel>($"call delete_all_comments_from_blog({id})");

            connection.Close();

            return output;
        }

        #endregion

        #region CategoryDbAccess

        public IEnumerable<CategoryModel> GetCategoryFromDb(long id)
        {
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
            var output = connection.Query<CategoryModel>("select * from get_categories()");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> AddCategoryToBlog(int categoriesId, int blog_id)
        {
            var output = connection.Query<CategoriesModel>($"call add_category_to_blog({categoriesId},{blog_id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> GetCategoriesFromDb()
        {
            var output = connection.Query<CategoriesModel>("SELECT * FROM get_all_category();");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> GetCategoriesIdFromDb(int id)
        {
            var output = connection.Query<CategoriesModel>($"SELECT * FROM get_category_by_id({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> RemoveCategoryFromBlog(int id)
        {
            var output = connection.Query<CategoriesModel>($"call remove_category_from_blog({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoriesModel> RemoveAllCategoriesFromBlog(int id)
        {
            var output = connection.Query<CategoriesModel>($"call remove_all_categories_from_blog({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<CategoryModel> AddCategoryToDb(string name)
        {
            var output = connection.Query<CategoryModel>($"call add_category_to_db('{name}')");

            connection.Close();

            return output;
        }

        #endregion

        #region ContactDbAccess
        public IEnumerable<Contact> AddContactInfoToDb(string name, string subject,string email, string message)
        {
            //Sanitizes Contact
            var s = new { Subject = subject.Replace("'", "''"), DbType.String, ParameterDirection.Input };
            var m = new { Message = message.Replace("'", "''"), DbType.String, ParameterDirection.Input };

            var output = connection.Query<Contact>($"call add_contact_info_to_db('{name}','{s.Subject}','{email}','{m.Message}')");

            connection.Close();

            return output;
        }

        public IEnumerable<Contact> GetContactId(int id)
        {
            var output = connection.Query<Contact>($"select * from get_contact_info_by_id({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<Contact> RemoveContact(int id)
        {
            var output = connection.Query<Contact>($"call remove_contact_from_db({id})");

            connection.Close();

            return output;
        }

        public IEnumerable<Contact> GetContactInfoToDb()
        {
            var output = connection.Query<Contact>($"select * from get_all_contact_info()");

            connection.Close();

            return output;
        }
        #endregion
    }
}