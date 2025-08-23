using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Models
{
    [Table("Matches")]
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Home Team")]
        public int HomeTeamId { get; set; }
        public virtual Team HomeTeam { get; set; } = null!;

        [Required]
        [Display(Name = "Away Team")]
        public int AwayTeamId { get; set; }
        public virtual Team AwayTeam { get; set; } = null!;

        [Required]
        [Display(Name = "Match Date")]
        public DateTime MatchDate { get; set; }

        [Display(Name = "Home Score")]
        public int? HomeScore { get; set; }

        [Display(Name = "Away Score")]
        public int? AwayScore { get; set; }

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; } = false;

        public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

        // Helper property to get match result
        [NotMapped]
        public string Result
        {
            get
            {
                if (!IsCompleted || !HomeScore.HasValue || !AwayScore.HasValue)
                    return "Pending";

                if (HomeScore > AwayScore) return "Home Win";
                if (AwayScore > HomeScore) return "Away Win";
                return "Draw";
            }
        }
    }
}
