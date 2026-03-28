using Microsoft.AspNetCore.Mvc;

namespace MetricsDemoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult getById(int id)
    {
        return Ok(new
        {
            OderId = id,
            Status = "Created",
            CreatedAtUtc = DateTime.UtcNow
        });
    }
}
