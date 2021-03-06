﻿using System.Web.Mvc;
using System.Web.Routing;
using SimpleBlog.Controllers;

namespace SimpleBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var namespaces = new[] {typeof(PostsController).Namespace};

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("", "post/{idAndSlug}", new { controller = "Posts", action = "Show" }, namespaces); //hack for using route URLs with dashes such as 1-a-post
            routes.MapRoute("Post", "post/{id}-{slug}", new { controller="Posts", action="show"}, namespaces);

            routes.MapRoute("", "tag/{idAndSlug}", new { controller = "Posts", action = "Tag" }, namespaces); //hack for using route URLs with dashes such as 1-a-tag
            routes.MapRoute("Tag", "tag/{id}-{slug}", new { controller = "Posts", action = "Tag" }, namespaces);
            
            routes.MapRoute("Login", "login", new { controller = "Auth", action= "Login", namespaces});

            routes.MapRoute("Logout", "logout", new { controller = "Auth", action = "Logout", namespaces });

            //Routes order is important: both Home and Sidebar have empty Url. Home is called because is declared first.
            routes.MapRoute("Home", "", new { controller = "Posts", action = "Index"}, namespaces);

            routes.MapRoute("Sidebar", "", new { controller = "Layout", action = "Sidebar" }, namespaces);

            routes.MapRoute("Error404", "errors/404", new { controller = "Errors", action = "NotFound" }, namespaces);
            routes.MapRoute("Error500", "errors/500", new { controller = "Errors", action = "Error" }, namespaces);
        }
    }
}
