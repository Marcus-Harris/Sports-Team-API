using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace TodoApi.Models
{
    public class TeamDBContext : DbContext
    {
        public TeamDBContext(DbContextOptions<TeamDBContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; } = null!;
    }
}
