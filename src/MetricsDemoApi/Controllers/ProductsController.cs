using MetricsDemoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MetricsDemoApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly DemoStore _store;

    public ProductsController(DemoStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        return Ok(_store.Products);
    }
}