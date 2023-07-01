using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameCore.Services;

namespace GuessGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransientController : ControllerBase
    {
        private readonly ILogger<TransientController> _logger;

        private readonly ITransient _transientService1;

       
        public TransientController(ILogger<TransientController> logger, ITransient service1)
        {
            _logger = logger;
            _transientService1 = service1;
           
        }

        [HttpGet]
        public string Get()
        {
            return "Input your guess: ";
        }

        [HttpGet("{guess}")]
        public ActionResult<string> Guess(int guess)
        {
            int random = _transientService1.GetRandom();
            int tries = _transientService1.GetTries();
            return Validate(guess, random, tries);
        }

        private string Validate(int guess, int random, int tries)
        {
            if (guess < random)
            {
                return $"{random} {guess} is too low.\nRemaining tries: {tries}\n";
            }
            else if (guess > random)
            {
                return $"{random}  {guess} is too high\nRemaining tries: {tries}\n";
            }
            else
            {
                return $"HOOOOOORRRAAAAYYYYYYY...Well done. Your guess of {guess} is correct with {tries} tries left.";
            }
        }
    }
}
