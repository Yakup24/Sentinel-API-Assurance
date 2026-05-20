# Operation Safety

## Risky Operation Examples

The framework treats state-changing operations as risky when their operation name contains configured keywords such as:

- `create`
- `update`
- `delete`
- `deactivate`
- `activate`
- `payment`
- `submit`
- `remove`
- `cancel`
- `upsert`
- `insert`

## Current Approach

Risky operations are blocked by default when `BlockDangerousOperationsWithoutExplicitApproval` is enabled.

A test case can explicitly opt in:

```json
"AllowStateChangingOperation": true
```

This should only be used with approved test data and controlled environments.

## Read-Only Detection

Operations starting with read-oriented prefixes such as `get`, `read`, `search`, `list`, `load`, `query`, `find`, `compare`, `check`, or `is` are treated as safe candidates.

## Dry-Run Behavior

Dry-run mode does not send real requests. It shows which cases would be blocked by safety policy.

## Allowlist Roadmap

The current model is keyword-based with per-case explicit approval. A stricter service/operation allowlist is planned for environments where production-like services must be protected more aggressively.
