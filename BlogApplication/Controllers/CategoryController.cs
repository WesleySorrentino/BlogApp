using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using DataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApplication.Controllers
{
    public class CategoryController : Controller
    {
        readonly DataAccess db = new DataAccess();
        readonly BlogPost blogModel = new BlogPost();

        public CategoryController()
        {

        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(long? id, string title)
        {
            if (id == null)
            {
                return NotFound();
            }

            blogModel.Blog = db.GetBlogIdFromDb((long)id);
            blogModel.Categories = db.GetCategoriesFromDb();
            blogModel.Category = db.GetCategoryFromDb((long)id);


            if (blogModel == null)
            {
                return NotFound();
            }

            return View(blogModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create([Bind("Name")] CategoryModel category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.AddCategoryToDb(category.Name);

                    return RedirectToAction("Index", "Blog");
                }

                return RedirectToAction("Index", "Blog");
            }
            catch 
            {

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            blogModel.Categories = db.GetCategoriesIdFromDb((int)id);

            if (blogModel.Categories == null)
            {
                return NotFound();
            }

            return View(blogModel.Categories.First());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                db.RemoveCategoryFromBlog(id);

                return RedirectToAction("Index", "Blog");
            }
            catch 
            {
                return View();
            }          
        }
    }
}
