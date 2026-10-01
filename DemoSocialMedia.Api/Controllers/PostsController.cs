using DemoSocialMedia.Api.Extensions;
using DemoSocialMedia.Application.Posts.Commands;
using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Application.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[Authorize]
public class PostsController : BaseController
{
    private readonly IMediator _mediator;
    public PostsController(IMediator mediator) => _mediator = mediator;

    // Feed ve detay anonim okunabilir; oturum varsa IsLiked/IsSaved doldurulur.
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetFeed()
        => await _mediator.Send(new GetFeedQuery(User.GetUserId()));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostDto>> GetPostDetail(Guid id)
    {
        var result = await _mediator.Send(new GetPostDetailQuery(id, User.GetUserId()));
        return result == null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost(CreatePostRequest request)
        => await _mediator.Send(new CreatePostCommand(CurrentUserId, request.Content, request.ImageUrl));

    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid id, CreateCommentRequest request)
        => await _mediator.Send(new CreateCommentCommand(id, CurrentUserId, request.Content));

    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> ToggleLike(Guid id)
        => Ok(new { liked = await _mediator.Send(new ToggleLikeCommand(id, CurrentUserId)) });

    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> ToggleSave(Guid id)
        => Ok(new { saved = await _mediator.Send(new ToggleSaveCommand(id, CurrentUserId)) });
}
