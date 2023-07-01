using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace GuessGameAPI.Models
{
    public class Usernames
    {
        [Key]
        public string Username { get; set; } = string.Empty;
    }
}
