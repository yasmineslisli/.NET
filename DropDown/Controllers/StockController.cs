using System;
using System.Collections.Generic;
using System.Globalization;
using Castle.Core.Resource;
using DropDown.Data;
using DropDown.Models;
using DropDown.Models.Cascade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;



namespace DropDown.Controllers
{
    public class StockController : Controller
    {
        private readonly DropDownContext context;
        public StockController(DropDownContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult Stock()
        {
            ViewBag.Date = DateTime.Now;

            
            
            var id = TempData["ObjectifId"];
            ViewBag.ObjectifId = id;

            var req =context.Objectifs
                
                            .Include(x => x.Exercice)
                            .Include(x => x.Dr)
                            .Include(x => x.ActionProj)
                            .Include(x => x.ActionProj.Projet)
                            .Include(x => x.ActionProj.Projet.Programme)
                            .Where(m => m.Id.Equals(id));
            ViewBag.Programme = req.First().ActionProj.Projet.Programme.Name;
            ViewBag.Projet = req.First().ActionProj.Projet.Name;
            ViewBag.ActionProj = req.First().ActionProj.Name;
            ViewBag.Exercice = req.First().Exercice.Annee;
            ViewBag.Dr = req.First().Dr.Name;
            


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Stock(int? ObjectifId, [Bind("ObjectifId,Nombre,Superficie,Name,Valeur,Date")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                context.Add(stock);
                await context.SaveChangesAsync();
                TempData["ObjectifId"] = stock.Id;
                return RedirectToAction(nameof(ListStock));
            }
            if (ObjectifId == null || context.Stocks == null)
            {
                return NotFound();
            }
            var act = await context.Stocks.Include(x=>x.Objectif)
                .Include(x=>x.Objectif.Exercice)
                .Include(x => x.Objectif.Dr)
                .Include(x=>x.Objectif.ActionProj)
                .Include(x => x.Objectif.ActionProj.Projet)
                .Include(x => x.Objectif.ActionProj.Projet.Programme)
                 .FirstOrDefaultAsync(m => m.ObjectifId == ObjectifId);

            ViewBag.Programme = act.Objectif.ActionProj.Projet.Programme.Name;
            if (act == null)
            {
                return NotFound();
            }

            return View(act);
        }
        public async Task<IActionResult> ListStock(string sortOrder, string currentFilter,
        string? SearchString, int? pageNumber)
        {


            ViewData["CurrentSort"] = sortOrder;
            ViewData["ProgrammeSortParm"] = sortOrder == "Programme" ? "Programme_desc" : "Programme";
            ViewData["ExerciceSortParm"] = sortOrder == "Exercice" ? "Exercice_desc" : "Exercice";
            ViewData["ProjetSortParm"] = sortOrder == "Projet" ? "Projet_desc" : "Projet";
            ViewData["ActionSortParm"] = sortOrder == "Action" ? "Action_desc" : "Action";
            ViewData["DrSortParm"] = sortOrder == "Dr" ? "Dr" : "Dr_desc";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "Date_desc" : "Date";


            if (HttpContext.Session.GetString("Profil") == "DR")
            {
                IQueryable<Stock> prevbad = context.Stocks.Include(i => i.Objectif)
                                                .Include(m => m.Objectif.Dr)
                                                .Include(o => o.Objectif.Exercice)
                                                .Include(p => p.Objectif.ActionProj)
                                                .Include(l => l.Objectif.ActionProj.Projet)
                                                .Include(k => k.Objectif.ActionProj.Projet.Programme)
                                                .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"));


                if (!String.IsNullOrEmpty(SearchString))
                {
                    prevbad = prevbad
                            .Where(s => s.Objectif.ActionProj.Name.Contains(SearchString)
                                               || s.Objectif.ActionProj.Projet.Name.Contains(SearchString)
                                               || s.Objectif.ActionProj.Projet.Programme.Name.Contains(SearchString)
                                               || s.Objectif.Dr.Name.Contains(SearchString)
                                               || s.Objectif.Exercice.Annee.Contains(SearchString));
                }
                switch (sortOrder)
                {
                    case "Programme_desc": //Programme
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.ActionProj.Projet.Programme.Name);
                        break;
                    case "Projet"://Projet
                        prevbad = prevbad.OrderBy(s => s.Objectif.ActionProj.Projet.Name);
                        break;
                    case "Projet_desc"://Projet
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.ActionProj.Projet.Name);
                        break;
                    case "Action"://Action
                        prevbad = prevbad.OrderBy(s => s.Objectif.ActionProj.Name);
                        break;
                    case "Action_desc"://Action
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.ActionProj.Name);
                        break;
                    case "Exercice": //Exercice
                        prevbad = prevbad.OrderBy(s => s.Objectif.Exercice.Annee);
                        break;
                    case "Exercice_desc": //Exercice
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.Exercice.Annee);
                        break;
                    case "Dr": //Dr
                        prevbad = prevbad.OrderBy(s => s.Objectif.Dr.Name);
                        break;
                    case "Dr_desc": //Dr
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.Dr.Name);
                        break;
                    case "Date": //Dr
                        prevbad = prevbad.OrderBy(s => s.Date);
                        break;
                    case "Date_desc": //Dr
                        prevbad = prevbad.OrderByDescending(s => s.Date);
                        break;
                    default:
                        prevbad = prevbad.OrderBy(s => s.Objectif.ActionProj.Projet.Programme.Name);
                        break;
                }
                int pageSize = 4;

