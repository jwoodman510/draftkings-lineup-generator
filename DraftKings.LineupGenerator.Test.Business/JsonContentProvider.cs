using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Test.Business
{
    public static class JsonContentProvider
    {
        public static async Task<T> GetAsync<T>(string fileName) where T : class
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", fileName);

            var json = await File.ReadAllTextAsync(path);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static Task<RulesModel> GetSalaryCapXflClassicRulesAsync() =>
            GetAsync<RulesModel>("salarycap_xfl_classic_rules.json");

        public static Task<DraftablesModel> GetSalaryCapXflClassicDraftablesAsync() =>
            GetAsync<DraftablesModel>("salarycap_xfl_classic_draftables.json");

        public static Task<RulesModel> GetSalaryCapMaddenShowdownRulesAsync() =>
            GetAsync<RulesModel>("salarycap_madden_showdown_rules.json");

        public static Task<DraftablesModel> GetSalaryCapMaddenShowdownDraftablesAsync() =>
            GetAsync<DraftablesModel>("salarycap_madden_showdown_draftables.json");
    }
}
