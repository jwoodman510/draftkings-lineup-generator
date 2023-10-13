using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupBags;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap showdown contests based on FPPG.
    /// Currently only supports CFB, Madden, NFL, and XFL game types.
    /// </summary>
    public class DefaultSalaryCapShowdownLineupGenerator : BaseSalaryCapShowdownLineupGenerator
    {
        public DefaultSalaryCapShowdownLineupGenerator(
            IShowdownLineupService showdownLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(new ProjectedPointsLineupsBag("FPPG"), showdownLineupService, incrementalLogger)
        {

        }
    }
}
