using System.Web.Mvc;
using System.Linq;
using NHibernate.Linq;
using SimpleBlog.Infrastructure;
using SimpleBlog.Models;
using SimpleBlog.Areas.Admin.ViewModels;
using System.Collections.Generic;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("users")]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return View(new UsersIndex
            {
                Users = Database.Session.Query<User>().ToList()
            });
        }

        public ActionResult New()
        {
            return View(new UsersNew
            {
                Roles = Database.Session.Query<Role>().Select(role => new RoleCheckbox
                {
                    Id = role.Id,
                    IsChecked = false,
                    Name = role.Name
                }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(UsersNew form)
        {
            var user = new User();
            SyncRoles(form.Roles, user.Roles);

            if (Database.Session.Query<User>().Any(u => u.Username == form.Username))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            user.Email = form.Email;
            user.Username = form.Username;

            user.SetPassword(form.Password);

            Database.Session.Save(user);

            return RedirectToAction("index");

        }

        public ActionResult Edit(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            
            return View(new UsersEdit
            {
                Username = user.Username,
                Email = user.Email,
                Roles = Database.Session.Query<Role>().Select(role => new RoleCheckbox
                {
                    Id = role.Id,
                    IsChecked = user.Roles.Contains(role),
                    Name = role.Name
                }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UsersEdit form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            SyncRoles(form.Roles, user.Roles);

            if (Database.Session.Query<User>().Any(u => u.Username == form.Username && u.Id != id))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            user.Username = form.Username;
            user.Email = form.Email;
            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        public ActionResult ResetPassword(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();


            return View(new UsersResetPassword
            {
                Username = user.Username
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetPassword(int id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            form.Username = user.Username;
            if (!ModelState.IsValid)
                return View(form);

            user.SetPassword(form.Password);
            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            Database.Session.Delete(user);
            return RedirectToAction("index");
        }

        #region "Helpers"

        /// <summary>
        /// Synchronize collections
        /// </summary>
        /// <param name="checkboxes"></param>
        /// <param name="roles"></param>
        private void SyncRoles(IList<RoleCheckbox> checkboxes, IList<Role> roles)
        {
            var selectedRoles = new List<Role>();

            //Get selected roles from view checkboxes
            foreach (var role in Database.Session.Query<Role>())
            {
                var checkbox = checkboxes.Single(c => c.Id == role.Id);
                checkbox.Name = role.Name;

                if (checkbox.IsChecked)
                    selectedRoles.Add(role);
            }

            //Loop through all roles which are in selectedRoles but not in the user roles. Then add them to user roles.
            foreach (var toAdd in selectedRoles.Where(r => !roles.Contains(r)))
                roles.Add(toAdd);

            //Loop through all roles which are in the user roles but not in the selectedRoles. Then remove them from user roles.
            foreach (var toRemove in roles.Where(r => !selectedRoles.Contains(r)).ToList())
                roles.Remove(toRemove);

        }

        #endregion
    }
}