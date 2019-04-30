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
        public NFLController(IToDoApiService toDoApiService, TodoContext context)
        {
            _toDoApiService = toDoApiService;
            _context = context;
        }

        // GET: api/nfl/players
        [HttpGet("Players")]
        public async Task<IEnumerable<Player>> GetPlayers()
        {
            return await _toDoApiService.GetPlayersAsync();
        }

        // GET: api/nfl/player/id/{id}
        [HttpGet("player/id/{id}")]
        public async Task<ActionResult<Player>> GetPlayerById(int id)
        {
            var player = await _toDoApiService.GetPlayerByIdAsync(id);
            if (player is null || player.ID < 1)
            {
                return NotFound();
            }
            return player;
        }

        // GET: api/nfl/player/lastname
        [HttpGet("player/lastname/{lastName}")]
        public async Task<ActionResult<List<Player>>> GetPlayerByName(string lastName)
        {
            var players = await _toDoApiService.GetPlayerByNameAsync(lastName);

            if (!players.Any())
            {
                return NotFound();
            }
            return players;
        }

        // GET: api/nfl/team/{teamName}/players
        [HttpGet("team/{teamName}/players")]
        public async Task<ActionResult<List<Player>>> GetPlayersByTeam(string teamName)
        {
            var players = await _toDoApiService.GetPlayersByTeamAsync(teamName);

            if (!players.Any())
            {
                return NotFound();
            }
            return players;
        }

        // GET: api/nfl/teams
        [HttpGet("teams")]
        public async Task<IEnumerable<Team>> GetTeams()
        {
            return await _toDoApiService.GetTeamsAsync();
        }

        // GET: api/nfl/team/{nameOrLocation}
        [HttpGet("team/{nameOrLocation}")]
        public async Task<ActionResult<List<Team>>> GetTeamByNameOrLocation(string nameOrLocation)
        {
            var teams = await _toDoApiService.GetTeamByNameOrLocationAsync(nameOrLocation);
            if (!teams.Any())
            {
                return NotFound();
            }
            return teams;
        }

        // GET: api/nfl/team/id/{id}
        [HttpGet("team/id/{id}")]
        public async Task<ActionResult<Team>> GetTeamById(int id)
        {
            var team = await _toDoApiService.GetTeamByIdAsync(id);
            if (team is null || team.ID < 1)
            {
                return NotFound();
            }
            return team;
        }

        //POST: api/nfl/team
       [HttpPost("team")]
        public async Task<ActionResult<Team>> CreateTeamAsync(Team team)
        {
            var newTeam = _toDoApiService.CreateTeamAsync(team);

            if(newTeam.Id ==  0)
            {
                var Message = "No two teams should exist with the same name and location";
                return BadRequest(Message);
            }

            return CreatedAtAction("GetTeamByIdAsync", new { id = team.ID }, team);
        }

        // POST: api/nfl/player
        [HttpPost("player")]
        public async Task<ActionResult<Player>> CreatePlayer(Player player)
        {
            var newPlayer = _toDoApiService.CreatePlayerAsync(player);
            if(newPlayer.Id == 0)
            {
                var Message = "A player already exist";
                return BadRequest(Message);
            }

            return CreatedAtAction("GetPlayerByIdAsync", new { id = player.ID }, player);
        }

        // DELETE: api/nfl/player/{id}
        [HttpDelete("player/{id}")]
        public async Task<IActionResult> DeletePlayerAsync(int id)
        {
            var result = _toDoApiService.DeletePlayerAsync(id);

            if (result.Result == 0)
                return NotFound();

            return  NoContent();
        }

        // PUT: api/nfl/player/{id}
        [HttpPut("player/{id}")]
        public async Task<IActionResult> PutPlayerAsync(int id, Player player)
        {
            var updatedPlayer = await _toDoApiService.PutPlayerAsync(id, player);

            if(updatedPlayer.ID == 0)
            {
                var message = "Player id does not exist";
                return BadRequest(message);
            }
            
            return NoContent();
        }

        // POST: api/nfl/team/{id}/player
        [HttpPatch("team/{id}/player")]
        public async Task<ActionResult<Player>> AddPlayerToTeamAsync(int id, Player player)
        {
            var newPlayer = await _toDoApiService.AddPlayerToTeamAsync(id, player);
            if(newPlayer.ID == 0)
            {
                var Message = "Team does not exist";
                return BadRequest(Message);
            }
            return CreatedAtAction("GetPlayerByIdAsync", new { id = player.ID }, player);
        }
    }
}