public class User
{
    public string Id { get; set; } // Firebase UserId hoặc GUID
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Role { get; set; } = "Researcher"; // Researcher, PI, Reviewer, Staff, Admin
    public string SocialLinks { get; set; } = string.Empty; // JSON hoặc chuỗi link phân cách
}
