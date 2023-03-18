using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;

namespace DraftKings.LineupGenerator.Api
{
    public interface IDraftKingsClient
    {
        IRulesClient Rules { get; }

        IDraftablesClient Draftables { get; }
    }
}
