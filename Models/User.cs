namespace SportComplex.Models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Document { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}