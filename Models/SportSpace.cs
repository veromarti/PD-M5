namespace SportComplex.Models;

public class SportSpace
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int Capacity { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}