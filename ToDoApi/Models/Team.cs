using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDoApi.Validation;

namespace ToDoApi.Models
{
    public class Team
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public List<Player> Players { get; set; }
    }
}
