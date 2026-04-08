using DemoSocialMedia.Application.Posts.DTOs;
using MediatR;

namespace DemoSocialMedia.Application.Posts.Commands;

public class CreatePostCommand : IRequest<PostDto>
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public CreatePostCommand(Guid userId, string content, string? imageUrl)
    {
        UserId = userId;
        Content = content;
        ImageUrl = imageUrl;
    }
} 