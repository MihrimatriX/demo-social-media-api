using Microsoft.AspNetCore.Mvc;
using DemoSocialMedia.Application.Map.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DemoSocialMedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class MapController : ControllerBase
{
    [HttpGet("grid")]
    public IActionResult GetGrid()
    {
        var width = 50;
        var height = 30;
        var cells = new List<GridCellDto>
        {
            new() { X = 2, Y = 3, ImageUrl = "/images/robot.png", Mahalle = "Mor Mahalle 1" },
            new() { X = 3, Y = 3, ImageUrl = "/images/indie.png", Mahalle = "Mor Mahalle 1" },
            new() { X = 10, Y = 5, ImageUrl = "/images/gamepad.png", Mahalle = "Joystick Caddesi 8" },
            new() { X = 20, Y = 10, ImageUrl = "/images/valley.png", Mahalle = "Valley Park 51" },
            new() { X = 25, Y = 12, ImageUrl = "/images/retro.png", Mahalle = "Retro Sokak 44" },
            // ... daha fazla dummy hücre eklenebilir
        };
        return Ok(new { width, height, cells });
    }
} 