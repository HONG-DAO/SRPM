public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty; // ID của nhà nghiên cứu chính
    public string Status { get; set; } = "Draft"; // Draft | InProgress | Completed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
