# Full-Stack .NET 10 + React 19 Template

A `dotnet new` template for building full-stack applications with .NET 10 minimal API and React 19.

## Stack

- **Backend:** .NET 10 Minimal API, EF Core SQLite, Serilog, OpenTelemetry, Scalar, ErrorOr, Gridify
- **Frontend:** React 19, Vite, TanStack Router, TanStack Table, shadcn/ui, Tailwind CSS v4, Orval
- **Auth:** OIDC (optional)

## Installation

```bash
dotnet new install .
```

## Usage

```bash
dotnet new fullstack --name MyApp
```

### Options

| Option | Default | Description |
|---|---|---|
| `--include-auth` | `true` | Include OIDC authentication support |
| `--include-open-telemetry` | `true` | Include OpenTelemetry observability |
| `--include-frontend` | `true` | Include React 19 frontend |

### Examples

```bash
# Full stack with all features
dotnet new fullstack --name MyApp

# API only, no frontend
dotnet new fullstack --name MyApp --include-frontend false

# No auth
dotnet new fullstack --name MyApp --include-auth false
```

## Uninstall

```bash
dotnet new uninstall .
```

## Project Structure

```
MyApp/
  src/
    Api/          # .NET 10 Minimal API
    web/          # React 19 frontend
  tests/
    Api.Tests/    # xUnit integration tests
  MyApp.sln
```
