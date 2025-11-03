namespace DemoSocialMedia.Application.Auth.DTOs
{
    public class SendMessageRequest
    {
        public Guid ChatRoomId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
} 