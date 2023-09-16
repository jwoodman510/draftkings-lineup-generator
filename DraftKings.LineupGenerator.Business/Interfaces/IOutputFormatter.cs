using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IOutputFormatter
    {
        string Type { get; }

        Task<string> FormatAsync(IEnumerable<LineupsModel> lineups, CancellationToken cancellationToken = default);

        Task<string> FormatAsync(IEnumerable<LineupModel> lineups, CancellationToken cancellationToken = default);
    }
}
