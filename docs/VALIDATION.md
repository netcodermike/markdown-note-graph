# Validation

Authoring date: November 6, 2026. Host: Windows. Target: Windows desktop.

## Local result

Local Windows build/model result is recorded after completion. GUI checks remain manual.

## Reproducible checks

The GitHub Actions workflow builds/runs the following on `windows-latest`:

```sh
dotnet build
dotnet run --no-build -- --self-test
```

## Manual acceptance

Open the example vault, navigate links/backlinks, search, save a changed link, cancel an unsaved discard, and test an external edit before saving.

A workflow file is not evidence of a passing run. Observed CI run links and source commits are recorded after publication. Native GUI, sound, sleep/resume, and platform permissions require direct target-platform acceptance even when model tests pass.
