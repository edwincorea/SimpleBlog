using System.Linq;
using System.Web.Mvc;
using NHibernate.Linq;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    public class LayoutController : Controller
    {
        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            //action method should be only accessible by child request, not by invoking it directly by requesting action.
            return View(new LayoutSidebar
            {
                IsLoggedIn = Auth.User != null,
                Username = Auth.User != null ? Auth.User.Username : string.Empty,
                IsAdmin = User.IsInRole("admin"),
                Tags = Database.Session.Query<Tag>()
                .Select(tag => new
                {
                    tag.Id,
                    tag.Name,
                    tag.Slug,
                    PostCount = tag.Posts.Count
                })
                .Where(t => t.PostCount > 0)
                .OrderByDescending(t => t.PostCount)
                .Select(tag => new SidebarTag(tag.Id, tag.Name, tag.Slug, tag.PostCount))
                .ToList()
            });
        }
    }
}