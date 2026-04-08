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
    public class FriendsController : BaseController
    {
        private readonly IMediator _mediator;
        public FriendsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("requests")]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestRequest request)
        {
            if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
            var command = new SendFriendRequestCommand(UserId.Value, request.ReceiverId);
            var result = await _mediator.Send(command);
            if (!result) return BadRequest(new { message = "İstek zaten mevcut veya gönderilemedi." });
            return Ok();
        }

        [HttpPut("requests/{requestId}/accept")]
        public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
        {
            if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
            var command = new AcceptFriendRequestCommand(requestId, UserId.Value);
            var result = await _mediator.Send(command);
            if (!result) return BadRequest(new { message = "İstek bulunamadı veya zaten kabul edilmiş." });
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
            var query = new GetFriendsQuery(UserId.Value);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequests([FromQuery] bool incoming = true)
        {
            if (UserId == null) return Unauthorized(new { message = "Oturum bulunamadı." });
            var query = new GetFriendRequestsQuery(UserId.Value, incoming);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 