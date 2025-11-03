using MediatR;
using DemoSocialMedia.Domain.Entities;

namespace DemoSocialMedia.Application.Auth.Queries
{
    public class GetFriendRequestsQuery : IRequest<List<FriendRequest>>
    {
        public Guid UserId { get; set; }
        public bool Incoming { get; set; } // true: bana gelen, false: benim gönderdiklerim
        public GetFriendRequestsQuery(Guid userId, bool incoming)
        {
            UserId = userId;
            Incoming = incoming;
        }
    }
} 