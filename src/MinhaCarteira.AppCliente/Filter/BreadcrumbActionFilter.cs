using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MinhaCarteira.AppCliente.Models;

namespace MinhaCarteira.AppCliente.Filter;

public class BreadcrumbActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Request.Path.HasValue &&
            context.HttpContext.Request.Path.Value.Contains("Api"))
        {
            // This is an API request.  
            base.OnActionExecuted(context);
            return;
        }

        var breadcrumbs = this.ConfigureBreadcrumb(context);

        var controller = context.Controller as Controller;
        controller.ViewBag.Breadcrumbs = breadcrumbs;

        base.OnActionExecuted(context);
    }


    private List<Breadcrumb> ConfigureBreadcrumb(ActionExecutedContext context)
    {
        var routeValues = context?.HttpContext?.Request?.RouteValues;
        var breadcrumbList = new List<Breadcrumb>();
        var homeControllerName = "Home";
        var area = routeValues == null || !routeValues.ContainsKey("area")
            ? string.Empty
            : routeValues["area"].ToString();


        breadcrumbList.Add(new Breadcrumb(
            text: "Página inicial",
            action: "Index",
            controller: homeControllerName, // Change this controller name to match your Home Controller.
            area: area,
            active: true));

        if (context.HttpContext.Request.Path.HasValue)
        {
            var pathSplit = context.HttpContext.Request.Path.Value.Split("/");

            for (var i = 0; i < pathSplit.Length; i++)
            {
                // Check if first element is equal to our Index (home) page.
                if (string.IsNullOrEmpty(pathSplit[i]) || string.Compare(pathSplit[i], homeControllerName, true) == 0)
                    continue;

                // First check if path is a Controller class.
                var controller = GetControllerType(pathSplit[i] + "Controller", area);

                // If this is a controller, does it have a default Index method? If so, that needs adding as a breadcrumb. Is the next path element called Index?
                if (controller != null)
                {
                    //var indexMethod = controller.GetMethod("Index");
                    var indexMethod = controller.GetMethods().FirstOrDefault(fo => fo.Name == "Index");

                    if (indexMethod != null)
                    {
                        breadcrumbList.Add(new Breadcrumb(
                            text: CamelCaseSpacing(pathSplit[i]),
                            action: "Index",
                            controller: pathSplit[i],
                            area: area,
                            active: true));

                        if (i + 1 < pathSplit.Length && string.Compare(pathSplit[i + 1], "Index", true) == 0)
                        {
                            // This is the last element in the breadcrumb list. This should be disabled.
                            breadcrumbList.LastOrDefault().Active = false;

                            // Next path item is the Index method. We can escape from this method now.
                            return breadcrumbList;
                        }
                    }
                }

                // If not a Controller, check if this is a method on the previous path element.
                if (i - 1 > 0)
                {
                    var controllerName = pathSplit[i - 1] + "Controller";
                    var prevController = GetControllerType(controllerName, area);

                    if (prevController != null)
                    {
                        //var method = prevController.GetMethod(pathSplit[i], new[] { typeof(string) });
                        var method = prevController.GetMethods().FirstOrDefault(fo => fo.Name == pathSplit[i]);

                        if (method != null)
                        {
                            // We've found an endpoint on the previous controller.
                            breadcrumbList.Add(new Breadcrumb(
                                text: CamelCaseSpacing(pathSplit[i]),
                                action: pathSplit[i],
                                controller: pathSplit[i - 1],
                                area: area,
                                active: false));
                        }
                    }
                }
            }
        }

        // There will always be at least 1 entry in the breadcrumb list. The last element should always be disabled.
        breadcrumbList.LastOrDefault().Active = false;

        return breadcrumbList;
    }

    private static Type GetControllerType(string name, string area)
    {
        Type controller = null;

        try
        {
            var ns = string.IsNullOrWhiteSpace(area) ? "Controllers" : $"Areas.{area}.Controllers";
            controller = Assembly.GetEntryAssembly().GetType($"MinhaCarteira.AppCliente.{ns}.{name}");
        }
        catch
        { }

        return controller;
    }

    private static string CamelCaseSpacing(string s)
    {
        // Sourced from https://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters.
        var r = new Regex(@"
(?<=[A-Z])(?=[A-Z][a-z]) |
(?<=[^A-Z])(?=[A-Z]) |
(?<=[A-Za-z])(?=[^A-Za-z])",
RegexOptions.IgnorePatternWhitespace);

        return r.Replace(s, " ");
    }
}