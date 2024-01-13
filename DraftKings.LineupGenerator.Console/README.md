## DraftKings.LineupGenerator.Console

### Install as a .NET CLI tool from [NuGet](https://www.nuget.org/packages/Woodman.DKGen/)
```
dotnet tool install --global Woodman.DKGen
```

## Command Line Arguments

| Name | Description | Default | Short Alias |
| ---- | ----------- | ------- | ----------- |
| --contestId | Required DraftKings contest numeric identifier |  | -c |
| --include-questionable | Includes draftables with a questionable status. | false | -iq |
| --include-base-salary | Includes draftables with the base salary (lowest possible). | false | -ibs |
| --output-format | The console output format (json or text). | text | -f |
| --give-me | The names of players to include in generated lineups (comma delimited) | | -g |
| --exclude-player | The names of players to exclude in generated lineups (comma delimited) | | -ep |

## Classic Contest Command Line Arguments
| Name | Description | Default | Short Alias |
| ---- | ----------- | ------- | ----------- |
| --min-fppg | Minimum fantasy points per game (per player - excluding DST). | 10.0 | -mp |

## Showdown Contest Command Line Arguments
| Name | Description | Default | Short Alias |
| ---- | ----------- | ------- | ----------- |
| --exclude-defense | Excludes DST positions from lineups. | false | -ed |
| --exclude-kickers | Excludes Kicker positions from lineups. | false | -ek |
| --give-me-captain | The names of captain players to include in generated lineups (comma delimited) | | -gc |
| --exclude-captain | The names of captain players to exclude in generated lineups (comma delimited) | | -ec |

## Example Command:
The following command will generate lineups including questionable players, only including lineups with Josh Allen and Travic Kelce

```
dotnet tool install --global Woodman.DKGen

dkgen -c 1234 -iq -g "Josh Allen, Kelce"
```

