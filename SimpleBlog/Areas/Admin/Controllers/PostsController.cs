using System;
using System.Linq;
using System.Web.Mvc;
using NHibernate.Linq;
using SimpleBlog.Infrastructure;
using SimpleBlog.Models;
using SimpleBlog.Areas.Admin.ViewModels;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [SelectedTab("posts")]
    public class PostsController: Controller
    {
        private const int PostsPerPage = 5;
        public ActionResult Index(int page = 1)
        {
            int totalPostCount = Database.Session.Query<Post>().Count();

            var currentPostsPage = Database.Session.Query<Post>()
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .ToList();

            return View(new PostsIndex
            {
                Posts = new PagedData<Post>(currentPostsPage, totalPostCount, page, PostsPerPage)
            });
        }

        public ActionResult New()
        {
            return View("Form", new PostsForm
            {
                IsNew = true
            });
        }

        public ActionResult Edit(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            return View("Form", new PostsForm
            {
                IsNew = false,
                PostId = id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Form(PostsForm form)
        {
            form.IsNew = form.PostId == null;

            if (!ModelState.IsValid)
                return View(form);

            Post post;
            if (form.IsNew)
            {
                post = new Models.Post
                {
                    CreatedAt = DateTime.UtcNow,
                    User = Auth.User
                };
            }
            else
            {
                post = Database.Session.Load<Post>(form.PostId);

                if (post == null)
                    return HttpNotFound();

                post.UpdatedAt = DateTime.UtcNow;
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("index");
        }
    }
}