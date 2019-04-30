using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
