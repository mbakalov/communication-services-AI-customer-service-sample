// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

/*
* Note: This sample app supports only one customer conversation at any given time.
* MemCache is used to keep the active state (customer identity, access-token, threadId, voice callId etc).
* Active state is reset via /debug API or expires after 1h
*/

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddBackendServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build a resource configuration action to set service information.
Action<ResourceBuilder> configureResource = r => r.AddService(
    serviceName: "my-contact-center",
    serviceVersion: "0.1",
    serviceInstanceId: Environment.MachineName);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(configureResource)
    .WithTracing(tracing =>
    {
        tracing.AddSource("MyActivitySource");
        tracing.SetSampler(new AlwaysOnSampler());
        tracing.AddOtlpExporter();
        tracing.AddConsoleExporter();
    });

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "ACS Mechanics");
});

app.UseCors(option =>
{
    option.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();