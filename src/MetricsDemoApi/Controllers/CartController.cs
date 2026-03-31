using MetricsDemoApi.Dtos;
using MetricsDemoApi.Models;
using MetricsDemoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace MetricsDemoApi.Controllers;

[ApiController]
[Route("cart")]
public class CartController : ControllerBase
{
    private readonly DemoStore _store;

    private static readonly Counter CartAddCounter = Metrics.CreateCounter(
        "cart_items_added_total",
        "Total number of items added to cart");

    public CartController(DemoStore store)
    {
        _store = store;
    }

    [HttpGet("{userId}")]
    public IActionResult GetCart(string userId)
    {
        if (_store.Carts.TryGetValue(userId, out var cart))
        {
            return Ok(cart);
        }

        return Ok(new Cart { UserId = userId });
    }

    [HttpPost("add")]
    public IActionResult AddToCart([FromBody] AddToCartRequest request)
    {
        var product = _store.Products.FirstOrDefault(p => p.Id == request.ProductId);
        if (product is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        if (request.Quantity <= 0)
        {
            return BadRequest(new { message = "Quantity must be greater than zero" });
        }

        if (product.Stock < request.Quantity)
        {
            return BadRequest(new { message = "Insufficient stock" });
        }

        if (!_store.Carts.TryGetValue(request.UserId, out var cart))
        {
            cart = new Cart { UserId = request.UserId };
            _store.Carts[request.UserId] = cart;
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existingItem is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            });
        }
        else
        {
            existingItem.Quantity += request.Quantity;
        }

        CartAddCounter.Inc();

        return Ok(cart);
    }
}