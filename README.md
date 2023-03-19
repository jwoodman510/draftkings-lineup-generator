# draftkings-lineup-generator

### DraftKings.LineupGenerator.Console

#### Command Line Arguments:
  - Required:
    - `--contestId {int}` Contest identifier from URL
  - Optional:
    - `--include-questionable` *[default: false]* Includes draftables with a questionable status.
    - `--include-base-salary` *[default: false]* Includes draftables with the base salary (lowest possible).
    - `--min-fppg` *[default: 10.0]* Minimum fantasy points per game (per player - excluding DST).
