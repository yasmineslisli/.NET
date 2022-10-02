using DropDown.Data;
using System;
using System.Collections.Generic;
using DropDown.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace DropDown.Controllers;
using System.Linq;

public class PrevisionController : Controller
{
    private readonly DropDownContext context;
    public PrevisionController(DropDownContext context)
    {
        this.context = context;
    }
    
    [HttpGet]
    public IActionResult Prevision()
    {
        ViewBag.Date = DateTime.Now;
        var id = TempData["ObjectifId"];
        ViewBag.ObjectifId = id;
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
        var act = await context.Prévisions.FirstOrDefaultAsync(m => m.Id == ObjectifId);

        if (act == null)
        {
            return NotFound();
        }

        return View(act);
    }
    public IActionResult ListPrevision()
    {
        if (HttpContext.Session.GetString("Profil") == "DR")
        {
            var prev = context.Prévisions.Include(i => i.Objectif)
                                                .ThenInclude(m => m.Dr)
                                                .Include(n => n.Objectif)
                                                .ThenInclude(o => o.Exercice)
                                                .Include(p => p.Objectif)
                                                .ThenInclude(j => j.ActionProj)
                                                .ThenInclude(l => l.Projet)
                                                .ThenInclude(k => k.Programme)
                                                .Where(x => x.Objectif.Drid == HttpContext.Session.GetInt32("StructureId"));
            ;
            return View(prev.ToList());
        }
        else
        {
            var prev = context.Prévisions.Include(i => i.Objectif)
                                                .ThenInclude(m => m.Dr)
                                                .Include(n => n.Objectif)
                                                .ThenInclude(o => o.Exercice)
                                                .Include(p => p.Objectif)
                                                .ThenInclude(j => j.ActionProj)
                                                .ThenInclude(l => l.Projet)
                                                .ThenInclude(k => k.Programme)

                                                ;
            return View(prev.ToList());
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
    public async Task<IActionResult> Details (int? Id, [Bind("NumDossier,Trn,IndiceTrn,NumTrn,Superficie,Valeur,Ddid,PrévisionId")] Detail detail)
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
    public async Task<IActionResult> ListDetails(int? id)
    {
        var dropDownContext = context.Details.Include(i => i.Dd)
                                    .Include(j => j.Prévision)
                                    .ThenInclude(l => l.Objectif)
                                    .ThenInclude(k => k.ActionProj)
                                    .ThenInclude(m => m.Projet)
                                    .ThenInclude(n => n.Programme)
                                    .Where(m => m.PrévisionId == id);
        return View(await dropDownContext.ToListAsync());
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

}
