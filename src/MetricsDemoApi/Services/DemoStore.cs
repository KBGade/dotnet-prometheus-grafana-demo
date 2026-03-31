using MetricsDemoApi.Models;

namespace MetricsDemoApi.Services;

public class DemoStore
{
    public List<Product> Products { get; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 75000, Stock = 10 },
        new Product { Id = 2, Name = "Keyboard", Price = 2500, Stock = 25 },
        new Product { Id = 3, Name = "Mouse", Price = 1200, Stock = 30 }
    ];

    public Dictionary<string, Cart> Carts { get; } = new();
    public List<Order> Orders { get; } = new();
}