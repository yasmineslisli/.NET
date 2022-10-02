using System;
using System.Collections.Generic;
using Castle.Core.Resource;
using DropDown.Data;
using DropDown.Models;
using DropDown.Models.Cascade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;



namespace DropDown.Controllers
{
    public class ActionProjController : Controller
    {
        private readonly DropDownContext context;
        public ActionProjController(DropDownContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(int? ActionProj)
        {
            if (ActionProj == null || context.ActionProjs == null)
            {
                return NotFound();
            }
            var act = await context.ActionProjs
                .Include(proj => proj.Projet)
                .ThenInclude(prog => prog.Programme)
                .FirstOrDefaultAsync(m => m.Id == ActionProj);

            if (act == null)
            {
                return NotFound();
            }
            ViewData["Drid"] = new SelectList(context.Drs, "Id", "Name");
            ViewData["ExerciceId"] = new SelectList(context.Exercices, "Id", "Annee");
            TempData["ActionProjId"] = ActionProj;
            TempData["Programme"] = act.Projet.Programme.Name;
            TempData["Projet"] = act.Projet.Name;
            TempData["Action"] = act.Name;
            return RedirectToAction("Create");
           
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            ViewData["Dr"] = HttpContext.Session.GetString("Structure");
            ViewData["Drid"] = HttpContext.Session.GetInt32("StructureId");
            ViewData["ExerciceId"] = new SelectList(context.Exercices, "Id", "Annee");
            ViewBag.Id = TempData["ActionProjId"];
            ViewBag.Programme = TempData["Programme"];
            ViewBag.Projet = TempData["Projet"];
            ViewBag.Action = TempData["Action"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? ActionProjId,int ExerciceId, int Drid, [Bind("ActionProjId,ExerciceId,Drid")] Objectif objectif)
        {
            var button = TempData["button"];
            
            if(ModelState.IsValid)
            {
                ViewBag.Dr = HttpContext.Session.GetString("Structure");



                
                var obj = context.Objectifs.Where(x => x.ActionProjId == ActionProjId && x.Drid== Drid && x.ExerciceId==ExerciceId);
                
                if(button.ToString() == "stockId")
                {
                    if (obj.Any())
                    {
                        var st = context.Stocks.Where(x => x.ObjectifId == obj.FirstOrDefault().Id);
                        if(st.Any())
                        {
                            return RedirectToAction("Index", "Stock");
                        }
                        else
                        {

                            TempData["ObjectifId"] = obj.SingleOrDefault().Id;
                            
                            return RedirectToAction("Stock", "Stock");
                        }
                        
                    }
                    else
                    {
                        context.Add(objectif);
                        await context.SaveChangesAsync();
                        TempData["ObjectifId"] = objectif.Id;
                        return RedirectToAction("Stock", "Stock");

                    }
                }

                if(button.ToString() == "prevId")
                {
                    if(obj.Any())
                    {
                        var prev = context.Prévisions.Where(x => x.ObjectifId == obj.SingleOrDefault().Id);
                        if(prev.Any())
                        {
                            
                            return RedirectToAction("Unfound", "Prevision");
                        }
                        else
                        {
                            TempData["ObjectifId"] = obj.SingleOrDefault().Id;
                            return RedirectToAction("Prevision", "Prevision");
                        }
                        
                    }
                    else
                    {
                        return RedirectToAction("Found", "Prevision");
                    }
                    
                }
            }

            if (ActionProjId == null || context.ActionProjs == null)
            {
                return NotFound();
            }
            var act = await context.ActionProjs.FirstOrDefaultAsync(m => m.Id == ActionProjId);

            if (act == null)
            {
                return NotFound();
            }

            return View(act);
        }

        
    }
}
