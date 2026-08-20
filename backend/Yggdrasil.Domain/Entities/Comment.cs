namespace Yggdrasil.Domain.Entities;

public sealed class Comment
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid AuthorId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Quiz Quiz { get; set; } = null!;
}
