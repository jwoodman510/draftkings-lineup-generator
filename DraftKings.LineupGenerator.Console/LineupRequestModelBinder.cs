using DraftKings.LineupGenerator.Models.Lineups;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Linq;

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
        private readonly Option<string> _outputFormat;
        private readonly Option<int> _lineupCountOption;
        private readonly Option<string> _giveMeOption;
        private readonly Option<string> _excludePlayerOption;
        private readonly Option<string> _giveMeCaptainOption;
        private readonly Option<string> _excludeCaptainOption;

        public LineupRequestModelBinder(
            Option<int> contestIdOption,
            Option<bool> includeQuestionableOption,
            Option<bool> includeBaseSalaryOption,
            Option<decimal> minFppgOption,
            Option<bool> excludeDefense,
            Option<bool> excludeKickers,
            Option<string> outputFormat,
            Option<int> lineupCountOption,
            Option<string> giveMeOption,
            Option<string> excludePlayerOption,
            Option<string> giveMeCaptainOption,
            Option<string> excludeCaptainOption)
        {
            _contestIdOption = contestIdOption;
            _includeQuestionableOption = includeQuestionableOption;
            _includeBaseSalaryOption = includeBaseSalaryOption;
            _minFppgOption = minFppgOption;
            _excludeDefense = excludeDefense;
            _excludeKickers = excludeKickers;
            _outputFormat = outputFormat;
            _lineupCountOption = lineupCountOption;
            _giveMeOption = giveMeOption;
            _excludePlayerOption = excludePlayerOption;
            _giveMeCaptainOption = giveMeCaptainOption;
            _excludeCaptainOption = excludeCaptainOption;
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
                ExcludeKickers = bindingContext.ParseResult.GetValueForOption(_excludeKickers),
                OutputFormat = bindingContext.ParseResult.GetValueForOption(_outputFormat),
                LineupCount = bindingContext.ParseResult.GetValueForOption(_lineupCountOption),
                PlayerRequests = new PlayerRequestsModel
                {
                    PlayerNameRequests = ParseCommaDelimitedOption(bindingContext, _giveMeOption),
                    CaptainPlayerNameRequests = ParseCommaDelimitedOption(bindingContext, _giveMeCaptainOption),
                    PlayerNameExclusionRequests = ParseCommaDelimitedOption(bindingContext, _excludePlayerOption),
                    CaptainPlayerNameExclusionRequests = ParseCommaDelimitedOption(bindingContext, _excludeCaptainOption)
                }
            };
        }

        private HashSet<string> ParseCommaDelimitedOption(BindingContext bindingContext, Option<string> option)
        {
            return bindingContext.ParseResult.GetValueForOption(option)
                        ?.Split(',')
                        ?.Select(x => x.Trim())
                        ?.Where(x => !string.IsNullOrEmpty(x))
                        ?.ToHashSet(StringComparer.OrdinalIgnoreCase)
                        ?? new HashSet<string>();
        }
    }
}
