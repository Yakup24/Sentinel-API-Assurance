# Test Strategy

Sentinel API Assurance supports layered service validation.

## Test levels

### 1. Dry-run validation

Purpose: Validate suite integrity without sending network requests.

Checks:

- Active case count
- Service registry presence
- Missing request templates
- Safety policy status

Command:

```powershell
dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run
```

### 2. Smoke tests

Purpose: Quickly verify that critical read-only service operations are reachable.

Recommended characteristics:

- Small number of cases
- Mostly read-only operations
- Low execution time
- No state-changing calls

Suite:

```text
Suites/voltran-smoke-suite.json
```

### 3. Regression tests

Purpose: Validate broad service behavior before release or integration migration.

Recommended characteristics:

- Full service coverage
- Stable test data
- Assertions beyond HTTP 200
- Response time threshold checks

Suite:

```text
Suites/voltran-enterprise-regression-suite.json
```

### 4. State-changing tests

Purpose: Validate create/update/submit/deactivate flows with approved test data.

Rules:

- Disabled by default
- Must use dedicated test records
- Must document cleanup
- Must be reviewed before enabling

## Assertion strategy

Do not rely only on HTTP 200.

Recommended minimum assertions for SOAP cases:

```json
[
  { "Type": "NoSoapFault" },
  { "Type": "MaxDurationMs", "MaxDurationMs": 5000 }
]
```

For stronger validation, add response-specific assertions:

```json
[
  { "Type": "XmlElementExists", "ElementName": "addressId" },
  { "Type": "NotContains", "Value": "Exception" }
]
```

## Test data strategy

Use stable, reusable and clearly fake test data.

Recommended test data classes:

- Valid active MSISDN
- Valid inactive MSISDN
- Customer with invoice
- Customer without invoice
- Test SIM card
- Test dealer
- Test cost center
- Test product ID

## Naming convention

Recommended test case ID format:

```text
<SERVICE_PREFIX>-<NUMBER>
```

Examples:

```text
ADDR-001
INV-001
SIM-001
PRD-001
```

## CI strategy

Every push and pull request should run:

1. Restore
2. Build
3. Dry-run

Live endpoint tests should be run only when secure network access and test credentials are available.

## Quality gates

A build should fail when:

- Project does not compile
- Dry-run fails because of invalid config
- Required request templates are missing
- Suite references unknown services

A live regression run should fail when:

- HTTP status does not match expectation
- SOAP Fault is detected
- Mandatory response element is missing
- Response time exceeds threshold

## Future improvements

- JUnit XML output for test report visualization
- Data-driven test cases
- WSDL-based request validation
- Response schema validation
- Historical trend reporting
- Dashboard for service-level quality status
