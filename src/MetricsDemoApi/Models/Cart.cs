namespace MetricsDemoApi.Models;

public class Cart
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();

    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
}