namespace SportComplex.Models;

public class Reservation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int SportSpaceId { get; set; }
    public SportSpace? SportSpace { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime? CreatedAt { get; set; }
}