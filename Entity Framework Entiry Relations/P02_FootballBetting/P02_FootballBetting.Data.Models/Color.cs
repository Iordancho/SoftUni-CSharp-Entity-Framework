using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{



    public class Color
    {
        [Key] public int ColorId { get; set; }
        [Required] [StringLength(100)] public string Name { get; set; }

        [InverseProperty(nameof(Team.PrimaryKitColor))]
        public ICollection<Team> PrimaryKitTeams { get; set; }

        [InverseProperty(nameof(Team.SecondaryKitColor))]
        public ICollection<Team> SecondaryKitTeams { get; set; }
    }
}