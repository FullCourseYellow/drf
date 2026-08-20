# AGENTS.md Conventional Commit Policy Design

## Context

The repository has no existing agent instruction file or Cocogitto configuration. Its recent commit history already follows Conventional Commits. The `main` branch tracks `origin/main`, whose URL is `git@github.com:FullCourseYellow/drf.git`.

## Goal

Add a concise root-level `AGENTS.md` that:

- requires Conventional Commits;
- requires every commit to be created with Cocogitto's `cog commit` command;
- prohibits direct `git commit` usage; and
- gives agents the supported command syntax and one scoped example.

The configured Git upstream will be reported to the user, not recorded in `AGENTS.md`.

## Non-Goals

- Adding broader repository, build, test, or architecture guidance.
- Adding or changing `cog.toml`.
- Adding commit hooks or CI enforcement.
- Changing the Git remote configuration.

## File Design

Create `/AGENTS.md` with one `Commit Policy` section. The policy will state:

1. Every commit must follow Conventional Commits.
2. Every commit must use `cog commit <type> "<message>" [scope]` rather than direct `git commit`.
3. Only intended files should be staged before committing.
4. A scoped example is `cog commit feat "add weather forecast endpoint" api`.

The syntax matches Cocogitto 7.0.0 installed in the development environment.

## Operational Flow

An agent stages only the files intended for the commit, then invokes `cog commit` with an appropriate type, message, and optional scope. Cocogitto validates the commit and creates it.

If Cocogitto rejects the commit, the agent corrects the type or message and retries. If Cocogitto is unavailable, the agent reports the blocker instead of falling back to direct `git commit`.

## Verification

- Confirm `AGENTS.md` exists at the repository root.
- Inspect the file for the required policy, prohibition, syntax, and example.
- Confirm the documented syntax remains consistent with `cog commit --help`.
- Run no application test suite because the change affects repository documentation only.

## Acceptance Criteria

- Agents reading the root instructions cannot reasonably interpret direct `git commit` as allowed.
- Agents have enough information to create an unscoped or scoped Conventional Commit with Cog.
- No unrelated repository guidance or Cog configuration is introduced.
- The user is informed that the current branch upstream is hosted on GitHub.
