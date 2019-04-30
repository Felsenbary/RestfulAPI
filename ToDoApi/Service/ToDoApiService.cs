using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace ToDoApi.Service
{
    public class ToDoApiService :IToDoApiService
    {
        private readonly TodoContext _context;

        public ToDoApiService(TodoContext context)
        {
            _context = context;

            if (_context.Players.Count() == 0 || _context.Teams.Count() == 0)
            {
                DbSetup(_context);
            }
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player is null || player.ID < 1)
            {
                return NotFound();
            }

            return player;
        }

        public async Task<List<Player>> GetPlayerByNameAsync(string lastName)
        {
            var players = await _context.Players.Where(x => x.LastName.ToLower() == lastName.ToLower()).ToListAsync();

            if (!players.Any())
            {
                return NotFound();
            }

            return players;
        }

        public async Task<IEnumerable<Player>> GetPlayersByTeamAsync(string teamName)
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

        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            return await _context.Teams.Include(x => x.Players).ToListAsync();
        }

        public async Task<List<Team>> GetTeamByNameOrLocationAsync(string nameOrLocation)
        {
            var teams = await _context.Teams
                .Where((x => x.Name.Replace(" ", String.Empty)
               .ToLower() == nameOrLocation
               .Replace(" ", String.Empty).ToLower()))
                .Include(x => x.Players)
                .ToListAsync();

            if (!teams.Any())
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

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team is null || team.ID < 1)
            {
                return NotFound();
            }

            await _context.Entry(team).Collection(x => x.Players).LoadAsync();

            return team;
        }

        public async Task<Team> CreateTeamAsync(Team team)
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

        public async Task<Player> CreatePlayerAsync(Player player)
        {
            var result = await GetPlayersAsync();
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

            var containSameNameAndLocation = ContainsKeyValue(dictionary, player.FirstName, player.LastName);

            if (containSameNameAndLocation)
            {
                var Message = "A player already exist";
                return BadRequest(Message);
            }

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPlayerByIdAsync", new { id = player.ID }, player);
        }

        public async Task<IActionResult> DeletePlayerAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player is null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

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

        public async Task<HttpResponseMessage> AddPlayerToTeamAsync(int id, Player player)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team is null)
            {
                var Message = "Team does not exist";
                return BadRequest(Message);
            }

            var result = await GetPlayersAsync();
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
                var playerToUpdte = tempPlayersList.Where(x => x.FirstName == player.FirstName && x.LastName == player.LastName).FirstOrDefault();
                player.ID = playerToUpdte.ID;

                await DeletePlayerAsync(playerToUpdte.ID);
            }
            player.TeamId = id;
            await CreatePlayerAsync(player);
            return CreatedAtAction("GetPlayerByIdAsync", new { id = player.ID }, player);
        }

        private bool ContainsKeyValue(Dictionary<string, List<string>> dictionary, string expectedKey, string expectedValue)
        {
            if (!dictionary.TryGetValue(expectedKey, out List<string> actualValues))
            {
                return false;
            }
            foreach (var value in actualValues)
            {
                if (value == expectedValue)
                    return true;
            }
            return false;
        }

        public static async void DbSetup(DbContext context)
        {
            var listPlayersNY = new List<Player>
            {
                new Player { FirstName = "Eli", LastName = "Manning", TeamId = 1 },
                new Player { FirstName = "Saquon", LastName = "Barkley", TeamId = 1 },
                new Player { FirstName = "Kyle", LastName = "Lauletta", TeamId = 1 },
                new Player { FirstName = "Evan", LastName = "Engram", TeamId = 1 },
                new Player { FirstName = "Sterling", LastName = "Shepard", TeamId = 1 },
                new Player { FirstName = "Alonzo", LastName = "Russell", TeamId = 1 },
                new Player { FirstName = "Golden", LastName = "Tate", TeamId = 1 },
                new Player { FirstName = "Robert", LastName = "Martin", TeamId = 1 }

            };

            context.AddRange(listPlayersNY);

            var team = new Team
            {
                Name = "NY Giants",
                Location = "New York",
                Players = listPlayersNY
            };

            context.Add(team);

            var listPlayersNC = new List<Player>
            {
                new Player { FirstName = "Cam", LastName = "Newton", TeamId = 2 },
                new Player { FirstName = "Greg", LastName = "Barkley", TeamId = 2 },
                new Player { FirstName = "Chris ", LastName = "Hogan", TeamId = 2 },
                new Player { FirstName = "Luke", LastName = "Kuechly", TeamId = 2 },
                new Player { FirstName = "TJ", LastName = "Barnes", TeamId = 2 },
                new Player { FirstName = "Ross", LastName = "Cockerell", TeamId = 2 },
                new Player { FirstName = "Alex", LastName = "Armah", TeamId = 2 },
                new Player { FirstName = "Isaiah", LastName = "Battle", TeamId = 2 }

            };

            context.AddRange(listPlayersNC);

            var team2 = new Team
            {
                Name = "Carolina Panthers",
                Location = "North Carolina",
                Players = listPlayersNC
            };

            context.Add(team2);

            var listPlayersNE = new List<Player>
            {
                new Player { FirstName = "Tom", LastName = "Brady", TeamId = 3 },
                new Player { FirstName = "Julian", LastName = "Elderman", TeamId = 3 },
                new Player { FirstName = "Stephen", LastName = "Gostkowski", TeamId = 3 },
                new Player { FirstName = "Rex", LastName = "Burkhead", TeamId = 3 },
                new Player { FirstName = "Josh", LastName = "Gordon", TeamId = 3 },
                new Player { FirstName = "Adam", LastName = "Butler", TeamId = 3 },
                new Player { FirstName = "John", LastName = "Simon", TeamId = 3 },
                new Player { FirstName = "Trent", LastName = "Harris", TeamId = 3 }

            };

            context.AddRange(listPlayersNE);

            var team3 = new Team
            {
                Name = "New England Patriots",
                Location = "Massachusetts",
                Players = listPlayersNE
            };

            context.Add(team3);

            await context.SaveChangesAsync();
        }
    }
}
