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

        public Task<string> FormatAsync<T>(T value, CancellationToken cancellationToken = default) where T : class
        {
            return Task.FromResult(JsonConvert.SerializeObject(value, Formatting.Indented));
        }

        public Task<string> FormatLineupsAsync(IEnumerable<LineupModel> lineups, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(JsonConvert.SerializeObject(lineups, Formatting.Indented));
        }
    }
}
