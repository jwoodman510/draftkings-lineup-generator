using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;

namespace DraftKings.LineupGenerator.Api
{
    public class DraftKingsClient : IDraftKingsClient
    {
        public DraftKingsClient(
            IRulesClient rulesClient,
            IDraftablesClient draftablesClient,
            IContestsClient contests)
        {
            Rules = rulesClient;
            Draftables = draftablesClient;
            Contests = contests;
        }

        public IRulesClient Rules { get; init; }

        public IDraftablesClient Draftables { get; init; }

        public IContestsClient Contests { get; init; }
    }
}
