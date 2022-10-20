using DropDown.Data;
using System;
using System.Collections.Generic;
using DropDown.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
namespace DropDown.Controllers;

using Microsoft.Data.SqlClient;
using Remotion.FunctionalProgramming;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using SqlBulkCopy = Microsoft.Data.SqlClient.SqlBulkCopy;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

public class PrevisionController : Controller
{
    private readonly DropDownContext context;
    private readonly IConfiguration configuration;
    public PrevisionController(DropDownContext context, IConfiguration configuration)
    {
        this.context = context;
        this.configuration = configuration;
    }

    [HttpGet]
    public IActionResult Prevision()
    {
        ViewBag.Date = DateTime.Now;
        var id = TempData["ObjectifId"];
        ViewBag.ObjectifId = id;
        var req = context.Objectifs
            
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


        var st=context.Stocks.Where(x => x.ObjectifId.Equals(id));
        ViewBag.StockSup=st.First().Superficie;
        ViewBag.StockNombre = st.First().Nombre;
        ViewBag.StockValeur = st.First().Valeur;

        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Prevision(int? ObjectifId, [Bind("ObjectifId,Nombre,Superficie,Name,Valeur,Date")] Prévision prévision)
    {
        if (ModelState.IsValid)
        {
            context.Add(prévision);
            await context.SaveChangesAsync();
            TempData["PrevisionId"] = prévision.Id;
            return RedirectToAction(nameof(ListPrevision));
        }

        

        if (ObjectifId == null || context.Prévisions == null)
        {
            return NotFound();
        }
        var act = await context.Prévisions
                                    .Include(x => x.Objectif)
                                    .Include(x => x.Objectif.ActionProj)
                                    .Include(x => x.Objectif.ActionProj.Projet)
                                    .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                    .FirstOrDefaultAsync(m => m.Id == ObjectifId);

        if (act == null)
        {
            return NotFound();
        }

        return View(act);
    }

    public async Task<IActionResult> ListPrevision(string sortOrder, string currentFilter,
        string? SearchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["ProgrammeSortParm"] = sortOrder == "Programme" ? "Programme_desc" : "Programme";
        ViewData["ExerciceSortParm"] = sortOrder == "Exercice" ? "Exercice_desc" : "Exercice";
        ViewData["ProjetSortParm"] = sortOrder == "Projet" ? "Projet_desc" : "Projet";
        ViewData["ActionSortParm"] = sortOrder == "Action" ? "Action_desc" : "Action";
        ViewData["DrSortParm"] = sortOrder == "Dr" ? "Dr_desc" : "Dr";
        ViewData["DateSortParm"] = sortOrder == "Date" ? "Date_desc" : "Date";


        if (HttpContext.Session.GetString("Profil") == "DR")
        {
            IQueryable<Prévision> prev = context.Prévisions
                                                .Include(x => x.Objectif.Dr)
                                                .Include(x => x.Objectif.Exercice)
                                                .Include(x => x.Objectif.ActionProj)
                                                .Include(x => x.Objectif.ActionProj.Projet)
                                                .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                                .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"))
                                                ;

            if (!String.IsNullOrEmpty(SearchString))
            {
                prev = prev
                    .Where(s => s.Objectif.ActionProj.Name.Contains(SearchString)
                                       || s.Objectif.ActionProj.Projet.Name.Contains(SearchString)
                                       || s.Objectif.ActionProj.Projet.Programme.Name.Contains(SearchString)
                                       || s.Objectif.Dr.Name.Contains(SearchString)
                                       || s.Objectif.Exercice.Annee.Contains(SearchString));
            }
            switch (sortOrder)
            {
                case "Programme_desc": //Programme
                    prev = prev.OrderByDescending(s => s.Objectif.ActionProj.Projet.Programme.Name);
                    break;
                case "Projet"://Projet
                    prev = prev.OrderBy(s => s.Objectif.ActionProj.Projet.Name);
                    break;
                case "Projet_desc"://Projet
                    prev = prev.OrderByDescending(s => s.Objectif.ActionProj.Projet.Name);
                    break;
                case "Action"://Action
                    prev = prev.OrderBy(s => s.Objectif.ActionProj.Name);
                    break;
                case "Action_desc"://Action
                    prev = prev.OrderByDescending(s => s.Objectif.ActionProj.Name);
                    break;
                case "Exercice": //Exercice
                    prev = prev.OrderBy(s => s.Objectif.Exercice.Annee);
                    break;
                case "Exercice_desc": //Exercice
                    prev = prev.OrderByDescending(s => s.Objectif.Exercice.Annee);
                    break;
                case "Dr": //Dr
                    prev = prev.OrderBy(s => s.Objectif.Dr.Name);
                    break;
                case "Dr_desc": //Dr
                    prev = prev.OrderByDescending(s => s.Objectif.Dr.Name);
                    break;
                case "Date": //Dr
                    prev = prev.OrderBy(s => s.Date);
                    break;
                case "Date_desc": //Dr
                    prev = prev.OrderByDescending(s => s.Date);
                    break;
                default:
                    prev = prev.OrderBy(s => s.Objectif.ActionProj.Projet.Programme.Name);
                    break;
            }

            int pageSize = 4;
            return View(await PaginatedList<Prévision>.CreateAsync(prev, pageNumber ?? 1, pageSize));

        }
        else
        {
            IQueryable<Prévision> prev = context.Prévisions
                                                .Include(x => x.Objectif.Dr)
                                                .Include(x => x.Objectif.Exercice)
                                                .Include(x => x.Objectif.ActionProj)
                                                .Include(x => x.Objectif.ActionProj.Projet)
                                                .Include(x => x.Objectif.ActionProj.Projet.Programme)

                                                ;
            if (!String.IsNullOrEmpty(SearchString))
            {
                prev = prev
                    .Where(s => s.Objectif.ActionProj.Name.Contains(SearchString)
                                       || s.Objectif.ActionProj.Projet.Name.Contains(SearchString)
                                       || s.Objectif.ActionProj.Projet.Programme.Name.Contains(SearchString)
                                       || s.Objectif.Dr.Name.Contains(SearchString)
                                       || s.Objectif.Exercice.Annee.Contains(SearchString));
            }
            switch (sortOrder)
            {
                case "Programme_desc": //Programme
                    prev = prev.OrderByDescending(s => s.Objectif.ActionProj.Projet.Programme.Name);
                    break;
                case "Projet"://Projet
                    prev = prev.OrderBy(s => s.Objectif.ActionProj.Projet.Name);
                    break;
                case "Projet_desc"://Projet
                    prev = prev.OrderByDescending(s => s.Objectif.ActionProj.Projet.Name);
                    break;
                case "Action"://Action
                    prev = prev.OrderBy(s => s.Objectif.ActionProj.Name);
                    break;
                case "Action_desc"://Action
                    prev = prev.OrderByDescending(s => s.Objectif.ActionProj.Name);
                    break;
                case "Exercice": //Exercice
                    prev = prev.OrderBy(s => s.Objectif.Exercice.Annee);
                    break;
                case "Exercice_desc": //Exercice
                    prev = prev.OrderByDescending(s => s.Objectif.Exercice.Annee);
                    break;
                case "Dr": //Dr
                    prev = prev.OrderBy(s => s.Objectif.Dr.Name);
                    break;
                case "Dr_desc": //Dr
                    prev = prev.OrderByDescending(s => s.Objectif.Dr.Name);
                    break;
                case "Date": //Dr
                    prev = prev.OrderBy(s => s.Date);
                    break;
                case "Date_desc": //Dr
                    prev = prev.OrderByDescending(s => s.Date);
                    break;
                default:
                    prev = prev.OrderBy(s => s.Objectif.ActionProj.Projet.Programme.Name);
                    break;
            }

            int pageSize = 4;
            return View(await PaginatedList<Prévision>.CreateAsync(prev, pageNumber ?? 1, pageSize));

        }



    }
    
    public async Task<IActionResult> Edit(int? id)
    {
        ViewBag.Date = DateTime.Now;

        ViewBag.Programme = TempData["Programme"];
        ViewBag.Projet = TempData["Projet"];
        ViewBag.Action = TempData["Action"];
        ViewBag.Dr = TempData["Dr"];

        //ViewData["ProfilId"] = new SelectList(context.Profils, "Id", "Name");
        //ViewData["StructureId"] = new SelectList(context.Structures, "Id", "Name");
        if (id == null || context.Prévisions == null)
        {
            return NotFound();
        }

        var prog = await context.Prévisions.FindAsync(id);
        if (prog == null)
        {
            return NotFound();
        }
        return View(prog);
    }

    //// POST: Indicateurs/Edit/5
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, [Bind("Id,ObjectifId,Nombre,Superficie,Valeur,Date")] Prévision prevision)
    {
        if (Id != prevision.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(prevision);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrévisionsExists(prevision.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ListPrevision));
        }
        return View(prevision);
    }

    private bool PrévisionsExists(int id)
    {
        return (context.Prévisions?.Any(e => e.Id == id)).GetValueOrDefault();
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || context.Prévisions == null)
        {
            return NotFound();
        }

        var prevision = await context.Prévisions
            .Include(i => i.Objectif)
            .ThenInclude(i => i.ActionProj)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (prevision == null)
        {
            return NotFound();
        }

        return View(prevision);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (context.Prévisions == null)
        {
            return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
        }
        var prevision = await context.Prévisions.FindAsync(id);

        if (prevision != null)
        {
            context.Prévisions.Remove(prevision);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(ListPrevision));
    }

    private bool IndicateurExists(int id)
    {
        return (context.Prévisions?.Any(e => e.Id == id)).GetValueOrDefault();
    }


    [HttpGet]
    public async Task<IActionResult> Details(int? Id)
    {
        var id = await context.Prévisions

                               .FirstOrDefaultAsync(i => i.Id == Id);
        ViewData["Ddid"] = new SelectList(context.Dds, "Id", "Name");
        ViewBag.PrévisionId = id;
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Details(int? Id, [Bind("NumDossier,Trn,IndiceTrn,NumTrn,Superficie,Valeur,Ddid,PrévisionId")] Detail detail)
    {
        if (ModelState.IsValid)
        {
            detail.IndiceTrn = detail.IndiceTrn.ToUpper();
            context.Add(detail);
            await context.SaveChangesAsync();
            TempData["DetailId"] = detail.Id;
            return RedirectToAction(nameof(ListPrevision));
        }
        if (Id == null || context.Details == null)
        {
            return NotFound();
        }
        var act = await context.Details.FirstOrDefaultAsync(m => m.Id == Id);

        if (act == null)
        {
            return NotFound();
        }

        return View(act);
    }
    public async Task<IActionResult> ListDetails(int? id,string sortOrder, string currentFilter,
        string? SearchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["ActionSortParm"] = sortOrder == "Action" ? "Action_desc" : "Action";
        ViewData["DdSortParm"] = sortOrder == "Dd" ? "Dd_desc" : "Dd";
        ViewData["NumDossierSortParm"] = sortOrder == "NumDossier" ? "NumDossier_desc" : "NumDossier";
        
            
            IQueryable<Detail> prev = context.Details.Include(i => i.Dd)
                                    .Include(j => j.Prévision)
                                    .Include(l => l.Prévision.Objectif)
                                    .Include(k => k.Prévision.Objectif.ActionProj)
                                    .Include(m => m.Prévision.Objectif.ActionProj.Projet)
                                    .Include(n => n.Prévision.Objectif.ActionProj.Projet.Programme)
                                    .Where(m => m.PrévisionId == id);
            if (!String.IsNullOrEmpty(SearchString))
            {
                prev = prev
                    .Where(s => s.Prévision.Objectif.ActionProj.Name.Contains(SearchString)
                                       || s.Dd.Name.Contains(SearchString)
                                       || s.NumDossier.Equals(SearchString));
            }
            switch (sortOrder)
            {
                case "Action": //Programme
                    prev = prev.OrderByDescending(s => s.Prévision.Objectif.ActionProj.Name);
                    break;
                case "Dd"://Projet
                    prev = prev.OrderBy(s => s.Dd);
                    break;
                case "Dd_desc"://Projet
                    prev = prev.OrderByDescending(s => s.Dd);
                    break;
                case "NumDossier"://Action
                    prev = prev.OrderBy(s => s.NumDossier);
                    break;
                case "NumDossier_desc"://Action
                    prev = prev.OrderByDescending(s => s.NumDossier);
                break;
                default:
                    prev = prev.OrderBy(s => s.Prévision.Objectif.ActionProj.Name);
                    break;
            }

            int pageSize = 4;
            return View(await PaginatedList<Detail>.CreateAsync(prev, pageNumber ?? 1, pageSize));

        
        
        
    }


    public IActionResult Unfound()
    {
        return View();
    }
    public IActionResult Found()
    {
        return View();
    }

    public async Task<IActionResult> DeleteDetails(int? id)
    {
        if (id == null || context.Details == null)
        {
            return NotFound();
        }

        var detail = await context.Details

            .FirstOrDefaultAsync(m => m.Id == id);

        if (detail == null)
        {
            return NotFound();
        }

        return View(detail);
    }

    [HttpPost, ActionName("DeleteDetails")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmedDetails(int id)
    {
        if (context.Details == null)
        {
            return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
        }
        var detail = await context.Details.FindAsync(id);

        if (detail != null)
        {
            context.Details.Remove(detail);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(ListPrevision));
    }



    /// <summary>
    /// ---------------Valider---------------------
    /// </summary>
    /// <param name="Valider"></param>
    /// <returns></returns>
    public IActionResult Valider(int? id)
    {
        ViewBag.Date = DateTime.Now;
        ViewBag.Motif = null;


        //ViewData["ProfilId"] = new SelectList(context.Profils, "Id", "Name");
        //ViewData["StructureId"] = new SelectList(context.Structures, "Id", "Name");
        if (id == null || context.Prévisions == null)
        {
            return NotFound();
        }

        var prog = context.Prévisions
            .Include(x => x.Objectif.ActionProj.Projet.Programme)
            .Include(x => x.Objectif.ActionProj.Projet)
            .Include(x => x.Objectif.ActionProj)
            .Include(x => x.Objectif.Exercice)
            .Include(x => x.Objectif.Dr)
            .FirstOrDefault(x => x.Id == id);
        ;
        if (prog == null)
        {
            return NotFound();
        }
        return View(prog);
    }

    //// POST: Indicateurs/Edit/5
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Valider(int Id, [Bind("Id,ObjectifId,Nombre,Superficie,Valeur,Date,Etat,MotifRejet")] Prévision prevision)
    {
        if (Id != prevision.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(prevision);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrévisionsExists(prevision.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ListPrevision));
        }
        return View(prevision);
    }




    /// <summary>
    /// ---------------Rejeter---------------------
    /// </summary>
    /// <param name="Rejeter"></param>
    /// <returns></returns>
    public IActionResult Rejeter(int? id)
    {
        ViewBag.Date = DateTime.Now;



        //ViewData["ProfilId"] = new SelectList(context.Profils, "Id", "Name");
        //ViewData["StructureId"] = new SelectList(context.Structures, "Id", "Name");
        if (id == null || context.Prévisions == null)
        {
            return NotFound();
        }

        var prog = context.Prévisions
            .Include(x => x.Objectif.ActionProj.Projet.Programme)
            .Include(x => x.Objectif.ActionProj.Projet)
            .Include(x => x.Objectif.ActionProj)
            .Include(x => x.Objectif.Exercice)
            .Include(x => x.Objectif.Dr)
            .FirstOrDefault(x => x.Id == id);
        ;
        if (prog == null)
        {
            return NotFound();
        }
        return View(prog);
    }

    //// POST: Indicateurs/Edit/5
    //// To protect from overposting attacks, enable the specific properties you want to bind to.
    //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rejeter(int Id, [Bind("Id,ObjectifId,Nombre,Superficie,Valeur,Date,Etat,MotifRejet")] Prévision prevision)
    {
        if (Id != prevision.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(prevision);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrévisionsExists(prevision.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ListPrevision));
        }
        return View(prevision);
    }



    

    public async Task<IActionResult> ListRejet(string sortOrder, string currentFilter,
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
            IQueryable<Prévision> prevbad = context.Prévisions
                                        .Include(x => x.Objectif.Dr)
                                        .Include(x => x.Objectif.Exercice)
                                        .Include(x => x.Objectif.ActionProj)
                                        .Include(x => x.Objectif.ActionProj.Projet)
                                        .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                        .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"))
                                        .Where(x => x.Etat == false)
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

            return View(await PaginatedList<Prévision>.CreateAsync(prevbad, pageNumber ?? 1, pageSize));
        }
        else
        {
            IQueryable<Prévision> prevbad = context.Prévisions
                                        .Include(x => x.Objectif.Dr)
                                        .Include(x => x.Objectif.Exercice)
                                        .Include(x => x.Objectif.ActionProj)
                                        .Include(x => x.Objectif.ActionProj.Projet)
                                        .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                        
                                        .Where(x => x.Etat == false)
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

            return View(await PaginatedList<Prévision>.CreateAsync(prevbad, pageNumber ?? 1, pageSize));

        }

    }
    public async Task<IActionResult> ListValide(string sortOrder, string currentFilter, string? SearchString, int? pageNumber)
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
            IQueryable<Prévision> prevbad = context.Prévisions
                                        .Include(x => x.Objectif.Dr)
                                        .Include(x => x.Objectif.Exercice)
                                        .Include(x => x.Objectif.ActionProj)
                                        .Include(x => x.Objectif.ActionProj.Projet)
                                        .Include(x => x.Objectif.ActionProj.Projet.Programme)
                                        .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"))
                                        .Where(x => x.Etat == true)
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

            return View(await PaginatedList<Prévision>.CreateAsync(prevbad, pageNumber ?? 1, pageSize));
        }
        else
        {
            IQueryable<Prévision> prevbad = context.Prévisions
                                        .Include(x => x.Objectif.Dr)
                                        .Include(x => x.Objectif.Exercice)
                                        .Include(x => x.Objectif.ActionProj)
                                        .Include(x => x.Objectif.ActionProj.Projet)
                                        .Include(x => x.Objectif.ActionProj.Projet.Programme)

                                        .Where(x => x.Etat == true)
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

            return View(await PaginatedList<Prévision>.CreateAsync(prevbad, pageNumber ?? 1, pageSize));

        }

    }

    

    public IActionResult ImportExcel()
    {
        return View();
    }
    [HttpPost]
    public IActionResult ImportExcel(IFormFile formFile)
    {
        try
        {
            if(formFile.Length>0)
            {
                var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadExcelFile");
                if (!Directory.Exists(mainPath))
                {
                    Directory.CreateDirectory(mainPath);
                }

                var filePath = Path.Combine(mainPath, formFile.FileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                var fileName = formFile.FileName;
                string extension = Path.GetExtension(fileName);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filePath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;
                    case ".xlsx":
                        conString = "Provider=Microsoft.Jet.OLEDB.12.0; Data Source=" + filePath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;

                }
                
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
                using (OleDbConnection conExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = conExcel;
                            conExcel.Open();
                            DataTable dtExcelSchema = conExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            conExcel.Close();

                        }
                    }
                }

                conString = configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "Details";
                        sqlBulkCopy.ColumnMappings.Add("numDossier", "numDossier");
                        sqlBulkCopy.ColumnMappings.Add("TRN", "TRN");
                        sqlBulkCopy.ColumnMappings.Add("indiceTRN", "indiceTRN");
                        sqlBulkCopy.ColumnMappings.Add("numTRN", "numTRN");
                        sqlBulkCopy.ColumnMappings.Add("Superficie", "Superficie");
                        sqlBulkCopy.ColumnMappings.Add("Valeur", "Valeur");
                        sqlBulkCopy.ColumnMappings.Add("DDId", "DDId");
                        sqlBulkCopy.ColumnMappings.Add("PrévisionId", "PrévisionId");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();

                    }
                }
                ViewBag.MESSAGE = "File Imported Successfuly, Data Saved into Database";
                return RedirectToAction("ListDetails");

            }
        }
        catch(Exception ex)
        {
            string msg = ex.Message;
        }
        return View();
    }



}