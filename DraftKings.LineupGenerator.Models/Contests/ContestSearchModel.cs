using System.Text.Json.Serialization;

namespace DraftKings.LineupGenerator.Models.Contests
{
    public class ContestSearchModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("n")]
        public string Name { get; set; }

        [JsonPropertyName ("mec")]
        public int MultiEntries { get; set; }

        [JsonPropertyName("nt")]
        public int Entries { get; set; }

        [JsonPropertyName("m")]
        public int MaxEntries { get; set; }

        [JsonPropertyName("po")]
        public decimal Prizes { get; set; }

        [JsonPropertyName("sdstring")]
        public string StartDateTimeDecription { get; set; }

        [JsonPropertyName("gameType")]
        public string GameType { get; set; }
    }
}
