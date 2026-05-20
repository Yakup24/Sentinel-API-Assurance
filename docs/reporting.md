# Reporting

Sentinel API Assurance currently writes three report formats after a non-dry-run execution:

- HTML
- JSON
- CSV

## HTML Report

The HTML report is human-readable and includes:

- Suite name
- Environment
- Run start and finish time
- Total, passed, failed, and skipped counts
- Service-level summary
- Case-level details
- Assertion results

## JSON Report

The JSON report serializes the full `RunResult` object. It is intended for machine processing, CI artifacts, and later dashboard ingestion.

## CSV Report

The CSV report is a compact case-level summary that can be opened in spreadsheet tools.

## Dry-Run Output

Dry-run does not generate HTML/JSON/CSV files. It prints validation output to the console, including active cases, service count, safety skips, missing services, and missing request templates.

## Planned Improvements

- Stable latest-report filename or folder
- CI artifact upload
- Historical report comparison
- Dashboard-friendly summary output
- Optional sensitive-value masking in report output
