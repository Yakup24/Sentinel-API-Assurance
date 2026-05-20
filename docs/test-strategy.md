# Test Strategy

## Test Levels

### Unit Tests

Unit tests verify framework components without calling external systems. Current tests cover template rendering, assertion behavior, risky operation guard, config validation, report writers, and request creation through fake HTTP handlers.

### Integration Tests

Integration tests should run against mock SOAP/REST services or controlled non-production services. They are planned and should not depend on real production endpoints.

### Contract Tests

Contract tests should validate request and response shapes against WSDL or OpenAPI definitions. This is planned.

### Regression Tests

Regression suites are represented by JSON files under `Suites`. These are intended to be versioned and executed repeatedly across environments.

### Smoke Tests

Smoke suites should contain a small number of safe read-only checks that confirm service availability.

### Negative Tests

Negative tests should cover SOAP faults, invalid response bodies, missing configuration, invalid suite definitions, and blocked risky operations.

## What Should Be Tested?

- Request template rendering
- SOAP envelope and SOAPAction construction
- REST method, path, and body generation
- Response parsing
- Assertion rules
- Risky operation guard behavior
- Report generation
- Invalid config and invalid suite inputs

## What Should Not Be Tested Directly?

- Real destructive production operations
- Real credentials or tokens
- Real customer data
- Uncontrolled live endpoints
- Denial-of-service behavior against service environments

## Test Data Strategy

Use placeholder data in the repository. Real environment-specific values should come from secure runtime configuration or environment variables.

## Mock Service Strategy

Framework-level tests should use fake HTTP handlers. Broader integration tests can use local mock services that return deterministic SOAP and REST payloads.

## CI Test Strategy

CI should run restore, build, unit tests, format verification, and dry-run suite validation. CI should not call real external service environments.
