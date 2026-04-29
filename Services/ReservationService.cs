using Microsoft.EntityFrameworkCore;
using SportComplex.Data;
using SportComplex.Models;
using SportComplex.Responses;

namespace SportComplex.Services;

public class ReservationService
{
    private readonly MySqlDbContext _context;
    private readonly EmailService _emailService;

    public ReservationService(MySqlDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public ServiceResponse<IEnumerable<Reservation>> GetAllReservations()
    {
        var reservations = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .ToList();
        return new ServiceResponse<IEnumerable<Reservation>>()
        {
            Success = true, 
            Data = reservations
        };
    }

    public ServiceResponse<IEnumerable<Reservation>> GetReservationsByUser(int userId)
    {
        var reservations = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .Where(r => r.UserId == userId)
            .ToList();
        return new ServiceResponse<IEnumerable<Reservation>>()
        {
            Success = true, 
            Data = reservations
        };
    }

    public ServiceResponse<IEnumerable<Reservation>> GetReservationsBySpace(int spaceId)
    {
        var reservations = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .Where(r => r.SportSpaceId == spaceId)
            .ToList();
        return new ServiceResponse<IEnumerable<Reservation>>()
        {
            Success = true, 
            Data = reservations
        };
    }

    public ServiceResponse<Reservation> GetReservationById(int id)
    {
        var reservation = _context.reservations
            .Include(r => r.User)
            .Include(r => r.SportSpace)
            .FirstOrDefault(r => r.Id == id);

        if (reservation != null)
            return new ServiceResponse<Reservation>()
            {
                Success = true, 
                Data = reservation, 
                Message = "Booking found"
            };

        return new ServiceResponse<Reservation>()
        {
            Success = false, 
            Data = null, 
            Message = "Booking not found"
        };
    }
    
    public ServiceResponse<bool> CanCreateReservation()
    {
        var hasUsers = _context.users.Any();
        var hasSpaces = _context.sport_spaces.Any();

        if (!hasUsers && !hasSpaces){
            return new ServiceResponse<bool>()
            {
                Success = false, 
                Message = "There must be at least one user and one sport space to create a reservation"
            };
        }

        if (!hasUsers){
            return new ServiceResponse<bool>()
            {
                Success = false, 
                Message = "There must be at least one registered user to create a reservation"
            };
        }

        if (!hasSpaces){
            return new ServiceResponse<bool>()
            {
                Success = false, 
                Message = "There must be at least one sport space to create a reservation"
            };
        }

        return new ServiceResponse<bool>()
        {
            Success = true, 
            Data = true
        };
    }

    public ServiceResponse<Reservation> SaveReservation(Reservation reservation)
    {
        try
        {
            if (reservation.Date.Date < DateTime.Today)
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = reservation, 
                    Message = "Cannot create bookings in past dates"
                };
            
            if (reservation.Date.Date == DateTime.Today && reservation.StartTime < DateTime.Now.TimeOfDay)
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = reservation, 
                    Message = "Cannot create bookings in past hours"
                };
            
            if (reservation.EndTime <= reservation.StartTime)
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = reservation, 
                    Message = "End time must be later than start time"
                };
            
            bool spaceOverlap = _context.reservations.Any(r =>
                r.SportSpaceId == reservation.SportSpaceId &&
                r.Date.Date == reservation.Date.Date &&
                r.Status == "Active" &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime);

