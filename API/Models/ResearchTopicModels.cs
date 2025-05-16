namespace SRPM.API.Models
{
    public class ResearchTopicDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public string? Requirements { get; set; }
        public decimal? AvailableFunding { get; set; }
        public bool IsActive { get; set; }
        public int CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateResearchTopicRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public string? Requirements { get; set; }
        public decimal? AvailableFunding { get; set; }
    }

    public class UpdateResearchTopicRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Objectives { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public string? Requirements { get; set; }
        public decimal? AvailableFunding { get; set; }
    }
}
