using MediatR;
using DemoSocialMedia.Application.Posts.DTOs;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class CreateCommentCommand : IRequest<CommentDto>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public CreateCommentCommand(Guid postId, Guid userId, string content)
    {
        PostId = postId;
        UserId = userId;
        Content = content;
    }
} 