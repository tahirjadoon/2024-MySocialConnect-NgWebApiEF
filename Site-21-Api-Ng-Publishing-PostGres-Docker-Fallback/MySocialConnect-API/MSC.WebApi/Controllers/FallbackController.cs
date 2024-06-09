using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace MSC.WebApi.Controllers;

//Inherit from Controller since we wil be using the view - index.html from angular app
public class FallbackController : Controller
{
    public ActionResult Index()
    {
        //get the index.html from wwwroot folder
        var currentDirectory = Directory.GetCurrentDirectory();
        var indexHtml = Path.Combine(currentDirectory, "wwwroot", "index.html");
        return PhysicalFile(indexHtml, "text/HTML");
    }
}
