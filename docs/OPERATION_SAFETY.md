# Operation Safety Policy

Sentinel API Assurance is designed to protect test environments from accidental state-changing calls.

## Why this matters

Many SOAP operations are not read-only. Some operations may create orders, update addresses, deactivate services, cancel billing processes or change customer state.

Running these operations with invalid or production-like data can cause operational risk.

## Default behavior

The framework blocks risky operations unless the test case explicitly allows them.

The blocking decision is handled by `OperationSafetyPolicy`.

## Blocked keyword examples

Operations containing these keywords are treated as risky by default:

- `submit`
- `create`
- `activate`
- `deactivate`
- `deactivation`
- `delete`
- `remove`
- `cancel`
- `upsert`
- `update`
- `set`
- `unset`
- `insert`
- `add`
- `change`
- `order`
- `callback`
- `inform`
- `correction`

The keyword list is managed in `appsettings.json`.

## Explicit approval

A state-changing operation can only run when the test case contains:

```json
"AllowStateChangingOperation": true
```

Example:

```json
{
  "Id": "ADDR-STATE-001",
  "Service": "AddressOperations_v1.0",
  "Operation": "upsertPostalAddress",
  "AllowStateChangingOperation": true
}
```

## Recommended approval rules

Before enabling a risky operation, confirm that:

1. The target environment is not production.
2. The request uses dedicated test data.
3. The operation is reversible or cleanup is documented.
4. The expected side effect is known.
5. The run is reviewed by QA or integration owner.

## Safe test data principles

Use clearly fake but structurally valid data:

- Dedicated test MSISDNs
- Dedicated test customer IDs
- Dedicated test invoice IDs
- Dedicated test dealer IDs
- Dedicated test addresses

Avoid:

- Real customer identifiers
- Real billing records
- Real SIM card numbers
- Real production tokens
- Any personal data

## Suggested future improvement

Add an approval metadata block to each state-changing test case:

```json
"Approval": {
  "ApprovedBy": "qa.owner",
  "Reason": "STB-only address update regression",
  "CleanupProcedure": "Reset test address after execution"
}
```

This would make state-changing test execution auditable.
