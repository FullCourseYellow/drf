# Dependency Upgrades Design

## Context

The `feat/base-ui-migration` branch has completed the shadcn/ui Base UI migration and is clean. The template currently targets .NET 10 and React 19, declares NuGet dependencies in two project files, and declares npm dependencies plus a lockfile in the frontend project.

A registry scan on 2026-08-21 found 14 direct npm packages with newer releases and five NuGet references whose restored versions were behind the latest stable release. The available updates include breaking major versions in the frontend build toolchain, API client generator, table library, backend logging packages, and .NET test tooling.

## Goals

- Upgrade every direct NuGet and npm dependency to the latest stable release.
- Include major-version upgrades and make the smallest compatibility changes required by their documented migrations.
- Preserve the repository's current dependency-range conventions.
- Regenerate the npm lockfile through npm rather than editing it manually.
- Raise the generated project's Node.js prerequisite to 22.18 or newer, as required by Orval 8.
- Preserve the Base UI migration, API contracts, authentication behavior, routing, weather table behavior, and pagination.
- Verify the template source and a representative matrix of generated projects.

## Non-Goals

- Adopting central NuGet package management, a NuGet lockfile, or a new dependency-update service.
- Pinning direct dependencies to exact versions.
- Upgrading to prerelease package versions.
- Refactoring application code that is unrelated to a package migration.
- Changing target frameworks, React's major version, application features, or generated API contracts.

## Selected Approach

Apply upgrades in compatibility groups rather than as one bulk change or one package at a time. The groups provide useful failure boundaries while avoiding repeated full lockfile churn:

1. NuGet runtime dependencies.
2. NuGet test tooling.
3. npm updates that stay within the current major version.
4. The Vite, React plugin, and TypeScript build toolchain.
5. Orval API client generation.
6. TanStack Table and its local consumers.

Run the relevant restore, compile, test, code-generation, or frontend build check after each group. Run the complete generated-project matrix after all groups pass.

## Dependency Policy

### npm

Keep caret ranges in `template/src/web/package.json`, but move each direct dependency's baseline to the latest stable version selected during implementation. Regenerate `template/src/web/package-lock.json` with npm. A subsequent clean `npm ci` must reproduce the dependency graph.

The registry snapshot identified these major transitions:

| Package | Current major | Target observed on 2026-08-21 |
|---|---:|---:|
| `@tanstack/react-table` | 8 | 9.1.2 |
| `@vitejs/plugin-react` | 4 | 6.1.0 |
| `orval` | 7 | 8.24.0 |
| `typescript` | 5 | 7.0.2 |
| `vite` | 6 | 8.2.2 |

All other direct npm dependencies receive their latest stable patch or minor baseline. If the registry publishes a newer stable release before implementation, use that release and apply the same policy.

Orval 8.24.0 requires Node.js 22.18 or newer. Update `template/README.md` from Node.js 20+ to Node.js 22.18+ so the generated project's documented prerequisite matches its toolchain. Vite 8 and its React plugin are compatible with that baseline.

### NuGet

Keep the current floating-range style in both template project files. Microsoft framework packages remain on `10.0.*`, while packages currently expressed as a floating major remain expressed that way. Advance a range only when the latest stable release crosses its current boundary.

The registry snapshot requires these range changes:

| Package | Current range | Target range |
|---|---:|---:|
| `Serilog.AspNetCore` | `9.*` | `10.*` |
| `Serilog.Sinks.File` | `6.*` | `7.*` |
| `Microsoft.NET.Test.Sdk` | `17.*` | `18.*` |
| `xunit.runner.visualstudio` | `2.*` | `4.*` |

Packages whose latest stable release remains inside the declared range keep that declaration. Restore with forced reevaluation when checking floating dependencies so cached assets do not hide a newer compatible release. Do not add a NuGet lockfile or central package file.

## Compatibility Boundaries

Dependency migration changes stay behind existing project boundaries:

- Backend runtime changes remain in `template/src/Api/Company.ProjectName.Api.csproj` and, only if required by an API change, the existing API startup or configuration code.
- Test-tooling changes remain in `template/tests/Api.Tests/Company.ProjectName.Api.Tests.csproj` and the existing integration tests.
- Vite and TypeScript compatibility changes remain in the frontend configuration and TypeScript configuration files.
- Orval compatibility changes remain in `orval.config.ts` and the generated-client integration boundary.
- TanStack Table compatibility changes remain in `src/components/data-table.tsx` and the weather route's column definitions.
- Node.js prerequisite documentation remains in the generated project README.

The existing weather request still flows from the generated Orval client into route state and then into the local `DataTable`. Server-side pagination inputs and counts, routing, Base UI wrappers, loading behavior, and rendered table semantics must remain unchanged.

## Failure Handling

Package-manager, restore, code-generation, type-checking, build, and test failures remain visible. Do not suppress diagnostics, add an obsolete compatibility dependency, retain an older major without documenting it, or hand-edit generated dependency records.

Use the applicable package's current migration documentation before changing consumer code. If the latest stable release remains incompatible after its documented migration steps, stop on that compatibility group and report the exact blocker rather than silently weakening the all-latest requirement.

## Verification

### Dependency State

- Force-reevaluate NuGet restore for both project files.
- Run the NuGet outdated check for both project files; no direct stable update may remain.
- Regenerate the npm lockfile from the updated manifest.
- Run a clean `npm ci` under Node.js 22.18 or newer.
- Run the npm outdated check at depth zero; no direct stable update may remain.

### Template Source

- Restore and build the .NET solution.
- Run the API integration tests.
- Build the API to produce `openapi.json`.
- Generate the Orval client.
- Run the TypeScript and Vite production build.
- Pack the template.
- Exercise publish integration so the frontend is included in the application output.

### Generated Project Matrix

Generate isolated projects from the local packed template with these option sets:

1. Default full-stack project.
2. Full-stack project without authentication.
3. Full-stack project without OpenTelemetry.
4. API-only project without the frontend.

Build and test the backend in every generated project. In each project that includes a frontend, perform a clean npm install, generate the API client, and run the production frontend build. Publish the default project to verify the combined artifact. Generated projects and build outputs must remain outside the repository or be ignored so verification does not pollute the branch.

### Browser Smoke Check

Run the default generated application and inspect it at desktop and mobile widths. Confirm that routing and API loading work, weather rows render, previous and next pagination change pages, disabled pagination controls remain disabled at their boundaries, and narrow viewports retain usable table overflow.

## Acceptance Criteria

- Every direct npm and NuGet dependency resolves to the latest stable release available during implementation.
- npm declarations retain caret ranges and NuGet declarations retain the existing floating-range convention.
- The npm manifest and lockfile agree, and clean installation succeeds.
- The documented Node.js minimum is 22.18 or newer.
- Required compatibility edits remain limited to existing dependency boundaries.
- The template source restore, build, tests, code generation, frontend build, pack, and publish checks pass.
- Default, no-auth, no-OpenTelemetry, and API-only generated projects pass their applicable checks.
- The generated default application's weather table, pagination, routing, and API loading retain their behavior at desktop and mobile widths.
- No prerelease versions, diagnostic suppressions, compatibility fallbacks, generated artifacts, or unrelated refactors are introduced.
