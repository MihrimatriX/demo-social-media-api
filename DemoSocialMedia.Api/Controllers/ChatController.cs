using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.DTOs;
using DemoSocialMedia.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DemoSocialMedia.Api.Controllers;

[Authorize]
public class ChatController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IHubContext<ChatHub> _hub;
    public ChatController(IMediator mediator, IHubContext<ChatHub> hub)
    {
        _mediator = mediator;
        _hub = hub;
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<Guid>> CreateRoom(CreateChatRoomRequest request)
        => await _mediator.Send(new CreateChatRoomCommand(request.Name, request.IsGroupChat, request.MemberIds, CurrentUserId));

    [HttpGet("rooms/{roomId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid roomId)
        => Ok(await _mediator.Send(new GetMessagesQuery(roomId, CurrentUserId)));

    [HttpPost("rooms/{roomId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid roomId, SendMessageRequest request)
    {
        var message = await _mediator.Send(new SendMessageCommand(roomId, CurrentUserId, request.Content));
        await _hub.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", message);
        return Ok(message);
    }
}
