using DraftKings.LineupGenerator.Models.Lineups;
using System.CommandLine;
using System.CommandLine.Binding;

namespace DraftKings.LineupGenerator
{
    public class LineupRequestModelBinder : BinderBase<LineupRequestModel>
    {
        private readonly Option<int> _contestIdOption;
        private readonly Option<bool> _includeQuestionableOption;
        private readonly Option<bool> _includeBaseSalaryOption;
        private readonly Option<decimal> _minFppgOption;
        private readonly Option<bool> _excludeDefense;
        private readonly Option<bool> _excludeKickers;

        public LineupRequestModelBinder(
            Option<int> contestIdOption,
            Option<bool> includeQuestionableOption,
            Option<bool> includeBaseSalaryOption,
            Option<decimal> minFppgOption,
            Option<bool> excludeDefense,
            Option<bool> excludeKickers)
        {
            _contestIdOption = contestIdOption;
            _includeQuestionableOption = includeQuestionableOption;
            _includeBaseSalaryOption = includeBaseSalaryOption;
            _minFppgOption = minFppgOption;
            _excludeDefense = excludeDefense;
            _excludeKickers = excludeKickers;
        }

        protected override LineupRequestModel GetBoundValue(BindingContext bindingContext)
        {
            var contestId = bindingContext.ParseResult.GetValueForOption(_contestIdOption);

            return new LineupRequestModel(contestId)
            {
                IncludeQuestionable = bindingContext.ParseResult.GetValueForOption(_includeQuestionableOption),
                IncludeBaseSalary = bindingContext.ParseResult.GetValueForOption(_includeBaseSalaryOption),
                MinFppg = bindingContext.ParseResult.GetValueForOption(_minFppgOption),
                ExcludeDefense = bindingContext.ParseResult.GetValueForOption(_excludeDefense),
                ExcludeKickers = bindingContext.ParseResult.GetValueForOption(_excludeKickers)
            };
        }
    }
}
