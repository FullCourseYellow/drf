# Gitignore Expansion Design

## Goal

Keep generated, local, and secret files out of both the template repository and projects created from the template without adding rules for unrelated technologies.

## Scope

Add a root `.gitignore` tailored to developing and packaging this .NET template repository. Expand `template/.gitignore` for generated applications that use the template's .NET backend and optional React/Vite frontend.

## Root Rules

Ignore:

- .NET build output and intermediate directories
- NuGet package artifacts
- Test results and coverage output
- Visual Studio, VS Code, Rider, and common user-specific IDE files
- Operating-system metadata
- Temporary files

The root rules will not ignore source files, project configuration, documentation, or template inputs.

## Generated-Project Rules

Retain the existing generated-project exclusions and organize them into named sections. Cover:

- .NET build output and user-specific files
- Node dependencies, Vite output, and frontend caches
- IDE and operating-system metadata
- Logs and SQLite database files, including journal and WAL sidecars
- Local environment files while explicitly preserving `.env.example`
- Test and coverage output
- Generated TanStack Router and API client output used by the template

Rules that only apply to generated applications belong in `template/.gitignore`, not the repository root.

## Verification

Use `git check-ignore` with representative paths from each category. Confirm `.env.example`, source files, project files, and documentation are not ignored. Inspect the final diff to ensure unrelated files are unchanged.
