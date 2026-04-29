using Microsoft.AspNetCore.Mvc;
using SportComplex.Models;
using SportComplex.Services;

namespace SportComplex.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        try
        {
            var response = _userService.GetAllUsers();
            return View(response.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Users loading failed: {ex.Message}";
            TempData["success"] = "False";
            return View(Enumerable.Empty<User>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Store(User user)
    {
        try
        {
            var result = _userService.SaveUser(user);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            TempData["message"] = $"User registration failed: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Create");
        }
    }

    public IActionResult Show(int id)
    {
        try
        {
            var result = _userService.GetUserById(id);
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
            TempData["message"] = $"Error fetching user: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            var result = _userService.GetUserById(id);
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
            TempData["message"] = $"Error loading user: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Update(User user)
    {
        try
        {
            var result = _userService.UpdateUser(user);
            TempData["message"] = result.Message;
            TempData["success"] = result.Success.ToString();
            if (result.Success)
                return RedirectToAction("Index");
            return RedirectToAction("Edit", new { id = user.Id });
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Unexpected error: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Edit", new { id = user.Id });
        }
    }

    [HttpPost]
    public IActionResult Destroy(User user)
    {
        try
        {
            var result = _userService.DeleteUser(user);
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