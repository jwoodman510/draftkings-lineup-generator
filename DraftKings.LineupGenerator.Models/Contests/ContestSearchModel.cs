using Newtonsoft.Json;

namespace DraftKings.LineupGenerator.Models.Contests
{
    public class ContestSearchModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("mec")]
        public int MultiEntries { get; set; }

        [JsonProperty("nt")]
        public int Entries { get; set; }

        [JsonProperty("m")]
        public int MaxEntries { get; set; }

        [JsonProperty("a")]
        public decimal EntryFee { get; set; }

        [JsonProperty("po")]
        public decimal Prizes { get; set; }

        [JsonProperty("sdstring")]
        public string StartDateTimeDecription { get; set; }

        [JsonProperty("gameType")]
        public string GameType { get; set; }
    }
}
