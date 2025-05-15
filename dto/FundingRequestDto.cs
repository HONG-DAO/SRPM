public class FundingRequestDto
{
    public Guid ProjectId { get; set; }
    public string RequestedById { get; set; }
    public decimal Amount { get; set; }
    public string Purpose { get; set; }
}
