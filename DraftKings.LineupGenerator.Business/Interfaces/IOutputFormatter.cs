using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IOutputFormatter
    {
        string Type { get; }

        Task<string> FormatAsync<T>(T value, CancellationToken cancellationToken = default) where T : class;

        Task<string> FormatLineupsAsync(IEnumerable<LineupsModel> lineups, CancellationToken cancellationToken = default);

        Task<string> FormatLineupsAsync(IEnumerable<LineupModel> lineups, CancellationToken cancellationToken = default);
    }
}
