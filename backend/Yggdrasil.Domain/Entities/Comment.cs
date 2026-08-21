namespace Yggdrasil.Domain.Entities;

public sealed class Comment
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid AuthorId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; set; }

    public Quiz Quiz { get; set; } = null!;
}
