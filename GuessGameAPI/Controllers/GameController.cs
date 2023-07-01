namespace GuessGameAPI.Controllers
{

    using GameCore.Services;
    using Microsoft.AspNetCore.Mvc;
    using GameCore;
  

    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
 
        private GameInstance? user;

        private readonly static GameRepositoryService gameRepositoryService = new();

        private readonly Play play = new ();


        

        [HttpGet]
        public IActionResult Get()
        {
            // get back if the username is valid before assigning the game instance? 
            // create a different router 
            // creates a new user
            if (user == null)
            {
                user = gameRepositoryService.NewGame();
                var result = new Model {
                    Message = $"A user id of {user.Id} has been allocated to you and random number has been generated. You have {user.Tries} tries left.",
                    Id = user.Id,
                    Tries = user.Tries,
                    Playing = true,
                };
                return Ok(result);
            }
            return Ok();

        }
        


    

        [HttpGet("{id}/{guess}")]
        public IActionResult Guess([FromRoute] int id, [FromRoute] int guess)
        {
            var instance = gameRepositoryService.GetGameInstanceById(id);
            Model result;
            if (instance != null)
            {
                try
                {
                    int random = instance.RandomNumber;
                    int tries = instance.DecrementTries();
                    Console.WriteLine(tries);
                    string msg = play.Validate(guess, random, tries);
                    if (msg == $"Well done! Your guess of {guess} is correct with {tries} chance(s) left. Restart the game if you want to play again.")
                    {
                        result = new Model
                        {
                            Message = $"Well done! Your guess of {guess} is correct with {tries} chance(s) left. Restart the game if you want to play again.",
                            Id = id,
                            Tries = tries,
                            Playing = false,
                            Lost = false,
                        };
                    } else
                    {
                        result = new Model
                        {
                            Message = play.Validate(guess, random, tries),
                            Id = id,
                            Tries = tries,
                            Playing = true,
                        };
                    }
                    return Ok(result);
                } catch(Exception)
                {
                    result = new Model { Message = "You've ran out of tries. Please restart the game.", 
                                         Playing = false,
                                         Lost = true,
                                        };
                    return Ok(result);
                }
                   
            }
            result = new Model { Message = "Please start game properly so an user id can be generated for you.",
                Playing = false, };
            return BadRequest(result);
 
        }
    }
}
