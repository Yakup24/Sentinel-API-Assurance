# Runbook

This runbook describes the standard operating procedure for running Sentinel API Assurance.

## 1. Local validation

Use dry-run before sending any request to an endpoint.

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

Dry-run checks:

- Active test case count
- Service registration status
- Missing request templates
- State-changing operations blocked by policy

## 2. Build validation

```powershell
dotnet restore src/SentinelApiAssurance/SentinelApiAssurance.csproj
dotnet build src/SentinelApiAssurance/SentinelApiAssurance.csproj --configuration Release
```

## 3. Smoke suite execution

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --suite Suites/voltran-smoke-suite.json
```

Use smoke suite when you want a fast health check.

## 4. Full regression execution

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --suite Suites/voltran-enterprise-regression-suite.json
```

Use full regression when validating a broader release or service migration.

## 5. PRP execution

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env PRP --suite Suites/voltran-enterprise-regression-suite.json
```

Before PRP execution, confirm that:

- Endpoint URLs are correct
- Test data is valid for PRP
- State-changing operations are reviewed
- Tokens and headers are available

## 6. Legacy Autopilot call XML execution

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --calls test-calls.xml
```

This mode imports `<call>` entries and generates a runtime suite.

## 7. Report locations

Reports are generated under the application output directory:

```text
bin/Debug/net8.0/Reports
bin/Release/net8.0/Reports
```

Generated report types:

- HTML
- JSON
- CSV

## 8. Common failures

### Service is not defined

Cause: The suite references a service that does not exist in the selected environment config.

Fix: Add the service under `Environments[].Services` in `appsettings.json`.

### Request body file not found

Cause: `RequestBodyFile` points to a missing XML template.

Fix: Add the missing file under `Requests/<Service>/<Operation>.xml`.

### SOAP Fault detected

Cause: Endpoint returned a SOAP fault.

Fix:

1. Check request namespace.
2. Check request operation name.
3. Check mandatory fields.
4. Check test data validity.
5. Check endpoint/service availability.

### State-changing operation skipped

Cause: Operation matched dangerous keyword policy.

Fix: Only after review, set:

```json
"AllowStateChangingOperation": true
```

## 9. Operational checklist

Before running against shared environments:

- [ ] Dry-run is clean
- [ ] Build passes
- [ ] Correct environment selected
- [ ] Test data is approved
- [ ] No production identifiers are used
- [ ] Risky operations are reviewed
- [ ] Reports are archived

## 10. CI behavior

GitHub Actions workflow runs on push and pull request to `main`.

Pipeline steps:

1. Checkout
2. Setup .NET 8
3. Restore
4. Build
5. Dry-run suite validation
