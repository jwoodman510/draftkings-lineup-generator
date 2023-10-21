# draftkings-lineup-generator

### DraftKings.LineupGenerator.Console

#### Command Line Arguments:
  - Required:
    - `--contestId {int}` Contest identifier from URL
  - Optional:
    - `--include-questionable` *[default: false]* Includes draftables with a questionable status.
    - `--include-base-salary` *[default: false]* Includes draftables with the base salary (lowest possible).
    - `--output-format` *[default=text]* The console output format. One of (json | text).
    - `--give-me` The names of players to include in generated lineups (comma delimited) e.g. `--give-me "mahomes, travis kelce"`
    - `--exclude-player` The names of players to exclude in generated lineups (comma delimited)
  - Option (Classic Only):
    - `--min-fppg` *[default: 10.0]* Minimum fantasy points per game (per player - excluding DST).
  - Option (Showdown Only):
    - `--exclude-defense` *[default: false]* Excludes DST positions from lineups.
    - `--exclude-kickers` *[default: false]* Excludes Kicker positions from lineups.
    - `--give-me-captain` The names of captain players to include in generated lineups (comma delimited) e.g. `--give-me-captain "mahomes, travis kelce"`
    - `--exclude-captain` The names of captain players to exclude in generated lineups (comma delimited)
