# Swissroamai MVP Backend

## 1) Architecture proposal (MVP) with Azure services

### Overview
Swissroamai ingests opt-in traveler location pings, enriches them with local context using a multi-agent pipeline, and returns a ranked insight feed with freshness and source attribution. Saved items can later power itinerary suggestions.

### Azure services (MVP)
| Capability | Azure service | Why it fits MVP |
| --- | --- | --- |
| **Ingestion API** | **ASP.NET Core Minimal APIs** on Azure App Service (Linux) | Fast iteration, straightforward hosting, and built-in middleware for auth, rate limiting, and observability. |
| **Processing pipeline** | **Azure Functions (isolated)** or **BackgroundService** workers hosted in App Service | For MVP, background workers in the same App Service keep ops simple. If workload grows, move the pipeline to Functions with Service Bus triggers. |
| **Retrieval layer** | **Providers** project with mocked providers now; later swap to Azure Maps, Event Grid, third-party APIs | Keeps integrations pluggable and testable. |
| **Itinerary generation** | Background worker or provider-based agent | Keeps generation asynchronous and scalable. |
| **Storage** | **Azure Cosmos DB** (session-centric, low-latency, JSON documents) | Best fit for geo/location + session-based storage with flexible schema. |
| **Caching** | **Azure Cache for Redis** | Cache geo context + hot insights; reduce external calls. |
| **Message/eventing** | **Azure Service Bus** (queues/topics) | Reliable ordering, retries, dead-lettering, and pub/sub. Storage Queue is cheaper but thinner; Event Grid is great for fan-out but not ideal for ordered per-session processing. |
| **Identity** | **Entra ID** for internal services; **API keys** for client MVP | Entra for operator APIs and Service-to-Service; API keys for client simplicity. |
| **Secrets** | **Azure Key Vault** | Securely store API keys, provider secrets, and connection strings. |
| **Rate limiting** | ASP.NET Core Rate Limiting middleware | Quick MVP-level protection with path-based rules. |

### Azure-first deployment plan
1. **App Service** runs the API + background worker(s) for MVP.
2. **Cosmos DB** for sessions, insights, and saved items.
3. **Service Bus** queues for location pings and insight generation events once processing decouples from request path.
4. **Key Vault** for secrets, with managed identity for App Service.
5. **Azure Monitor / Application Insights** for structured logs, traces, and metrics.

### Functions vs Minimal APIs + background workers
**Recommendation: ASP.NET Core minimal APIs + background workers (MVP).**
- **Pros**: Lower cognitive overhead, fewer moving parts, straightforward debugging, easiest to run locally.
- **Cons**: Not as elastic as Functions.

Once ingestion volume grows, migrate asynchronous processing to **Azure Functions** using **Service Bus triggers** to keep ingestion lean and scale-out heavy lifting.

---

## 2) Domain model + core flows

### Entities
- **TravelerSession**: session-based tracking (opt-in) with TTL + active flag.
- **LocationPing**: location + accuracy + timestamp + dedupe key.
- **Insight**: ranked insight with freshness and source attribution.
- **SourceAttribution**: provider name, URL, retrieval timestamp, confidence.
- **SavedItem**: user saved insight.
- **ItineraryPlan**: suggested plan for another day.

### Commands/events
- **LocationReceived**: emitted when a location ping arrives.
- **InsightsComputed**: emitted when insights are generated.
- **ItemSaved**: emitted when a user saves an insight.
- **ItineraryRequested**: emitted when itinerary generation is requested.

### Idempotency + dedup strategy
- **Session + time bucket + rounded lat/lon** makes a deterministic dedupe key.
- **In-memory dedupe** for MVP; replace with distributed cache (Redis) later.
- **Idempotency window** (ex: 30 seconds) prevents replay storms from mobile clients.

---

## 3) Repo scaffolding

Projects:
- **Swissroamai.Api**: minimal API, auth, rate limiting, health checks, OpenAPI.
- **Swissroamai.Application**: application services, agent contracts, core flows.
- **Swissroamai.Domain**: entities + domain events.
- **Swissroamai.Infrastructure**: persistence, idempotency, storage adapters (in-memory for now).
- **Swissroamai.Providers**: mock providers and deterministic agents.
- **Swissroamai.Tests**: testing (xUnit).

### Local development
```bash
# Run the API
cd Swissroamai.Api
DOTNET_ENVIRONMENT=Development dotnet run
```

Request example:
```bash
curl -X POST \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: local-dev-key" \
  http://localhost:5033/api/sessions/11111111-1111-1111-1111-111111111111/locations \
  -d '{"latitude":47.37,"longitude":8.54,"accuracyMeters":12.5,"travelerAlias":"demo"}'
```

### Docker
```bash
docker compose up --build
```

---

## 4) Multi-agent orchestration approach

### Agents
1. **GeoContextAgent**: reverse geocode + region.
2. **EventsRetrievalAgent**: events and festivals.
3. **LocalFactsAgent**: POIs, tips, culture.
4. **RankingAgent**: scoring + freshness labeling.
5. **ItineraryAgent**: create day plan from saved items.

### Agent interface pattern
Each agent implements `IAgent<TInput, TResult>` and returns:
- **Payload** (typed result)
- **Confidence** (0-1)
- **Citations** (sources)

Agents are currently deterministic and use mock providers; the interfaces are ready to be swapped to LLM or real APIs later.

---

## 5) Implementation tasks (backlog)

### Epics + acceptance criteria

- [ ] **Async processing pipeline**
  - **Acceptance**: Location pings are enqueued to Service Bus; processing happens out-of-band; API returns 202 quickly.

- [ ] **Cosmos DB persistence**
  - **Acceptance**: Session, insights, and saved items are stored and queryable per session.

- [ ] **Redis cache**
  - **Acceptance**: Geo context + insights are cached with TTL; cache hit ratio logged.

- [ ] **Provider integrations**
  - **Acceptance**: At least one live provider for events and one for POIs; failures degrade gracefully.

- [ ] **Privacy retention controls**
  - **Acceptance**: Configurable retention window; scheduled purge job; audit logs recorded.

- [ ] **Observability**
  - **Acceptance**: Structured logs with correlation IDs, traces and metrics exported to Application Insights.

- [ ] **Security hardening**
  - **Acceptance**: Entra ID integration; API keys rotated via Key Vault; rate limits per client.

- [ ] **Testing**
  - **Acceptance**: Unit + integration tests covering insight pipeline and dedupe behavior.
