// Models/ViewModels/LeaderboardViewModel.cs
namespace FootballBetting.Models.ViewModels
{
    public class LeaderboardViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Points { get; set; }
        public int NumberOfPredictions { get; set; }
        public int Position { get; set; }
    }
}