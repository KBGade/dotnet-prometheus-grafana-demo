using MetricsDemoApi.Dtos;
using MetricsDemoApi.Models;
using MetricsDemoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace MetricsDemoApi.Controllers;

[ApiController]
[Route("checkout")]
public class CheckoutController : ControllerBase
{
    private readonly DemoStore _store;

    private static readonly Counter CheckoutSuccessCounter = Metrics.CreateCounter(
        "checkout_success_total",
        "Total successful checkouts");

    private static readonly Counter CheckoutFailureCounter = Metrics.CreateCounter(
        "checkout_failure_total",
        "Total failed checkouts");

    public CheckoutController(DemoStore store)
    {
        _store = store;
    }

    [HttpPost]
    public IActionResult Checkout([FromBody] CheckoutRequest request)
    {
        if (!_store.Carts.TryGetValue(request.UserId, out var cart) || cart.Items.Count == 0)
        {
            CheckoutFailureCounter.Inc();
            return BadRequest(new { message = "Cart is empty" });
        }

        var paymentSucceeded = Random.Shared.Next(1, 101) <= 80;

        if (!paymentSucceeded)
        {
            CheckoutFailureCounter.Inc();
            return StatusCode(500, new
            {
                message = "Payment failed",
                userId = request.UserId
            });
        }

        var order = new Order
        {
            UserId = request.UserId,
            Items = cart.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            TotalAmount = cart.TotalAmount,
            Status = "Paid"
        };

        _store.Orders.Add(order);
        _store.Carts[request.UserId] = new Cart { UserId = request.UserId };

        CheckoutSuccessCounter.Inc();

        return Ok(order);
    }

    [HttpGet("orders/{orderId:guid}")]
    public IActionResult GetOrder(Guid orderId)
    {
        var order = _store.Orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order is null)
        {
            return NotFound(new { message = "Order not found" });
        }

        return Ok(order);
    }
}