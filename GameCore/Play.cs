namespace GameCore
{
    using GameCore.Services;
    using System;
    using System.ComponentModel.Design;

    public class Play 
    {
        public Random rand;

        public int random;

        public int score;

        public bool keepPlaying;

        public bool hasWon;

        public int limit;

        private readonly IUserInput _userInputService;

        public Play(IUserInput userInputService) 
        {
            _userInputService = userInputService;
            rand = new Random();
            score = 0;
            keepPlaying = true;
            hasWon = false;
            limit = 5;
        }

        public Play()
        {

        }


        public int GenerateRandom()
        {
            int randomNumber = rand.Next(1, 21);
            return randomNumber;
        }

        public int GetRandom()
        {
            return random;
        }

        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }

        public string Validate(int guess, int random)
        {
            if (guess < random)
            {
                limit--;
                return $"\n{guess} is too low\nRemaining tries: {limit}\n";
            }
            else if (guess > random)
            {
                limit--;
                return $"\n{guess} is too high\nRemaining tries: {limit}\n";
            }
            else
            {
                score++;
                hasWon = true;
                return $"HOOOOOORRRAAAAYYYYYYY...Well done. Your guess of {guess} is correct";
            }
        }


        public string Validate(int guess, int random, int tries)
        {
            if (guess < random & tries == 0)
            {
                return $"Debug output: {random}. Your guess of {guess} is too low.\nRemaining chance(s): {tries}.\nYou've ran out of tries. Game over, bye!\n";
            } else if (guess < random)
            {
                return $"Debug output: {random}. Your guess of {guess} is too low.\nRemaining chance(s): {tries}.\n";
            }
            else if (guess > random && tries == 0)
            {
                return $"Debug output: {random}. Your guess of {guess} is too high\nRemaining chance(s): {tries}.\nYou've ran out of tries. Game over, bye!\n";
            }
            else if (guess > random)
            {
               return $"Debug output: {random}. Your guess of {guess} is too high\nRemaining chance(s): {tries}.\n";
            } else
            {
                return $"Your guess of {guess} is correct with {tries} chance(s) left. Restart the game if you want to play again.";
            }
            

        }


        public void Start()
        {
            random = GenerateRandom();
            while (keepPlaying & limit >= 0)
            {
                int guess = GetUserInput();
                String message = Validate(guess, random);
                PrintMessage(message);

                if (hasWon)
                {
                    keepPlaying = ReplayGame();
                }

                if (limit == 0 & hasWon != true)
                {
                    PrintMessage("\nYou lose");
                    keepPlaying = ReplayGame();
                }
            }
            PrintMessage("Your score is: " + score);
        }


        public int GetUserInput()
        {
            Console.Write("Input you guess: ");
            var UserGuess = _userInputService.ReadLine();
             

            int numValue;
            while (!int.TryParse(UserGuess, out numValue))
            {
                PrintMessage("Please enter a digit.");
                Console.Write("Input you guess: ");
                UserGuess = _userInputService.ReadLine();
            }
            return numValue;
        }



        public bool ReplayGame()
        {
            Console.Write("Do you want to play again (y/n)? ");

            while (true)
            {
                var keyStroke = _userInputService.ReadKey();


                if (keyStroke.Key == ConsoleKey.Y)
                {
                    hasWon = false;
                    random = GenerateRandom();
                    limit = 5;
                    return true;
                }
                else if (keyStroke.Key == ConsoleKey.N)
                {
                    return false;
                }
                else
                {
                    Console.Write("\nPlease input either y or n: ");
                }
            }
        }
    }
}