            if (spaceOverlap){
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = reservation, 
                    Message = $"The Sport Complex: {reservation.SportSpace} is already booked at that time"
                };
            }
            
            bool userOverlap = _context.reservations.Any(r =>
                r.UserId == reservation.UserId &&
                r.Date.Date == reservation.Date.Date &&
                r.Status == "Active" &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime);

            if (userOverlap){
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = reservation, 
                    Message = "The user already has a booking at that time"
                };
            }

            reservation.Status = "Active";
            reservation.CreatedAt = DateTime.Now;
            _context.reservations.Add(reservation);
            _context.SaveChanges();
            
            var user = _context.users.Find(reservation.UserId);
            var space = _context.sport_spaces.Find(reservation.SportSpaceId);
            if (user?.Email != null)
            {
                string subject = "Booking confirmation - Sport Complex";
                string body = $"Dear {user.Name},\n\n" +
                              $"Your booking has been confirmed:\n" +
                              $"Sport Complex: {space?.Name} ({space?.Type})\n" +
                              $"Date: {reservation.Date:dd/MM/yyyy}\n" +
                              $"Schedule: {reservation.StartTime:hh\\:mm} - {reservation.EndTime:hh\\:mm}\n\n" +
                              $"Thanks for using our system.\nSport Complex";
                _emailService.SendEmail(user.Email, subject, body);
            }

            return new ServiceResponse<Reservation>()
            {
                Success = true, 
                Data = reservation, 
                Message = "Booking created successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation>()
            {
                Success = false, 
                Data = reservation, 
                Message = $"Booking creation failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<Reservation> UpdateReservation(Reservation reservation)
    {
        try
        {
            var reservationDb = _context.reservations.Find(reservation.Id);
            if (reservationDb == null){
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = reservation, 
                    Message = "Booking not found"
                };
            }

            string previousStatus = reservationDb.Status ?? "";

            reservationDb.UserId = reservation.UserId;
            reservationDb.SportSpaceId = reservation.SportSpaceId;
            reservationDb.Date = reservation.Date;
            reservationDb.StartTime = reservation.StartTime;
            reservationDb.EndTime = reservation.EndTime;
            reservationDb.Status = reservation.Status;
            _context.SaveChanges();

            
            if (previousStatus != "Cancelled" && reservation.Status == "Cancelled")
            {
                var user = _context.users.Find(reservationDb.UserId);
                var space = _context.sport_spaces.Find(reservationDb.SportSpaceId);
                if (user?.Email != null)
                {
                    string subject = "Booking cancelled - Sport Complex";
                    string body = $"Dear {user.Name},\n\n" +
                                  $"Your booking was cancelled:\n" +
                                  $"Sport Complex: {space?.Name}\n" +
                                  $"Date: {reservationDb.Date:dd/MM/yyyy}\n" +
                                  $"Schedule: {reservationDb.StartTime:hh\\:mm} - {reservationDb.EndTime:hh\\:mm}\n\n" +
                                  $"Sport Complex";
                    _emailService.SendEmail(user.Email, subject, body);
                }
            }

            return new ServiceResponse<Reservation>()
            {
                Success = true, 
                Data = reservation, 
                Message = "Booking updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation>()
            {
                Success = false, 
                Data = reservation, 
                Message = $"Booking update failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<Reservation> CancelReservation(int id)
    {
        try
        {
            var reservationDb = _context.reservations.Find(id);
            if (reservationDb == null)
                return new ServiceResponse<Reservation>()
                {
                    Success = false, 
                    Data = null, 
                    Message = "Booking not found"
                };

            reservationDb.Status = "Cancelled";
            _context.SaveChanges();

            var user = _context.users.Find(reservationDb.UserId);
            var space = _context.sport_spaces.Find(reservationDb.SportSpaceId);
            if (user?.Email != null)
            {
                string subject = "Booking cancelled - Sport Complex";
                string body = $"Dear {user.Name},\n\n" +
                              $"Your booking was cancelled:\n" +
                              $"Sport Complex: {space?.Name}\n" +
                              $"Date: {reservationDb.Date:dd/MM/yyyy}\n" +
                              $"Schedule: {reservationDb.StartTime:hh\\:mm} - {reservationDb.EndTime:hh\\:mm}\n\n" +
                              $"Sport Complex";
                _emailService.SendEmail(user.Email, subject, body);
            }

            return new ServiceResponse<Reservation>()
            {
                Success = true, 
                Data = reservationDb, 
                Message = "Booking cancelled successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation>()
            {
                Success = false, 
                Data = null, 
                Message = $"Booking cancellation failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<Reservation> DeleteReservation(Reservation reservation)
    {
        try
        {
            var reservationDb = _context.reservations.Find(reservation.Id);
            if (reservationDb == null)
                return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = "Booking not found" };

            _context.reservations.Remove(reservationDb);
            _context.SaveChanges();
            return new ServiceResponse<Reservation> { Success = true, Data = reservation, Message = "Booking removed successfully" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Reservation> { Success = false, Data = reservation, Message = $"Booking deletion failed: {ex.Message}" };
        }
    }
    
    public ServiceResponse<IEnumerable<Reservation>> GetFilteredReservations(int? userId, int? spaceId)
    {
        if (userId.HasValue)
            return GetReservationsByUser(userId.Value);

        if (spaceId.HasValue)
            return GetReservationsBySpace(spaceId.Value);

        return GetAllReservations();
    }
}