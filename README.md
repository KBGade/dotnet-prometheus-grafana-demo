# .NET 8 Observability Demo using Prometheus & Grafana

This project demonstrates a complete end-to-end observability setup for an ASP.NET Core (.NET 8) Web API using Prometheus and Grafana.

It shows how to expose application metrics, monitor system behavior, and visualize performance using real-world observability patterns.

---

## 🚀 Key Features

- .NET 8 ASP.NET Core Web API
- Prometheus metrics exposed via `/metrics`
- Health check endpoint `/health`
- Custom business metrics:
  - `orders_created_total`
  - `orders_failed_total`
  - `order_processing_duration_seconds`
- Built-in HTTP metrics (request count, latency)
- Prometheus scraping configuration
- Grafana dashboard with:
  - Request rate
  - Total requests
  - Average latency
  - Error rate
  - Business metrics visualization
- No Docker required (runs on low-memory setup)

---

## 🧠 Architecture
Client
↓
ASP.NET Core API
↓
/metrics endpoint
↓
Prometheus (scrapes metrics)
↓
Grafana (visualizes dashboards)


### Flow

1. API exposes metrics via `/metrics`
2. Prometheus scrapes metrics at regular intervals
3. Grafana queries Prometheus
4. Dashboards visualize system performance

---

## 📁 Project Structure

```
dotnet-prometheus-grafana-demo/
│
├── src/
│ └── MetricsDemoApi/
│
├── monitoring/
│ └── prometheus/
│ └── prometheus.yml
│
├── docs/
│ └── screenshots/
│ ├── grafana-dashboard.png
│ ├── prometheus-targets.png
│ ├── metrics-endpoint.png
│ ├── app-health-check.png
│ └── prometheus-query.png
│
├── README.md
└── setup-commands.txt
```

## 🔌 API Endpoints

| Endpoint | Description |
|----------|------------|
| `/` | API info |
| `/orders` | Create order (success metric) |
| `/orders/fail` | Simulate failure |
| `/api/Order/{id}` | Sample controller endpoint |
| `/metrics` | Prometheus metrics |
| `/health` | Health check |

---

## 📊 Metrics

### Custom Metrics

- `orders_created_total`
- `orders_failed_total`
- `order_processing_duration_seconds`

### HTTP Metrics

- `http_requests_received_total`
- `http_request_duration_seconds_sum`
- `http_request_duration_seconds_count`

---

## 📈 Grafana Dashboard Queries

### Orders Created
```promql
sum(orders_created_total)
```

### Failed Orders
```promql
sum(orders_failed_total)
```

### Request Rate
```promql
sum(rate(http_requests_received_total[5m]))
```

### Total Requests
```promql
sum(http_requests_received_total)
```

### Average Latency
```promql
sum(rate(http_request_duration_seconds_sum[5m])) / sum(rate(http_request_duration_seconds_count[5m]))
```

### Error Rate
```promql
sum(rate(orders_failed_total[5m])) / sum(rate(http_requests_received_total[5m]))
```
---

⚙️ Setup & Run
1. Run API
cd src/MetricsDemoApi
dotnet restore
dotnet run

Application runs on:

http://localhost:5204

2. Run Prometheus
prometheus.exe --config.file=prometheus.yml

Prometheus UI:

http://localhost:9090

3. Run Grafana
http://localhost:3000

Add Prometheus data source:

http://localhost:9090

---

🔄 Generate Sample Traffic

PowerShell:

Invoke-WebRequest http://localhost:5204/
Invoke-WebRequest http://localhost:5204/api/Order/1
Invoke-WebRequest -Method Post http://localhost:5204/orders
Invoke-WebRequest -Method Post http://localhost:5204/orders/fail

📸 Screenshots
Grafana Dashboard

Prometheus Targets

Metrics Endpoint

Health Check

Prometheus Query

🎯 What This Demonstrates
Real-world observability in .NET applications
Metrics collection using Prometheus
Dashboard visualization using Grafana
Monitoring request rate, latency, and failures
Custom business metrics integration
💡 Interview Talking Points
Difference between /metrics and /health
Prometheus pull-based model
Rate vs total metrics
Latency calculation using histogram
Error rate derivation
Scaling observability in cloud environments (AWS ECS/EKS)
🚀 Future Enhancements
Structured logging with Serilog
ELK stack integration
Distributed tracing with OpenTelemetry
AWS deployment (EC2/ECS)
Grafana alerting setup
⭐ Summary

This project demonstrates a production-style observability pipeline using .NET, Prometheus, and Grafana — ideal for backend, cloud, and system design discussions.