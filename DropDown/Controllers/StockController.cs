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

            ViewBag.Programme = TempData["Programme"];
            ViewBag.Projet = TempData["Projet"];
            ViewBag.Action = TempData["Action"];
            ViewBag.Dr = TempData["Dr"];

            var id = TempData["ObjectifId"];
            ViewBag.ObjectifId = id;
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
            var act = await context.Stocks.FirstOrDefaultAsync(m => m.Id == ObjectifId);

            if (act == null)
            {
                return NotFound();
            }

            return View(act);
        }
        public  IActionResult ListStock()
        {
            if (HttpContext.Session.GetString("Profil") == "DR")
            {
                var st = context.Stocks.Include(i => i.Objectif)
                                                .ThenInclude(m => m.Dr)
                                                .Include(n => n.Objectif)
                                                .ThenInclude(o => o.Exercice)
                                                .Include(p => p.Objectif)
                                                .ThenInclude(j => j.ActionProj)
                                                .ThenInclude(l => l.Projet)
                                                .ThenInclude(k => k.Programme)
                                                .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"));
                
                return View(st.ToList());
            }
            else
            {
                var st = context.Stocks.Include(i => i.Objectif)
                                                .ThenInclude(m => m.Dr)
                                                .Include(n => n.Objectif)
                                                .ThenInclude(o => o.Exercice)
                                                .Include(p => p.Objectif)
                                                .ThenInclude(j => j.ActionProj)
                                                .ThenInclude(l => l.Projet)
                                                .ThenInclude(k => k.Programme)
                                                ;
                return View(st.ToList());
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
