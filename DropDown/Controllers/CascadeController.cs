using DropDown.Data;
using DropDown.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Controllers
{
    public class CascadeController : Controller
    {
        private readonly DropDownContext context;
        public CascadeController(DropDownContext context)
        {
            this.context = context;
        }

        

        public IActionResult CascadeDropDown(string button)
        {
            TempData["button"]=button;
            return View();
        }
        
        public JsonResult Programme()
        {
            var prog = context.Programmes.ToList();
            return new JsonResult(prog);
        }
        public JsonResult Projet(int id)
        {
            var proj = context.Projets.Where(z => z.Programme.Id == id).ToList();
            return new JsonResult(proj);
        }
        public JsonResult ActionProj(int id)
        {
            var action = context.ActionProjs.Where(z => z.Projet.Id == id).ToList();
            return new JsonResult(action);
        }
       
        public IActionResult ListProg()
        {
            var prog =context.Programmes.ToList();
            return View(prog);
        }
        public IActionResult ListProj(int? id)
        {
            var proj = context.Projets.Where(m => m.ProgrammeId == id);
            TempData["progId"] = id;
            return View(proj.ToList());
        }
        public IActionResult ListAction(int?id)
        {
            var act = context.ActionProjs.Where(m => m.ProjetId == id);
            TempData["ProjetId"] = id;
            return View(act.ToList());
        }

        /// <summary>
        /// ----------------------Programme------------------------
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateProgramme()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgramme([Bind("Name")] Programme programme)
        {
            if (ModelState.IsValid)
            {
                context.Add(programme);
                await context.SaveChangesAsync();
                
                return RedirectToAction(nameof(ListProg));
            }
            if (programme.Id == null || context.Programmes == null)
            {
                return NotFound();
            }
            var act = await context.Programmes.FirstOrDefaultAsync(m => m.Id == programme.Id);

            if (act == null)
            {
                return NotFound();
            }

            return View();
        }

        // GET: Indicateurs/Edit/5
        public async Task<IActionResult> EditProgramme(int? id)
        {
            if (id == null || context.Programmes == null)
            {
                return NotFound();
            }

            var prog = await context.Programmes.FindAsync(id);
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
        public async Task<IActionResult> EditProgramme(int Id, [Bind("Id,Name")] Programme programme)
        {
            if (Id != programme.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(programme);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgrammeExists(programme.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListProg));
            }
            return View(programme);
        }

        private bool ProgrammeExists(int id)
        {
            return (context.Programmes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Indicateurs/Delete/5
        public async Task<IActionResult> DeleteProgramme(int? id)
        {
            if (id == null || context.Programmes == null)
            {
                return NotFound();
            }

            var prog = await context.Programmes
                
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prog == null)
            {
                return NotFound();
            }

            return View(prog);
        }

        // POST: Indicateurs/Delete/5
        [HttpPost, ActionName("DeleteProgramme")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedProgramme(int id)
        {
            if (context.Programmes == null)
            {
                return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
            }
            var prog = await context.Programmes.FindAsync(id);
            if (prog != null)
            {
                context.Programmes.Remove(prog);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(ListProg));
        }


        /// <summary>
        /// ----------------------Projet------------------------
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateProjet()
        {
            ViewBag.ProgrammeId = TempData["progId"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProjet([Bind("Name,ProgrammeId")] Projet projet)
        {
            if (ModelState.IsValid)
            {
                context.Add(projet);
                await context.SaveChangesAsync();
                
                return RedirectToAction(nameof(ListProj));
            }
            if (projet.Id == null || context.Projets == null)
            {
                return NotFound();
            }
            var act = await context.Projets.FirstOrDefaultAsync(m => m.Id == projet.Id);

            if (act == null)
            {
                return NotFound();
            }

            return View();
        }


        // GET: Indicateurs/Edit/5
        public async Task<IActionResult> EditProjet(int? id)
        {
            if (id == null || context.Projets == null)
            {
                return NotFound();
            }

            var prog = await context.Projets.FindAsync(id);
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
        public async Task<IActionResult> EditProjet(int Id, [Bind("Id,Name")] Projet projet)
        {
            if (Id != projet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(projet);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgrammeExists(projet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListProj));
            }
            return View(projet);
        }

        private bool ProjetExists(int id)
        {
            return (context.Projets?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Indicateurs/Delete/5
        public async Task<IActionResult> DeleteProjet(int? id)
        {
            if (id == null || context.Projets == null)
            {
                return NotFound();
            }

            var prog = await context.Projets

                .FirstOrDefaultAsync(m => m.Id == id);
            if (prog == null)
            {
                return NotFound();
            }

            return View(prog);
        }

        // POST: Indicateurs/Delete/5
        [HttpPost, ActionName("DeleteProjet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedProjet  (int id)
        {
            if (context.Projets == null)
            {
                return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
            }
            var prog = await context.Projets.FindAsync(id);
            if (prog != null)
            {
                context.Projets.Remove(prog);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(ListProj));
        }

        /// <summary>
        /// -----------------Action--------------------
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateAction()
        {
            ViewBag.ProjetId = TempData["ProjetId"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAction([Bind("Name,ProjetId")] ActionProj actionProj)
        {
            if (ModelState.IsValid)
            {
                context.Add(actionProj);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(ListAction));
            }
            if (actionProj.Id == null || context.ActionProjs == null)
            {
                return NotFound();
            }
            var act = await context.ActionProjs.FirstOrDefaultAsync(m => m.Id == actionProj.Id);

            if (act == null)
            {
                return NotFound();
            }

            return View();
        }
        // GET: Indicateurs/Edit/5
        public async Task<IActionResult> EditAction(int? id)
        {
            if (id == null || context.ActionProjs == null)
            {
                return NotFound();
            }

            var prog = await context.ActionProjs.FindAsync(id);
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
        public async Task<IActionResult> EditAction(int Id, [Bind("Id,Name")] ActionProj action)
        {
            if (Id != action.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(action);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionExists(action.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListAction));
            }
            return View(action);
        }

        private bool ActionExists(int id)
        {
            return (context.ActionProjs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Indicateurs/Delete/5
        public async Task<IActionResult> DeleteAction(int? id)
        {
            if (id == null || context.ActionProjs == null)
            {
                return NotFound();
            }

            var prog = await context.ActionProjs

                .FirstOrDefaultAsync(m => m.Id == id);
            if (prog == null)
            {
                return NotFound();
            }

            return View(prog);
        }

        // POST: Indicateurs/Delete/5
        [HttpPost, ActionName("DeleteAction")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAction(int id)
        {
            if (context.ActionProjs == null)
            {
                return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
            }
            var prog = await context.ActionProjs.FindAsync(id);
            if (prog != null)
            {
                context.ActionProjs.Remove(prog);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(ListAction));
        }
       


    }
}