                return View(await PaginatedList<Stock>.CreateAsync(prevbad, pageNumber ?? 1, pageSize));
            }
            else
            {
                IQueryable<Stock> prevbad = context.Stocks.Include(i => i.Objectif)
                                                .Include(m => m.Objectif.Dr)
                                                .Include(o => o.Objectif.Exercice)
                                                .Include(p => p.Objectif.ActionProj)
                                                .Include(l => l.Objectif.ActionProj.Projet)
                                                .Include(k => k.Objectif.ActionProj.Projet.Programme)
                                                ;

                if (!String.IsNullOrEmpty(SearchString))
                {
                    prevbad = prevbad
                            .Where(s => s.Objectif.ActionProj.Name.Contains(SearchString)
                                               || s.Objectif.ActionProj.Projet.Name.Contains(SearchString)
                                               || s.Objectif.ActionProj.Projet.Programme.Name.Contains(SearchString)
                                               || s.Objectif.Dr.Name.Contains(SearchString)
                                               || s.Objectif.Exercice.Annee.Contains(SearchString));
                }
                switch (sortOrder)
                {
                    case "Programme_desc": //Programme
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.ActionProj.Projet.Programme.Name);
                        break;
                    case "Projet"://Projet
                        prevbad = prevbad.OrderBy(s => s.Objectif.ActionProj.Projet.Name);
                        break;
                    case "Projet_desc"://Projet
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.ActionProj.Projet.Name);
                        break;
                    case "Action"://Action
                        prevbad = prevbad.OrderBy(s => s.Objectif.ActionProj.Name);
                        break;
                    case "Action_desc"://Action
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.ActionProj.Name);
                        break;
                    case "Exercice": //Exercice
                        prevbad = prevbad.OrderBy(s => s.Objectif.Exercice.Annee);
                        break;
                    case "Exercice_desc": //Exercice
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.Exercice.Annee);
                        break;
                    case "Dr": //Dr
                        prevbad = prevbad.OrderBy(s => s.Objectif.Dr.Name);
                        break;
                    case "Dr_desc": //Dr
                        prevbad = prevbad.OrderByDescending(s => s.Objectif.Dr.Name);
                        break;
                    case "Date": //Dr
                        prevbad = prevbad.OrderBy(s => s.Date);
                        break;
                    case "Date_desc": //Dr
                        prevbad = prevbad.OrderByDescending(s => s.Date);
                        break;
                    default:
                        prevbad = prevbad.OrderBy(s => s.Objectif.ActionProj.Projet.Programme.Name);
                        break;
                }
                int pageSize = 4;

                return View(await PaginatedList<Stock>.CreateAsync(prevbad, pageNumber ?? 1, pageSize));

            }
            
            
        }

        // GET: Indicateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            ViewBag.Date = DateTime.Now;

            ViewBag.Programme = TempData["Programme"];
            ViewBag.Projet = TempData["Projet"];
            ViewBag.Action = TempData["Action"];
            ViewBag.Dr = TempData["Dr"];

           
            //ViewBag.ObjectifId = TempData["ObjectifId"];
            //ViewData["ProfilId"] = new SelectList(context.Profils, "Id", "Name");
            //ViewData["StructureId"] = new SelectList(context.Structures, "Id", "Name");
            if (id == null || context.Stocks == null)
            {
                return NotFound();
            }

            var prog = await context.Stocks.FindAsync(id);
            if (prog == null)
            {
                return NotFound();
            }
            ViewBag.ObjectifId=prog.ObjectifId;
            return View(prog);
        }

        //// POST: Indicateurs/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, [Bind("Id,ObjectifId,Nombre,Superficie,Valeur,Date")] Stock stock)
        {
            if (Id != stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(stock);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListStock));
            }
            return View(stock);
        }

        private bool StockExists(int id)
        {
            return (context.Stocks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Stocks == null)
            {
                return NotFound();
            }

            var stock = await context.Stocks
                .Include(i => i.Objectif)
                .ThenInclude(i => i.ActionProj)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Stocks == null)
            {
                return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
            }
            var stock = await context.Stocks.FindAsync(id);
            var obj =context.Objectifs.Where(x => x.Id == stock.ObjectifId).FirstOrDefault();

            if (stock != null)
            {
                context.Stocks.Remove(stock);
            }
            if (obj != null)
            {
                context.Objectifs.Remove(obj);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(ListStock));
        }

        private bool IndicateurExists(int id)
        {
            return (context.Stocks?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
