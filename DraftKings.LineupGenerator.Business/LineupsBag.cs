using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Concurrent;

namespace DraftKings.LineupGenerator.Business
{
    public class LineupsBag : ConcurrentDictionary<decimal, ConcurrentBag<LineupModel>>
    {
        private readonly ConcurrentDictionary<decimal, ConcurrentBag<LineupModel>> _lineupsBag = new();
    }
}
