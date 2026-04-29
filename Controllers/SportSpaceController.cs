using Microsoft.AspNetCore.Mvc;
using SportComplex.Models;
using SportComplex.Services;

namespace SportComplex.Controllers;

public class SportSpaceController : Controller
{
    private readonly SportSpaceService _spaceService;
    
    public SportSpaceController(SportSpaceService spaceService)
    {
        _spaceService = spaceService;
    }

    public IActionResult Index(string? type)
    {
        var response = _spaceService.GetFilteredSpaces(type);

        ViewBag.SelectedType = type;
        ViewBag.Types = response.Success
            ? _spaceService.GetSpaceTypes()
            : new List<string>();

        if (!response.Success)
        {
            TempData["message"] = response.Message;
            TempData["success"] = "False";
            return View(Enumerable.Empty<SportSpace>());
        }

        return View(response.Data);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Store(SportSpace space)
    {
        try
        {
            var result = _spaceService.SaveSpace(space);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Sport Complex registration failed: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Create");
        }
    }

    public IActionResult Show(int id)
    {
        try
        {
            var result = _spaceService.GetSpaceById(id);
            if (!result.Success)
            {
                TempData["message"] = result.Message;
                TempData["success"] = "False";
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error fetching sport complex: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            var result = _spaceService.GetSpaceById(id);
            if (!result.Success)
            {
                TempData["message"] = result.Message;
                TempData["success"] = "False";
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error loading sport complex: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Update(SportSpace space)
    {
        try
        {
            var result = _spaceService.UpdateSpace(space);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Edit", new { id = space.Id });
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Unexpected error: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Edit", new { id = space.Id });
        }
    }

    [HttpPost]
    public IActionResult Destroy(SportSpace space)
    {
        try
        {
            var result = _spaceService.DeleteSpace(space);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Unexpected error: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }
}