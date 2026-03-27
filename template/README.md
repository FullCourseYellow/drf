# Company.ProjectName

<!-- #if (includeFrontend) -->
Full-stack application built with .NET 10 Minimal API and React 19.
<!-- #else -->
API application built with .NET 10 Minimal API.
<!-- #endif -->

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
<!-- #if (includeFrontend) -->
- [Node.js 20+](https://nodejs.org/)
<!-- #endif -->

## Project Structure

```
src/
  Api/          .NET 10 Minimal API (EF Core SQLite, Serilog, Scalar, Gridify, ErrorOr)
<!-- #if (includeFrontend) -->
  web/          React 19 frontend (Vite, TanStack Router/Table, shadcn/ui, Tailwind v4, Orval)
<!-- #endif -->
tests/
  Api.Tests/    xUnit integration tests
```

## Development

<!-- #if (includeFrontend) -->
The API and frontend run as separate processes during development. The Vite dev server proxies `/api` requests to the backend, so both need to be running.

<!-- #endif -->
### API

```bash
cd src/Api
dotnet run
```

The API starts on `https://localhost:5001`. Interactive API docs are available at [https://localhost:5001/scalar/v1](https://localhost:5001/scalar/v1).

<!-- #if (includeFrontend) -->
### Frontend

```bash
cd src/web
npm install
npm run dev
```

The Vite dev server starts on `http://localhost:5173` and proxies all `/api/*` requests to the backend.

To override the backend URL, set the `VITE_API_URL` environment variable or create a `.env.local` file (see `.env.example`).

<!-- #endif -->
### Tests

```bash
cd tests/Api.Tests
dotnet test
```

Tests use `WebApplicationFactory<Program>` with an in-memory SQLite database, so they don't require a running API or any external database.

<!-- #if (includeFrontend) -->
### Generating the API Client

The backend generates an OpenAPI spec at build time. To regenerate the TypeScript API client after changing endpoints:

```bash
cd src/Api
dotnet build        # writes openapi.json into src/web/
cd ../web
npm run generate-api  # runs Orval to regenerate src/api/generated/
```

<!-- #endif -->
## Debugging

<!-- #if (includeFrontend) -->
### Debugging the API and frontend separately

The API and frontend are independent processes. You can debug each one on its own.

**API (Visual Studio / VS Code with C# Dev Kit / Rider):**
<!-- #else -->
### Debugging the API

**Visual Studio / VS Code with C# Dev Kit / Rider:**
<!-- #endif -->
Open the solution file (`Company.ProjectName.sln`) and press F5, or launch the `Api` project directly. The API runs with the `Development` environment by default, which uses `appsettings.Development.json` and a separate SQLite database (`app.Development.db`).

<!-- #if (includeFrontend) -->
**Frontend (VS Code / any editor with browser DevTools):**
Run `npm run dev` and open `http://localhost:5173`. Vite provides hot module replacement (HMR) so changes appear instantly. Use your browser's DevTools for breakpoints and network inspection. If you use VS Code, the built-in JavaScript debugger can attach to the Vite dev server.

**Running them together:**
Start the API first, then start the frontend. The Vite proxy forwards `/api/*` calls to `https://localhost:5001` by default (configurable via `VITE_API_URL`). The proxy accepts self-signed certificates, so no extra TLS setup is needed for local development.

<!-- #endif -->
## Publishing

<!-- #if (includeFrontend) -->
When you run `dotnet publish`, the API project builds and bundles the React frontend automatically:

```bash
dotnet publish src/Api/Company.ProjectName.Api.csproj -c Release -o ./publish
```

This triggers the `PublishFrontend` MSBuild target, which:

1. Runs `npm run generate-api` to regenerate the TypeScript client from the latest OpenAPI spec
2. Runs `npm run build` to produce the React production bundle
3. Copies `src/web/dist/` into `wwwroot/` in the publish output

The result is a single deployment artifact. In production, the API serves the React SPA from `wwwroot/` and handles client-side routing via `MapFallbackToFile("index.html")`.
<!-- #else -->
```bash
dotnet publish src/Api/Company.ProjectName.Api.csproj -c Release -o ./publish
```
<!-- #endif -->

```bash
# Run the published app
cd publish
dotnet Company.ProjectName.Api.dll
```

## Configuration

Configuration follows the standard ASP.NET Core layering: `appsettings.json` is the base, `appsettings.{Environment}.json` overrides per environment, and environment variables or user secrets can override individual values.

### `ConnectionStrings`

| Key | Default | Description |
|-----|---------|-------------|
| `DefaultConnection` | `Data Source=app.db` | SQLite connection string. Development uses `app.Development.db` to keep data separate. |

### `Serilog`

Logging is configured through Serilog. The sinks (console and rolling file at `logs/app-.log`) are defined in code. `appsettings.json` controls minimum log levels only.

| Key | Default (Production) | Default (Development) | Description |
|-----|---------------------|-----------------------|-------------|
| `Serilog:MinimumLevel:Default` | `Information` | `Debug` | Base log level for application code |
| `Serilog:MinimumLevel:Override:Microsoft` | `Warning` | `Information` | Log level for Microsoft framework logs |
| `Serilog:MinimumLevel:Override:Microsoft.Hosting.Lifetime` | `Information` | *(inherits)* | Startup/shutdown lifecycle messages |
| `Serilog:MinimumLevel:Override:System` | `Warning` | `Warning` | Log level for System namespace |

> **Note:** Do not add `WriteTo` entries in `appsettings.json` -- sinks are owned in code to avoid duplicate output.

<!-- #if (includeAuth) -->
### `Authentication`

Authentication is opt-in at runtime. If the `Authentication:JwtBearer` section is missing or `Authority` is empty, the app runs without auth middleware.

| Key | Description |
|-----|-------------|
| `Authentication:JwtBearer:Authority` | OIDC provider URL (e.g., `https://login.example.com/realm`) |
| `Authentication:JwtBearer:Audience` | Expected JWT audience claim |

These values are best stored in user secrets during development:

```bash
cd src/Api
dotnet user-secrets set "Authentication:JwtBearer:Authority" "https://your-provider.example.com"
dotnet user-secrets set "Authentication:JwtBearer:Audience" "your-api"
```

<!-- #endif -->
<!-- #if (includeOpenTelemetry) -->
### `OpenTelemetry`

OpenTelemetry instrumentation activates when the `OpenTelemetry` configuration section exists. By default, traces and metrics are exported to the console. To enable it, add an `OpenTelemetry` section to `appsettings.json`:

```json
{
  "OpenTelemetry": {}
}
```

The presence of the section is enough to activate instrumentation. The exporter and instrumentation details are configured in code.

<!-- #endif -->
<!-- #if (includeFrontend) -->
### Frontend Environment Variables

The frontend uses Vite environment variables (prefixed with `VITE_`). Create a `.env.local` file in `src/web/` based on `.env.example`:

| Variable | Default | Description |
|----------|---------|-------------|
<!-- #if (includeAuth) -->
| `VITE_OIDC_AUTHORITY` | -- | OIDC provider URL |
| `VITE_OIDC_CLIENT_ID` | -- | OAuth client ID |
| `VITE_OIDC_REDIRECT_URI` | `window.location.origin` | OAuth redirect URI |
<!-- #endif -->
| `VITE_API_URL` | `https://localhost:5001` | Backend URL for the Vite dev proxy |
<!-- #endif -->
