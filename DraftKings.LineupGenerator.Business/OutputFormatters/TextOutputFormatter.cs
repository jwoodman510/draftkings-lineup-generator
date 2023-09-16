using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.OutputFormatters
{
    public class TextOutputFormatter : IOutputFormatter
    {
        public string Type => "text";

        public Task<string> FormatAsync(IEnumerable<LineupsModel> lineups, CancellationToken cancellationToken = default)
        {
            var lineupsModel = lineups.ToList();

            if (!lineupsModel.Any(x => x.Lineups?.Count > 0))
            {
                return Task.FromResult(string.Empty);
            }

            var output = new StringBuilder();

            var uniquePositions = GetUniquePositions(lineupsModel);

            var longestNameLength = lineups
                .SelectMany(x => x.Lineups)
                .SelectMany(x => x.Draftables)
                .Select(x => x.Name)
                .Max(x => x.Length);

            var records = lineupsModel.SelectMany(x =>
            {
                return x.Lineups.Select(lineup =>
                {
                    var dict = new Dictionary<string, string>
                    {
                        ["Generator"] = x.Description,
                        ["Projected Points"] = lineup.Draftables.Sum(x => x.ProjectedFppg).ToString()
                    };

                    var draftables = lineup.Draftables.OrderBy(x => x.RosterPosition).ToList();

                    for (var i = 0; i < lineup.Draftables.Count; i++)
                    {
                        var draftable = draftables[i];
                        var position = uniquePositions[i];

                        dict[position] = $"[{position}] {draftable.Name.PadRight(longestNameLength, ' ')}";
                    }

                    return dict;
                });
            })
            .OrderByDescending(x => x["Projected Points"])
            .ToList();

            foreach (var item in records)
            {
                output.Append("| ");

                foreach (var key in item.Keys)
                {
                    output.Append(item[key]);
                    output.Append(" | ");
                }

                output.AppendLine();
            }

            return Task.FromResult(output.ToString());
        }

        private static List<string> GetUniquePositions(List<LineupsModel> lineupsModel)
        {
            var positions = lineupsModel
                .Select(x => x.Lineups.First())
                .First().Draftables
                .Select(x => x.RosterPosition.ToString())
                .OrderBy(x => x)
                .ToList();

            var uniquePositions = new List<string>();
            var positionMatches = positions.Distinct().ToDictionary(x => x, _ => 0);

            foreach (var position in positions)
            {
                var positionCount = positionMatches[position];

                var uniquePosition = positionCount > 0 ? $"{position}{positionCount}" : position;

                uniquePositions.Add(uniquePosition);

                positionMatches[position] = positionMatches[position] + 1;
            }

            return uniquePositions;
        }
    }
}
