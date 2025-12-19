using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using src.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using src.Presentation.Attributes;

namespace src.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre") ?? "Usuario";
        ViewBag.CorreoUsuario = HttpContext.Session.GetString("Correo") ?? "correo@ejemplo.com";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult AccesoDenegado()
    {
       
        return View(); 
    }
}
