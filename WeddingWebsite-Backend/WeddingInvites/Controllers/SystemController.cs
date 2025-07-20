using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WeddingInvites.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/system")]
public class SystemController : ControllerBase
{
    private readonly ILogger<SystemController> _logger;

    public SystemController(ILogger<SystemController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> HealthCheck()
    {
        _logger.LogInformation("Health check");
        
        return Ok("Healthy");
    }
}