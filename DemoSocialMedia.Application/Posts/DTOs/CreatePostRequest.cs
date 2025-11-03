namespace DemoSocialMedia.Application.Posts.DTOs;
 
public class CreatePostRequest
{
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
} 