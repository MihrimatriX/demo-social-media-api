using MediatR;
using DemoSocialMedia.Domain.Entities;

namespace DemoSocialMedia.Application.Auth.Queries
{
    public class GetFriendsQuery : IRequest<List<User>>
    {
        public Guid UserId { get; set; }
        public GetFriendsQuery(Guid userId)
        {
            UserId = userId;
        }
    }
} 