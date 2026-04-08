using DemoSocialMedia.Application.Posts.Commands;
using DemoSocialMedia.Application.Posts.DTOs;
using DemoSocialMedia.Application.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PostsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPostDetail(Guid id)
    {
        Guid? userId = null;
        if (HttpContext.Items["UserId"] is Guid uid)
            userId = uid;
        var result = await _mediator.Send(new GetPostDetailQuery(id, userId));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("{id}/comments")]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid id, [FromBody] CreateCommentRequest request)
    {
        if (HttpContext.Items["UserId"] is not Guid userId)
            return Unauthorized("Kullanıcı kimliği bulunamadı.");
        var result = await _mediator.Send(new CreateCommentCommand(id, userId, request.Content));
        return Ok(result);
    }

    [HttpPost("{id}/like")]
    public async Task<ActionResult> ToggleLike(Guid id)
    {
        if (HttpContext.Items["UserId"] is not Guid userId)
            return Unauthorized("Kullanıcı kimliği bulunamadı.");
        var liked = await _mediator.Send(new ToggleLikeCommand(id, userId));
        return Ok(new { liked });
    }

    [HttpPost("{id}/save")]
    public async Task<ActionResult> ToggleSave(Guid id)
    {
        if (HttpContext.Items["UserId"] is not Guid userId)
            return Unauthorized("Kullanıcı kimliği bulunamadı.");
        var saved = await _mediator.Send(new ToggleSaveCommand(id, userId));
        return Ok(new { saved });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PostDto>>> GetFeed()
    {
        var result = await _mediator.Send(new GetFeedQuery());
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostRequest request)
    {
        if (HttpContext.Items["UserId"] is not Guid userId)
            return Unauthorized("Kullanıcı kimliği bulunamadı.");
        var result = await _mediator.Send(new CreatePostCommand(userId, request.Content, request.ImageUrl));
        return Ok(result);
    }
} 