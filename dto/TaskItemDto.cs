public class TaskItemDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AssignedToId { get; set; }
    public DateTime DueDate { get; set; }
}
