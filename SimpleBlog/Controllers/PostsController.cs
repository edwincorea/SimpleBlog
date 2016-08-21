using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using NHibernate.Linq;
using SimpleBlog.Infrastructure;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    public class PostsController : Controller
    {
        private const int PostsPerPage = 10;
        public ActionResult Index(int page = 1)
        {
            var baseQuery = Database.Session.Query<Post>()
                .Where(t => t.DeletedAt == null)
                .OrderByDescending(t => t.CreatedAt);

            int totalPostCount = baseQuery.Count();

            var postIds = baseQuery
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Select(t => t.Id)
                .ToArray();

            var currentPostsPage = baseQuery
                .Where(t => postIds.Contains(t.Id))
                .FetchMany(t => t.Tags)
                .Fetch(t => t.User)
                .ToList();

            return View(new PostsIndex
            {
                Posts = new PagedData<Post>(currentPostsPage, totalPostCount, page, PostsPerPage)
            });
        }

        public ActionResult Tag(string idAndSlug, int page = 1)
        {
            var parts = SeparateIdAndSlug(idAndSlug);
            if (parts == null)
                return HttpNotFound();

            var tag = Database.Session.Load<Tag>(parts.Item1);
            if (tag == null)
                return HttpNotFound();

            if (!tag.Slug.Equals(parts.Item2, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("Tag", new { id = parts.Item1, slug = tag.Slug });

            int totalPostCount = tag.Posts.Count;

            var postIds = tag.Posts
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.DeletedAt == null)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .Select(t => t.Id)
                .ToArray();

            var currentPostsPage = Database.Session.Query<Post>()
                .OrderByDescending(p => p.CreatedAt)
                .Where(p => postIds.Contains(p.Id))
                .FetchMany(p => p.Tags)
                .Fetch(p => p.User)
                .ToList();

            return View(new PostsTag
            {
                Tag = tag,
                Posts = new PagedData<Post>(currentPostsPage, totalPostCount, page, PostsPerPage)
            });
        }

        public ActionResult Show(string idAndSlug)
        {
            var parts = SeparateIdAndSlug(idAndSlug);
            if (parts == null)
                return HttpNotFound();

            var post = Database.Session.Load<Post>(parts.Item1);
            if (post == null || post.IsDeleted)
                return HttpNotFound();

            //invalid slug for given post id: redirect to correct slug. SEO optimization.
            //http://blog.dev/post/43-my-slug   invalid slug
            //http://blog.dev/post/43-test      redirect to valid slug
            if (!post.Slug.Equals(parts.Item2, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("Post", new { id=parts.Item1, slug=post.Slug });

            return View(new PostsShow
            {
                Post = post
            });
        }

        #region "Helpers"

        private Tuple<int, string> SeparateIdAndSlug(string idAndSlug)
        {
            var matches = Regex.Match(idAndSlug, @"^(\d+)\-(.*)?$");
            if (!matches.Success)
                return null;

            int id = int.Parse(matches.Result("$1"));
            string slug = matches.Result("$2");

            return Tuple.Create(id, slug);
        }

        #endregion
    }
}