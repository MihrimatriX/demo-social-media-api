using System;
using System.Collections.Generic;

namespace DemoSocialMedia.Application.Posts.DTOs;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public int SaveCount { get; set; }
    public int CommentCount { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public bool IsLiked { get; set; }
    public bool IsSaved { get; set; }
} 