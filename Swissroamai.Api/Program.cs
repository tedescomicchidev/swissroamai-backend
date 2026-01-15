using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using Swissroamai.Api.Auth;
using Swissroamai.Api.Contracts;
using Swissroamai.Api.Middleware;
using Swissroamai.Application;
using Swissroamai.Application.Interfaces;
using Swissroamai.Domain.Entities;
using Swissroamai.Infrastructure;
using Swissroamai.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = ApiKeyAuthOptions.DefaultHeaderName,
        Description = "API key required for protected endpoints."
    });
});

builder.Services.AddAuthentication("ApiKey")
    .AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>("ApiKey", _ => { });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddHealthChecks();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddProviders();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapPost("/api/sessions/{sessionId:guid}/locations", async (
        Guid sessionId,
        LocationPingRequest request,
        ITravelerSessionRepository sessionRepository,
        ILocationPingDeduplicator deduplicator,
        IInsightService insightService,
        CancellationToken cancellationToken) =>
    {
        var session = await sessionRepository.GetOrCreateAsync(sessionId, request.TravelerAlias, cancellationToken);
        if (session.ExpiresAt < DateTimeOffset.UtcNow)
        {
            var refreshed = session with { ExpiresAt = DateTimeOffset.UtcNow.AddHours(4), IsActive = true };
            await sessionRepository.UpdateAsync(refreshed, cancellationToken);
        }

        var occurredAt = request.OccurredAt ?? DateTimeOffset.UtcNow;
        var dedupeKey = BuildDedupeKey(sessionId, request.Latitude, request.Longitude, occurredAt);
        var ping = new LocationPing(
            Guid.NewGuid(),
            sessionId,
            request.Latitude,
            request.Longitude,
            request.AccuracyMeters,
            occurredAt,
            DateTimeOffset.UtcNow,
            dedupeKey);

        if (deduplicator.IsDuplicate(ping))
        {
            return Results.Accepted($"Duplicate ping {ping.DedupeKey}");
        }

        var feed = await insightService.GenerateInsightsAsync(ping, cancellationToken);
        var response = new InsightFeedResponse(
            feed.SessionId,
            feed.Insights.Select(InsightResponse.FromDomain).ToList(),
            feed.GeneratedAt);

        return Results.Ok(response);
    })
    .RequireAuthorization()
    .WithName("PostLocationPing")
    .WithOpenApi();

app.MapPost("/api/sessions/{sessionId:guid}/saved-items", async (
        Guid sessionId,
        SavedItemRequest request,
        ISavedItemRepository savedItemRepository,
        CancellationToken cancellationToken) =>
    {
        var item = new SavedItem(
            Guid.NewGuid(),
            sessionId,
            request.InsightId,
            request.Note,
            DateTimeOffset.UtcNow);

        await savedItemRepository.AddAsync(item, cancellationToken);
        return Results.Created($"/api/sessions/{sessionId}/saved-items/{item.Id}", item);
    })
    .RequireAuthorization()
    .WithName("SaveItem")
    .WithOpenApi();

app.MapPost("/api/sessions/{sessionId:guid}/itinerary", async (
        Guid sessionId,
        ItineraryRequest request,
        IItineraryService itineraryService,
        CancellationToken cancellationToken) =>
    {
        var plan = await itineraryService.BuildItineraryAsync(sessionId, request.TargetDate, cancellationToken);
        return Results.Ok(ItineraryResponse.FromDomain(plan));
    })
    .RequireAuthorization()
    .WithName("BuildItinerary")
    .WithOpenApi();

app.Run();

static string BuildDedupeKey(Guid sessionId, double latitude, double longitude, DateTimeOffset occurredAt)
{
    var roundedLat = Math.Round(latitude, 4);
    var roundedLon = Math.Round(longitude, 4);
    var bucket = occurredAt.ToUnixTimeSeconds() / 30;
    return $"{sessionId:N}:{roundedLat:F4}:{roundedLon:F4}:{bucket}";
}
