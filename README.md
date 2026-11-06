# Markdown Note Graph

Open a folder of Markdown notes, inspect wiki links and backlinks, explore a neighbor graph, and edit note text.

[![Build and verify](https://github.com/netcodermike/markdown-note-graph/actions/workflows/verify.yml/badge.svg)](https://github.com/netcodermike/markdown-note-graph/actions/workflows/verify.yml)

**Language:** C# · **Platform:** Windows desktop · **App type:** Linked-note desktop editor

First-release learning implementation, with explicit scope below. This is one independently runnable app, not a placeholder in a combined repository.

## Features

- Top-level .md vault with name/text search and a raw Markdown editor.
- [[Note]] and [[Note|label]] link extraction.
- Outgoing links, missing targets, backlinks, and a clickable neighbor graph.
- Explicit save, unsaved-change prompts, and a basic external-change check.

## Requirements

Windows 10/11 and the .NET 10 SDK/Desktop runtime. WPF is Windows-only. No NuGet dependencies.

## Quick start

```sh
git clone https://github.com/netcodermike/markdown-note-graph.git
cd markdown-note-graph
dotnet run -- examples
# Or open your own folder:
dotnet run -- C:\path\to\notes
```

Commands run from the repository root. Lines marked for a particular operating system apply only there. Language tools are prerequisites, not installed by the app. Close desktop windows normally; terminal/server apps stop with Ctrl+C unless their usage notes say otherwise.

## Example workflow

Launch with examples, select Welcome, and click Ideas on the graph. Inspect backlinks and the Reading alias link. Edit a note and save, refresh the folder, and verify the graph updates. Change the same file in another editor to exercise the external-change warning.

## Architecture, configuration, and data

`Program.cs` defines the Note model, Vault file/link helpers, and a WPF UI. Notes are identified by case-insensitive filename stem. Only immediate .md children are loaded, capped at 500 notes and 1 MiB per file. Graph layout places the active note centrally with up to ten distinct incoming/outgoing neighbors around it. Link lists include missing targets. Save compares the original text with the current file before replacing it through a sibling temporary file. The examples folder is a three-note vault. --self-test checks parsing/deduplication and a real temporary vault read/write round trip.

## Verification

Build with the quick-start command where needed, then run:

```sh
dotnet run -- --self-test
```

Manual acceptance: Open the example vault, navigate links/backlinks, search, save a changed link, cancel an unsaved discard, and test an external edit before saving.

See [validation record](docs/VALIDATION.md) for observed checks and their limits. CI results verify only what the workflow runs; they do not establish native GUI correctness or production readiness.

## Limitations and next steps

Raw-text editing only: no rendered Markdown, nested folders, note creation/deletion, full wiki syntax, anchors, aliases as titles, automatic watching, tag system, or full-vault graph layout. Wiki-like text inside code blocks is still extracted. A neighbor outside the current search results may not be navigable until the search is cleared. The external-change check is not a transactional multi-writer lock. File permissions/metadata may change on replacement. Keep a versioned backup of important notes.

## Troubleshooting

Build on Windows with .NET 10 Desktop components. Pass a folder or use Open / refresh after entering one. Missing links must match filename stems; .md and directories are not part of the link syntax. Read/permission/size errors are shown. Clear the search before graph navigation if a neighbor is hidden. Unsaved changes require explicit discard or save.

## Source map

- `NoteGraph.csproj`
- `Program.cs`
- `examples/Ideas.md`
- `examples/Reading.md`
- `examples/Welcome.md`

## Development and license

Read [contributing](CONTRIBUTING.md) before changing the app, and [security/privacy](SECURITY.md) before supplying private data. Use synthetic fixtures for tests. No telemetry, paid API key, or account is required by the app itself. Dependencies, when present, are stated in Requirements.

[MIT license](LICENSE), copyright 2026 netcodermike.
