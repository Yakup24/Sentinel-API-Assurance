# Contributing

Thanks for considering a contribution to Sentinel API Assurance. Keep changes practical, testable, and safe for API automation workflows.

## Local Setup

```bash
git clone https://github.com/Yakup24/Sentinel-API-Assurance.git
cd Sentinel-API-Assurance
dotnet restore SentinelApiAssurance.sln
dotnet build SentinelApiAssurance.sln
dotnet test SentinelApiAssurance.sln
```

## Branch Naming

Use clear branch names:

- `feature/add-json-assertion`
- `fix/report-writer-empty-run`
- `docs/update-security-model`
- `test/add-risk-guard-tests`

## Commit Style

Prefer concise, scoped commit messages:

- `docs: update architecture guide`
- `test: add assertion engine coverage`
- `fix: validate missing service endpoints`
- `ci: run dry-run validation`

## Running Tests

Run the full validation set before opening a pull request:

```bash
dotnet format SentinelApiAssurance.sln --verify-no-changes
dotnet build SentinelApiAssurance.sln --configuration Release
dotnet test SentinelApiAssurance.sln --configuration Release
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

## Adding a New Test Case

1. Add or update a request template under `src/SentinelApiAssurance/Requests`.
2. Add a case to the relevant suite under `src/SentinelApiAssurance/Suites`.
3. Use placeholder test data only.
4. Keep `AllowStateChangingOperation` set to `false` unless the operation is approved for controlled test data.
5. Run dry-run validation.

## Adding a New Report Format

1. Add a writer under `src/SentinelApiAssurance/Reporting`.
2. Keep the writer focused on one output format.
3. Add unit tests for empty and populated run results.
4. Update README and `docs/reporting.md`.

## Pull Request Checklist

- [ ] Code builds locally.
- [ ] Unit tests pass.
- [ ] Dry-run validation passes.
- [ ] Documentation reflects the actual behavior.
- [ ] No real secrets, credentials, customer data, or internal endpoints were added.
- [ ] Risky operations remain blocked unless explicitly justified.
