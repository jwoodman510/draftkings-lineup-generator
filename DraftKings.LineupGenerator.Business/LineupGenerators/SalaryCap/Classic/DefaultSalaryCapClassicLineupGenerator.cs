using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupBags;

namespace DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic
{
    /// <summary>
    /// The default lineup generator for salary cap classic contests based on FPPG.
    /// Currently only supports Madden, NFL, and XFL game types.
    /// </summary>
    public class DefaultSalaryCapClassicLineupGenerator : BaseSalaryCapClassicLineupGenerator
    {
        public DefaultSalaryCapClassicLineupGenerator(
            IClassicLineupService classicLineupService,
            IIncrementalLineupLogger incrementalLogger)
            : base(new ProjectedPointsLineupsBag("FPPG"), classicLineupService, incrementalLogger)
        {

        }
    }
}
