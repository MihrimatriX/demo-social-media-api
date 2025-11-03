using System;

namespace DemoSocialMedia.Domain.Entities;

public class Save
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
} 