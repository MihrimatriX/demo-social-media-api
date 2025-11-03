using MediatR;
using DemoSocialMedia.Domain.Entities;

namespace DemoSocialMedia.Application.Auth.Queries
{
    public class GetMessagesQuery : IRequest<List<Message>>
    {
        public Guid ChatRoomId { get; set; }
        public GetMessagesQuery(Guid chatRoomId)
        {
            ChatRoomId = chatRoomId;
        }
    }
} 