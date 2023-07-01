using System.Threading.Tasks.Dataflow;
using System.ComponentModel.DataAnnotations;
namespace GuessGameAPI.Models
{
    public class GameState
    {
        [Key]
        public int Id { get; set; }
        public bool WonLoss { get; set; }

        public int Tries { get;  set; }

        public string? Username { get; set; }

        public int NumberToGuess { get; set; }
        
    }
}
