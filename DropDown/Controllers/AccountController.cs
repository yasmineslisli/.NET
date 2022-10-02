using DropDown.Data;
using DropDown.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountContext acc;
        public AccountController(AccountContext acc)
        {
            this.acc = acc;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterNow()
        {
            ViewData["ProfilId"] = new SelectList(acc.Profils, "Id", "Name");
            ViewData["StructureId"] = new SelectList(acc.Structures, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult RegisterNow(User user)
        {
            try
            {
                user.Role = "Staff";
                user.Statut = "Active";
                acc.Users.Add(user);
                acc.SaveChanges();
                ViewBag.Message = user.Nom +" "+user.Prenom +" "+"Registered Now";
                return RedirectToAction("CascadeDropDown", "Cascade");
            }
            catch
            {
                ViewBag.Message = "Unable to save new user, please try again";
            }
            ViewData["ProfilId"] = new SelectList(acc.Profils, "Id", "Name");
            ViewData["StructureId"] = new SelectList(acc.Structures, "Id", "Name");
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            User log = acc.Users.Where(a => a.Email == user.Email && a.Password == user.Password)
                                            .Include(a=>a.Profil)
                                            .Include(a=>a.structure)
                                            .SingleOrDefault();
            if (log == null)
            {
                ViewBag.Message = "Wrong email or password. Please try again";
            }
            else
            {
                ViewBag.Message = "Logged in Successfuly";

                HttpContext.Session.SetString("Profil", log.Profil.Name);

                HttpContext.Session.SetInt32("ProfilId", log.Profil.Id);
                HttpContext.Session.SetInt32("Exercice",DateTime.Now.Year);
                HttpContext.Session.SetString("Structure", log.structure.Name);
                HttpContext.Session.SetInt32("StructureId",log.structure.Id);
                HttpContext.Session.SetString("Email", log.Email);
                HttpContext.Session.SetString("Nom", log.Nom);
                HttpContext.Session.SetString("Prenom", log.Prenom);
                HttpContext.Session.SetString("Role", log.Role);

                
                return RedirectToAction("Index", "Home");
            }

            return View();

            
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
            
        }

        


    }
}
