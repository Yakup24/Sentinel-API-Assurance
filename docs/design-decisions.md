# Design Decisions

## Why .NET and C#?

.NET provides a mature HTTP stack, XML handling, JSON serialization, cross-platform CLI support, and straightforward CI integration. C# is suitable for strongly typed test configuration and service execution logic.

## Why Suite-Based Execution?

Suite files make API checks versionable, reviewable, and repeatable. They also allow teams to separate smoke, regression, and environment-specific execution plans.

## Why Dry-Run Mode?

Dry-run provides a safe validation layer before any real network call is made. It is especially useful when a suite contains state-changing operations.

## Why HTML/JSON/CSV Reporting?

Different audiences need different report formats:

- HTML for quick human review
- JSON for automation and dashboards
- CSV for spreadsheet analysis

## Why Config-Driven Test Cases?

Config-driven cases keep the execution engine generic. New operations can be added by changing suite and request files instead of recompiling code.

## Why Risky Operation Guard?

SOAP/API regression suites often mix read-only and state-changing operations. The guard reduces accidental execution of destructive calls.

## Alternatives Considered

### Postman/Newman

Good for REST collections, but less natural for large SOAP inventories and custom safety policies.

### SoapUI

Strong SOAP tooling, but framework behavior, reporting, and CI policy are harder to version in code.

### Custom Scripts

Fast to start, but difficult to scale across many services and environments.

## Advantages and Trade-Offs

Advantages:

- Source-controlled suites and request templates
- Centralized safety policy
- CI-friendly console execution
- Extensible reporting and assertion model

Trade-offs:

- Requires request templates to match service contracts
- Does not replace full contract testing yet
- No dashboard or UI is currently included
