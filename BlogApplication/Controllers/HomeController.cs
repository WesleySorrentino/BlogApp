using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DataLibrary.Models;
using DataLibrary;
using Microsoft.AspNetCore.Http;
using NToastNotify;

namespace BlogApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Contacts contacts = new Contacts();
        private readonly DataAccess db = new DataAccess();
        private readonly ShowContacts showContacts = new ShowContacts();
        private readonly IToastNotification _toastNotification;

        public HomeController(ILogger<HomeController> logger, IToastNotification toastNotification)
        {
            _logger = logger;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact([Bind("Name,Subject,Email,Message")] Contact contact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.AddContactInfoToDb(contact.Name, contact.Subject ,contact.Email, contact.Message);

                    _toastNotification.AddSuccessToastMessage("Successfully Submitted Form!");

                    return RedirectToAction(nameof(Index));
                }              

                return View();
            }
            catch
            {
                return View();
            }           
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowAllContacts()
        {
            showContacts.Contacts = db.GetContactInfoToDb();

            return View(showContacts);
        }

        [Authorize(Roles ="Admin")]
        public IActionResult DeleteContact(int? id)
        {
            if (id == null)
            {
                _toastNotification.AddErrorToastMessage("Error Occured tyring to find id.");

                return RedirectToAction("ShowAllContacts");
            }

            contacts.Contact = db.GetContactId((int)id);

            return View(contacts.Contact.First());
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult DeleteContact(int id)
        {
            db.RemoveContact(id);
            _toastNotification.AddInfoToastMessage("Removed Contact");

            return RedirectToAction("ShowAllContacts");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
