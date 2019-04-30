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
        public async Task<Player> GetPlayerById(int id)
        {
            return await _toDoApiService.GetPlayerByIdAsync(id);
        }

        // GET: api/nfl/player/lastname
        [HttpGet("player/lastname/{lastName}")]
        public async Task<List<Player>> GetPlayerByName(string lastName)
        {
            return await _toDoApiService.GetPlayerByNameAsync(lastName);
        }

        // GET: api/nfl/team/{teamName}/players
        [HttpGet("team/{teamName}/players")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByTeam(string teamName)
        {
            return await _toDoApiService.GetPlayersByTeamAsync(teamName);
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
            return await _toDoApiService.GetTeamByNameOrLocationAsync(nameOrLocation);
        }

        // GET: api/nfl/team/id/{id}
        [HttpGet("team/id/{id}")]
        public async Task<ActionResult<Team>> GetTeamById(int id)
        {
            return await _toDoApiService.GetTeamByIdAsync(id);
        }

        //POST: api/nfl/team
       [HttpPost("team")]
        public async Task<ActionResult<Team>> CreateTeamAsync(Team team)
        {
            return _toDoApiService.CreateTeamAsync(team);
        }

        // POST: api/nfl/player
        [HttpPost("player")]
        public async Task<Player> CreatePlayer(Player player)
        {
            return _toDoApiService.CreatePlayerAsync(player);
        }

        // DELETE: api/nfl/player/{id}
        [HttpDelete("player/{id}")]
        public async Task<IActionResult> DeletePlayerAsync(int id)
        {
            return _toDoApiService.DeletePlayerAsync(id);
        }

        // PUT: api/nfl/player/{id}
        [HttpPut("player/{id}")]
        public async Task<IActionResult> PutPlayerAsync(int id, Player player)
        {
            return await _toDoApiService.PutPlayerAsync(id, player);
        }

        // POST: api/nfl/team/{id}/player
        [HttpPatch("team/{id}/player")]
        public async Task<ActionResult<Player>> AddPlayerToTeamAsync(int id, Player player)
        {
            return await _toDoApiService.AddPlayerToTeamAsync(id, player);
        }
    }
}