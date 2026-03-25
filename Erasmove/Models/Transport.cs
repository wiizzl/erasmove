namespace Erasmove.Models;

public class Transport
{
    public int Id { get; set; }
    public int TransportTypeId { get; set; }
    public string Company { get; set; }
    public string Reference { get; set; }

    public TransportType TransportType { get; set; }
}