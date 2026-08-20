# shadcn Base UI Migration Design

## Context

The generated React application currently identifies itself as a shadcn/ui project, but its `Button` wrapper uses Radix Slot and `components.json` still selects the legacy `default` style. The template contains only two shadcn UI wrappers: `Button` and a native HTML `Table`. Radix is used only by `Button`; the data table consumes these local wrappers without importing primitives directly.

shadcn/ui now supports Base UI as its default primitive foundation. New projects generated from this template should use the current `base-nova` preset and remain ready for consumers to add compatible components with the shadcn CLI.

## Goals

- Make newly generated frontend projects use shadcn/ui with Base UI instead of Radix.
- Adopt the complete current `base-nova` foundation rather than performing only a primitive dependency swap.
- Keep local UI wrappers under `src/components/ui` as the application-facing component boundary.
- Preserve the existing weather table's data and pagination behavior.
- Leave generated projects ready for future `shadcn add` commands.
- Remove all direct and lockfile references to Radix from the template frontend.

## Non-Goals

- Migrating applications that were already generated from an earlier template version.
- Providing an `asChild` compatibility shim or migration tooling.
- Changing backend behavior, routing, API generation, authentication, or table pagination.
- Adding a frontend test framework solely for stock registry components.
- Refactoring unrelated template or application code.

## Selected Approach

Use the official `base-nova` registry output as the source of truth and apply the migration explicitly. This is preferable to re-running initialization because it keeps the diff bounded and reviewable, and preferable to a minimal primitive swap because all configuration, tokens, and future generated components remain coherent.

The implementation should update each existing UI wrapper individually rather than bulk-overwriting the project. The template has no wrapper customizations that need a compatibility merge, but current application-facing imports must remain stable.

## Configuration And Dependencies

Update `template/src/web/components.json` to:

- Select the `base-nova` style.
- Keep `rsc` disabled and TypeScript enabled.
- Preserve the existing Vite source paths and aliases.
- Add the current Tailwind prefix and Lucide icon-library fields expected by the preset.

Update `template/src/web/package.json` to remove `@radix-ui/react-slot` and include the current packages required by the complete Base Nova foundation. This includes the Base UI runtime, Lucide icons, animation CSS, and shadcn Tailwind integration while retaining the template's existing React, Tailwind, CVA, and utility dependencies.

Regenerate `template/src/web/package-lock.json` through npm. Do not hand-edit lockfile package records. The resulting manifest and lockfile must contain no `@radix-ui` entries.

Replace the legacy HSL-only setup in `template/src/web/src/index.css` with the current Base Nova Tailwind v4 foundation. The stylesheet must include the preset's required imports, theme-variable mappings, light and dark tokens, and base element styles. This makes the checked-in wrappers and future CLI-added components use the same token system.

## Component Boundaries

### Button

`src/components/ui/button.tsx` will wrap `@base-ui/react/button` and retain the local `Button` and `buttonVariants` exports. Props flow from the local wrapper through its CVA variant selection into the Base UI primitive.

The wrapper will adopt Base Nova's current variants, sizes, data slot, and interaction styles. Its composition API becomes Base UI's `render` prop. The Radix-specific `asChild` prop is removed rather than emulated because this design targets only new generated projects and no current template consumer uses it.

Base UI owns native button behavior, disabled interaction, focus behavior, and accessibility state. Consumers continue to use `Button` from `@/components/ui/button`.

### Table

`src/components/ui/table.tsx` will use the current Base Nova registry structure and styling. It remains a set of typed native HTML wrappers and does not introduce a headless table primitive. Standard data slots and the complete registry exports, including footer and caption, will be present so future consumers receive the expected shadcn API.

### Data Table

`src/components/data-table.tsx` keeps its current TanStack Table state, row rendering, pagination callbacks, and local wrapper imports. It requires no data-flow changes. The Base Nova wrappers alter presentation only, while the table container continues to provide horizontal overflow behavior for narrow viewports.

## Documentation And Metadata

Update all user-visible stack descriptions so the repository and generated project consistently identify the frontend as `shadcn/ui (Base UI)`:

- `README.md`
- `template/README.md`
- `MyTemplate.csproj`
- `template/.template.config/template.json`

No migration guide for existing generated applications will be added.

## Failure Handling

There is no new runtime application error path. Existing API loading and pagination behavior remain unchanged, while Base UI handles button interaction semantics.

Dependency, type-checking, template-generation, or production-build errors are migration failures and must remain visible. The implementation must not retain Radix as a fallback. A final source and lockfile sweep is required because a successful build alone would not prove that obsolete Radix references were removed.

## Verification

Verification will include:

1. Install or refresh frontend dependencies using npm so the lockfile reflects the manifest.
2. Run the frontend TypeScript and Vite production build.
3. Search the repository's template frontend for leftover `@radix-ui` imports and dependency records.
4. Generate a full-stack project from the local template in an isolated temporary location and build its frontend.
5. Run the generated application and inspect the existing weather table and pagination buttons at desktop and mobile widths, including disabled button behavior and horizontal table overflow.

The repository currently has no frontend test runner. Adding one is outside this migration's scope because the changed wrappers follow official registry output and the meaningful integration coverage is template generation plus production build and browser smoke testing.

## Acceptance Criteria

- New frontend projects declare `base-nova` in `components.json`.
- `Button` is backed by `@base-ui/react/button` and exposes Base UI composition semantics.
- `Table` matches the Base Nova registry contract without changing data-table behavior.
- The complete Base Nova Tailwind v4 foundation is present for future shadcn components.
- The frontend manifest, lockfile, and source contain no `@radix-ui` references.
- Repository and template metadata identify shadcn/ui as Base UI-backed.
- The template frontend and an independently generated frontend both build successfully.
- The weather table remains usable at desktop and mobile widths.
