# Architecture

## High-Level Architecture

Sentinel API Assurance is a .NET console application that executes API checks from versioned test suites.

```text
User / CI
  -> Test Suite Loader
  -> Request Template Rendering
  -> SOAP / REST Client
  -> Response Parser
  -> Assertion Engine
  -> Report Generator
```

The framework keeps test definition, request body, environment configuration, execution, assertion, and reporting concerns separated.

## Component Responsibilities

| Component | Responsibility |
| --------- | -------------- |
| `Configuration` | Load `appsettings.json`, JSON suites, and legacy XML call lists. |
| `Models` | Define runtime objects such as config, suite, test case, and run results. |
| `Services` | Execute SOAP and REST calls through `HttpClient`. |
| `Execution` | Coordinate test execution and evaluate assertions. |
| `Safety` | Block risky operations unless explicitly approved. |
| `Reporting` | Write HTML, JSON, and CSV reports. |
| `Utilities` | Provide CLI parsing, template rendering, and file logging. |

## Data Flow

1. The CLI selects environment, suite, config path, or legacy calls file.
2. `ConfigLoader` loads and validates configuration.
3. `ConfigLoader` or `LegacyCallImporter` loads test cases.
4. `OperationSafetyPolicy` checks whether a case should be skipped.
5. `SoapTestExecutor` or `RestTestExecutor` renders placeholders and sends the request.
6. `AssertionEngine` evaluates the response.
7. Report writers serialize the final `RunResult`.

## Test Execution Lifecycle

Each active test case goes through these states:

1. Resolve service from the selected environment.
2. Apply risky operation policy.
3. Select an executor by protocol.
4. Render request/header/path templates.
5. Send HTTP request unless dry-run mode was selected.
6. Capture response status, body, and elapsed time.
7. Evaluate configured assertions.
8. Store a passed, failed, or skipped result.

## Error Handling Approach

Executor-level failures are captured as failed raw responses with an explanatory message. Retry count and retry delay are configured in `appsettings.json`.

Configuration errors are detected early by `ConfigLoader` so invalid environments, missing service endpoints, or malformed suite cases fail before execution.

## Configuration Flow

`appsettings.json` is the main source of runtime configuration. It defines:

- Environment names and base URLs
- Service endpoints and SOAP metadata
- Global headers
- Placeholder test data
- Safety keywords
- Default suite path and report directory

The CLI can override environment, suite, config path, or legacy call input.
