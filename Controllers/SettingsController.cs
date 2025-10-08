using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace magazynek.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("update")]
    public IActionResult UpdateSetting()
    {
        return Ok("Tylko admin może to zmienić");
    }

    [Authorize(Roles = "PremiumUser,Admin")]
    [HttpGet("premium")]
    public IActionResult GetPremiumFeature()
    {
        return Ok("Tylko premium i admin");
    }

    [Authorize]
    [HttpGet("basic")]
    public IActionResult GetBasic()
    {
        return Ok("Każdy zalogowany");
    }
}
}