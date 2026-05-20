# Security Model

## Secret Handling

Secrets must not be committed to the repository. Configuration files should contain placeholders only.

Use environment variable placeholders when a runtime value is needed:

```xml
<token>{{ENV:SENTINEL_API_TOKEN}}</token>
```

## Environment Variables

The template renderer supports `{{ENV:VARIABLE_NAME}}` syntax. If the environment variable is not set, the token remains unchanged.

## Config Placeholder Usage

`appsettings.json` includes placeholder test data for local and CI validation. Real service credentials, API tokens, customer data, or internal endpoint details should never be committed.

## Risky Operation Blocking

The safety policy checks operation names against centrally configured keywords such as `create`, `update`, `delete`, `deactivate`, `payment`, and `submit`.

Read-only prefixes such as `get`, `read`, `search`, and `is` are allowed by default.

## Dry-Run Mode

Dry-run mode validates suites, service registry entries, request templates, and safety skips without sending SOAP or REST requests.

## Logging Safety

Current logging writes runtime messages to local log files. Log masking is not implemented yet, so sensitive values should not be placed into request templates or configuration.

## Production Usage Warning

This framework is not designed to trigger destructive operations against production services without strict approval, controlled test data, and environment-specific safeguards.

## Responsible Usage

Use this framework for repeatable validation, not uncontrolled probing. Do not run tests against systems you are not authorized to test.
