using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace ToDoApi.Service
{
    public interface IToDoApiService
    {
        Task<IEnumerable<Player>> GetPlayersAsync();
    }
}
