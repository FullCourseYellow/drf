# Commit Policy

All commits must follow the [Conventional Commits](https://www.conventionalcommits.org/) specification.

Create every commit with Cocogitto:

```bash
cog commit <type> "<message>" [scope]
```

Do not use `git commit` directly. Stage only the files intended for the commit before invoking Cog. If Cog rejects a commit or is unavailable, correct the issue or report the blocker; do not bypass Cog.

Example:

```bash
cog commit feat "add weather forecast endpoint" api
```
