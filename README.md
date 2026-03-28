# .NET 8 + Prometheus + Grafana Demo

This project demonstrates how to expose Prometheus metrics from an ASP.NET Core Web API and visualize them in Grafana.

## Tech Stack

- .NET 8 Web API
- prometheus-net.AspNetCore
- Prometheus
- Grafana

## Features

- `/metrics` endpoint exposed from ASP.NET Core
- Built-in HTTP request metrics
- Custom business metrics:
  - `orders_created_total`
  - `order_processing_duration_seconds`

## Project Structure

```text
dotnet-prometheus-grafana-demo/
├─ src/
│  └─ MetricsDemoApi/
├─ monitoring/
│  ├─ prometheus/
│  │  └─ prometheus.yml
│  └─ grafana/
├─ README.md
└─ setup-commands.txt