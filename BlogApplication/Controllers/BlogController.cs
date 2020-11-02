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

namespace TestingDB.Controllers
{
    public class BlogController : Controller
    {
        readonly DataAccess db = new DataAccess();
        readonly BlogPost blogModel = new BlogPost();
        // GET: BlogController
        public ActionResult Index()
        {                         
            blogModel.Blog = db.GetBlogsFromDB();
            blogModel.Category = db.GetCategoryFromDb();
            blogModel.Categories = db.GetCategoriesFromDb();

            return View(blogModel);
        }

        // GET: BlogController/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }           

            blogModel.Blog = db.GetBlogIdFromDb((long)id);
            blogModel.Category = db.GetCategoryFromDb((long)id);
            blogModel.Comments = db.GetComments((long)id);

            if (blogModel == null)
            {
                return NotFound();
            }

            return View(blogModel);
        }

        // GET: BlogController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogController/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Title,Content,Author")] BlogModel blog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.AddBlogToDb(blog.Title, blog.Content, blog.Author);


                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(blog);
            }
        }

        // GET: BlogController/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,Title,Content,Author")] BlogModel blog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.UpdateBlog(id, blog.Title, blog.Content, blog.Author);                                      

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                db.RemoveCommentsFromBlog(id);
                db.RemoveAllCategoriesFromBlog(id);
                db.RemovePost(id); 

                return RedirectToAction("Index", "Blog");
            }
            catch
            {
                return View();
            }
        }
    }
}
