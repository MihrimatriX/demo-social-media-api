using MediatR;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleSaveCommand : IRequest<bool>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ToggleSaveCommand(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
    }
} 