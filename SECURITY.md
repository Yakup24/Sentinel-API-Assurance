# Security Policy

## Supported Versions

Sentinel API Assurance is currently maintained from the `main` branch. Security fixes are applied to the latest source code until the project starts publishing versioned releases.

| Version | Supported |
| ------- | --------- |
| `main`  | Yes       |
| Older commits or forks | No |

## Reporting a Vulnerability

Please report suspected vulnerabilities privately. Do not open a public GitHub issue for security-sensitive findings.

Use one of these options:

- Open a private vulnerability report through GitHub Security Advisories if it is available for this repository.
- If private reporting is not available, contact the repository owner through GitHub: [Yakup24](https://github.com/Yakup24).

When reporting, include as much detail as possible:

- Affected component, file, endpoint, or workflow
- Steps to reproduce
- Expected and actual impact
- Any logs, proof of concept, or screenshots that help explain the issue
- Suggested fix, if you already have one

The maintainer will try to acknowledge valid security reports within 7 days. Accepted vulnerabilities will be investigated and fixed in the default branch as soon as reasonably possible.

## Secret Management

Do not commit real service credentials, API tokens, customer data, or internal endpoint details.

Use placeholders in committed configuration and inject sensitive values at runtime through environment variables:

```xml
<token>{{ENV:SENTINEL_API_TOKEN}}</token>
```

## Sensitive Data Handling

Examples, suites, and request templates should use synthetic identifiers only. Avoid storing real MSISDN values, customer IDs, invoice IDs, LDAP names, access tokens, or internal hostnames.

Current limitation: log masking is not implemented. Do not run with sensitive request data until masking is added or logs are controlled by environment policy.

## Risky Operation Policy

State-changing operations are blocked by default through `OperationSafetyPolicy`. Operations containing keywords such as `create`, `update`, `delete`, `deactivate`, `payment`, or `submit` require explicit case-level approval.

Use `AllowStateChangingOperation=true` only with approved test data and controlled non-production services.

## Production Usage Warning

This framework is not intended to trigger destructive production operations without explicit approval, controlled test data, and environment-specific safety controls.

## Out Of Scope

- Findings that require unauthorized access to third-party systems
- Denial-of-service testing against live service environments
- Issues caused only by intentionally modified unsafe local configuration
