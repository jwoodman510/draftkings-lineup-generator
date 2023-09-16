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

        private const string LineSeperator = "=====================================================";

        public Task<string> FormatAsync(IEnumerable<LineupsModel> lineups, CancellationToken cancellationToken = default)
        {
            var lineupsModel = lineups.ToList();

            if (!lineupsModel.Any(x => x.Lineups?.Count > 0))
            {
                return Task.FromResult(string.Empty);
            }

            var uniquePositions = GetUniquePositions(lineupsModel.Select(x => x.Lineups.First()));

            var records = lineupsModel
                .SelectMany(x => x.Lineups)
                .OrderByDescending(x => x.Draftables.Sum(x => x.ProjectedFppg))
                .Select(x => GetRecords(x, uniquePositions));

            var output = Format(records);

            return Task.FromResult(output);
        }

        public Task<string> FormatAsync(IEnumerable<LineupModel> lineups, CancellationToken cancellationToken = default)
        {
            var lineupsList = lineups.ToList();

            if (!lineupsList.Any())
            {
                return Task.FromResult(string.Empty);
            }

            var uniquePositions = GetUniquePositions(lineupsList);

            var records = lineupsList
                .OrderByDescending(x => x.Draftables.Sum(x => x.ProjectedFppg))
                .Select(x => GetRecords(x, uniquePositions));

            var output = Format(records);

            return Task.FromResult(output);
        }

        private static string Format(IEnumerable<IEnumerable<string>> records)
        {
            var output = new StringBuilder();

            output.AppendLine(LineSeperator);

            foreach (var record in records)
            {
                foreach (var item in record)
                {
                    output.AppendLine(item);
                }

                output.AppendLine(LineSeperator);
            }

            return output.ToString();
        }

        private static IEnumerable<string> GetRecords(LineupModel lineup, List<string> uniquePositions)
        {
            var records = new List<string>();

            if (!string.IsNullOrEmpty(lineup.Description))
            {
                records.Add($"Generator: {lineup.Description}");
            }

            records.Add($"Projected Points: {lineup.Draftables.Sum(x => x.ProjectedFppg)}");

            var draftables = lineup.Draftables.OrderBy(x => x.RosterPosition).ToList();

            for (var i = 0; i < lineup.Draftables.Count; i++)
            {
                var draftable = draftables[i];
                var position = uniquePositions[i];

                records.Add($"[{position}]\t{draftable.Name} ({draftable.ProjectedFppg})");
            }

            return records;
        }

        private static List<string> GetUniquePositions(IEnumerable<LineupModel> lineupsModel)
        {
            var positions = lineupsModel
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
