using Prometheus;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checkpoint
builder.Services.AddHealthChecks();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Built-in HTTP metrics for ASP.NET Core
app.UseHttpMetrics();

// Custom business metrics
var ordersCreatedCounter = Metrics.CreateCounter(
    "orders_created_total",
    "Total number of orders created");

var orderProcessingDuration = Metrics.CreateHistogram(
    "order_processing_duration_seconds",
    "Time spent processing order requests in seconds");

// Customer failurer metric
var failedOrdersCounter = Metrics.CreateCounter(
    "orders_failed_total",
    "Total number of failed orders");

// Sample GET endpoint
app.MapGet("/", () => Results.Ok(new
{
    Message = "Metrics Demo API is running",
    MetricsEndpoint = "/metrics",
    Swagger = "/swagger",
    HealthEndpoint = "/health"
}));

// Sample POST endpoint with custom metrics
app.MapPost("/orders", () =>
{
    using (orderProcessingDuration.NewTimer())
    {
        Thread.Sleep(100); // demo only
        ordersCreatedCounter.Inc();

        return Results.Ok(new
        {
            Message = "Order created successfully",
            CreatedAtUtc = DateTime.UtcNow
        });
    }
});

// Sample POST endpoint with failed order metrics
app.MapPost("/orders/fail", () =>
{
    failedOrdersCounter.Inc();
    return Results.Problem("Order failed", statusCode: 500);
});


// Optional controller support
app.MapControllers();

// Expose Prometheus endpoint
app.MapMetrics();

// Health Checks
app.MapHealthChecks("/health");

app.Run();