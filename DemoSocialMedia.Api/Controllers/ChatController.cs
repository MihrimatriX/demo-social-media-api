using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.Queries;
using DemoSocialMedia.Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IMediator _mediator;
        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateChatRoomCommand command)
        {
            if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
            command.UserId = UserId.Value;
            var roomId = await _mediator.Send(command);
            return Ok(roomId);
        }

        [HttpGet("rooms/{roomId}/messages")]
        public async Task<IActionResult> GetMessages(Guid roomId)
        {
            var query = new GetMessagesQuery(roomId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("rooms/{roomId}/messages")]
        public async Task<IActionResult> SendMessage(Guid roomId, [FromBody] SendMessageRequest request)
        {
            if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
            var command = new SendMessageCommand(roomId, UserId.Value, request.Content);
            var result = await _mediator.Send(command);
            if (!result) return BadRequest(new { message = "Mesaj gönderilemedi." });
            return Ok();
        }
    }
} 