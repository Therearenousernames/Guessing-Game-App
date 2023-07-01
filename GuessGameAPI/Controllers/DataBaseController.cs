namespace GuessGameAPI.Controllers
{

    using GameCore.Services;
    using Microsoft.AspNetCore.Mvc;
    using GameCore;
    using System.Text.RegularExpressions;
    using GuessGameAPI.Data;
    using GuessGameAPI.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Data;
    using System.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public partial class DataController :ControllerBase
    {
        private GameInstance? user;

        private readonly static GameRepositoryService gameRepositoryService = new();

        private readonly Play play = new();

        private readonly GameDatabaseContext context = new ();

        private Usernames? newUsername;

 

        protected Usernames? Exists(string username)
        {
            var person = context.Usernames.Where(c=> EF.Functions.Collate(c.Username, "SQL_Latin1_General_CP1_CS_AS") == username).FirstOrDefault();
           
            return person;
        }

        protected List<GameState> GetHistory(string username)
        {
            var history = context.GameStates.Where(p => EF.Functions.Collate(p.Username, "SQL_Latin1_General_CP1_CS_AS") == username).ToList();
            return history;
        }


        private void AddUsername(Usernames username)
        {
            context.Add(username);
            context.SaveChanges();
        }


        // update gamestate if the user exists the game 
        private void UpdateState(int id, bool progress, int tries)
        {
            var state = context.GameStates.Where(i => i.Id == id).FirstOrDefault();
          

           if (state != null)
            {
               if (progress)
                {
                   state.WonLoss = true;
                    state.Tries = tries;

                } else 
                {
                    state.WonLoss = false;
                    state.Tries = tries;
                    
                } 
                
               context.SaveChanges();
            }

        }

        private void AddState(GameState game)
        {
            context.Add(game);
            context.SaveChanges();
        }

        private void DeleteHistory(string username)
        {
            var history = GetHistory(username);
            foreach (GameState state in history)
            {
                context.Remove(state);
            }
            context.SaveChanges();
        }

        private void DeleteHistoryAndUsername(string username)
        {
            DeleteHistory(username);

            var exists = Exists(username);
            context.Remove(exists);
            context.SaveChanges();
        }

        [HttpGet("history/{username}")]
        public IActionResult GetUserHistory([FromRoute] string username)
        {
            Model result = UsernameFromDatabase(username);
            if (result?.Message == $"Welcome back {username}! May the forces be with you...\n")
            {
                result.GameStates = GetHistory(username);
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("deletehistory/{username}")]
        public IActionResult DeleteUserHistory([FromRoute] string username)
        {
            Model response;
            Model result = UsernameFromDatabase(username);
            if (result?.Message == $"Welcome back {username}! May the forces be with you...\n")
            {
                DeleteHistory(username);
                response = new()
                {
                    Message = $"Your history {username} has been deleted.",
                };
                return Ok(response);
            }
            return BadRequest(result);
        }

        [HttpGet("deleteuser/{username}")]
        public IActionResult DeleteUsernameAndHistory([FromRoute] string username)
        {
            Model result;
            Model response = UsernameFromDatabase(username);
            if (response?.Message == $"Welcome back {username}! May the forces be with you...\n")
            {
                DeleteHistoryAndUsername(username);
                result = new()
                {
                    Message = $"Your history { username } and the username has been deleted."
                };
                return Ok(result);
            }
            return Ok(response);
        }

        protected Model UsernameFromDatabase(string username)
        {
            Model result;
            var exists = Exists(username);
            if (exists != null)
            {
                result = new()
                {
                    Message = $"Welcome back {username}! May the forces be with you...\n",
                    Username = username,
                };
            } else
            {

                result = new()
                {
                    Message = $"The username {username} could not be found. Please register.\n"
                };
            }
            return result;
        }



        [HttpGet("{username}")]
        public IActionResult DatabaseSearch([FromRoute] string username)
        {
            Model result = UsernameFromDatabase(username);
            if (result?.Message == $"Welcome back {username}! May the forces be with you...\n")
            {
                return Ok(result);
            } else
            
            return Ok(result);
        }

    


       

        [HttpGet("register/{username}")]
        public IActionResult GetUserName([FromRoute] string userName)
        {
            Model result;
            if (string.IsNullOrEmpty(userName))
            {
                result = new Model
                {
                    Message = "Username cannot be empty or null."
                };
                return BadRequest(result);
            }

            bool isValid = ValidateUsername(userName);

   
            if (true)
            {
                var exists = Exists(userName);
                if (exists == null)
                {
                    newUsername = new()
                    {
                        Username = userName,
                    };

                    AddUsername(newUsername);
                    result = new Model
                    {
                        Message = $"Username {userName} has been registered.",
                        Username = userName,
                    };

                    return Ok(result);
                } else
                {
                    result = new Model
                    {
                        Message = $"{userName} already exists. Please try again."
                    };

                    return Ok(result);
                }
               
            } else
            {
                result = new Model
                {
                    Message = $"{userName} is invalid. Try again."
                };

                return Ok(result);
            }
        }

        static bool ValidateUsername(string username)
        { 
            if (username.All(Char.IsLetter)) {
                return true;
            }
            return false;
           
        }

        
        [HttpGet("{username}/{id}/{guess}")]
        public IActionResult Guess([FromRoute] string username, [FromRoute] int id, [FromRoute] int guess)
        {
           var instance = gameRepositoryService.GetGameInstanceById(id);
            Model result;
         
            if (instance != null)
            {
                
                int random = instance.RandomNumber;
                int tries = instance.DecrementTries();
                string msg = play.Validate(guess, random, tries);

                if (msg == $"Your guess of {guess} is correct with {tries} chance(s) left. Restart the game if you want to play again.")
                {
                   
                   
                    UpdateState(id, true, tries);

                    result = new Model
                    {
                        Message = $"Well done! {newUsername?.Username}. {msg}.",
                        Id = id,
                        Tries = tries,
                        Playing = false,
                        Lost = false,
                        Username =username,
                    };
                    return Ok(result);
                } else if (msg == $"Debug output: {random}. Your guess of {guess} is too low.\nRemaining chance(s): {tries}.\nYou've ran out of tries. Game over, bye!\n" ||
                    msg == $"Debug output: {random}. Your guess of {guess} is too high\nRemaining chance(s): {tries}.\nYou've ran out of tries. Game over, bye!\n")
                {

                    UpdateState(id, false, tries) ;
       

                    result = new Model
                    {
                        Message = msg,
                        Id = id,
                        Tries = 0,
                        Playing = false,
                        Lost = true,
                        Username = username,
                    };
                    return Ok(result);
                }
                else
                {
                    result = new Model
                    {
                        Message = msg,
                        Id = id,
                        Tries = tries,
                        Playing = true,
                        Username=username,
                    };
                }
                return Ok(result);

            } else
            {
                result = new Model
                {
                    Message = "Please start game properly so an user id can be generated for you.",
                    Playing = false,
                };
            }
            return Ok(result);
        }

        [HttpGet("start/{username}")]
        public IActionResult Get([FromRoute] string username)
        {
            // creates a new user
            if (user == null)
            {
               
                user = gameRepositoryService.NewGame();

                GameState state = new()
                {
                    WonLoss = false,
                    Tries = user.Tries,
                    NumberToGuess = user.RandomNumber,
                    Username = username,
                };

                AddState(state);
                user.Id = state.Id;
                var result = new Model
                {
                    Message = $"A user id of {state.Id} has been allocated to you and random number has been generated. You have {state.Tries} tries left.",
                    Id = state.Id,
                    Tries = user.Tries,
                    Playing = true,
                    Username = username,
                };
                return Ok(result);
            } else
            {
                return Ok();
            }
            // More code here for when the user that we are checking is null
            // Should we loop back to the beginning? What sort of error are we sending?
            // user stands for game instance

        }

        [GeneratedRegex("^[a-zA-Z]+$")]
        private static partial Regex MyRegex();
    }
}
