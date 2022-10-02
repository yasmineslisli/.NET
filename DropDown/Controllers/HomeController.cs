using DropDown.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using DropDown.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.Primitives;
using Castle.Core.Resource;

namespace DropDown.Controllers
{
    public class HomeController : Controller
    {

        private readonly DropDownContext context;
        public HomeController(DropDownContext context)
        {
            this.context = context;
        }
       
        public IActionResult Index1()
        {
            return View();
        }
        public IActionResult Index()
        { 
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.Nom = HttpContext.Session.GetString("Nom");
            ViewBag.Prenom = HttpContext.Session.GetString("Prenom");
            ViewBag.Email = HttpContext.Session.GetString("Email");

            
            if(HttpContext.Session.GetString("Profil")=="DR")
            {
                var obj = context.Objectifs.Include(x => x.Stocks)
                                                  .Include(x => x.Prévisions)
                                                  .Include(x => x.ActionProj)
                                                  .ThenInclude(x => x.Projet)
                                                  .ThenInclude(x => x.Programme)
                                                  .Include(x => x.Dr)
                                                  .Include(x => x.Exercice)
                                                  .Where(x => x.Drid == HttpContext.Session.GetInt32("StructureId"));
                return View(obj.ToList());
            }
            else
            {
                var obj = context.Objectifs.Include(x => x.Stocks)
                                                 .Include(x => x.Prévisions)
                                                 .Include(x => x.ActionProj)
                                                 .ThenInclude(x => x.Projet)
                                                 .ThenInclude(x => x.Programme)
                                                 .Include(x => x.Dr)
                                                 .Include(x => x.Exercice)
                                                 ;
                return View(obj.ToList());
            }
           
        }

        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CSV()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Programme");
            builder.Append("Projet");
            builder.Append("Action");
            //, "Prévision","","","", "Stock","","","");
            //builder.AppendLine("","","","","Nombre","Superficie","Valeur","", "Nombre", "Superficie", "Valeur");
            var obj = context.Objectifs.Include(x => x.Stocks)
                                                 .Include(x => x.Prévisions)
                                                 .Include(x => x.ActionProj)
                                                 .ThenInclude(x => x.Projet)
                                                 .ThenInclude(x => x.Programme)
                                                 .Include(x => x.Dr)
                                                 .Include(x => x.Exercice)
                                                 ;
            foreach( var item in obj)
            {
                builder.AppendLine($"{item.ActionProj.Projet.Programme.Name},{item.ActionProj.Projet.Name},{item.ActionProj.Name}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Objectif.csv");
        }
    }
}