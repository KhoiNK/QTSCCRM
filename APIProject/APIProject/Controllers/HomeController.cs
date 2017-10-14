using APIProject.Model.Models;
using APIProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using APIProject.ViewModels;
using System.Net.Mail;
using System.Net;

namespace APIProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IEmailService _emailService;
        public HomeController(ICategoryService _categoryService, IEmailService _emailService)
        {
            this._categoryService = _categoryService;
            this._emailService = _emailService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";


            //MailMessage message = new MailMessage(
            // "qtsccrmemailsender@gmail.com",
            // "tungtnguyense@gmail.com",
            // "TestCRMSubject",
            // "TestCRMBody"
            //       );
            //NetworkCredential credent = new NetworkCredential(
            // "qtsccrmemailsender@gmail.com",
            // "kenejnzwmzwboknd"
            //     );

            //_emailService.SendEmail(message, credent);

            return View();
        }

        public ActionResult Test()
        {
            return this.Json(_categoryService.GetCategories(), JsonRequestBehavior.AllowGet);
        }
    }
}
