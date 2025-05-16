namespace SRPM.API.Models
{
    public class FundingRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string? Purpose { get; set; }
        public string? JustificationDocumentUrl { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public int RequestedById { get; set; }
        public string? RequestedByName { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateFundingRequestRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Purpose { get; set; }
        public string? JustificationDocumentUrl { get; set; }
        public int ProjectId { get; set; }
    }

    public class UpdateFundingRequestRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Purpose { get; set; }
        public string? JustificationDocumentUrl { get; set; }
    }

    public class ApproveFundingRequestRequest
    {
        // Additional properties can be added if needed
    }

    public class RejectFundingRequestRequest
    {
        public string Reason { get; set; }
    }
}
