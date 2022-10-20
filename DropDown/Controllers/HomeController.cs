using DropDown.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using DropDown.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.Primitives;
using Castle.Core.Resource;
using HttpPostedFileHelper;
using LinqToExcel;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DropDown.Models.Cascade;

namespace DropDown.Controllers
{
    public class HomeController : Controller
    {

        private readonly DropDownContext context;
        public HomeController(DropDownContext context)
        {
            this.context = context;
        }
       
       
        public async Task<IActionResult> Index1(string sortOrder, string currentFilter, string? SearchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ProgrammeSortParm"] = sortOrder == "Programme" ? "Programme_desc" : "Programme";
            ViewData["ExerciceSortParm"] = sortOrder == "Exercice" ? "Exercice_desc" : "Exercice";
            ViewData["ProjetSortParm"] = sortOrder == "Projet" ? "Projet_desc" : "Projet";
            ViewData["ActionSortParm"] = sortOrder == "Action" ? "Action_desc" : "Action";
            ViewData["DrSortParm"] = sortOrder == "Dr" ? "Dr_desc" : "Dr";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "Date_desc" : "Date";

            
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.Nom = HttpContext.Session.GetString("Nom");
            ViewBag.Prenom = HttpContext.Session.GetString("Prenom");
            ViewBag.Email = HttpContext.Session.GetString("Email");


            if (HttpContext.Session.GetString("Profil") == "DR")
            {
                var obj = context.Objectifs.Include(x => x.Stocks)
                                                  .Include(x => x.Prévisions)
                                                  .Include(x => x.ActionProj)
                                                  .Include(x => x.ActionProj.Projet)
                                                  .Include(x => x.ActionProj.Projet.Programme)
                                                  .Include(x => x.Dr)
                                                  .Include(x => x.Exercice)
                                                  .Where(x => x.Drid == HttpContext.Session.GetInt32("StructureId"));
                if (!String.IsNullOrEmpty(SearchString))
                {
                    obj = obj
                        .Where(s => s.ActionProj.Name.Contains(SearchString)
                                           || s.ActionProj.Projet.Name.Contains(SearchString)
                                           || s.ActionProj.Projet.Programme.Name.Contains(SearchString)
                                           || s.Dr.Name.Contains(SearchString)
                                           || s.Exercice.Annee.Contains(SearchString));
                }
                switch (sortOrder)
                {
                    case "Programme_desc": //Programme
                        obj = obj.OrderByDescending(s => s.ActionProj.Projet.Programme.Name);
                        break;
                    case "Projet"://Projet
                        obj = obj.OrderBy(s => s.ActionProj.Projet.Name);
                        break;
                    case "Projet_desc"://Projet
                        obj = obj.OrderByDescending(s => s.ActionProj.Projet.Name);
                        break;
                    case "Action"://Action
                        obj = obj.OrderBy(s => s.ActionProj.Name);
                        break;
                    case "Action_desc"://Action
                        obj = obj.OrderByDescending(s => s.ActionProj.Name);
                        break;
                    case "Exercice": //Exercice
                        obj = obj.OrderBy(s => s.Exercice.Annee);
                        break;
                    case "Exercice_desc": //Exercice
                        obj = obj.OrderByDescending(s => s.Exercice.Annee);
                        break;
                    case "Dr": //Dr
                        obj = obj.OrderBy(s => s.Dr.Name);
                        break;
                    case "Dr_desc": //Dr
                        obj = obj.OrderByDescending(s => s.Dr.Name);
                        break;

                    default:
                        obj = obj.OrderBy(s => s.ActionProj.Projet.Programme.Name);
                        break;
                        
                }
                int pageSize = 4;
                return View(await PaginatedList<Objectif>.CreateAsync(obj, pageNumber ?? 1, pageSize));

            }
            else
            {
                IQueryable<Objectif> obj = context.Objectifs.Include(x => x.Stocks)
                                                  .Include(x => x.Prévisions)
                                                  .Include(x => x.ActionProj)
                                                  .Include(x => x.ActionProj.Projet)
                                                  .Include(x => x.ActionProj.Projet.Programme)
                                                  .Include(x => x.Dr)
                                                  .Include(x => x.Exercice)
                                                  ;
                if (!String.IsNullOrEmpty(SearchString))
                {
                    obj = obj
                        .Where(s => s.ActionProj.Name.Contains(SearchString)
                                           || s.ActionProj.Projet.Name.Contains(SearchString)
                                           || s.ActionProj.Projet.Programme.Name.Contains(SearchString)
                                           || s.Dr.Name.Contains(SearchString)
                                           || s.Exercice.Annee.Contains(SearchString));
                }
                switch (sortOrder)
                {
                    case "Programme_desc": //Programme
                        obj = obj.OrderByDescending(s => s.ActionProj.Projet.Programme.Name);
                        break;
                    case "Projet"://Projet
                        obj = obj.OrderBy(s => s.ActionProj.Projet.Name);
                        break;
                    case "Projet_desc"://Projet
                        obj = obj.OrderByDescending(s => s.ActionProj.Projet.Name);
                        break;
                    case "Action"://Action
                        obj = obj.OrderBy(s => s.ActionProj.Name);
                        break;
                    case "Action_desc"://Action
                        obj = obj.OrderByDescending(s => s.ActionProj.Name);
                        break;
                    case "Exercice": //Exercice
                        obj = obj.OrderBy(s => s.Exercice.Annee);
                        break;
                    case "Exercice_desc": //Exercice
                        obj = obj.OrderByDescending(s => s.Exercice.Annee);
                        break;
                    case "Dr": //Dr
                        obj = obj.OrderBy(s => s.Dr.Name);
                        break;
                    case "Dr_desc": //Dr
                        obj = obj.OrderByDescending(s => s.Dr.Name);
                        break;

                    default:
                        obj = obj.OrderBy(s => s.ActionProj.Projet.Programme.Name);
                        break;

                }
                int pageSize = 4;
                return View(await PaginatedList<Objectif>.CreateAsync(obj, pageNumber ?? 1, pageSize));

            }

        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")))
            {
                return RedirectToAction("Login", "Account");
            }
            var prevgood = (context.Prévisions
                                                .Include(x => x.Objectif.Dr)
                                                .Include(x => x.Objectif.Exercice)
                                                .Include(x => x.Objectif.ActionProj)
                                                .Include(x => x.Objectif.ActionProj.Projet)
                                                .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                                .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"))
                                                .Where(x => x.Etat == true)).Count()
                                                ;
            var prevbad = (context.Prévisions
                                                .Include(x => x.Objectif.Dr)
                                                .Include(x => x.Objectif.Exercice)
                                                .Include(x => x.Objectif.ActionProj)
                                                .Include(x => x.Objectif.ActionProj.Projet)
                                                .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                                .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"))
                                                .Where(x => x.Etat == false)).Count()
                                                ;

            ViewBag.Rejet = prevbad;
            ViewBag.Valide = prevgood;
            return View();
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


        


        
    }



}
