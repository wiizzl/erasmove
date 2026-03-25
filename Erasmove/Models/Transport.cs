namespace Erasmove.Models;

public class Transport
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public int TransportTypeId { get; set; }

    public TransportType? TransportType { get; set; }
}