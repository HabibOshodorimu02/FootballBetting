using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Models
{
    [Table("Predictions")]
    public class Prediction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [Required]
        public int MatchId { get; set; }
        public virtual Match Match { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Predicted Outcome")]
        public string PredictedOutcome { get; set; } = string.Empty; // "Home Win", "Away Win", "Draw"

        [Display(Name = "Predicted Home Score")]
        public int? PredictedHomeScore { get; set; }

        [Display(Name = "Predicted Away Score")]
        public int? PredictedAwayScore { get; set; }

        [Display(Name = "Points Earned")]
        public int PointsEarned { get; set; } = 0;

        [Display(Name = "Prediction Date")]
        public DateTime PredictionDate { get; set; } = DateTime.Now;

        public bool IsProcessed { get; set; } = false;
    }
}
