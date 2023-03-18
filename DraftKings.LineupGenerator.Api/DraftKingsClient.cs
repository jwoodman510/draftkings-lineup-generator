using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;

namespace DraftKings.LineupGenerator.Api
{
    public class DraftKingsClient : IDraftKingsClient
    {
        public DraftKingsClient(
            IRulesClient rulesClient,
            IDraftablesClient draftablesClient)
        {
            Rules = rulesClient;
            Draftables = draftablesClient;
        }

        public IRulesClient Rules { get; init; }

        public IDraftablesClient Draftables { get; init; }
    }
}
