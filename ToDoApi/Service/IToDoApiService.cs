using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace ToDoApi.Service
{
    public interface IToDoApiService
    {
        Task<IEnumerable<Player>> GetPlayersAsync();
        Task<Player> GetPlayerByIdAsync(int id);
        Task<List<Player>> GetPlayerByNameAsync(string lastName);
        Task<List<Player>> GetPlayersByTeamAsync(string teamName);
        Task<IEnumerable<Team>> GetTeamsAsync();
        Task<List<Team>> GetTeamByNameOrLocationAsync(string nameOrLocation);
        Task<Team> GetTeamByIdAsync(int id);
        Task<Team> CreateTeamAsync(Team team);
        Task<Player> CreatePlayerAsync(Player player);
        Task<int> DeletePlayerAsync(int id);
        Task<Player> PutPlayerAsync(int id, Player player);
        Task<Player> AddPlayerToTeamAsync(int id, Player player);
    }
}
