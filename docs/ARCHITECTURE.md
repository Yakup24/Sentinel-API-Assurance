# Architecture

Sentinel API Assurance is structured as a layered .NET 8 console framework for service-level regression testing.

## High-level flow

```text
CLI arguments
   ↓
ConfigLoader
   ↓
Environment + Suite selection
   ↓
OperationSafetyPolicy
   ↓
ITestExecutor implementation
   ↓
AssertionEngine
   ↓
HTML / JSON / CSV reports
```

## Main layers

### Configuration

Responsible for loading framework configuration and test suites.

- `ConfigLoader`
- `LegacyCallImporter`
- `appsettings.json`
- `Suites/*.json`

### Execution

Responsible for test orchestration and assertion evaluation.

- `TestRunner`
- `AssertionEngine`

### Services

Responsible for protocol-specific execution.

- `ITestExecutor`
- `SoapTestExecutor`
- `RestTestExecutor`

The framework can grow by adding new executor implementations, for example:

- GraphQL executor
- gRPC executor
- Database assertion executor

### Safety

Responsible for blocking potentially state-changing operations unless explicit approval is given in the test case.

- `OperationSafetyPolicy`

### Reporting

Responsible for producing test outputs.

- `HtmlReportWriter`
- `JsonReportWriter`
- `CsvReportWriter`

## Configuration model

`appsettings.json` contains global runtime settings, test data, environments and service registry.

```json
{
  "DefaultEnvironment": "STB",
  "TimeoutSeconds": 30,
  "RetryCount": 1,
  "Environments": []
}
```

Each environment contains a `BaseUrl` and service definitions.

```json
{
  "Name": "STB",
  "BaseUrl": "http://stb-soa-gateway.company.local/services",
  "Services": {
    "AddressOperations_v1.0": {
      "Endpoint": "AddressOperations_v1.0",
      "SoapVersion": "1.1",
      "SoapActionFormat": "{operation}"
    }
  }
}
```

## Test case model

Each test case defines protocol, service, operation, request template, expected HTTP status and assertions.

```json
{
  "Id": "ADDR-001",
  "Protocol": "SOAP",
  "Service": "AddressOperations_v1.0",
  "Operation": "getAddressByMsisdn",
  "RequestBodyFile": "Requests/AddressOperations_v1.0/getAddressByMsisdn.xml",
  "ExpectedHttpStatus": 200,
  "Assertions": [
    { "Type": "NoSoapFault" },
    { "Type": "MaxDurationMs", "MaxDurationMs": 5000 }
  ]
}
```

## Request rendering

Request templates support token replacement.

```xml
<msisdn>{{Msisdn}}</msisdn>
<token>{{ENV:VOLTRAN_TEST_TOKEN}}</token>
```

`TemplateRenderer` resolves values from:

1. `TestData` in `appsettings.json`
2. Environment variables with the `ENV:` prefix

## Extension points

Recommended extension points:

- Add new assertion types in `AssertionEngine`
- Add new protocol executors implementing `ITestExecutor`
- Add new report writers under `Reporting`
- Add external test data providers under `Configuration`

## Production hardening recommendations

Before using this framework against sensitive environments:

- Use dedicated test customers / test MSISDNs only
- Keep state-changing operations blocked by default
- Move secrets to environment variables or secret stores
- Mask MSISDN, customer ID and token values in logs
- Store historical reports in CI artifacts
- Add JUnit XML output for CI test visualization
