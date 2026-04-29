using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportComplex.Data;
using SportComplex.Models;

namespace SportComplex.Controllers;

public class HomeController : Controller
{
    private readonly MySqlDbContext _context;
 
    public HomeController(MySqlDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        ViewBag.TotalUsers    = _context.users.Count();
        ViewBag.TotalSpaces   = _context.sport_spaces.Count();
        ViewBag.TotalBookings = _context.reservations.Count();
        ViewBag.ActiveBookings = _context.reservations.Count(r => r.Status == "Active");
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
}
