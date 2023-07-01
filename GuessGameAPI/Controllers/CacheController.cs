using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace GuessGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
   

        public CacheController(IMemoryCache memoryCache)
        {
            // initialisation 
            _memoryCache = memoryCache;
            int random, tries;

            if (!_memoryCache.TryGetValue("random", out _) && !_memoryCache.TryGetValue("tries", out _)) 
            {
                random = new Random().Next(0, 21);
                tries = 5;


                // storing into cache
                _memoryCache.Set("random", random);
                _memoryCache.Set("tries", tries);
            }
          
        }

        [HttpGet]
        public string Get()
        {
            return "Input your guess: ";
        }

        [HttpGet("{guess}")]
        public ActionResult<string> Guess(int guess)
        {
            int random, tries; 

            object? cacheRandom = _memoryCache.Get("random");
            object? cacheTries = _memoryCache.Get("tries");
            if (cacheRandom != null && cacheTries != null)
            {
                random = (int)cacheRandom;
                tries = (int)cacheTries - 1;
                _memoryCache.Set("tries", tries);
                return Validate(guess, random, tries);
            }
            return "";
           
          

        }

        private string Validate(int guess, int random, int tries)
        {
            if (tries == 0)
            {
                return $"Oops! You are out of tries.\n";
            }
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
                _memoryCache.Remove("random");
                _memoryCache.Remove("tries");
                return $"HOOOOOORRRAAAAYYYYYYY...Well done. Your guess of {guess} is correct with {tries} tries left.";
            }
        }
    }
}
