using Microsoft.AspNetCore.Components;

namespace DraftKings.LineupGenerator.Razor.Components
{
    public abstract class ContestComponent : ComponentBase
    {
        protected abstract Task RemoveContestAsync();
    }
}
