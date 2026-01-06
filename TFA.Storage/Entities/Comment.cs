using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFA.Storage.Entities;

public class Comment
{
    [Key]
    public Guid CommentId { get; set; }

    public required string Text { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public required User Author { get; set; }

    public Guid TopicId { get; set; }

    [ForeignKey(nameof(TopicId))]
    public required Topic Topic { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}