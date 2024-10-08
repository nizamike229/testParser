namespace WebApplication1.Models;

public class News
{
    public required string Title { get; init; }
    public required string Content { get; init; }
    public required DateTime PublishDate { get; init; }
}