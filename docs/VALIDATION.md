# Validation

Authoring date: November 6, 2026. Host: Windows. Target: Windows desktop.

## Local result

Local Windows build passed with zero warnings/errors; wiki-link and temporary-vault self-tests passed. GUI checks remain manual.

## Reproducible checks

The GitHub Actions workflow builds/runs the following on `windows-latest`:

```sh
dotnet build
dotnet run --no-build -- --self-test
```

## Manual acceptance

Open the example vault, navigate links/backlinks, search, save a changed link, cancel an unsaved discard, and test an external edit before saving.

A workflow file is not evidence of a passing run. Observed CI run links and source commits are recorded after publication. Native GUI, sound, sleep/resume, and platform permissions require direct target-platform acceptance even when model tests pass.

## Observed CI result

Passed: [target-platform build/check](https://github.com/netcodermike/markdown-note-graph/actions/runs/42204053636). Source commit: `48596fe984d9b2281d74c5a3b2d0e1dfed3884fa`. Later documentation-only changes do not alter the tested implementation. The reproducible commands above identify test scope; native GUI/audio/permissions are not implied by compilation.
