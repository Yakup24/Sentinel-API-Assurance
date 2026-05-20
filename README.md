# Sentinel API Assurance

.NET-based SOAP/REST API test automation framework for suite-based regression checks, dry-run validation, risky operation guards, and exportable reports.

GitHub repository name: `sentinel-api-assurance`

## Overview

Sentinel API Assurance reduces manual service verification by running API checks from versioned test suites. It is designed for QA engineers, integration teams, and developers who need repeatable smoke or regression validation for SOAP and REST services.

The current implementation is a .NET 8 console application. It loads configuration from JSON, executes active test cases, evaluates assertions, blocks risky operations unless explicitly allowed, and writes HTML, JSON, and CSV reports.

## Problem

Enterprise service testing often depends on manual SOAP UI collections, ad hoc scripts, or environment-specific notes. That creates several issues:

- Regression checks are difficult to repeat consistently.
- SOAP and REST calls are scattered across tools and personal workspaces.
- State-changing operations can be triggered accidentally.
- Test evidence is hard to collect in a standard report format.
- CI validation is limited when service inventories are not versioned.

## Solution

Sentinel API Assurance organizes service checks as config-driven suites:

- Test suites define service, operation, request file, expected HTTP status, and assertions.
- Request templates use placeholders such as `{{CustomerId}}` and `{{ENV:API_TOKEN}}`.
- SOAP and REST executors build outbound requests from configuration.
- The assertion engine validates response body, XML elements, JSON fields, duration, and HTTP status.
- Dry-run mode validates suite, service registry, request templates, and safety skips without sending API requests.
- Risky operations are blocked by default unless a case explicitly enables them.
- Reports are exported as HTML, JSON, and CSV.

## Design Philosophy

Sentinel API Assurance is designed around three principles:

1. Safety first  
   Risky operations should never be executed accidentally. Dry-run and operation guards reduce the chance of destructive test execution.

2. Repeatable validation  
   API checks should be suite-based, versionable, and runnable from CI to reduce manual verification effort.

3. Observable results  
   Test results should be exported in machine-readable and human-readable formats for debugging, reporting, and audit purposes.

## Architecture

```text
User / CI
  -> Test Suite Loader
  -> Request Template Rendering
  -> SOAP / REST Client
  -> Assertion Engine
  -> Report Generator
```

The main runtime flow is:

1. Load `appsettings.json`.
2. Resolve the selected environment and service registry.
3. Load a JSON test suite or legacy `<call />` XML list.
4. Run dry-run validation or execute active test cases.
5. Apply risky operation guard before sending requests.
6. Execute SOAP or REST request.
7. Evaluate assertions.
8. Write HTML, JSON, and CSV reports.

More detail is available in [docs/architecture.md](docs/architecture.md).

## Core Features

- SOAP operation testing
- REST endpoint testing
- Suite-based execution
- Legacy `<call active="true" service="..." operation="..." />` import
- Dry-run mode
- Risky operation guard
- Config-driven test cases
- Template variables for request body, headers, and REST path values
- HTTP status, text, XML, JSON, SOAP fault, and duration assertions
- HTML, JSON, and CSV report generation
- GitHub Actions CI validation

## Tech Stack

- .NET 8
- C#
- xUnit for automated tests
- GitHub Actions for CI
- JSON and XML configuration/request handling
- Console application CLI

## Project Structure

```text
.
|-- .github/
|   |-- ISSUE_TEMPLATE/
|   |-- workflows/
|   `-- pull_request_template.md
|-- docs/
|-- examples/
|-- src/
|   `-- SentinelApiAssurance/
|       |-- Catalog/
|       |-- Configuration/
|       |-- Execution/
|       |-- Models/
|       |-- Reporting/
|       |-- Requests/
|       |-- Safety/
|       |-- Services/
|       |-- Suites/
|       |-- Utilities/
|       |-- appsettings.json
|       |-- SentinelApiAssurance.csproj
|       `-- test-calls.xml
|-- tests/
|   `-- SentinelApiAssurance.Tests/
|-- CHANGELOG.md
|-- CONTRIBUTING.md
|-- LICENSE
|-- README.md
|-- SECURITY.md
`-- SentinelApiAssurance.sln
```

## Getting Started

Clone and build:

```bash
git clone https://github.com/Yakup24/Sentinel-API-Assurance.git
cd Sentinel-API-Assurance
dotnet restore SentinelApiAssurance.sln
dotnet build SentinelApiAssurance.sln --configuration Release
```

Run tests:

```bash
dotnet test SentinelApiAssurance.sln --configuration Release
```

Run dry-run validation:

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

Run the default suite:

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB
```

