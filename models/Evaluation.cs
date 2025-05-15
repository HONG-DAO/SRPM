public class Evaluation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string EvaluatorId { get; set; } // ID người đánh giá
    public int Score { get; set; } // Điểm (ví dụ: 0–100)
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
