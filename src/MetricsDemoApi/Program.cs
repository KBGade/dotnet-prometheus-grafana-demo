using MetricsDemoApi.HealthChecks;
using MetricsDemoApi.Middleware;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using Serilog.Events;
using MetricsDemoApi.Services;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "Demo.Observability.Api";
var serviceVersion = "1.0.0";

// ---------------------------
// Serilog Configuration
// ---------------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ---------------------------
// Services
// ---------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DemoStore>();

builder.Services.AddHealthChecks()
    .AddCheck<DemoHealthCheck>("demo_api");

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(
        serviceName: serviceName,
        serviceVersion: serviceVersion))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation());

// ---------------------------
// Prometheus Custom Metrics
// ---------------------------
var ordersCreatedCounter = Metrics.CreateCounter(
    "orders_created_total",
    "Total number of orders created");

var failedOrdersCounter = Metrics.CreateCounter(
    "orders_failed_total",
    "Total number of failed orders");

var orderProcessingDuration = Metrics.CreateHistogram(
    "order_processing_duration_seconds",
    "Time spent processing order requests in seconds");

// ---------------------------
// Build app
// ---------------------------
var app = builder.Build();

// ---------------------------
// Middleware Pipeline
// ---------------------------
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        if (httpContext.Items.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            diagnosticContext.Set("CorrelationId", correlationId);
        }

        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpMetrics();

// ---------------------------
// Health Endpoints
// ---------------------------
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

// ---------------------------
// Demo Endpoints
// ---------------------------
app.MapGet("/", () => Results.Ok(new
{
    Message = "Metrics Demo API is running",
    MetricsEndpoint = "/metrics",
    Swagger = "/swagger",
    HealthEndpoint = "/health",
    ReadyEndpoint = "/ready"
}));

app.MapGet("/demo-log", (ILogger<Program> logger) =>
{
    logger.LogInformation("Demo log endpoint hit at {Time}", DateTime.UtcNow);
    return Results.Ok(new { message = "Log generated" });
});

app.MapGet("/correlation-test", (HttpContext context, ILogger<Program> logger) =>
{
    var correlationId = context.Items["X-Correlation-ID"]?.ToString();

    logger.LogInformation(
        "Correlation test endpoint called with CorrelationId {CorrelationId}",
        correlationId);

    return Results.Ok(new
    {
        message = "Correlation working",
        correlationId
    });
});

app.MapGet("/external-call", async (IHttpClientFactory factory, ILogger<Program> logger) =>
{
    var client = factory.CreateClient();
    var response = await client.GetAsync("https://jsonplaceholder.typicode.com/todos/1");

    logger.LogInformation(
        "External API call completed with status code {StatusCode}",
        response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json");
});

app.MapGet("/fail", (ILogger<Program> logger) =>
{
    logger.LogError("Intentional failure endpoint triggered");
    throw new Exception("Demo exception for observability");
});

app.MapPost("/orders", () =>
{
    using (orderProcessingDuration.NewTimer())
    {
        Thread.Sleep(100); // demo only
        ordersCreatedCounter.Inc();

        return Results.Ok(new
        {
            message = "Order created successfully",
            createdAtUtc = DateTime.UtcNow
        });
    }
});

app.MapPost("/orders/fail", () =>
{
    failedOrdersCounter.Inc();
    return Results.Problem("Order failed", statusCode: 500);
});

app.Map("/error", () =>
{
    return Results.Problem(
        title: "An unexpected error occurred",
        statusCode: 500);
});

app.MapControllers();
app.MapMetrics();

app.Run();