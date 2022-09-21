using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TodoApi.Models
{
    [Index(nameof(Team.Name), IsUnique = true)]
    [Index(nameof(Team.Location), IsUnique = true)]
    public class Team
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
        public IList<Player> Players { get; set; } = new List<Player>();
    }
}
