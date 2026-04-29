using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportComplex.Models;
using SportComplex.Responses;
using SportComplex.Services;

namespace SportComplex.Controllers;

public class ReservationController : Controller
{
    private readonly ReservationService _reservationService;
    private readonly UserService _userService;
    private readonly SportSpaceService _spaceService;

    public ReservationController(ReservationService reservationService, UserService userService, SportSpaceService spaceService)
    {
        _reservationService = reservationService;
        _userService = userService;
        _spaceService = spaceService;
    }

    public IActionResult Index(int? userId, int? spaceId)
    {
        var response = _reservationService.GetFilteredReservations(userId, spaceId);

        ViewBag.Users = new SelectList(_userService.GetAllUsers().Data, "Id", "Name");
        ViewBag.Spaces = new SelectList(_spaceService.GetAllSpaces().Data, "Id", "Name");
        ViewBag.SelectedUserId = userId;
        ViewBag.SelectedSpaceId = spaceId;

        if (!response.Success)
        {
            TempData["message"] = response.Message;
            TempData["success"] = "False";
            ViewBag.Users = new SelectList(Enumerable.Empty<User>(), "Id", "Name");
            ViewBag.Spaces = new SelectList(Enumerable.Empty<SportSpace>(), "Id", "Name");
            return View(Enumerable.Empty<Reservation>());
        }

        return View(response.Data);
    }

    public IActionResult Create()
    {
        var canCreate = _reservationService.CanCreateReservation();
        if (!canCreate.Success)
        {
            TempData["message"] = canCreate.Message;
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }

        PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public IActionResult Store(Reservation reservation)
    {
        var canCreate = _reservationService.CanCreateReservation();
        if (!canCreate.Success)
        {
            TempData["message"] = canCreate.Message;
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }

        var result = _reservationService.SaveReservation(reservation);
        TempData["message"] = result.Message;
        TempData["success"] = result.Success.ToString();

        return RedirectToAction(result.Success ? "Index" : "Create");
    }

    public IActionResult Show(int id)
    {
        try
        {
            var result = _reservationService.GetReservationById(id);
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
            TempData["message"] = $"Error fetching booking: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            var result = _reservationService.GetReservationById(id);
            if (!result.Success)
            {
                TempData["message"] = result.Message;
                TempData["success"] = "False";
                return RedirectToAction("Index");
            }
            PopulateDropdowns();
            return View(result.Data);
        }
        catch (Exception ex)
        {
            TempData["message"] = $"Error loading booking: {ex.Message}";
            TempData["success"] = "False";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Update(Reservation reservation)
    {
        try
        {
            var result = _reservationService.UpdateReservation(reservation);
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

    [HttpPost]
    public IActionResult Cancel(int id)
    {
        try
        {
            var result = _reservationService.CancelReservation(id);
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

    [HttpPost]
    public IActionResult Destroy(Reservation reservation)
    {
        try
        {
            var result = _reservationService.DeleteReservation(reservation);
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

    private void PopulateDropdowns()
    {
        ViewBag.Users = new SelectList(_userService.GetAllUsers().Data, "Id", "Name");
        ViewBag.Spaces = new SelectList(_spaceService.GetAllSpaces().Data, "Id", "Name");
        ViewBag.Statuses = new SelectList(new[] { "Active", "Cancelled", "Finished" });
    }
}