# Full-Stack .NET 10 + React 19 Template

A `dotnet new` template for building full-stack applications with .NET 10 minimal API and React 19.

## Stack

- **Backend:** .NET 10 Minimal API, EF Core SQLite, Serilog, OpenTelemetry, Scalar, ErrorOr, Gridify
- **Frontend:** React 19, Vite, TanStack Router, TanStack Table, shadcn/ui, Tailwind CSS v4, Orval
- **Auth:** OIDC (optional)

## Installation

### From a local folder

Clone or download the repository, then install from the project directory:

```bash
git clone https://github.com/FullCourseYellow/drf.git
cd drf
dotnet new install .
```

Or if you already have the folder:

```bash
dotnet new install /path/to/drf
```

### From GitHub (NuGet package)

Once the package is published to NuGet:

```bash
dotnet new install FullCourseYellow.Drf.Template
```

### From a specific GitHub release (nupkg)

Download the `.nupkg` from the [releases page](https://github.com/FullCourseYellow/drf/releases) and install it directly:

```bash
dotnet new install FullCourseYellow.Drf.Template.1.0.0.nupkg
```

## Update

### Installed from a local folder

Re-run install from the same folder to pick up the latest changes:

```bash
cd /path/to/drf
git pull
dotnet new install .
```

### Installed from NuGet

```bash
dotnet new install FullCourseYellow.Drf.Template --force
```

## Usage

```bash
dotnet new drf --name MyApp
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
dotnet new drf --name MyApp

# API only, no frontend
dotnet new drf --name MyApp --include-frontend false

# No auth
dotnet new drf --name MyApp --include-auth false
```

## Uninstall

### Installed from a local folder

```bash
dotnet new uninstall /path/to/drf
```

### Installed from NuGet

```bash
dotnet new uninstall FullCourseYellow.Drf.Template
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
