using MediatR;
using System;

namespace DemoSocialMedia.Application.Posts.Commands;

public class ToggleLikeCommand : IRequest<bool>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ToggleLikeCommand(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
    }
} 