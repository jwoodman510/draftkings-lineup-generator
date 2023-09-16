using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.OutputFormatters
{
    public class JsonOutputFormatter : IOutputFormatter
    {
        public string Type => "json";

        public Task<string> FormatAsync(IEnumerable<LineupsModel> lineups, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(JsonConvert.SerializeObject(lineups, Formatting.Indented));
        }
    }
}
