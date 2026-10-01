using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[Authorize]
public class FriendsController : BaseController
{
    private readonly IMediator _mediator;
    public FriendsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("requests")]
    public async Task<IActionResult> SendFriendRequest(SendFriendRequestRequest request)
    {
        await _mediator.Send(new SendFriendRequestCommand(CurrentUserId, request.ReceiverId));
        return Ok();
    }

    [HttpPut("requests/{requestId:guid}/accept")]
    public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
    {
        await _mediator.Send(new AcceptFriendRequestCommand(requestId, CurrentUserId));
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<UserSummaryDto>>> GetFriends()
        => await _mediator.Send(new GetFriendsQuery(CurrentUserId));

    // incoming=true: bana gelenler, false: benim gönderdiklerim (sadece Pending)
    [HttpGet("requests")]
    public async Task<IActionResult> GetFriendRequests([FromQuery] bool incoming = true)
        => Ok(await _mediator.Send(new GetFriendRequestsQuery(CurrentUserId, incoming)));
}
