using DemoSocialMedia.Application.Posts.DTOs;
using MediatR;
using System.Collections.Generic;

namespace DemoSocialMedia.Application.Posts.Queries;
 
public class GetFeedQuery : IRequest<List<PostDto>>
{
} 