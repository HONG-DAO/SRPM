public class FundingRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string RequestedById { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending | Approved | Rejected
    public string? ReviewNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
