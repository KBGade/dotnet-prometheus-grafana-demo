namespace MetricsDemoApi.Models;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Created";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}