using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoApi.Models;
using ToDoApi.Service;

namespace ToDoApi.Controllers
{
    [Route("api/NFL")]
    [ApiController]
    public class NFLController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly IToDoApiService _toDoApiService;
        public NFLController(IToDoApiService toDoApiService)
        {
            _toDoApiService = toDoApiService;
        }

        // GET: api/nfl/players
        [HttpGet("Players")]
        public async Task<IEnumerable<Player>> GetPlayers()
        {
            return await _toDoApiService.GetPlayersAsync();
        }

        // GET: api/nfl/player/id/{id}
        [HttpGet("player/id/{id}")]
        public async Task<ActionResult<Player>> GetPlayerByIdAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player is null || player.ID < 1)
            {
                return NotFound();
            }

            return player;
        }

        // GET: api/nfl/player/lastname
        [HttpGet("player/lastname/{lastName}")]
        public async Task<ActionResult<List<Player>>> GetPlayerByNameAsync(string lastName)
        {
            var players =  await _context.Players.Where(x => x.LastName.ToLower() == lastName.ToLower()).ToListAsync();

            if (!players.Any())
            {
                return NotFound();
            }

            return players;
        }

        // GET: api/nfl/team/{teamName}/players
        [HttpGet("team/{teamName}/players")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByTeamAsync(string teamName)
        {
            var Team = await _context.Teams
                .Where(x => x.Name.Replace(" ", String.Empty).ToLower() == teamName
                .Replace(" ", String.Empty)
                .ToLower())
                .Include(x => x.Players)
                .FirstOrDefaultAsync();

            if (Team is null)
            {
                return NotFound();
            }

            return Team.Players;
        }

        // GET: api/nfl/teams
        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeamsAsync()
        {
            return await _context.Teams.Include(x=>x.Players).ToListAsync();
        }

        // GET: api/nfl/team/{nameOrLocation}
        [HttpGet("team/{nameOrLocation}")]
        public async Task<ActionResult<List<Team>>> GetTeamByNameOrLocationAsync(string nameOrLocation)
        {
            var teams = await _context.Teams
                .Where( (x=>x.Name.Replace(" ", String.Empty)
                .ToLower() == nameOrLocation
                .Replace(" ", String.Empty).ToLower()))
                .Include(x=>x.Players)
                .ToListAsync();

            if(!teams.Any())
            {
                teams = await _context.Teams
                    .Where(x => x.Location
                    .Replace(" ", String.Empty)
                    .ToLower() == nameOrLocation
                    .Replace(" ", String.Empty).ToLower())
                    .Include(x => x.Players)
                    .ToListAsync();

                if (!teams.Any())
                {
                    return NotFound();
                }
            }

            return teams;
        }

        // GET: api/nfl/team/id/{id}
        [HttpGet("team/id/{id}")]
        public async Task<ActionResult<Team>> GetTeamByIdAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team is null || team.ID < 1)
            {
                return NotFound();
            }

            await _context.Entry(team).Collection(x => x.Players).LoadAsync();

            return team;
        }

        //POST: api/nfl/team
       [HttpPost("team")]
        public async Task<ActionResult<Team>> CreateTeamAsync(Team team)
        {
            var result = await GetTeamsAsync();
            var list = result.Value.ToList();
            var dictionary = new Dictionary<string, List<string>>();

            foreach (var item in list)
            {
                List<string> dicList = new List<string>();
                if (!dictionary.ContainsKey(item.Name))
                {
                    dicList.Add(item.Location);
                    dictionary.Add(item.Name, dicList);
                }
                else
                {
                    dictionary[item.Name].Add(item.Location);
                }
            }
           
            var containSameNameAndLocation = ContainsKeyValue(dictionary, team.Name, team.Location);

            if (containSameNameAndLocation)
            {
                var Message = "No two teams should exist with the same name and location";
                return BadRequest(Message);
            }

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTeamByIdAsync", new { id = team.ID }, team);
        }

        // POST: api/nfl/player
        [HttpPost("player")]
        public async Task<ActionResult<Player>> CreatePlayerAsync(Player player)
        {
            var result = await GetPlayers();
            //var list = result.Value.ToList();
            var dictionary = new Dictionary<string, List<string>>();
            
            foreach (var item in result)
            {
                List<string> dicList = new List<string>();
                if (!dictionary.ContainsKey(item.FirstName))
                {
                    dicList.Add(item.LastName);
                    dictionary.Add(item.FirstName, dicList);
                }
                else
                {
                    dictionary[item.FirstName].Add(item.LastName);
                }
            }
            
            var containSameNameAndLocation =  ContainsKeyValue(dictionary, player.FirstName, player.LastName);

            if (containSameNameAndLocation)
            {
                var Message = "A player already exist";
                return BadRequest(Message);
            }

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPlayerByIdAsync", new { id = player.ID }, player);
        }

        // DELETE: api/nfl/player/{id}
        [HttpDelete("player/{id}")]
        public async Task<IActionResult> DeletePlayerAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player is null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/nfl/player/{id}
        [HttpPut("player/{id}")]
        public async Task<IActionResult> PutPlayerAsync(int id, Player player)
        {
            try
            {
                if (id != player.ID)
                {
                    var message = "Player id does not exist";
                    return BadRequest(message);
                }

                _context.Entry(player).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch 
            {
                var message = "Player does not exist";
                return BadRequest(message);
            }
            
        }

        // POST: api/nfl/team/{id}/player
        [HttpPatch("team/{id}/player")]
        public async Task<ActionResult<Player>> AddPlayerToTeamAsync(int id, Player player)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team is null)
            {
                var Message = "Team does not exist";
                return BadRequest(Message);
            }

            var result = await GetPlayers();
            var dictionary = new Dictionary<string, List<string>>();

            foreach (var item in result)
            {
                List<string> dicList = new List<string>();
                if (!dictionary.ContainsKey(item.FirstName))
                {
                    dicList.Add(item.LastName);
                    dictionary.Add(item.FirstName, dicList);
                }
                else
                {
                    dictionary[item.FirstName].Add(item.LastName);
                }
            }

            if (ContainsKeyValue(dictionary, player.FirstName, player.LastName))
            {
                var tempPlayersList = await GetPlayerByNameAsync(player.LastName);
                var playersList = tempPlayersList.Value.ToList();
                var playerToUpdte = playersList.Where(x => x.FirstName == player.FirstName && x.LastName == player.LastName).FirstOrDefault();
                player.ID = playerToUpdte.ID;

                await DeletePlayerAsync(playerToUpdte.ID);
            }
            player.TeamId = id;
            await CreatePlayerAsync(player);
            return CreatedAtAction("GetPlayerByIdAsync", new { id = player.ID }, player);
        }


        private bool ContainsKeyValue(Dictionary<string,List<string>> dictionary, string expectedKey, string expectedValue)
        {
            if (!dictionary.TryGetValue(expectedKey, out List<string> actualValues))
            {
                return false;
            }
            foreach(var value in actualValues)
            {
                if(value == expectedValue)
                    return true;
            }
            return false;
        }
    }
}