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
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        readonly DataAccess db = new DataAccess();

        public ActionResult Create()
        {
            return View();
        }

        //Adds category to blog
        [HttpPost]
        public ActionResult Create([Bind("Categories_Id,Blog_Id")] CategoriesModel categories)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.AddCategoryToBlog(categories.Categories_Id, categories.Blog_Id);
                }

                return RedirectToAction("Index", "Blog");
            }
            catch
            {
                return View();
            }
        }
    }
}
