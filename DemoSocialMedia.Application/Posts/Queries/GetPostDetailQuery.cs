using DemoSocialMedia.Application.Posts.DTOs;
using MediatR;
using System;

namespace DemoSocialMedia.Application.Posts.Queries;

public class GetPostDetailQuery : IRequest<PostDto>
{
    public Guid PostId { get; set; }
    public Guid? CurrentUserId { get; set; } // Beğendi/kaydetti mi için
    public GetPostDetailQuery(Guid postId, Guid? currentUserId)
    {
        PostId = postId;
        CurrentUserId = currentUserId;
    }
} 