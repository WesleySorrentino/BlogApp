using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataLibrary.Models;
using DataLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using NToastNotify;

namespace BlogApplication.Controllers
{
    public class CommentController : Controller
    {
        readonly DataAccess db = new DataAccess();
        readonly BlogPost blogModel = new BlogPost();
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IToastNotification _toastNotification;

        public CommentController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _toastNotification = toastNotification;
        }


        public ActionResult Index()
        {
            return RedirectToAction("Index", "Blog");
        }

        // GET: CommentController/Create
        public ActionResult Create()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return View();
            }

            return RedirectToAction("Login","Account");
        }

        // POST: CommentController/Create      
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(long? id, [Bind("Content")] CommentModel comment)
        {
            try
            {
                if (id == null )
                {
                    return View();
                } 
                else if(comment.Content == null)
                {
                    _toastNotification.AddErrorToastMessage("Content must have text.");

                    return View();
                }

                var user = _userManager.GetUserId(User);

                var name = _userManager.GetUserName(User);

                if (user == null)
                {
                    return View();
                }

                if (_signInManager.IsSignedIn(User))
                {
                    if (ModelState.IsValid)
                    {
                        db.AddCommentToDb((long)id, name, user, comment.Content);
                    }

                    _toastNotification.AddSuccessToastMessage("Successfully Added comment to blog.");

                    return RedirectToAction("Details", "Blog", new { id });
                }

                _toastNotification.AddErrorToastMessage("An Error Occured");

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.GetUserId(User);

            blogModel.Comments = db.GetCommentIdFromDb((long)id, user);

            foreach (var item in blogModel.Comments)
            {
                if (user == item.Blog_User_Id)
                {
                    if (blogModel.Comments == null)
                    {
                        return NotFound();
                    }                   

                    return View(blogModel.Comments.First());
                }
            }

            return RedirectToAction("Index", "Blog");
        }

        // POST: CommentController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Content")] CommentModel comment)
        {
            try
            {
                var user = _userManager.GetUserId(User);

                blogModel.Comments = db.GetCommentIdFromDb(id, user);

                if (ModelState.IsValid)
                {
                    foreach (var item in blogModel.Comments)
                    {
                        if (user == item.Blog_User_Id || User.IsInRole("Admin"))
                        {
                            db.UpdateComment(id, comment.Content, user);
                        }

                        _toastNotification.AddSuccessToastMessage("Successfully Edited and Saved Comment");

                        return RedirectToAction("Details", new RouteValueDictionary(
                                new { controller = "Blog", action = "Details", Id = item.Blog_Id }));
                    }
                }

                return View(comment);
                
            }
            catch
            {
                return View(comment);
            }
        }

        // GET: CommentController/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.GetUserId(User);


            blogModel.Comments = db.GetCommentIdFromDb((long)id,user);

            foreach (var item in blogModel.Comments)
            {
                if (user == item.Blog_User_Id || User.IsInRole("Admin"))
                {
                    if (blogModel.Comments == null)
                    {
                        return NotFound();
                    }

                    _toastNotification.AddWarningToastMessage("You are about to delete a comment.");

                    return View(blogModel.Comments.First());
                }
            }

            return RedirectToAction("Index", "Blog");            
        }

        // POST: CommentController/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var user = _userManager.GetUserId(User);

                blogModel.Comments = db.GetCommentIdFromDb((long)id, user);

                foreach (var item in blogModel.Comments)
                {
                    if (blogModel.Comments == null)
                    {
                        return NotFound();
                    }

                    if (user == item.Blog_User_Id)
                    {
                        db.RemoveComment(id, user);

                        return RedirectToAction("Details", "Blog", new { id });
                    }
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
