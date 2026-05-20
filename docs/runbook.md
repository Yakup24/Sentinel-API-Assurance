# Runbook

This runbook describes the standard operating procedure for running Sentinel API Assurance.

## 1. Local Validation

Use dry-run before sending any request to an endpoint:

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

Dry-run checks:

- Active test case count
- Service registration status
- Missing request templates
- State-changing operations blocked by policy

## 2. Build Validation

```bash
dotnet restore SentinelApiAssurance.sln
dotnet build SentinelApiAssurance.sln --configuration Release
dotnet test SentinelApiAssurance.sln --configuration Release
```

## 3. Smoke Suite Execution

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --suite Suites/voltran-smoke-suite.json
```

Use smoke suites for fast health checks.

## 4. Full Regression Execution

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --suite Suites/voltran-enterprise-regression-suite.json
```

Use full regression when validating a broader release or service migration.

## 5. PRP Execution

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env PRP --suite Suites/voltran-enterprise-regression-suite.json
```

Before PRP execution, confirm that:

- Endpoint URLs are placeholders or controlled non-production URLs.
- Test data is valid for the target environment.
- State-changing operations are reviewed.
- Tokens and headers are provided securely at runtime.

## 6. Legacy Call XML Execution

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --calls test-calls.xml
```

This mode imports `<call>` entries and generates a runtime suite.

## 7. Report Locations

Reports are generated under the application output directory:

```text
bin/Debug/net8.0/Reports
bin/Release/net8.0/Reports
```

Generated report types:

- HTML
- JSON
- CSV

## 8. Common Failures

| Symptom | Likely Cause | Action |
| ------- | ------------ | ------ |
| Missing service | Service is not registered for selected environment | Add service entry to `appsettings.json`. |
| Missing request | Request template file does not exist | Add the file under `Requests/<Service>/<Operation>.xml`. |
| Skipped risky operation | Safety policy blocked state-changing operation | Review data and set `AllowStateChangingOperation=true` only if approved. |
| SOAP fault | Service returned a fault payload | Inspect request body, SOAPAction, endpoint, and test data. |
| Timeout | Endpoint did not respond in configured time | Check environment availability and timeout settings. |
