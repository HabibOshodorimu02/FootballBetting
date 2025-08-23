using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Models
{
    [Table("FactTables")]
    public class FactTable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;

        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int TotalPredictions { get; set; } = 0;

        [NotMapped]
        public double WinPercentage =>
            TotalPredictions > 0
                ? (double)Wins / TotalPredictions * 100
                : 0;
    }
}