## Configuration

Runtime configuration lives in [src/SentinelApiAssurance/appsettings.json](src/SentinelApiAssurance/appsettings.json).

Key sections:

- `DefaultEnvironment`: default environment name.
- `DefaultSuitePath`: JSON suite loaded when `--suite` is not provided.
- `Environments`: base URL, headers, and service registry per environment.
- `GlobalHeaders`: headers applied to every request.
- `TestData`: placeholder values used by request templates.
- `DangerousOperationKeywords`: centralized keywords used by the risky operation guard.

Template values:

```xml
<customerId>{{CustomerId}}</customerId>
<token>{{ENV:SENTINEL_API_TOKEN}}</token>
```

Use placeholders only. Do not commit real credentials, customer data, tokens, or internal endpoint details.

## Usage

Validate suite structure without sending requests:

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

Run a specific suite:

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --suite Suites/voltran-enterprise-regression-suite.json
```

Run legacy call XML:

```bash
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --env STB --calls test-calls.xml
```

Reports are written under the configured report directory after execution.

## Testing

The repository includes an xUnit test project under [tests/SentinelApiAssurance.Tests](tests/SentinelApiAssurance.Tests).

Current automated tests cover:

- SOAP envelope, SOAPAction, and header generation through a fake HTTP client
- REST method, path template rendering, and body template rendering
- HTTP status, text, XML, JSON, SOAP fault, and duration assertions
- Risky operation blocking behavior
- JSON and CSV report writer behavior
- Configuration validation for invalid suite/config inputs

Planned test expansion:

- Contract tests against mock WSDL/OpenAPI definitions
- More negative test cases for malformed SOAP and REST responses
- Dedicated mock service for end-to-end integration validation

## Reporting

The current implementation writes:

- HTML summary report
- JSON result report
- CSV result report

Report writers are implemented in `src/SentinelApiAssurance/Reporting`.

## Security and Safety

Sentinel API Assurance is built with safety-first defaults:

- State-changing operations are blocked by default.
- Dry-run mode checks suite configuration without sending requests.
- Secrets should be injected with environment variables, not committed.
- Request examples use placeholder values only.
- Production environments should not be targeted without explicit approval and controlled test data.

Current limitation: log masking is not implemented yet. Sensitive values should not be placed in request templates or configuration until masking support is added.

More detail is available in [docs/security-model.md](docs/security-model.md) and [docs/operation-safety.md](docs/operation-safety.md).

## Limitations

- Real service credentials are not stored in this repository.
- Request XML files are generated templates and may need WSDL-specific field alignment before live use.
- Some services require mock or controlled test environments.
- Production usage requires explicit approval and test data governance.
- Coverage percentages are not claimed unless produced by a real test run.
- The current CLI is intentionally focused on batch execution and does not include a dashboard.

## Roadmap

- WSDL/OpenAPI operation discovery
- Stronger config schema validation
- Log masking for sensitive values
- More assertion types
- Docker support
- Report dashboard
- Coverage badge after CI coverage collection is added

## My Contributions

This repository currently includes:

- .NET 8 console framework structure
- SOAP and REST executors
- Suite-based test runner
- Legacy XML call importer
- Risky operation guard
- Dry-run validation
- HTML, JSON, and CSV reports
- VOLTRAN-style operation catalog and request templates
- xUnit test project for key framework behavior
- GitHub Actions CI workflow
- Security, contribution, changelog, and issue template documentation

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
