public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed
    public DateTime CreatedAt { get; set; }
    public string? AssignedToId { get; set; }

    public Project? Project { get; set; }
    public User? AssignedTo { get; set; }
}
