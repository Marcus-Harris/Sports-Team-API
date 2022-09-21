using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/Teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly TeamDBContext _context;
        private readonly PlayerDBContext _context2;
        private readonly PlayersController playersController;

        public TeamsController(TeamDBContext context, PlayerDBContext context2)
        {
            _context = context;
            _context2 = context2;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.Include(t => t.Players).ToListAsync();
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(long id)
        {
            var teams = await _context.Teams.Include(t => t.Players).ToListAsync();
            var team = (from selectedTeam in teams where selectedTeam.Id == id select selectedTeam).SingleOrDefault();


            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> AddPlayer(long id, Player player)
        {

            var existingTeam = _context.Teams.Include(t => t.Players).Where(c => c.Id.Equals(id)).FirstOrDefault();
            var existingPlayer = _context2.Players.AsNoTracking().Where(p => p.Id == player.Id).FirstOrDefault();
            IList<Player> playersArray = existingTeam.Players;

            if (existingTeam == null)
            {
                throw new Exception("Team does not exists.");
            }
            else
            {
                if(existingPlayer != null)
                {
                    throw new Exception("Player is already on the team.");
                } else
                {
                    _context.Teams.Remove(existingTeam);
                    await _context.SaveChangesAsync();

                    if (playersArray.Count < 9)
                    {
                        playersArray.Add(player);
                    }
                    else
                    {
                        throw new Exception("Player limit reached.");
                    }
                    var editedTeam = new Team()
                    {
                        Id = existingTeam.Id,
                        Name = existingTeam.Name,
                        Location = existingTeam.Location,
                        Players = playersArray.Concat(existingTeam.Players).ToList()
                    };

                    _context.Teams.Add(editedTeam);
                    await _context.SaveChangesAsync();
                }
            }

            return NoContent();
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id} & {playerId}")]
        public async Task<IActionResult> RemovePlayer(long id, int playerId)
        {

            var existingTeam = _context.Teams.AsNoTracking().Include(t => t.Players).Where(c => c.Id.Equals(id)).FirstOrDefault();
            var existingPlayer = existingTeam.Players.Where(p => p.Id == playerId).FirstOrDefault();
            IList<Player> playersArray = existingTeam.Players;

            if (existingTeam == null)
            {
                throw new Exception("Team does not exists.");
            }
            else
            {
                if (existingPlayer == null)
                {
                    throw new Exception("Player is not on the team.");
                }
                else
                {
                    _context.Teams.Remove(existingTeam);
                    await _context.SaveChangesAsync();
                    playersArray.Remove(existingPlayer);
                    var editedTeam = new Team()
                    {
                        Id = existingTeam.Id,
                        Name = existingTeam.Name,
                        Location = existingTeam.Location,
                        Players = playersArray.Concat(existingTeam.Players).ToList()
                    };

                    _context.Teams.Add(editedTeam);
                    await _context.SaveChangesAsync();
                }
            }

            return NoContent();
        }

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            foreach (var player in team.Players)
            {
                if(_context2.Players.AsNoTracking().ToList().Find(p => p.Id == player.Id) != null)
                {
                    _context2.Remove(player);
                    await _context2.SaveChangesAsync();
                }
                _context2.Players.Add(player);
                await _context2.SaveChangesAsync();
            }

            if(team.Players.Count < 9)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
            } else
            {
                throw new Exception("Player limit reached.");
            }

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(long id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamExists(long id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
