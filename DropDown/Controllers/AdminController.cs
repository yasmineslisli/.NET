using DropDown.Data;
using DropDown.Models;
using DropDown.Models.Cascade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Controllers
{
    public class AdminController : Controller
    {
        private readonly AccountContext acc;
        public AdminController(AccountContext acc)
        {
            this.acc = acc;
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewData["ProfilId"] = new SelectList(acc.Profils, "Id", "Name");
            ViewData["StructureId"] = new SelectList(acc.Structures, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            try
            {
                user.Role = "Staff";
                user.Statut = "Active";
                acc.Users.Add(user);
                acc.SaveChanges();
                ViewBag.Message = user.Nom + " " + user.Prenom + " " + "Registered Now";
                return RedirectToAction("ListUser", "Admin");
            }
            catch
            {
                ViewBag.Message = "Unable to save new user, please try again";
            }
            ViewData["ProfilId"] = new SelectList(acc.Profils, "Id", "Name");
            ViewData["StructureId"] = new SelectList(acc.Structures, "Id", "Name");
            return View();
        }

        public IActionResult ListUser()
        {
            var user = acc.Users.Include(x => x.structure)
                                .Include(x => x.Profil);

            return View(user.ToList());
            
        }

        // GET: Indicateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ProfilId"] = new SelectList(acc.Profils, "Id", "Name");
            ViewData["StructureId"] = new SelectList(acc.Structures, "Id", "Name");
            if (id == null || acc.Users == null)
            {
                return NotFound();
            }

            var prog = await acc.Users.FindAsync(id);
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
        public async Task<IActionResult> Edit(int Id, [Bind("Id,Nom,Prenom,CIN,Email,Password,StructureId,ProfilId,Statut,Role")] User user)
        {
            if (Id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    acc.Update(user);
                    await acc.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListUser));
            }
            return View(user);
        }

        private bool UserExists(int id)
        {
            return (acc.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Indicateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || acc.Users == null)
            {
                return NotFound();
            }

            var prog = await acc.Users

                .FirstOrDefaultAsync(m => m.Id == id);
            if (prog == null)
            {
                return NotFound();
            }

            return View(prog);
        }

        // POST: Indicateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (acc.Users == null)
            {
                return Problem("Entity set 'projetCOMContext.Indicateurs'  is null.");
            }
            var prog = await acc.Users.FindAsync(id);
            if (prog != null)
            {
                acc.Users.Remove(prog);
            }

            await acc.SaveChangesAsync();
            return RedirectToAction(nameof(ListUser));
        }

    }
}
