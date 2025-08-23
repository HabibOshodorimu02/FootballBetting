namespace FootballBetting.Models.ViewModels
{
    public class PredictionViewModel
    {
        public int MatchId { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string PredictedOutcome { get; set; } = string.Empty;
        public int? PredictedHomeScore { get; set; }
        public int? PredictedAwayScore { get; set; }

        public List<string> OutcomeOptions { get; set; } = new List<string>
        {
            "Home Win", "Away Win", "Draw"
        };
    }
}