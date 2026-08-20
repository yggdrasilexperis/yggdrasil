using Yggdrasil.Domain.Enums;

namespace Yggdrasil.Domain.Entities;

public sealed class Quiz
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Difficulty Difficulty { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
