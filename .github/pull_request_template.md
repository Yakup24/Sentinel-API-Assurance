## Summary

Describe what changed and why.

## Validation

- [ ] `dotnet format SentinelApiAssurance.sln --verify-no-changes`
- [ ] `dotnet build SentinelApiAssurance.sln --configuration Release`
- [ ] `dotnet test SentinelApiAssurance.sln --configuration Release`
- [ ] `dotnet run --project src/SentinelApiAssurance/SentinelApiAssurance.csproj -- --dry-run`

## Safety Checklist

- [ ] No real credentials, tokens, customer data, or internal endpoints were committed.
- [ ] Risky operations remain blocked by default.
- [ ] Documentation matches actual behavior.
- [ ] New report or assertion behavior includes tests.

## Notes

Add implementation details, trade-offs, or follow-up work.
