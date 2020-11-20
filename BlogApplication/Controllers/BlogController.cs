using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataLibrary;
using DataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;
using System.Web;

namespace TestingDB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        readonly DataAccess db = new DataAccess();
        readonly BlogPost blogModel = new BlogPost();
        private readonly IToastNotification _toastNotification;

        public BlogController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }

        // GET: BlogController
        //Shows all blogs
        [AllowAnonymous]
        public ActionResult Index()
        {                         
            blogModel.Blog = db.GetBlogsFromDB();
            blogModel.Category = db.GetCategoryFromDb();
            blogModel.Categories = db.GetCategoriesFromDb();

            return View(blogModel);
        }

        [AllowAnonymous]
        // GET: BlogController/Details/5
        public ActionResult Details(long? id, string title)        
        {
            if (id == null && title == null)
            {
                _toastNotification.AddErrorToastMessage("Id and Title can't be null! Please try again.");

                return RedirectToAction("Index");
            }

            blogModel.Blog = db.GetBlogIdFromDb((long)id);
            blogModel.Category = db.GetCategoryFromDb((long)id);
            blogModel.Comments = db.GetComments((long)id);

            if (blogModel == null || blogModel.Blog.Count() == 0)
            {
                _toastNotification.AddErrorToastMessage($"Could not find the desired blog post with:<br/>Id of {id} and title of {title}.");

                return RedirectToAction("Index");
            }

            return View(blogModel);
        }

        // GET: BlogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Title,Content,Author,ShowPost")] BlogModel blog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.AddBlogToDb(blog.Title, blog.Content, blog.Author,blog.ShowPost);

                    _toastNotification.AddSuccessToastMessage("Created a new Blog");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Error Creating new blog");

                return View(blog);
            }
        }

        // GET: BlogController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            blogModel.Blog = db.GetBlogIdFromDb((long)id);

            if (blogModel.Blog == null)
            {
                return NotFound();
            }

            return View(blogModel.Blog.First());
        }

        // POST: BlogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,Title,Content,Author,ShowPost")] BlogModel blog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.UpdateBlog(id, blog.Title, blog.Content, blog.Author, blog.ShowPost);

                    _toastNotification.AddSuccessToastMessage($"Successfully Updated blog: <br/>{blog.Id}<br/>{blog.Title}");

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(blog);
            }                                             

        }

        // GET: BlogController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }          

            blogModel.Blog = db.GetBlogIdFromDb((long)id);

            if (blogModel == null)
            {
                return NotFound();
            }

            return View(blogModel.Blog.First());
        }

        // POST: BlogController/Delete/5
        //Removes Blog from Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                //Gets all comments from blog
                db.RemoveCommentsFromBlog(id);
                //Gets all categories from blog
                db.RemoveAllCategoriesFromBlog(id);
                //Removes blog post
                db.RemovePost(id);

                _toastNotification.AddSuccessToastMessage("Successfully Removed Blog Post.");

                return RedirectToAction("Index", "Blog");
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Error deleting blog post");

                return View();
            }
        }
    }
}
