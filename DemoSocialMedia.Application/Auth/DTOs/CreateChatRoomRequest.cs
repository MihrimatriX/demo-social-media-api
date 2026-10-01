namespace DemoSocialMedia.Application.Auth.DTOs;

public class CreateChatRoomRequest
{
    public string? Name { get; set; }
    public bool IsGroupChat { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
}
