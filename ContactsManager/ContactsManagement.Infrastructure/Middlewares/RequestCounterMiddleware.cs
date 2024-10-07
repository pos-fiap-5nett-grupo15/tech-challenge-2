﻿using Azure.Core;
using Microsoft.AspNetCore.Http;
using Prometheus;
using System.Diagnostics;
using System.Net;

namespace ContactsManagement.Infrastructure.Middlewares
{
    public class RequestCounterMiddleware
    {
        private readonly RequestDelegate _next;
        //private static readonly Counter RequestCounter = Prometheus.Metrics.CreateCounter("http_requests_total",
        //                                                                                  "Total number of HTTP requests",
        //                                                                                  new { "path", "duration_in_seconds" });
        private static readonly Histogram RequestAnalyzerInfo = Metrics.CreateHistogram(
            "http_requests_info_total",
            "Duração das requisições HTTP em segundos",
            new HistogramConfiguration
            {
                LabelNames = new[] { "path", "request_status" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 10)
            }
        );

        public RequestCounterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestTime = Stopwatch.StartNew();
            var requestPath = context.Request.Path.ToString() ?? "unknown";

            await _next(context);

            if (!requestPath.Contains("metric", StringComparison.InvariantCultureIgnoreCase))
            {
                var responseStatus = ((HttpStatusCode)context.Response.StatusCode).ToString() ?? "unknown";

                requestTime.Stop();
                RequestAnalyzerInfo
                    .WithLabels(requestPath.ToLower().Trim(),
                                responseStatus.ToLower().Trim())
                    .Observe(requestTime.Elapsed.TotalSeconds);
            }
        }
    }
}
