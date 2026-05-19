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

## Response Expectations

The maintainer will try to acknowledge valid security reports within 7 days. Accepted vulnerabilities will be investigated and fixed in the default branch as soon as reasonably possible.

If the report is accepted, the fix may be handled privately until a patch is available. If the report is declined, the maintainer will explain why it is not considered a security issue.

## Scope

Security reports are most useful when they involve:

- Secret leakage or unsafe handling of credentials
- Unsafe logging of sensitive data such as MSISDN, customer IDs, tokens, or invoice identifiers
- Unsafe default behavior that can trigger state-changing SOAP/API operations
- CI/CD configuration risks
- XML, SOAP, or request-template handling issues

Out of scope:

- Vulnerabilities caused only by intentionally modified local configuration
- Findings that require access to private systems without authorization
- Denial-of-service tests against real service environments
